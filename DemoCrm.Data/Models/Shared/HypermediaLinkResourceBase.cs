using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Data.Models
{
    public abstract class HypermediaLinkResourceBase
    {
        public List<HypermediaLink> Links { get; set; }
            = new List<HypermediaLink>();
    }
}
