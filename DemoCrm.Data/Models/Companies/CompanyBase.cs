using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class CompanyBase
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        [Required(ErrorMessage = "The Phone field is required.")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid CrmUserId { get; set; }

        [Required]
        public int ObjectTypeId { get; set; }
    }
}
