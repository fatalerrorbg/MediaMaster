using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class BaseResult
    {
        public MediaFile File { get; private set; }

        public List<Exception> Exceptions { get; private set; } 

        public BaseResult(MediaFile file)
        {
            this.File = file;
            this.Exceptions = new List<Exception>();
        }
    }
}
