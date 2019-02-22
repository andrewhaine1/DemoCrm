using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DemoCrm.Data.Models
{
    [DataContract]
    public class HypermediaLink
    {
        [DataMember]
        public string Href { get; private set; }
        [DataMember]
        public string Rel { get; private set; }
        [DataMember]
        public string Method { get; private set; }

        public HypermediaLink()
        {
        }
        
        public HypermediaLink(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
