using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("Companies")]
    public class Company : DemoCrmEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [ForeignKey("CrmUserId")]
        public CrmUser CrmUser { get; set; }

        public Guid CrmUserId { get; set; }

        [ForeignKey("ObjectTypeId")]
        public CrmObjectType ObjectType { get; set; }

        [Required]
        public int ObjectTypeId { get; set; }
    }
}
