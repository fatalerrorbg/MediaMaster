using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MediaMaster
{
    public class MediaFileMetadata
    {
        public static readonly MediaFileMetadata DefaultMetadata = new MediaFileMetadata();

        public string UniqeId { get; protected set; }

        public string Url { get; protected set; }

        public string ThumbnailLink { get; protected set; }

        public string DownloadLink { get; protected set; }

        public string FileExtension { get; protected set; }

        public string FileName { get; set; }

        public int FileLength { get; protected set; }

        public MediaFileMetadata(string url, string thumbnailLink, string downloadLink, string fileName)
        {
            this.UniqeId = Guid.NewGuid().ToString();
            this.Url = url;
            this.ThumbnailLink = thumbnailLink;
            this.DownloadLink = downloadLink;
            this.FileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            this.FileExtension = Path.GetExtension(downloadLink);

            this.InitializeFields();
        }

        private MediaFileMetadata()
        {
        }

        protected virtual void InitializeFields()
        {
        }
    }
}
