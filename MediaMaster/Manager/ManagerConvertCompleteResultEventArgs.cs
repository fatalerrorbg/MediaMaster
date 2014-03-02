using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class ManagerConvertCompleteResultEventArgs
    {
        public ConvertResult ConvertResult { get; private set; }

        public ManagerConvertCompleteResultEventArgs(ConvertResult convertResult)
        {
            this.ConvertResult = convertResult;
        }
    }
}
