using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public class MediaDownloadFinishedEvenArgs : MediaDownloaderEventArgs
    {
        public string OutputPath { get; private set; }
        public MediaDownloadFinishedEvenArgs(MediaFile file, string outputPath)
            : base(file)
        {
            this.OutputPath = outputPath;
        }
    }
}
