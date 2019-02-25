using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models
{
    public abstract class StaffMemberBase
    {
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
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
        public Guid DepartmentId { get; set; }
    }
}
