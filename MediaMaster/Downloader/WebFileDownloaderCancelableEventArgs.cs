using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public abstract class WebFileDownloaderCancelableEventArgs : WebFileDownloaderEventArgs
    {
        public bool Cancel { get; set; }

        public WebFileDownloaderCancelableEventArgs(WebFile file)
            :base(file)
        {
        }
    }
}
