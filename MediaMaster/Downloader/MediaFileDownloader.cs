using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.Diagnostics;
using MediaMaster.Converter;
using System.Collections.ObjectModel;
using System.Threading;

namespace MediaMaster
{
    public class MediaFileDownloader
    {
        protected static readonly object SyncRoot = new object();

        private static ReadOnlyCollection<string> userAgents = new ReadOnlyCollection<string>(new List<string>
                    {
                        "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1664.3 Safari/537.36",
                        "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.16 Safari/537.36",
                        "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.17 Safari/537.36",
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0",
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:25.0) Gecko/20100101 Firefox/25.0",
                        "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:21.0) Gecko/20130331 Firefox/21.0",
                        "Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0",
                        "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)",
                        "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)"
                    });
        private static Random rnd = new Random();

        public static ReadOnlyCollection<string> UserAgents
        {
            get
            {
                return userAgents;
            }
        }

        public static string GetRandomUserAgent()
        {
            return UserAgents[rnd.Next(0, UserAgents.Count)];
        }

        public IEnumerable<CompositeResult<DownloadResult, ConvertResult>> DownloadAndConvert
            (IEnumerable<MediaFile> files, string tempFolderName, string convertTo = "", bool deleteCreatedFolder = true)
        {
            string tempFolderPath = Path.Combine(Path.GetTempPath(), tempFolderName);
            Directory.CreateDirectory(tempFolderPath);

            Task<CompositeResult<DownloadResult, ConvertResult>>[] tasks = this.CreateDownloadAndConvertTasks(files, tempFolderPath, convertTo);

            Task.WaitAll(tasks);

            if (deleteCreatedFolder)
            {
                try
                {
                    Directory.Delete(tempFolderPath, true);
                }
                catch { }
            }

            return tasks.Select(x => x.Result);
        }

        protected virtual Task<CompositeResult<DownloadResult, ConvertResult>>[] CreateDownloadAndConvertTasks(IEnumerable<MediaFile> files, string tempFolderPath, string convertTo = "")
        {
            List<Task<CompositeResult<DownloadResult, ConvertResult>>> tasks = new List<Task<CompositeResult<DownloadResult, ConvertResult>>>();
            foreach (MediaFile file in files)
            {
                if (file.IsValid)
                {
                    Task<CompositeResult<DownloadResult, ConvertResult>> newTask =
                       Task<DownloadResult>.Factory.StartNew(() =>
                       {
                           return this.Download(file, tempFolderPath);
                       })
                       .ContinueWith<CompositeResult<DownloadResult, ConvertResult>>(t =>
                       {
                           DownloadResult dwResult = t.Result;
                           ConvertResult cResult = new ConvertResult(dwResult.File)
                           {
                               IsConverted = false,
                           };

                           if (dwResult.IsDownloaded)
                           {
                               cResult = this.ConvertSingleFile(file, t.Result.DownloadPath, tempFolderPath, convertTo);
                           }

                           return new CompositeResult<DownloadResult, ConvertResult>(dwResult.File, dwResult, cResult);
                       });

                    tasks.Add(newTask);
                }
                else
                {
                    tasks.Add(new Task<CompositeResult<DownloadResult, ConvertResult>>(() =>
                        new CompositeResult<DownloadResult, ConvertResult>(file, new DownloadResult(file) { IsDownloaded = false }, new ConvertResult(file) { IsConverted = false })));
                }
            }

            return tasks.ToArray();
        }

        protected virtual ZipFile CreateZipFile(string tempFolderPath)
        {
            string zipPath = Path.Combine(tempFolderPath, string.Format("{0}.zip", Guid.NewGuid().ToString()));
            ZipFile zip = new ZipFile(zipPath);
            zip.AlternateEncoding = Encoding.UTF8;
            zip.AlternateEncodingUsage = ZipOption.AsNecessary;

            return zip;
        }

        public DownloadResult Download(MediaFile file, string tempFolderPath)
        {
            DownloadResult result = new DownloadResult(file);
            result.IsDownloaded = true;
            MediaFileMetadata metadata = file.GetMetadata();
            string outputPath = Path.Combine(tempFolderPath, metadata.FileName + metadata.FileExtension);

            if (!this.OnMediaFileDownloadStarting(file, outputPath))
            {
                result.IsDownloaded = false;
                return result;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metadata.DownloadLink);
            request.Method = "GET";
            request.UserAgent = MediaFileDownloader.GetRandomUserAgent();
            request.KeepAlive = true;
            request.Timeout = Timeout.Infinite;
            try
            {
                bool fileExists = File.Exists(outputPath);
                if (!fileExists || (fileExists && !this.IsFileLocked(new FileInfo(outputPath))))
                {
                    this.CreateFileDownloadRequest(file, outputPath, request);
                }
                else
                {
                    result.IsDownloaded = false;
                    result.Exceptions.Add(new IOException("File {0} is locked, try killing the process that has locked it"));
                    return result;
                }
            }
            catch (WebException webEx)
            {
                Debug.WriteLine("File " + file.GetMetadata().FileName + " Could not be downloaded " + webEx + " " + webEx.InnerException);
                result.IsDownloaded = false;
                result.Exceptions.Add(webEx);
            }

            result.DownloadPath = outputPath;
            return result;
        }

        protected virtual void CreateFileDownloadRequest(MediaFile file, string outputPath, HttpWebRequest request)
        {
            using (WebResponse response = request.GetResponse())
            {
                long contentLength = response.ContentLength;
                using (Stream responseStream = response.GetResponseStream())
                {
                    bool canceled = false;
                    using (FileStream fileStream = new FileStream(@outputPath, FileMode.OpenOrCreate))
                    {
                        int bytesRead = -1;
                        byte[] buffer = new byte[1024];
                        long totalRead = 0;
                        while (bytesRead != 0)
                        {
                            if (!this.OnMediaFileDownloadProgress(file, contentLength, totalRead))
                            {
                                canceled = true;
                                break;
                            }

                            bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            fileStream.Write(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                        }
                    }

                    if (!canceled)
                    {
                        this.OnMediaFileDownloadFinished(file, outputPath);
                    }
                    else
                    {
                        try { File.Delete(outputPath); }
                        catch { }
                    }
                }
            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public ConvertResult ConvertSingleFile(MediaFile file, string filePathToConvert, string tempFolderPath, string convertTo = "")
        {
            ConvertResult result = new ConvertResult(file);
            result.IsConverted = true;
            if (filePathToConvert == string.Empty)
            {
                result.IsConverted = false;
                result.Exceptions.Add(new IOException("Incorrect output path - empty"));
                return result;
            }

            MediaFileMetadata metadata = file.GetMetadata();
            if (convertTo == "" || convertTo != metadata.FileExtension)
            {
                string mediaFileOutputPath = Path.Combine(tempFolderPath, metadata.FileName + convertTo);
                try
                {
                    if (this.OnMediaFileConversionStarting(file, metadata.FileExtension, convertTo))
                    {
                        MediaConverter converter = new MediaConverter();
                        converter.ConvertSingleFile(filePathToConvert, mediaFileOutputPath, new MediaConverterMetadata(Bitrates.Kbps192));
                        this.OnMediaFileConvertionComplete(file, metadata.FileExtension, convertTo);
                    }

                    result.ConvertedPath = mediaFileOutputPath;
                }
                catch (Exception ex)
                {
                    result.IsConverted = false;
                    result.Exceptions.Add(ex);
                }
                
                return result;
            }

            result.ConvertedPath = Path.Combine(tempFolderPath, metadata.FileName + metadata.FileExtension);
            return result;
        }

        #region Events

        public event EventHandler<MediaFileDownloadStartingEventArgs> MediaFileDownloadStarting;

        protected virtual bool OnMediaFileDownloadStarting(MediaFile file, string outputPath)
        {
            if (this.MediaFileDownloadStarting != null)
            {
                var args = new MediaFileDownloadStartingEventArgs(file, outputPath);
                this.MediaFileDownloadStarting(this, args);
                return !args.Cancel;
            }

            return true;    
        }

        public event EventHandler<MediaFileDownloadProgressEventArgs> MediaFileDownloadProgress;

        protected virtual bool OnMediaFileDownloadProgress(MediaFile file, long fileSize, long downloadedSize)
        {
            if (this.MediaFileDownloadProgress != null)
            {
                var args = new MediaFileDownloadProgressEventArgs(file, fileSize, downloadedSize);
                this.MediaFileDownloadProgress(this, args);

                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<MediaFileDownloadFinishedEvenArgs> MediaFileDownloadFinished;

        protected virtual void OnMediaFileDownloadFinished(MediaFile file, string outputPath)
        {
            if (this.MediaFileDownloadFinished != null)
            {
                this.MediaFileDownloadFinished(this, new MediaFileDownloadFinishedEvenArgs(file, outputPath));
            }
        }

        public event EventHandler<MediaFileConversionEventArgs> MediaFileConversionStarting;

        protected virtual bool OnMediaFileConversionStarting(MediaFile file, string inputExtension, string outputExtension)
        {
            if (this.MediaFileConversionStarting != null)
            {
                var args = new MediaFileConversionEventArgs(file, inputExtension, outputExtension);
                this.MediaFileConversionStarting(this, args);
                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<MediaFileConversionEventArgs> MediaFileConvertionCompelete;

        protected virtual void OnMediaFileConvertionComplete(MediaFile file, string inputExtension, string outputExtension)
        {
            if (this.MediaFileConvertionCompelete != null)
            {
                this.MediaFileConvertionCompelete(this, new MediaFileConversionEventArgs(file, inputExtension, outputExtension));
            }
        }
        #endregion
    }
}
