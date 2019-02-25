using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models
{
    public abstract class DepartmentBase
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        public Guid? ManagerId { get; set; }
    }
}
