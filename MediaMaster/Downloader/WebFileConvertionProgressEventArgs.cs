using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaToolkit;

namespace MediaMaster
{
    public class WebFileConvertionProgressEventArgs : WebFileDownloaderEventArgs
    {
        public string InputExtension { get; private set; }

        public string OutputExtension { get; private set; }

        public ConvertProgressEventArgs ConvertionEventArgs { get; private set; }

        public WebFileConvertionProgressEventArgs(WebFile file, string inputExtension, string outputExtension, ConvertProgressEventArgs convertionEventArgs)
            : base(file)
        {
            this.InputExtension = inputExtension;
            this.OutputExtension = outputExtension;
            this.ConvertionEventArgs = convertionEventArgs;
        }
    }
}
