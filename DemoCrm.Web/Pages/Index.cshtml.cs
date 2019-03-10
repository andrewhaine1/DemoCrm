using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DemoCrm.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private IDemoCrmRepository _demoCrmRepository;

        public IndexModel(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get logged in user's Oauth user Id. 
            if (User.Claims == null)
                throw new Exception("Could not access user claims");

            var oauthId = Guid.Parse(User.Claims.Single(c => c.Type.Equals("sub")).Value);
            if (oauthId == null)
                throw new Exception("Could not access user oauth id from claims");

            // If the CRM User object exists, redirect to Company page.
            if (await _demoCrmRepository.CrmUserExitsAsync(oauthId))
            {
                var userId = await _demoCrmRepository.GetCrmUserIdAsync(oauthId);
                return RedirectToPage("/DemoCompany/CrmCompany", new { userId });
            }

            // If the CRM user object does not exist yet, go to CRM User page.
            return RedirectToPage("/DemoUser/CrmUser", new { oauthId });
        }

        private async Task ShowIdentityInformation()
        {
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
