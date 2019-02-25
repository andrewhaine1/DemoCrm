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
    [Route("api/staffpositions")]
    [ApiController]
    public class StaffPositionsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "StaffPosition";
        private const string ControllerNamePlural = "StaffPositions";

        public StaffPositionsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetStaffPositions")]
        [HttpHead]
        public async Task<IActionResult> GetStaffPositions([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<StaffPosition, Data.Entities.StaffPosition>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<StaffPosition>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var staffPositions = await _demoCrmRepository.GetStaffPositionsAsync(crmResourceParameters);

            var staffPositionModels = StaffPositionProfile.GetStaffPositionModelsFromEntities(staffPositions);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = staffPositions.TotalCount,
                    pageSize = staffPositions.PageSize,
                    currentPage = staffPositions.CurrentPage,
                    totalPages = staffPositions.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    staffPositions.HasNext, staffPositions.HasPrevious, ControllerNamePlural);

                var shapedStaffPositions = staffPositionModels.ShapeData(crmResourceParameters.Fields);


                var shapedStaffPositionsWithLinks = shapedStaffPositions.Select(position =>
                {
                    var staffPositionsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)staffPositionsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    staffPositionsAsDictionary.Add("links", positionLinks);

                    return staffPositionsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedStaffPositionsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = staffPositions.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = staffPositions.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = staffPositions.TotalCount,
                    pageSize = staffPositions.PageSize,
                    currentPage = staffPositions.CurrentPage,
                    totalPages = staffPositions.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(staffPositionModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetStaffPosition")]
        public async Task<IActionResult> GetStaffPosition(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<StaffPosition>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var staffPositionEntity = await _demoCrmRepository.GetStaffPositionAsync(id);

            // If the user is not found return 404.
            if (staffPositionEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndStaffPositionModel = StaffPositionProfile.GetStaffPositionModelFromEntity(staffPositionEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndStaffPositionModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndStaffPositionModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var staffPositionModel = StaffPositionProfile.GetStaffPositionModelFromEntity(staffPositionEntity);

            return Ok(staffPositionModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateStaffPosition")]
        public async Task<IActionResult> AddStaffPosition([FromBody] StaffPositionCreate staffPositionCreate)
        {
            if (staffPositionCreate == null)
                return BadRequest();

            if (await _demoCrmRepository.StaffPositionNameExitsAsync(staffPositionCreate.Name))
            {
                ModelState.AddModelError(nameof(staffPositionCreate.Name),
                    $"A Staff position with the name '{staffPositionCreate.Name}' already exists.");
                return Conflict(ModelState);
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var staffPositionEntity = StaffPositionProfile.GetStaffPositionEntityFromCreateModel(staffPositionCreate);
            _demoCrmRepository.AddStaffPosition(staffPositionEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a staff position failed on save");

            var staffPositionModel = StaffPositionProfile.GetStaffPositionModelFromEntity(staffPositionEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, staffPositionModel.Id, null, ControllerName);
            var linkedResource = staffPositionEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetDepartment", new { id = staffPositionEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateStaffPosition")]
        public async Task<IActionResult> PartiallyUpdateStaffPosition(Guid id,
            [FromBody] JsonPatchDocument<StaffPositionUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var staffPosition = await _demoCrmRepository.GetStaffPositionAsync(id);
            if (staffPosition == null)
                return NotFound();

            var staffPositionToUpdate = StaffPositionProfile.GetStaffPositionUpdateModelFromEntity(staffPosition);

            patchDoc.ApplyTo(staffPositionToUpdate);

            TryValidateModel(staffPositionToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            StaffPositionProfile.MapStaffPositionUpdateModelToEntity(staffPositionToUpdate, staffPosition);

            _demoCrmRepository.UpdateStaffPosition(staffPosition);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching company with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateStaffPosition")]
        public async Task<IActionResult> FullyUpdateStaffPosition(Guid id, [FromBody] StaffPositionUpdate staffPositionUpdate)
        {
            if (staffPositionUpdate == null)
                return BadRequest();

            var staffPositionEntity = await _demoCrmRepository.GetStaffPositionAsync(id);
            if (staffPositionEntity == null)
                return NotFound();

            TryValidateModel(staffPositionUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            StaffPositionProfile.MapStaffPositionUpdateModelToEntity(staffPositionUpdate, staffPositionEntity);

            // apply update
            _demoCrmRepository.UpdateStaffPosition(staffPositionEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating department with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteStaffPosition")]
        public async Task<IActionResult> DeleteStaffPosition(Guid id)
        {
            var staffPositionToDelete = await _demoCrmRepository.GetStaffPositionAsync(id);
            if (staffPositionToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteStaffPosition(staffPositionToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting staff position with Id {id} failed on save.");

            _logger.LogInformation(100, $"A Staff position with Id {id} has been deleted.");

            return NoContent();
        }
    }
}