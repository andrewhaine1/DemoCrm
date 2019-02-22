using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Data.Models
{
    public class LinkedCollectionResourceWrapper<T> : HypermediaLinkResourceBase
        where T : HypermediaLinkResourceBase
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapper(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
