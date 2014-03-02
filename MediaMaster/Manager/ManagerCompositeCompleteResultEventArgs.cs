using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class ManagerCompositeCompleteResultEventArgs
    {
        public CompositeResult<DownloadResult, ConvertResult> CompositeResult { get; private set; }

        public ManagerCompositeCompleteResultEventArgs(CompositeResult<DownloadResult, ConvertResult> compositeResult)
        {
            this.CompositeResult = compositeResult;
        }
    }
}
