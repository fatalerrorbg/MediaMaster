using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class MediaFileConversionEventArgs : MediaFileDownloaderCancelableEventArgs
    {
        public string InputExtension { get; private set; }

        public string OutputExtension { get; private set; }

        public MediaFileConversionEventArgs(MediaFile MediaFile, string inputExtension, string outputExtension)
            : base(MediaFile)
        {
            this.InputExtension = inputExtension;
            this.OutputExtension = outputExtension;
        }
    }
}
