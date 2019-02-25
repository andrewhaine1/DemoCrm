using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("Departments")]
    public class Department : DemoCrmEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey("ManagerId")]
        public StaffMember Manager { get; set; }

        public Guid? ManagerId { get; set; }
    }
}
