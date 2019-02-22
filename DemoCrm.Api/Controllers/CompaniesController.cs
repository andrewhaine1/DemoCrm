using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Helpers;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<CrmUsersController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public CompaniesController(IDemoCrmRepository demoCrmRepository,
            ILogger<CrmUsersController> logger, IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _demoCrmRepository = demoCrmRepository
                ?? throw new ArgumentNullException(nameof(demoCrmRepository));

            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetCompanies")]
        [HttpHead]
        public async Task<IActionResult> GetCompanies([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<Data.Models.Company, Data.Entities.Company>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<Data.Models.Company>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var companies = await _demoCrmRepository.GetCompaniesAsync(crmResourceParameters);

            var companyModels = CompanyProfile.GetCompanyModelsFromEntities(companies);

            return Ok(companyModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] Data.Models.Company company)
        {
            if (company == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            return Ok();
        }
    }
}