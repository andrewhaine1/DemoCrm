using DemoCrm.Data.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Api.Filters
{
    public class CrmUsersResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, 
            ResultExecutionDelegate next)
        {
            var actionResult = context.Result as ObjectResult;
            if (actionResult?.Value == null
                || actionResult.StatusCode < 200
                || actionResult.StatusCode >= 300)
            {
                await next();
                return;
            }

            actionResult.Value = CrmUserProfile.GetCrmUserModelsFromEntities(actionResult.Value as IEnumerable<Data.Entities.CrmUser>);

            await next();
        }
    }
}
