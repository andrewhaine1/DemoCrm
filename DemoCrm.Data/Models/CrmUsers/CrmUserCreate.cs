using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class CrmUserCreate : CrmUserBase
    {
        [Required(ErrorMessage = "A valid OAuth ID is required")]
        public Guid OauthId { get; set; }
    }
}
