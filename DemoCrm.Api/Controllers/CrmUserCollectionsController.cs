using DemoCrm.Api.Helpers;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Api.Controllers
{
    [Route("api/crmusercollections")]
    public class CrmUserCollectionsController : Controller
    {
        private IDemoCrmRepository _demoCrmRepository;

        public CrmUserCollectionsController(IDemoCrmRepository demoCrmRepository)
        {
            _demoCrmRepository = demoCrmRepository
                ?? throw new ArgumentNullException(nameof(demoCrmRepository));
        }

        [HttpGet("({ids})", Name ="GetCrmUsersCollection")]
        public async Task<IActionResult> GetCrmUsers(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var crmUserEntities = await _demoCrmRepository.GetCrmUserCollectionAsync(ids);

            if (crmUserEntities.Count() != ids.Count())
                return NotFound();

            var crmUsers = CrmUserProfile.GetCrmUserModelsFromEntities(crmUserEntities);
            return Ok(crmUsers);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCrmUsers([FromBody] IEnumerable<CrmUserCreate> crmUsers)
        {
            if (crmUsers == null)
                return BadRequest();

            var crmUserEntities = CrmUserProfile.GetCrmUserEntitiesFromCreateModels(crmUsers);

            foreach (var crmUserEntity in crmUserEntities)
                _demoCrmRepository.AddCrmUser(crmUserEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating multiple CRM users failed on save.");

            var crmUsersCollection = CrmUserProfile.GetCrmUserModelsFromEntities(crmUserEntities);
            var idsAsString = string.Join(",", crmUsersCollection.Select(c => c.Id));

            return CreatedAtRoute("GetCrmUsersCollection", new { ids = idsAsString }, crmUsersCollection);

            //return Ok();
        }
    }
}
