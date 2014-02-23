using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public interface IMediaFileWithMetadata<TMetadata>
        where TMetadata : MediaFileMetadata
    {
        bool InitializeMetadataOnDemand { get; }

        TMetadata Metadata { get; }
    }
}
