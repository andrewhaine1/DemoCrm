using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Web.ViewComponents.Departments
{
    public class DepartmentsViewComponent : ViewComponent
    {
        private IDemoCrmRepository _demoCrmRepository;

        public DepartmentsViewComponent(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid companyId)
        {
            var departmentEntities = await _demoCrmRepository.GetDepartmentsForCompanyAsync(companyId);
            var departmentModels = DepartmentProfile.GetDepartmentModelsFromEntities(departmentEntities);
            return View("Default", departmentModels);
        }
    }
}
