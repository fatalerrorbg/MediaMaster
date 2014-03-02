using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Converter;

namespace MediaMaster
{
    public class MediaFileConversionEventArgs : MediaDownloaderCancelableEventArgs
    {
        public MediaConverterMetadata OutputMetadata { get; private set; }

        public MediaFileConversionEventArgs(MediaFile mediaFile, MediaConverterMetadata outputMetadata)
            : base(mediaFile)
        {
            this.OutputMetadata = outputMetadata;
        }
    }
}
