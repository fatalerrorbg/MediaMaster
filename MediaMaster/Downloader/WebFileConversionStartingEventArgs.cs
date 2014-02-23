using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class WebFileConversionEventArgs : WebFileDownloaderCancelableEventArgs
    {
        public string InputExtension { get; private set; }

        public string OutputExtension { get; private set; }

        public WebFileConversionEventArgs(WebFile webFile, string inputExtension, string outputExtension)
            : base(webFile)
        {
            this.InputExtension = inputExtension;
            this.OutputExtension = outputExtension;
        }
    }
}
