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
    //[Route("api/[controller]")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<CrmUsersController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "Company";
        private const string ControllerNamePlural = "Companies";

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
            if (!_propertyMappingService.ValidMappingExistsFor<Company, Data.Entities.Company>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<Company>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var companies = await _demoCrmRepository.GetCompaniesAsync(crmResourceParameters);

            var companyModels = CompanyProfile.GetCompanyModelsFromEntities(companies);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = companies.TotalCount,
                    pageSize = companies.PageSize,
                    currentPage = companies.CurrentPage,
                    totalPages = companies.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    companies.HasNext, companies.HasPrevious, ControllerNamePlural);

                var shapedCompanies = companyModels.ShapeData(crmResourceParameters.Fields);


                var shapedCrmUsersWithLinks = shapedCompanies.Select(crmUser =>
                {
                    var crmUsersAsDictionary = crmUser as IDictionary<string, object>;
                    var crmUserLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)crmUsersAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

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
                var prevousPageLink = companies.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = companies.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = companies.TotalCount,
                    pageSize = companies.PageSize,
                    currentPage = companies.CurrentPage,
                    totalPages = companies.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(companyModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetCompany")]
        public async Task<IActionResult> GetCompany(Guid id, [FromQuery] string fields, 
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<Company>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var companyEntity = await _demoCrmRepository.GetCompanyAsync(id);

            // If the user is not found return 404.
            if (companyEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndcompanyModel = CompanyProfile.GetCompanyModelFromEntity(companyEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, "Company");
                var vndLinkedResource = vndcompanyModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndcompanyModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var companyModel = CompanyProfile.GetCompanyModelFromEntity(companyEntity);

            return Ok(companyModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateCompany")]
        public async Task<IActionResult> AddCompany([FromBody] CompanyCreate company)
        {
            if (company == null)
                return BadRequest();

            if (await _demoCrmRepository.CompanyNameExistsAsync(company.Name))
            {
                ModelState.AddModelError(nameof(company),
                    $"A company with the name {company.Name} already exists.");
                return Conflict(ModelState);
            }

            if (!await _demoCrmRepository.ObjectTypeExistsAsync(company.ObjectTypeId))
            {
                ModelState.AddModelError(nameof(company.ObjectTypeId),
                    $"Foreign key error: CrmObjectType with Id '{company.ObjectTypeId}' does not exist.");
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var companyEntity = CompanyProfile.GetCompanyEntityFromCreateModel(company);
            _demoCrmRepository.AddCompany(companyEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a company failed on save");

            var companyModel = CompanyProfile.GetCompanyModelFromEntity(companyEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, companyEntity.Id, null, ControllerName);
            var linkedResource = companyModel.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCompany", new { id = linkedResource["Id"] }, linkedResource);
        }    

        [HttpPatch("{id}", Name = "PartiallyUpdateCompany")]
        public async Task<IActionResult> PartiallyUpdateCompany(Guid id,
            [FromBody] JsonPatchDocument<CompanyUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var company = await _demoCrmRepository.GetCompanyAsync(id);
            if (company == null)
                return NotFound();

            var companyToUpdate = CompanyProfile.GetCompanyUpdateModelFromEntity(company);

            patchDoc.ApplyTo(companyToUpdate);

            TryValidateModel(companyToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            CompanyProfile.MapCompanyUpdateModelToEntity(companyToUpdate, company);

            _demoCrmRepository.UpdateCompany(company);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching company with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateCompany")]
        public async Task<IActionResult> FullyUpdateCompany(Guid id, [FromBody] CompanyUpdate companyUpdate)
        {
            if (companyUpdate == null)
                return BadRequest();

            var companyEntity = await _demoCrmRepository.GetCompanyAsync(id);
            if (companyUpdate == null)
                return NotFound();

            TryValidateModel(companyUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            CompanyProfile.MapCompanyUpdateModelToEntity(companyUpdate, companyEntity);

            // apply update
            _demoCrmRepository.UpdateCompany(companyEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating company with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCompany")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var companyToDelete = await _demoCrmRepository.GetCompanyAsync(id);
            if (companyToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteCompany(companyToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting Company with Id {id} failed on save.");

            _logger.LogInformation(100, $"A CRM User with Id {id} has been deleted.");

            return NoContent();
        }

        //private IEnumerable<HypermediaLink> CreateActionLinksExpando(Guid id, string fields)
        //{
        //    var links = new List<HypermediaLink>();

        //    if (string.IsNullOrWhiteSpace(fields))
        //    {
        //        links.Add(new HypermediaLink(
        //            _urlHelper.Link("GetCompany", new { id }),
        //            "self",
        //            "GET"));
        //    }
        //    else
        //    {
        //        links.Add(new HypermediaLink(
        //            _urlHelper.Link("GetCompany", new { id, fields }),
        //            "self",
        //            "GET"));
        //    }

        //    links.Add(new HypermediaLink(_urlHelper.Link("CreateCompany",
        //        null),
        //        "create-company",
        //        "POST"));

        //    links.Add(new HypermediaLink(_urlHelper.Link("PartialyUpdateCrmUser",
        //        new { id }),
        //        "partially-update-company",
        //        "PATCH"));

        //    links.Add(new HypermediaLink(_urlHelper.Link("UpdateCrmUser",
        //        new { id }),
        //        "fully-update-company",
        //        "PUT"));

        //    //links.Add(new HypermediaLink(
        //    //    _urlHelper.Link("DeleteCrmUser", new { id, fields }),
        //    //    "delete-crm-user",
        //    //    "DELETE"));

        //    return links;
        //}

        //private string CreateCrmUsersRecourceUri(CrmResourceParameters crmUsersResourceParameters,
        //   ResourceUriType resourceUriType)
        //{
        //    switch (resourceUriType)
        //    {
        //        case ResourceUriType.PreviousPage:
        //            return _urlHelper.Link("GetCrmUsers",
        //              new
        //              {
        //                  fields = crmUsersResourceParameters.Fields,
        //                  orderBy = crmUsersResourceParameters.OrderBy,
        //                  searchQuery = crmUsersResourceParameters.SearchQuery,
        //                  sampleUser = crmUsersResourceParameters.Type,
        //                  pageNumber = crmUsersResourceParameters.PageNumber - 1,
        //                  pageSize = crmUsersResourceParameters.PageSize
        //              });
        //        case ResourceUriType.NextPage:
        //            return _urlHelper.Link("GetCrmUsers",
        //              new
        //              {
        //                  fields = crmUsersResourceParameters.Fields,
        //                  orderBy = crmUsersResourceParameters.OrderBy,
        //                  searchQuery = crmUsersResourceParameters.SearchQuery,
        //                  sampleUser = crmUsersResourceParameters.Type,
        //                  pageNumber = crmUsersResourceParameters.PageNumber + 1,
        //                  pageSize = crmUsersResourceParameters.PageSize
        //              });
        //        case ResourceUriType.Current:
        //        default:
        //            return _urlHelper.Link("GetCrmUsers",
        //            new
        //            {
        //                fields = crmUsersResourceParameters.Fields,
        //                orderBy = crmUsersResourceParameters.OrderBy,
        //                searchQuery = crmUsersResourceParameters.SearchQuery,
        //                sampleUser = crmUsersResourceParameters.Type,
        //                pageNumber = crmUsersResourceParameters.PageNumber,
        //                pageSize = crmUsersResourceParameters.PageSize
        //            });
        //    }
        //}
    }
}