using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class MediaDownloaderEventArgs : EventArgs
    {
        public MediaFile MediaFile { get; private set; }

        public MediaDownloaderEventArgs(MediaFile file)
        {
            this.MediaFile = file;
        }
    }
}
