using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class MediaDownloadProgressEventArgs : MediaDownloaderCancelableEventArgs
    {
        public double PercentageComplete { get; private set; }

        public long FileSize { get; private set; }

        public long DownloadedSize { get; private set; }

        public MediaDownloadProgressEventArgs(MediaFile file, long fileSize, long downloadedSize)
            :base(file)
        {
            this.FileSize = fileSize;
            this.DownloadedSize = downloadedSize;
            this.PercentageComplete = ((double)downloadedSize / (double)fileSize) * 100.0f;
        }


    }
}
