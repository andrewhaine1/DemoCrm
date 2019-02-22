using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models
{
    public abstract class CrmUserBase : HypermediaLinkResourceBase
    {
        [Required(ErrorMessage = "A user needs to have a first name :-)")]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(150)]
        public string Phone { get; set; }

        [Required]
        public int ObjectTypeId { get; set; }
    }
}
