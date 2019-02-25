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
    [Route("api/staffmembers")]
    [ApiController]
    public class StaffMembersController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "StaffMember";
        private const string ControllerNamePlural = "StaffMembers";

        public StaffMembersController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetStaffMembers")]
        [HttpHead]
        public async Task<IActionResult> GetStaffMembers([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<StaffMember, Data.Entities.StaffMember>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<StaffMember>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var staffMembers = await _demoCrmRepository.GetStaffMembersAsync(crmResourceParameters);

            var staffMemberModels = StaffMemberProfile.GetStaffMemberModelsFromEntities(staffMembers);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = staffMembers.TotalCount,
                    pageSize = staffMembers.PageSize,
                    currentPage = staffMembers.CurrentPage,
                    totalPages = staffMembers.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    staffMembers.HasNext, staffMembers.HasPrevious, ControllerNamePlural);

                var shapedStaffMembers = staffMemberModels.ShapeData(crmResourceParameters.Fields);


                var shapedStaffMembersWithLinks = staffMemberModels.Select(position =>
                {
                    var staffMembersAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)staffMembersAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    staffMembersAsDictionary.Add("links", positionLinks);

                    return staffMembersAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedStaffMembersWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = staffMembers.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = staffMembers.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = staffMembers.TotalCount,
                    pageSize = staffMembers.PageSize,
                    currentPage = staffMembers.CurrentPage,
                    totalPages = staffMembers.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(staffMemberModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetStaffMember")]
        public async Task<IActionResult> GetStaffMember(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<StaffMember>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var staffMemberEntity = await _demoCrmRepository.GetStaffMemberAsync(id);

            // If the user is not found return 404.
            if (staffMemberEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndStaffMemberModel = StaffMemberProfile.GetStaffMemberModelFromEntity(staffMemberEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndStaffMemberModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndStaffMemberModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var staffMemberModel = StaffMemberProfile.GetStaffMemberModelFromEntity(staffMemberEntity);

            return Ok(staffMemberModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateStaffMember")]
        public async Task<IActionResult> AddStaffMember([FromBody] StaffMemberCreate staffMemberCreate)
        {
            if (staffMemberCreate == null)
                return BadRequest();

            if (await _demoCrmRepository.StaffMemberEmailExitsAsync(staffMemberCreate.Email))
            {
                ModelState.AddModelError(nameof(staffMemberCreate.Email),
                    $"A Staff member with the email '{staffMemberCreate.Email}' already exists.");
                return Conflict(ModelState);
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var staffMemberEntity = StaffMemberProfile.GetStaffMemberEntityFromCreateModel(staffMemberCreate);
            _demoCrmRepository.AddStaffMember(staffMemberEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a staff member failed on save");

            var staffMemberModel = StaffMemberProfile.GetStaffMemberModelFromEntity(staffMemberEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, staffMemberModel.Id, null, ControllerName);
            var linkedResource = staffMemberEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetStaffMember", new { id = staffMemberEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateStaffMember")]
        public async Task<IActionResult> PartiallyUpdateStaffMember(Guid id,
            [FromBody] JsonPatchDocument<StaffMemberUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var staffMember = await _demoCrmRepository.GetStaffMemberAsync(id);
            if (staffMember == null)
                return NotFound();

            var staffMemberToUpdate = StaffMemberProfile.GetStaffMemberUpdateModelFromEntity(staffMember);

            patchDoc.ApplyTo(staffMemberToUpdate);

            TryValidateModel(staffMemberToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            StaffMemberProfile.MapStaffMemberUpdateModelToEntity(staffMemberToUpdate, staffMember);

            _demoCrmRepository.UpdateStaffMember(staffMember);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching staff member with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateStaffMember")]
        public async Task<IActionResult> FullyUpdateStaffMember(Guid id, [FromBody] StaffMemberUpdate staffMemberUpdate)
        {
            if (staffMemberUpdate == null)
                return BadRequest();

            var staffMemberEntity = await _demoCrmRepository.GetStaffMemberAsync(id);
            if (staffMemberEntity == null)
                return NotFound();

            TryValidateModel(staffMemberUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            StaffMemberProfile.MapStaffMemberUpdateModelToEntity(staffMemberUpdate, staffMemberEntity);

            // apply update
            _demoCrmRepository.UpdateStaffMember(staffMemberEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating staff member with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteStaffMember")]
        public async Task<IActionResult> DeleteStaffMember(Guid id)
        {
            var staffMemberToDelete = await _demoCrmRepository.GetStaffMemberAsync(id);
            if (staffMemberToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteStaffMember(staffMemberToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting staff member with Id {id} failed on save.");

            _logger.LogInformation(100, $"A Staff member with Id {id} has been deleted.");

            return NoContent();
        }
    }
}