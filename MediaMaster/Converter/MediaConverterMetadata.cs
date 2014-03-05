using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Converter;

namespace MediaMaster
{
    public class MediaConverterMetadata
    {
        public const string DefaultFileName = "New_File";

        public static MediaConverterMetadata CreateDefaultMetadata(MediaFile file)
        {
            return new MediaConverterMetadata(Bitrates.Kbps192, file.GetMetadata().FileName, file.GetMetadata().FileExtension);
        }

        public SupportedConversionFormats Extension { get; set; }

        public string FileName { get; set; }

        public Bitrates AudioBitrate { get; set; }

        public MediaConverterMetadata(Bitrates audioBitrate)
            : this(audioBitrate, DefaultFileName, SupportedConversionFormats.Mp3)
        {

        }

        public MediaConverterMetadata()
            : this(Bitrates.Kbps192, DefaultFileName, SupportedConversionFormats.Mp3)
        {

        }

        public MediaConverterMetadata(Bitrates autioBitrate, string filename, SupportedConversionFormats extension)
        {
            this.AudioBitrate = autioBitrate;
            this.FileName = filename;
            this.Extension = extension;
        }

        public MediaConverterMetadata(Bitrates audioBitrate, string fileName, string extension)
            : this(audioBitrate, fileName, SupportedConversionFormats.Parse(extension))
        {
        }
    }
}
