using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class MediaFileDownloaderEventArgs : EventArgs
    {
        public MediaFile MediaFile { get; private set; }

        public MediaFileDownloaderEventArgs(MediaFile file)
        {
            this.MediaFile = file;
        }
    }
}
