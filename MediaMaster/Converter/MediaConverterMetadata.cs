using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.Converter
{
    public class MediaConverterMetadata
    {
        public string Extension { get; set; }

        public string FileName { get; set; }

        public Bitrates AudioBitrate { get; set; }

        public MediaConverterMetadata(Bitrates audioBitrate)
            : this(audioBitrate, "New_File", ".mp3")
        {

        }

        public MediaConverterMetadata()
            : this(Bitrates.Kbps192, "New_File", ".mp3")
        {

        }

        public MediaConverterMetadata(Bitrates autioBitrate, string filename, string extension)
        {
            this.AudioBitrate = autioBitrate;
            this.FileName = filename;
            this.Extension = extension;
        }
    }
}
