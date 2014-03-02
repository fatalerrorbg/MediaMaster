using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class MediaDownloaderCancelableEventArgs : MediaDownloaderEventArgs
    {
        public bool Cancel { get; set; }

        public MediaDownloaderCancelableEventArgs(MediaFile file)
            :base(file)
        {
        }
    }
}
