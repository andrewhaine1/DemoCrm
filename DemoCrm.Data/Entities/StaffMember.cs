using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("StaffMembers")]
    public class StaffMember : DemoCrmEntity
    {
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
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [ForeignKey("StaffPositionId")]
        public StaffPosition StaffPosition { get; set; }
        public Guid StaffPositionId { get; set; }

        [Required]
        public bool IsManager { get; set; } = false;

        [Required]
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
