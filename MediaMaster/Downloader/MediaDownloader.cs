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
    public class MediaDownloader
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

        public DownloadResult Download(MediaFile file, string tempFolderPath, bool resumePreviousDownload = false)
        {
            DownloadResult result = new DownloadResult(file);
            result.IsDownloaded = true;
            MediaFileMetadata metadata = file.Metadata;
            string outputPath = Path.Combine(tempFolderPath, metadata.FileName + metadata.FileExtension);

            if (!this.OnMediaFileDownloadStarting(file, outputPath))
            {
                result.IsDownloaded = false;
                return result;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(metadata.DownloadLink);
            request.Method = WebRequestMethods.File.DownloadFile;
            request.UserAgent = MediaDownloader.GetRandomUserAgent();
            request.KeepAlive = true;
            request.Timeout = Timeout.Infinite;
            try
            {
                bool fileExists = File.Exists(outputPath);
                if (!fileExists || !this.IsFileLocked(new FileInfo(outputPath)))
                {
                    this.CreateFileDownloadRequest(file, outputPath, request, resumePreviousDownload);
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
                Debug.WriteLine("File " + file.Metadata.FileName + " Could not be downloaded " + webEx + " " + webEx.InnerException);
                result.IsDownloaded = false;
                result.Exceptions.Add(webEx);
            }

            result.DownloadPath = outputPath;
            return result;
        }

        protected virtual void CreateFileDownloadRequest(MediaFile file, string outputPath, HttpWebRequest request, bool resumePreviousDownload)
        {
            long totalRead = 0;
            FileMode fileMode = FileMode.OpenOrCreate;
            if (resumePreviousDownload && File.Exists(@outputPath))
            {
                totalRead = new FileInfo(outputPath).Length;
                request.AddRange(totalRead);
                fileMode = FileMode.Append;
            }

            using (WebResponse response = request.GetResponse())
            {
                long contentLength = response.ContentLength + totalRead;
                using (Stream responseStream = response.GetResponseStream())
                {
                    bool canceled = false;
                    using (FileStream fileStream = new FileStream(@outputPath, fileMode))
                    {
                        int bytesRead = -1;
                        byte[] buffer = new byte[1024];
                        while (bytesRead != 0)
                        {
                            if (!this.OnMediaFileDownloadProgress(file, contentLength, totalRead))
                            {
                                canceled = true;
                                break;
                            }

                            if (totalRead > contentLength)
	                        {
                                throw new WebException("Downloaded more than actual content of file " + file.Url);
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
                }
            }
        }

        protected bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        #region Events

        public event EventHandler<MediaDownloadStartingEventArgs> MediaFileDownloadStarting;

        protected virtual bool OnMediaFileDownloadStarting(MediaFile file, string outputPath)
        {
            if (this.MediaFileDownloadStarting != null)
            {
                var args = new MediaDownloadStartingEventArgs(file, outputPath);
                this.MediaFileDownloadStarting(this, args);
                return !args.Cancel;
            }

            return true;    
        }

        public event EventHandler<MediaDownloadProgressEventArgs> MediaFileDownloadProgress;

        protected virtual bool OnMediaFileDownloadProgress(MediaFile file, long fileSize, long downloadedSize)
        {
            if (this.MediaFileDownloadProgress != null)
            {
                var args = new MediaDownloadProgressEventArgs(file, fileSize, downloadedSize);
                this.MediaFileDownloadProgress(this, args);

                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<MediaDownloadFinishedEvenArgs> MediaFileDownloadFinished;

        protected virtual void OnMediaFileDownloadFinished(MediaFile file, string outputPath)
        {
            if (this.MediaFileDownloadFinished != null)
            {
                this.MediaFileDownloadFinished(this, new MediaDownloadFinishedEvenArgs(file, outputPath));
            }
        }

        
        #endregion
    }
}
