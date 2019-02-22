using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("CrmUsers")]
    public class CrmUser : DemoCrmEntity
    {
        [Required]
        public Guid OauthId { get; set; }

        [Required]
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
        public string PhoneNumber { get; set; }

        [ForeignKey("ObjectTypeId")]
        public virtual CrmObjectType ObjectType { get; set; }

        [Required]
        public int ObjectTypeId { get; set; }
    }
}
