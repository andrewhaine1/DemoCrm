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

        [ForeignKey("StaffMember.Id")]
        [Column("AccountOwner")]
        public Guid StaffMemberId { get; set; }

        [Required]
        [ForeignKey("Company.Id")]
        public Guid CompanyId { get; set; }
    }
}
