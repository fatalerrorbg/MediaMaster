using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class WebFileDownloadStartingEventArgs : WebFileDownloaderCancelableEventArgs
    {
        public string OutputPath { get; private set; }

        public WebFileDownloadStartingEventArgs(WebFile file, string outputPath)
            : base(file)
        {
            this.OutputPath = outputPath;
        }
    }
}
