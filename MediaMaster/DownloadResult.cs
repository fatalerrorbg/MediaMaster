using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class DownloadResult : BaseResult
    {
        public bool IsDownloaded { get; set; }

        public string DownloadPath { get; set; }

        public DownloadResult(MediaFile file)
            :base(file)
        {
        }
    }
}
