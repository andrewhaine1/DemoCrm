using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models
{
    public abstract class CustomerContactBase
    {
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Department { get; set; }

        [MaxLength(150)]
        public string Position { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public Guid CustomerAccountId { get; set; }
    }
}
