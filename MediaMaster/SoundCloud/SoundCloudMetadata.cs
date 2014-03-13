using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class SoundCloudMetadata : MediaFileMetadata
    {
        public string TrackId { get; private set; }

        public string Description { get; private set; }

        public TimeSpan Duration { get; private set; }

        public SoundCloudMetadata(string url, string thumbnailLink, string downloadLink, string fileName, string trackId, string description, string duration)
            :base(url, thumbnailLink, downloadLink, fileName)
        {
            this.TrackId = trackId;
            this.Description = description;
            this.Duration = TimeSpan.Parse(duration);
        }

        protected override void InitializeFields()
        {
            this.FileExtension = SupportedConversionFormats.Mp3.Value;
        }
    }
}
