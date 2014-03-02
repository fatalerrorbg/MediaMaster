using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class ManagerDownloadCompleteResultEventArgs
    {
        public DownloadResult DownloadResult { get; private set; }

        public ManagerDownloadCompleteResultEventArgs(DownloadResult downloadResult)
        {
            this.DownloadResult = downloadResult;
        }
    }
}
