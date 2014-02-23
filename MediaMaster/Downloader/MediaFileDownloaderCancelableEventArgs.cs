using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class MediaFileDownloaderCancelableEventArgs : MediaFileDownloaderEventArgs
    {
        public bool Cancel { get; set; }

        public MediaFileDownloaderCancelableEventArgs(MediaFile file)
            :base(file)
        {
        }
    }
}
