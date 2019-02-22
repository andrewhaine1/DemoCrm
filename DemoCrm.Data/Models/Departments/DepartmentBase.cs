using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models.Departments
{
    public abstract class DepartmentBase
    {
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
    }
}
