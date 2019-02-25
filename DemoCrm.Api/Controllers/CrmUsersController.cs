using DemoCrm.Api.Filters;
using DemoCrm.Data.Helpers;
using DemoCrm.Data.Models;
using DemoCrm.Data.Profiles;
using DemoCrm.Data.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Api.Controllers
{
    [Route("api/crmusers")]
    [ApiController]
    public class CrmUsersController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<CrmUsersController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        const int MaxUserPageSize = 10;

        public CrmUsersController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetCrmUsers")]
        [HttpHead]
        //[CrmUsersResultFilter]
        public async Task<IActionResult> GetCrmUsers([FromQuery] CrmResourceParameters crmUsersResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<CrmUser, Data.Entities.CrmUser>
                (crmUsersResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<CrmUser>(crmUsersResourceParameters.Fields))
                return BadRequest();

            // Get all CrmUsers.
            var crmUserEntities = await _demoCrmRepository.GetCrmUsersAsync(crmUsersResourceParameters);
           
            var crmUserModels = CrmUserProfile.GetCrmUserModelsFromEntities(crmUserEntities);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = crmUserEntities.TotalCount,
                    pageSize = crmUserEntities.PageSize,
                    currentPage = crmUserEntities.CurrentPage,
                    totalPages = crmUserEntities.TotalPages,
                };


                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                var links = CreateCrmLinksExpandoList(crmUsersResourceParameters,
                    crmUserEntities.HasNext, crmUserEntities.HasPrevious);

                var shapedCrmUsers = crmUserModels.ShapeData(crmUsersResourceParameters.Fields);


                var shapedCrmUsersWithLinks = shapedCrmUsers.Select(crmUser =>
                {
                    var crmUsersAsDictionary = crmUser as IDictionary<string, object>;
                    var crmUserLinks = CreateCrmLinksExpando(
                        (Guid)crmUsersAsDictionary["Id"], crmUsersResourceParameters.Fields);

                    crmUsersAsDictionary.Add("links", crmUserLinks);

                    return crmUsersAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedCrmUsersWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = crmUserEntities.HasPrevious ?
                CreateCrmUsersRecourceUri(crmUsersResourceParameters,
                ResourceUriType.PreviousPage) : null;

                var nextPageLink = crmUserEntities.HasNext ?
                    CreateCrmUsersRecourceUri(crmUsersResourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = crmUserEntities.TotalCount,
                    pageSize = crmUserEntities.PageSize,
                    currentPage = crmUserEntities.CurrentPage,
                    totalPages = crmUserEntities.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(crmUserModels.ShapeData(crmUsersResourceParameters.Fields));
            }
        }
       
        [HttpGet]
        //[CrmUserResultFilter]
        [Route("{id}", Name = "GetCrmUser")]
        public async Task<IActionResult> GetCrmUser(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<CrmUser>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var crmUserEntity = await _demoCrmRepository.GetCrmUserAsync(id);

            // If the user is not found return 404.
            if (crmUserEntity == null)
                return NotFound();

            var crmUserModel = CrmUserProfile.GetCrmUserModelFromEntity(crmUserEntity);

            var links = CreateCrmLinksExpando(id, fields);
            var linkedResource = crmUserModel.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            // If found return 200 with CrmUser object.
            return Ok(linkedResource);
        }

        [HttpPost(Name = "CreateCrmUser")]
        public async Task<IActionResult> CreateCrmUser([FromBody] CrmUserCreate crmUser)
        {
            if (crmUser == null)
                return BadRequest();

            // Check if Guid is null (set to all zero's). If so, set model state to invalid.
            if (crmUser.OauthId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ModelState.AddModelError(nameof(crmUser),
                    "A valid Oauth Id is required.");
            }

            if (crmUser.FirstName == crmUser.LastName)
            {
                ModelState.AddModelError(nameof(crmUser),
                    "A user's name cannot be the same as their surname.");
            }

            /* WARNING!!! - foreign key values need to be checked before doing save as non existant 
             * foreign key cause errror 500. */

            if (!await _demoCrmRepository.ObjectTypeExistsAsync(crmUser.ObjectTypeId))
            {
                ModelState.AddModelError(nameof(crmUser.ObjectTypeId),
                    $"Foreign key error: CrmObjectType with Id '{crmUser.ObjectTypeId}' does not exist.");
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            if (await _demoCrmRepository.CrmUserExitsAsync(crmUser.OauthId))
                return Conflict();

            var crmUserEntity = CrmUserProfile.GetCrmUserEntityFromCreateModel(crmUser);
            _demoCrmRepository.AddCrmUser(crmUserEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a CrmUser failed on save");

            var crmUserModel = CrmUserProfile.GetCrmUserModelFromEntity(crmUserEntity);

            var links = CreateCrmLinksExpando(crmUserModel.Id, null);
            var linkedResource = crmUserModel.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            // return 201 created with the GetCrmUser, Location header and user id
            return CreatedAtRoute("GetCrmUser", new { id = linkedResource["Id"] }, linkedResource);
        }

        [HttpPut("{id}", Name = "UpdateCrmUser")]
        public async Task<IActionResult> UpdateCrmUser(Guid id, [FromBody] CrmUserUpdate crmUserUpdate)
        {
            if (crmUserUpdate == null)
                return BadRequest();

            var crmUserEntity = await _demoCrmRepository.GetCrmUserAsync(id);
            if (crmUserEntity == null)      // If user does not exist, upsert it.
            {
                crmUserEntity = CrmUserProfile.GetCrmUserEntityFromUpdateModel(crmUserUpdate);

                crmUserEntity.Id = id;
                crmUserEntity.OauthId = Guid.NewGuid();
                crmUserEntity.FirstName = crmUserUpdate.FirstName;
                crmUserEntity.LastName = crmUserUpdate.LastName;
                crmUserEntity.Email = crmUserUpdate.Email;
                crmUserEntity.PhoneNumber = crmUserUpdate.Phone;

                _demoCrmRepository.AddCrmUser(crmUserEntity);

                if (!await _demoCrmRepository.SaveChangesAsync())
                    throw new Exception($"Upserting CrmUser with Id {id} failed on save.");

                var createdCrmUser = CrmUserProfile.GetCrmUserModelFromEntity(crmUserEntity);

                return CreatedAtRoute("GetCrmUser", new { id }, createdCrmUser);
            }

            // Map
            crmUserEntity.FirstName = crmUserUpdate.FirstName;
            crmUserEntity.LastName = crmUserUpdate.LastName;
            crmUserEntity.Email = crmUserUpdate.Email;
            crmUserEntity.PhoneNumber = crmUserUpdate.Phone;

            // Apply update
            _demoCrmRepository.UpdateCrmUser(crmUserEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating CrmUser with Id {id} failed on save.");

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartialyUpdateCrmUser")]
        public async Task<IActionResult> PartialUpdateCrmUser(Guid id,
            [FromBody] JsonPatchDocument<CrmUserUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            if (!await _demoCrmRepository.CrmUserIdExits(id))
                return NotFound();

            var crmUser = await _demoCrmRepository.GetCrmUserAsync(id);
            if (crmUser == null)
                return NotFound();

            var crmUserToUpdate = CrmUserProfile.GetCrmUserUpdateModelFromEntity(crmUser);

            patchDoc.ApplyTo(crmUserToUpdate);

            //if (crmUserToUpdate.FirstName == null) ModelState.AddModelError(nameof(CrmUserUpdate), "A First Name is required.");
            //if (crmUserToUpdate.LastName == null) ModelState.AddModelError(nameof(CrmUserUpdate), "A Last Name is required.");
            //if (crmUserToUpdate.Email == null) ModelState.AddModelError(nameof(CrmUserUpdate), "An Email address is required.");
            //if (crmUserToUpdate.Phone == null ) ModelState.AddModelError(nameof(CrmUserUpdate), "A Phone numer is required.");

            TryValidateModel(crmUserToUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            crmUser.FirstName = crmUserToUpdate.FirstName;
            crmUser.LastName = crmUserToUpdate.LastName;
            crmUser.Email = crmUserToUpdate.Email;
            crmUser.PhoneNumber = crmUserToUpdate.Phone;
            //crmUser.ObjectTypeId = crmUserToUpdate.ObjectTypeId;

            _demoCrmRepository.UpdateCrmUser(crmUser);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching CrmUser with id {id} failed on save");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCrmUser")]
        public async Task<IActionResult> DeleteCrmUser(Guid id)
        {
            if (!await _demoCrmRepository.CrmUserIdExits(id))
                return NotFound();

            var crmUser = await _demoCrmRepository.GetCrmUserAsync(id);
            if (crmUser == null)
                return NotFound();

            _demoCrmRepository.DeleteCrmUser(crmUser);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting CrmUser with Id {id} failed on save");

            _logger.LogInformation(100, $"A CRM User with Id {id} has been deleted.");

            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockCrmUserCreation(Guid id)
        {
            if (await _demoCrmRepository.CrmUserIdExits(id))
                return new StatusCodeResult(StatusCodes.Status409Conflict);

            return NotFound();
        }

        private string CreateCrmUsersRecourceUri(CrmResourceParameters crmUsersResourceParameters,
           ResourceUriType resourceUriType)
        {
            switch (resourceUriType)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCrmUsers",
                      new
                      {
                          fields = crmUsersResourceParameters.Fields,
                          orderBy = crmUsersResourceParameters.OrderBy,
                          searchQuery = crmUsersResourceParameters.SearchQuery,
                          sampleUser = crmUsersResourceParameters.Type,
                          pageNumber = crmUsersResourceParameters.PageNumber - 1,
                          pageSize = crmUsersResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCrmUsers",
                      new
                      {
                          fields = crmUsersResourceParameters.Fields,
                          orderBy = crmUsersResourceParameters.OrderBy,
                          searchQuery = crmUsersResourceParameters.SearchQuery,
                          sampleUser = crmUsersResourceParameters.Type,
                          pageNumber = crmUsersResourceParameters.PageNumber + 1,
                          pageSize = crmUsersResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetCrmUsers",
                    new
                    {
                        fields = crmUsersResourceParameters.Fields,
                        orderBy = crmUsersResourceParameters.OrderBy,
                        searchQuery = crmUsersResourceParameters.SearchQuery,
                        sampleUser = crmUsersResourceParameters.Type,
                        pageNumber = crmUsersResourceParameters.PageNumber,
                        pageSize = crmUsersResourceParameters.PageSize
                    });
            }
        }

        //private CrmUser CreateCrmUserLinks(CrmUser crmUser)
        //{
        //    crmUser.Links.Add(
        //        new HypermediaLink(_urlHelper.Link("GetCrmUser",
        //        new { crmUser.Id }),
        //        "self",
        //        "GET"));
        //    //crmUser.Links.Add(
        //    //    new HypermediaLink(_urlHelper.Link("CreateCrmUser",
        //    //    new { crmUser.Id }),
        //    //    "create_crm_user",
        //    //    "POST"));
        //    crmUser.Links.Add(
        //        new HypermediaLink(_urlHelper.Link("PartialyUpdateCrmUser",
        //        new { crmUser.Id }),
        //        "update_partial_crm_user",
        //        "PATCH"));
        //    crmUser.Links.Add(
        //        new HypermediaLink(_urlHelper.Link("UpdateCrmUser",
        //        new { crmUser.Id }),
        //        "update_full_crm_user",
        //        "PUT"));
        //    crmUser.Links.Add(
        //        new HypermediaLink(_urlHelper.Link("DeleteCrmUser",
        //        new { crmUser.Id }),
        //        "delete_crm_user",
        //        "DELETE"));
        //    return crmUser;
        //}

        //private LinkedCollectionResourceWrapper<CrmUser> CreateCrmUserListLinks(
        //    LinkedCollectionResourceWrapper<CrmUser> crmUserWrapper)
        //{
        //    crmUserWrapper.Links.Add(
        //        new HypermediaLink(_urlHelper.Link("GetCrmUsers", new { }),
        //        "self",
        //        "GET"));
        //    return crmUserWrapper;
        //}

        // Creates links with Data shaping capabilities for one CrmUser
        private IEnumerable<HypermediaLink> CreateCrmLinksExpando(Guid id, string fields)
        {
            var links = new List<HypermediaLink>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new HypermediaLink(
                    _urlHelper.Link("GetCrmUser", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new HypermediaLink(
                    _urlHelper.Link("GetCrmUser", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new HypermediaLink(
                _urlHelper.Link("DeleteCrmUser", new { id, fields }),
                "delete-crm-user",
                "DELETE"));

            links.Add(new HypermediaLink(_urlHelper.Link("CreateCrmUser",
                new { id }),
                "create-crm-user",
                "POST"));

            links.Add(new HypermediaLink(_urlHelper.Link("PartialyUpdateCrmUser",
                new { id }),
                "partially-update-crm-user",
                "PATCH"));

            links.Add(new HypermediaLink(_urlHelper.Link("UpdateCrmUser",
                new { id }),
                "fully-update-crm-useruser",
                "PUT"));

            return links;
        }

        // Creates links with Data shaping capabilities for a list of CrmUsers
        private IEnumerable<HypermediaLink> CreateCrmLinksExpandoList(
            CrmResourceParameters crmUsersResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<HypermediaLink>();

            links.Add(
                new HypermediaLink(CreateCrmUsersRecourceUri(crmUsersResourceParameters,
                ResourceUriType.Current),
                "self", "GET"));

            if (hasNext)
                links.Add(
                new HypermediaLink(CreateCrmUsersRecourceUri(crmUsersResourceParameters,
                ResourceUriType.NextPage),
                "nextPage", "GET"));

            if (hasPrevious)
                links.Add(
                new HypermediaLink(CreateCrmUsersRecourceUri(crmUsersResourceParameters,
                ResourceUriType.PreviousPage),
                "previousPage", "GET"));

            return links;
        }

        [HttpOptions]
        public IActionResult GetCrmUserOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
