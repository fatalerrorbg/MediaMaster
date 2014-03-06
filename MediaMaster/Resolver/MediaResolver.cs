using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.Resolver
{
    public abstract class MediaResolver
    {
        public abstract string ResolveByName(string name);

        public abstract string ResolveByUrl(string url);
    }
}
