using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoCrm.Web.Pages.DemoCompany
{
    public class CrmCompanyModel : PageModel
    {
        private IDemoCrmRepository _demoCrmRepository;

        public Company Company { get; private set; }
        public CrmUser CrmUser { get; set; }
        public int ObjectId { get; private set; }

        public CrmCompanyModel(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository;
        }

        public async Task OnGet(Guid userId)
        {
            var companyEntity = await _demoCrmRepository.GetCompanyByUserIdAsync(userId);
            Company = CompanyProfile.GetCompanyModelFromEntity(companyEntity);

            var crmUserEntity = await _demoCrmRepository.GetCrmUserAsync(userId);
            CrmUser = CrmUserProfile.GetCrmUserModelFromEntity(crmUserEntity);

            // Get Object Type Id. Only select Demo as sample will not be used in the application.
            var crmObjectType = await _demoCrmRepository.GetCrmObjectTypeIdAsync("Demo");
            ObjectId = crmObjectType.Id;
        }

        public async Task<IActionResult> OnPostAsync(CompanyCreate companyCreate)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var crmCompanyEntity = CompanyProfile.GetCompanyEntityFromCreateModel(companyCreate);

            _demoCrmRepository.AddCompany(crmCompanyEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Could not add Company.");

            return Page();
        }
    }
}