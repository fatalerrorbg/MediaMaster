using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Converter;

namespace MediaMaster
{
    public class MediaDownloadConvertManager
    {
        public const int DefaultMaxParallelRequests = 5;
        protected static object SyncRoot = new object();

        private int maxParallelRequests;
        private bool initialized;

        public Queue<DownloadConvertRequest> Queue { get; private set; }

        public HashSet<DownloadConvertRequest> CurrentlyProcessingRequests { get; private set; }

        public int CurrentlyProcessingRequestsCount
        {
            get { return this.CurrentlyProcessingRequests.Count; }
        }

        public MediaDownloader Downloader { get; private set; }

        public MediaConverter Converter { get; private set; }

        public int MaxParallelRequests
        {
            get { return this.maxParallelRequests; }
            set
            {
                this.maxParallelRequests = value;
                this.DequeueRequests();
            }
        }

        public MediaDownloadConvertManager()
            : this(DefaultMaxParallelRequests)
        {
            this.Queue = new Queue<DownloadConvertRequest>();
            this.Downloader = new MediaDownloader();
            this.Converter = new MediaConverter();
            this.CurrentlyProcessingRequests = new HashSet<DownloadConvertRequest>();
            this.initialized = true;
        }

        public MediaDownloadConvertManager(int maxParallelRequests)
        {
            this.MaxParallelRequests = maxParallelRequests;
        }

        public void EnqueueDownloadAndConvertRequest(MediaFile file, string outputPath, MediaConverterMetadata convertMetadata)
        {
            this.EnqueueDownloadAndConvertRequest(file, outputPath, outputPath, convertMetadata);
        }

        public void EnqueueDownloadAndConvertRequest(MediaFile file, string downloadPath, string convertPath, MediaConverterMetadata convertMetadata)
        {
            DownloadConvertRequest request = new DownloadConvertRequest(file, downloadPath, convertPath, convertMetadata);
            this.Queue.Enqueue(request);
        }

        public void StartDownload()
        {
            this.DequeueRequests();
        }

        public void StartDownload(DownloadConvertRequest request)
        {
            if (this.Queue.Contains(request))
            {
                var list = this.Queue.ToList();
                list.Remove(request);
                this.Queue.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    this.Queue.Enqueue(list[i]);
                }
            }

            this.DequeueRequest(request);
        }

        protected virtual void DequeueRequests()
        {
            if (!this.initialized)
            {
                return;
            }

            while (this.CurrentlyProcessingRequestsCount < this.MaxParallelRequests && this.Queue.Count > 0)
            {
                this.DequeueRequest(this.Queue.Dequeue());
            }
        }

        protected virtual void DequeueRequest(DownloadConvertRequest currentRequest)
        {
            if (this.CurrentlyProcessingRequestsCount >= this.MaxParallelRequests)
            {
                return;
            }

            this.CurrentlyProcessingRequests.Add(currentRequest);
            Task.Factory.StartNew(() =>
            {
                DownloadResult downloadResult = this.Downloader.Download(currentRequest.MediaFile, currentRequest.DownloadPath);
                this.OnDownloadResult(downloadResult);

                ConvertResult convertResult;
                if (downloadResult.IsDownloaded)
                {
                    convertResult = this.Converter.Convert(currentRequest.MediaFile, downloadResult.DownloadPath, currentRequest.ConvertPath, currentRequest.ConvertMetadata);
                }
                else
                {
                    convertResult = new ConvertResult(currentRequest.MediaFile) { IsConverted = false };
                }

                this.OnConvertResult(convertResult);

                CompositeResult<DownloadResult, ConvertResult> finalResult = new
                    CompositeResult<DownloadResult, ConvertResult>(currentRequest.MediaFile, downloadResult, convertResult);

                lock (SyncRoot)
                {
                    this.CurrentlyProcessingRequests.Remove(currentRequest);
                }

                this.OnDownloadConvertResult(finalResult);
            });
        }

        #region Events
        public event EventHandler<ManagerDownloadCompleteResultEventArgs> DownloadResult;

        protected virtual void OnDownloadResult(DownloadResult result)
        {
            if (this.DownloadResult != null)
            {
                this.DownloadResult(this, new ManagerDownloadCompleteResultEventArgs(result));
            }
        }

        public event EventHandler<ManagerConvertCompleteResultEventArgs> ConvertResult;

        protected virtual void OnConvertResult(ConvertResult result)
        {
            if (this.ConvertResult != null)
            {
                this.ConvertResult(this, new ManagerConvertCompleteResultEventArgs(result));
            }
        }

        public event EventHandler<ManagerCompositeCompleteResultEventArgs> DownloadConvertResult;

        protected virtual void OnDownloadConvertResult(CompositeResult<DownloadResult, ConvertResult> result)
        {
            this.DequeueRequests();
            if (this.DownloadConvertResult != null)
            {
                this.DownloadConvertResult(this, new ManagerCompositeCompleteResultEventArgs(result));
            }
        }
        #endregion

    }
}
