using DemoCrm.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Web.Factories.Department
{
    public class DepartmentViewComponentModel
    {
        public IEnumerable<Data.Models.Department> Departments { get; set; }

        public DepartmentViewComponentModel(Guid companyId)
        {
        }
    }
}
