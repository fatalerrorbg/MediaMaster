using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public interface IWebFileWithMetadata<TMetadata>
        where TMetadata : WebFileMetadata
    {
        bool InitializeMetadataOnDemand { get; }

        TMetadata Metadata { get; }
    }
}
