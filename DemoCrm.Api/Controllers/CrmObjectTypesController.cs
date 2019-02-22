using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Entities;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrmObjectTypesController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<CrmObjectTypesController> _logger;
        private IUrlHelper _urlHelper;

        public CrmObjectTypesController(IDemoCrmRepository demoCrmRepository,
            ILogger<CrmObjectTypesController> logger, IUrlHelper urlHelper)
        {
            _demoCrmRepository = demoCrmRepository
                ?? throw new ArgumentNullException(nameof(demoCrmRepository));

            _logger = logger;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCrmObjectTypes()
        {
            var crmObjectTypes = await _demoCrmRepository.GetCrmObjectTypesAsync();

            return Ok(crmObjectTypes);
        }

        [HttpGet("{id}", Name = "GetCrmObjectType")]
        public async Task<IActionResult> GetCrmObjectType(int id)
        {
            var crmObject = await _demoCrmRepository.GetCrmObjectTypeAsync(id);

            if (crmObject == null)
                return NotFound();

            return Ok(crmObject);
        }

        [HttpPost]
        public async Task<IActionResult> CreateObjectType([FromBody] CrmObjectType crmObjectType)
        {
            if (crmObjectType == null)
                return BadRequest();

            if (string.IsNullOrEmpty(crmObjectType.Name))
            {
                ModelState.AddModelError(nameof(crmObjectType.Name),
                    "A valid type name is required.");
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            if (await _demoCrmRepository.CrmObjectTypeExitsAsync(crmObjectType.Name))
                return Conflict();

            _demoCrmRepository.AddObjectType(crmObjectType);

            if (! await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a CrmUser failed on save");

            // return 201 created with the GetCrmUser, Location header and user id
            return CreatedAtRoute("GetCrmObjectType", new { id = crmObjectType.Id }, crmObjectType);
        }
    }
}