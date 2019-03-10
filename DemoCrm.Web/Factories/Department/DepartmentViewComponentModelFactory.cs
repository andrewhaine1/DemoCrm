using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Web.Factories.Department
{
    public static class DepartmentViewComponentModelFactory
    {
        public static DepartmentViewComponentModel CreateDepartmentViewComponentModel(Guid companyId)
        {
            return new DepartmentViewComponentModel(companyId);
        }
    }
}
