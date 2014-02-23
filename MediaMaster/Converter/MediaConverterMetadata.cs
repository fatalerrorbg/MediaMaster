using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.Converter
{
    public class MediaConverterMetadata
    {
        public Bitrates AudioBitrate { get; set; }

        public MediaConverterMetadata()
            : this(Bitrates.Kbps192)
        {

        }

        public MediaConverterMetadata(Bitrates autioBitrate)
        {
            this.AudioBitrate = autioBitrate;
        }
    }
}
