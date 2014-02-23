using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaToolkit;

namespace MediaMaster
{
    public class WebFileConversionCompletedEventArgs : WebFileDownloaderEventArgs
    {
        public string InputExtension { get; private set; }

        public string OutputExtension { get; private set; }

        public ConversionCompleteEventArgs ConvertionEventArgs { get; private set; }

        public WebFileConversionCompletedEventArgs(WebFile file, string inputExtension, string outputExtension, ConversionCompleteEventArgs convertionEventArgs)
            : base(file)
        {
            this.InputExtension = inputExtension;
            this.OutputExtension = outputExtension;
            this.ConvertionEventArgs = convertionEventArgs;
        }
    }
}
