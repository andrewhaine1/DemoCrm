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
        [ForeignKey("StaffPosition.Id")]
        public Guid StaffPositionId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsManager { get; set; }

        [Required]
        [ForeignKey("Department.Id")]
        public Guid DepartmentId { get; set; }
    }
}
