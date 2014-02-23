using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class WebFileDownloadFinishedEvenArgs : WebFileDownloaderEventArgs
    {
        public string OutputPath { get; private set; }
        public WebFileDownloadFinishedEvenArgs(WebFile file, string outputPath)
            : base(file)
        {
            this.OutputPath = outputPath;
        }
    }
}
