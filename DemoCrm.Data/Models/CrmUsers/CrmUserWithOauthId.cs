using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class CrmUserWithOauthId : CrmUser
    {
        public Guid OauthId { get; set; }
    }
}
