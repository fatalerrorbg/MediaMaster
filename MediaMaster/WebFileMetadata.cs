using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MediaMaster
{
    public abstract class WebFileMetadata
    {
        public string UniqeId { get; private set; }

        public string Url { get; private set; }

        public string ThumbnailLink { get; private set; }

        public string DownloadLink { get; private set; }

        public string FileExtension { get; private set; }

        public string FileName { get; private set; }

        public WebFileMetadata(string url, string thumbnailLink, string downloadLink, string fileName)
        {
            this.UniqeId = Guid.NewGuid().ToString();
            this.Url = url;
            this.ThumbnailLink = thumbnailLink;
            this.DownloadLink = downloadLink;
            this.FileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            this.FileExtension = Path.GetExtension(downloadLink);
        }
    }
}
