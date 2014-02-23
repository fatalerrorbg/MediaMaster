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

namespace MediaMaster
{
    public class WebFileDownloader
    {
        protected static readonly object SyncRoot = new object();

        public IEnumerable<FileInfo> DownloadAndConvert(IEnumerable<WebFile> files, string tempFolderName, string convertTo = "", bool deleteCreatedFolder = true)
        {
            string tempFolderPath = Path.Combine(Path.GetTempPath(), tempFolderName);
            Directory.CreateDirectory(tempFolderPath);

            Task<FileInfo>[] tasks = this.CreateDownloadAndConvertTasks(files, tempFolderPath, convertTo);

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

        protected virtual Task<FileInfo>[] CreateDownloadAndConvertTasks(IEnumerable<WebFile> files, string tempFolderPath, string convertTo = "")
        {
            List<Task<FileInfo>> tasks = new List<Task<FileInfo>>();
            foreach (WebFile file in files)
            {
                if (file.IsValid)
                {
                    Task<FileInfo> newTask =
                       Task<FileInfo>.Factory.StartNew(() =>
                       {
                           return this.Download(file, tempFolderPath);
                       })
                       .ContinueWith<FileInfo>(t =>
                       {
                           return this.ConvertSingleFile(file, t.Result.FullName, tempFolderPath, convertTo);
                       });

                    tasks.Add(newTask);
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

        public FileInfo Download(WebFile file, string tempFolderPath)
        {
            WebFileMetadata metadata = file.GetMetadata();
            string outputPath = Path.Combine(tempFolderPath, metadata.FileName + metadata.FileExtension);

            if (!this.OnWebFileDownloadStarting(file, outputPath))
            {
                return null;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metadata.DownloadLink);
            request.Method = "GET";
            try
            {
                bool fileExists = File.Exists(outputPath);
                if (!fileExists ||(fileExists && !this.IsFileLocked(new FileInfo(outputPath))))
                {
                    this.CreateFileDownloadRequest(file, outputPath, request);
                }
                else
                {
                    //TODO: Raise Error
                }
            }
            catch (WebException webEx)
            {
                Debug.WriteLine("File " + file.GetMetadata().FileName + " Could not be downloaded " + webEx + " " + webEx.InnerException);
            }

            return new FileInfo(outputPath);
        }

        protected virtual void CreateFileDownloadRequest(WebFile file, string outputPath, HttpWebRequest request)
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
                            if (!this.OnWebFileDownloadProgress(file, contentLength, totalRead))
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
                        this.OnWebFileDownloadFinished(file, outputPath);
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

        public FileInfo ConvertSingleFile(WebFile file, string filePathToConvert, string tempFolderPath, string convertTo = "")
        {
            if (filePathToConvert == string.Empty)
            {
                return null;
            }

            WebFileMetadata metadata = file.GetMetadata();
            if (convertTo == "" || convertTo != metadata.FileExtension)
            {
                string mediaFileOutputPath = Path.Combine(tempFolderPath, metadata.FileName + convertTo);
                if(this.OnWebFileConversionStarting(file, metadata.FileExtension, convertTo))
                {
                    MediaConverter converter = new MediaConverter();
                    converter.ConvertSingleFile(filePathToConvert, mediaFileOutputPath, new MediaConverterMetadata(Bitrates.Kbps192));
                    this.OnWebFileConvertionComplete(file, metadata.FileExtension, convertTo);
                }

                return new FileInfo(mediaFileOutputPath);
            }

            return new FileInfo(Path.Combine(tempFolderPath, metadata.FileName + metadata.FileExtension));
        }

        #region Events

        public event EventHandler<WebFileDownloadStartingEventArgs> WebFileDownloadStarting;

        protected virtual bool OnWebFileDownloadStarting(WebFile file, string outputPath)
        {
            if (this.WebFileDownloadStarting != null)
            {
                var args = new WebFileDownloadStartingEventArgs(file, outputPath);
                this.WebFileDownloadStarting(this, args);
                return !args.Cancel;
            }

            return true;    
        }

        public event EventHandler<WebFileDownloadProgressEventArgs> WebFileDownloadProgress;

        protected virtual bool OnWebFileDownloadProgress(WebFile file, long fileSize, long downloadedSize)
        {
            if (this.WebFileDownloadProgress != null)
            {
                var args = new WebFileDownloadProgressEventArgs(file, fileSize, downloadedSize);
                this.WebFileDownloadProgress(this, args);

                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<WebFileDownloadFinishedEvenArgs> WebFileDownloadFinished;

        protected virtual void OnWebFileDownloadFinished(WebFile file, string outputPath)
        {
            if (this.WebFileDownloadFinished != null)
            {
                this.WebFileDownloadFinished(this, new WebFileDownloadFinishedEvenArgs(file, outputPath));
            }
        }

        public event EventHandler<WebFileConversionEventArgs> WebFileConversionStarting;

        protected virtual bool OnWebFileConversionStarting(WebFile file, string inputExtension, string outputExtension)
        {
            if (this.WebFileConversionStarting != null)
            {
                var args = new WebFileConversionEventArgs(file, inputExtension, outputExtension);
                this.WebFileConversionStarting(this, args);
                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<WebFileConversionEventArgs> WebFileConvertionCompelete;

        protected virtual void OnWebFileConvertionComplete(WebFile file, string inputExtension, string outputExtension)
        {
            if (this.WebFileConvertionCompelete != null)
            {
                this.WebFileConvertionCompelete(this, new WebFileConversionEventArgs(file, inputExtension, outputExtension));
            }
        }
        #endregion
    }
}
