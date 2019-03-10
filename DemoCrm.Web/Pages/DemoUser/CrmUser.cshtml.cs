using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoCrm.Web.Pages.DemoUser
{
    public class CrmUserModel : PageModel
    {
        private IDemoCrmRepository _demoCrmRepository;
        public CrmUser CrmUser { get; private set; }

        public Guid OauthId { get; private set; }
        public int ObjectId { get; set; }

        public CrmUserModel(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository;
        }

        public async Task OnGetAsync(Guid oauthId)
        {
            // Set the User's Oath Id passed by the index page.
            OauthId = oauthId;

            // Get Object Type Id. Only select Demo as sample will not be used in the application.
            var crmObjectType = await _demoCrmRepository.GetCrmObjectTypeIdAsync("Demo");
            ObjectId = crmObjectType.Id;

            var crmUserEntity = await _demoCrmRepository.GetCrmUserAsync(oauthId);

            CrmUser = CrmUserProfile.GetCrmUserModelFromEntity(crmUserEntity) ?? null;
        }

        public async Task<IActionResult> OnPostAsync(CrmUserCreate crmUserCreate)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var crmUserEntity = CrmUserProfile.GetCrmUserEntityFromCreateModel(crmUserCreate);

            _demoCrmRepository.AddCrmUser(crmUserEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Could not add CRM User.");

            var crmUserId = await _demoCrmRepository.GetCrmUserIdAsync(crmUserEntity.OauthId);

            return RedirectToPage("/DemoCompany/CrmCompany", new { userId = crmUserId });
        }
    }
}