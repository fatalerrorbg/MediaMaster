using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class ConvertResult : BaseResult
    {
        public bool IsConverted { get; set; }

        public string ConvertedPath { get; set; }

        public ConvertResult(MediaFile file)
            :base(file)
        {
        }
    }
}
