using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class VboxFileMetadata : MediaFileMetadata
    {
        public string VideoId { get; private set; }

        public VboxFileMetadata(string url, string thumbnailLink, string downloadLink, string videoId, string fileName)
            :base(url, thumbnailLink, downloadLink, fileName)
        {
            this.VideoId = videoId;
        }
    }
}
