using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Converter;

namespace MediaMaster
{
    public class DownloadConvertRequest : IComparable<DownloadConvertRequest>
    {
        public MediaFile MediaFile { get; private set; }

        public string DownloadPath { get; private set; }

        public string ConvertPath { get; private set; }

        public MediaConverterMetadata ConvertMetadata { get; private set; }

        public int Priority { get; set; }

        public DownloadConvertRequest(MediaFile mediaFile, string downloadPath, string convertPath, MediaConverterMetadata convertMetadata)
        {
            this.MediaFile = mediaFile;
            this.DownloadPath = downloadPath;
            this.ConvertPath = convertPath;
            this.ConvertMetadata = convertMetadata;
        }

        #region IComparable<DownloadConvertRequest> Members

        public int CompareTo(DownloadConvertRequest other)
        {
            return this.Priority.CompareTo(other.Priority);
        }

        #endregion
    }
}
