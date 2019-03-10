using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Web.ViewComponents.StaffMembers
{
    public class StaffMembersViewComponent : ViewComponent
    {
        private IDemoCrmRepository _demoCrmRepository;

        public StaffMembersViewComponent(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid companyId)
        {

        }
    }
}
