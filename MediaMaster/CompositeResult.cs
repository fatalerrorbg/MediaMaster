using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class CompositeResult<TDownloadResult, TConversionResult> : BaseResult
        where TDownloadResult : DownloadResult
        where TConversionResult : ConvertResult
    {
        public TDownloadResult DownloadResult { get; private set; }
        public TConversionResult ConversionResult { get; private set; } 

        public CompositeResult(MediaFile file, TDownloadResult downloadResult, TConversionResult conversionResult)
            :base(file)
        {
            this.DownloadResult = downloadResult;
            this.ConversionResult = conversionResult;
        }
    }
}
