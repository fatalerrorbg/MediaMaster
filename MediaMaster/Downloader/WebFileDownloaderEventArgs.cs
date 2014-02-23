using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class WebFileDownloaderEventArgs : EventArgs
    {
        public WebFile WebFile { get; private set; }

        public WebFileDownloaderEventArgs(WebFile file)
        {
            this.WebFile = file;
        }
    }
}
