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
    [Route("api/customeraccounts")]
    [ApiController]
    public class CustomerAccountsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "BusinessLead";
        private const string ControllerNamePlural = "BusinessLeads";

        public CustomerAccountsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetCustomerAccounts")]
        [HttpHead]
        public async Task<IActionResult> GetCustomerAccounts([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<CustomerAccount, Data.Entities.BusinessLead>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<CustomerAccount>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var customerAccountS = await _demoCrmRepository.GetCustomerAccountsAsync(crmResourceParameters);

            var customerAccountModels = CustomerAccountProfile.GetCustomerAccountModelsFromEntities(customerAccountS);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = customerAccountS.TotalCount,
                    pageSize = customerAccountS.PageSize,
                    currentPage = customerAccountS.CurrentPage,
                    totalPages = customerAccountS.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    customerAccountS.HasNext, customerAccountS.HasPrevious, ControllerNamePlural);

                var shapedBusinessLeads = customerAccountS.ShapeData(crmResourceParameters.Fields);


                var shapedCustomerAccountsWithLinks = customerAccountS.Select(position =>
                {
                    var customerAccountsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)customerAccountsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    customerAccountsAsDictionary.Add("links", positionLinks);

                    return customerAccountsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedCustomerAccountsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = customerAccountS.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = customerAccountS.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = customerAccountS.TotalCount,
                    pageSize = customerAccountS.PageSize,
                    currentPage = customerAccountS.CurrentPage,
                    totalPages = customerAccountS.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(customerAccountModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetCustomerAccount")]
        public async Task<IActionResult> GetCustomerAccount(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CustomerAccount>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var customerAccountEntity = await _demoCrmRepository.GetCustomerAccountAsync(id);

            // If the user is not found return 404.
            if (customerAccountEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndCustomerAccountModel = CustomerAccountProfile.GetCustomerAccountModelFromEntity(customerAccountEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndCustomerAccountModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndCustomerAccountModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var customerAccountModel = CustomerAccountProfile.GetCustomerAccountModelFromEntity(customerAccountEntity);

            return Ok(customerAccountModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateCustomerAccount")]
        public async Task<IActionResult> AddBusinessLead([FromBody] CustomerAccountCreate customerAccountCreate)
        {
            if (customerAccountCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var customerAccountEntity = CustomerAccountProfile.GetCustomerAccountEntityFromCreateModel(customerAccountCreate);
            _demoCrmRepository.AddCustomerAccount(customerAccountEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a business lead failed on save");

            var businessLeadModel = CustomerAccountProfile.GetCustomerAccountModelFromEntity(customerAccountEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, businessLeadModel.Id, null, ControllerName);
            var linkedResource = customerAccountEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCustomerAccount", new { id = customerAccountEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCustomerAccount")]
        public async Task<IActionResult> PartiallyUpdateCustomerAccount(Guid id,
            [FromBody] JsonPatchDocument<CustomerAccountUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var customerAccount = await _demoCrmRepository.GetCustomerAccountAsync(id);
            if (customerAccount == null)
                return NotFound();

            var customerAccountToUpdate = CustomerAccountProfile.GetCustomerAccountUpdateModelFromEntity(customerAccount);

            patchDoc.ApplyTo(customerAccountToUpdate);

            TryValidateModel(customerAccountToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            CustomerAccountProfile.MapCustomerAccountUpdateModelToEntity(customerAccountToUpdate, customerAccount);

            _demoCrmRepository.UpdateCustomerAccount(customerAccount);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching customer account" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateCustomerAccount")]
        public async Task<IActionResult> FullyUpdateCustomerAccount(Guid id, [FromBody] CustomerAccountUpdate customerAccountUpdate)
        {
            if (customerAccountUpdate == null)
                return BadRequest();

            var customerAccountEntity = await _demoCrmRepository.GetCustomerAccountAsync(id);
            if (customerAccountEntity == null)
                return NotFound();

            TryValidateModel(customerAccountUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            CustomerAccountProfile.MapCustomerAccountUpdateModelToEntity(customerAccountUpdate, customerAccountEntity);

            // apply update
            _demoCrmRepository.UpdateCustomerAccount(customerAccountEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating customer account with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCustomerAccount")]
        public async Task<IActionResult> DeleteCustomerAccount(Guid id)
        {
            var customerToDelete = await _demoCrmRepository.GetCustomerAccountAsync(id);
            if (customerToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteCustomerAccount(customerToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting a customer account with Id {id} failed on save.");

            _logger.LogInformation(100, $"A customer account with Id {id} has been deleted.");

            return NoContent();
        }
    }
}