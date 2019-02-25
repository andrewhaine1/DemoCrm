using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Helpers;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoCrm.Api.Controllers
{
    [Route("api/businessleads")]
    [ApiController]
    public class BusinessLeadsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "BusinessLead";
        private const string ControllerNamePlural = "BusinessLeads";

        public BusinessLeadsController(IDemoCrmRepository demoCrmRepository,
            ILogger<DepartmentsController> logger, IUrlHelper urlHelper,
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

        [HttpGet(Name = "GetBusinessLeads")]
        [HttpHead]
        public async Task<IActionResult> GetBusinessLeads([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<BusinessLead, Data.Entities.BusinessLead>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<BusinessLead>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var businessLeads = await _demoCrmRepository.GetBusinessLeadsAsync(crmResourceParameters);

            var businessLeadModels = BusinessLeadProfile.GetBusinessLeadModelsFromEntities(businessLeads);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = businessLeads.TotalCount,
                    pageSize = businessLeads.PageSize,
                    currentPage = businessLeads.CurrentPage,
                    totalPages = businessLeads.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    businessLeads.HasNext, businessLeads.HasPrevious, ControllerNamePlural);

                var shapedBusinessLeads = businessLeadModels.ShapeData(crmResourceParameters.Fields);


                var shapedBusinessLeadsWithLinks = businessLeadModels.Select(position =>
                {
                    var businessLeadsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)businessLeadsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    businessLeadsAsDictionary.Add("links", positionLinks);

                    return businessLeadsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedBusinessLeadsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = businessLeads.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = businessLeads.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = businessLeads.TotalCount,
                    pageSize = businessLeads.PageSize,
                    currentPage = businessLeads.CurrentPage,
                    totalPages = businessLeads.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(businessLeadModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetBusinessLead")]
        public async Task<IActionResult> GetBusinessLead(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<BusinessLead>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var businessLeadEntity = await _demoCrmRepository.GetBusinessLeadAsync(id);

            // If the user is not found return 404.
            if (businessLeadEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndBusinessLeadModel = BusinessLeadProfile.GetBusinessLeadModelFromEntity(businessLeadEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndBusinessLeadModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndBusinessLeadModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var businessLeadModel = BusinessLeadProfile.GetBusinessLeadModelFromEntity(businessLeadEntity);

            return Ok(businessLeadModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateBusinessLead")]
        public async Task<IActionResult> AddBusinessLead([FromBody] BusinessLeadCreate businessLeadCreate)
        {
            if (businessLeadCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var businessLeadEntity = BusinessLeadProfile.GetBusinessLeadEntityFromCreateModel(businessLeadCreate);
            _demoCrmRepository.AddBusinessLead(businessLeadEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a business lead failed on save");

            var businessLeadModel = BusinessLeadProfile.GetBusinessLeadModelFromEntity(businessLeadEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, businessLeadModel.Id, null, ControllerName);
            var linkedResource = businessLeadEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetbusinessLead", new { id = businessLeadEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBusinessLead")]
        public async Task<IActionResult> PartiallyUpdateBusinessLead(Guid id,
            [FromBody] JsonPatchDocument<BusinessLeadUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var businessLead = await _demoCrmRepository.GetBusinessLeadAsync(id);
            if (businessLead == null)
                return NotFound();

            var businessLeadToUpdate = BusinessLeadProfile.GetBusinessLeadUpdateModelFromEntity(businessLead);

            patchDoc.ApplyTo(businessLeadToUpdate);

            TryValidateModel(businessLeadToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            BusinessLeadProfile.MapBusinessLeadUpdateModelToEntity(businessLeadToUpdate, businessLead);

            _demoCrmRepository.UpdateBusinessLead(businessLead);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching business lead" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateBusinessLead")]
        public async Task<IActionResult> FullyUpdateBusinessLead(Guid id, [FromBody] BusinessLeadUpdate businessLeadUpdate)
        {
            if (businessLeadUpdate == null)
                return BadRequest();

            var businessLeadEntity = await _demoCrmRepository.GetBusinessLeadAsync(id);
            if (businessLeadEntity == null)
                return NotFound();

            TryValidateModel(businessLeadUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            BusinessLeadProfile.MapBusinessLeadUpdateModelToEntity(businessLeadUpdate, businessLeadEntity);

            // apply update
            _demoCrmRepository.UpdateBusinessLead(businessLeadEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating business lead with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteBusinessLead")]
        public async Task<IActionResult> DeleteBusinessLead(Guid id)
        {
            var businessLeadToDelete = await _demoCrmRepository.GetBusinessLeadAsync(id);
            if (businessLeadToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteBusinessLead(businessLeadToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting a business lead with Id {id} failed on save.");

            _logger.LogInformation(100, $"A business with Id {id} has been deleted.");

            return NoContent();
        }
    }
}