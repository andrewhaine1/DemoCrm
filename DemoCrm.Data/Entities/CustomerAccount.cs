using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("CustomerAccounts")]
    public class CustomerAccount : DemoCrmEntity
    {
        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Address { get; set; }

        [Required]
        [MaxLength(150)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(150)]
        public string VatNumber { get; set; }

        [Required]
        [MaxLength(150)]
        public string PhoneNumber { get; set; }

        [ForeignKey("StaffMemberId")]
        public StaffMember AccountManager { get; set; }

        [Required]
        public Guid StaffMemberId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }
}
