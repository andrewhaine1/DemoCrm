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
    [Route("api/customercontacts")]
    [ApiController]
    public class CustomerContactsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "CustomerContact";
        private const string ControllerNamePlural = "CustomerContacts";

        public CustomerContactsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetCustomerContacts")]
        [HttpHead]
        public async Task<IActionResult> GetCustomerContacts([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<CustomerContact, Data.Entities.CustomerContact>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<CustomerContact>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var customerContacts = await _demoCrmRepository.GetCustomerContactsAsync(crmResourceParameters);

            var customerContactModels = CustomerContactProfile.GetCustomerContactModelsFromEntities(customerContacts);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = customerContacts.TotalCount,
                    pageSize = customerContacts.PageSize,
                    currentPage = customerContacts.CurrentPage,
                    totalPages = customerContacts.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    customerContacts.HasNext, customerContacts.HasPrevious, ControllerNamePlural);

                var shapedCustomerContacts = customerContacts.ShapeData(crmResourceParameters.Fields);


                var shapedCustomerContactsWithLinks = customerContacts.Select(position =>
                {
                    var customerContactsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)customerContactsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    customerContactsAsDictionary.Add("links", positionLinks);

                    return customerContactsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedCustomerContactsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = customerContacts.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = customerContacts.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = customerContacts.TotalCount,
                    pageSize = customerContacts.PageSize,
                    currentPage = customerContacts.CurrentPage,
                    totalPages = customerContacts.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(customerContactModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetCustomerContact")]
        public async Task<IActionResult> GetCustomerContact(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CustomerContact>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var customerContactEntity = await _demoCrmRepository.GetCustomerContactAsync(id);

            // If the user is not found return 404.
            if (customerContactEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndcustomerContactModel = CustomerContactProfile.GetCustomerContactModelFromEntity(customerContactEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndcustomerContactModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndcustomerContactModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var customerContactModel = CustomerContactProfile.GetCustomerContactModelFromEntity(customerContactEntity);
            return Ok(customerContactModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateCustomerContact")]
        public async Task<IActionResult> AddCustomerContact([FromBody] CustomerContactCreate customerContactCreate)
        {
            if (customerContactCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var customerContactEntity = CustomerContactProfile.GetCustomerContactEntityFromCreateModel(customerContactCreate);
            _demoCrmRepository.AddCustomerContact(customerContactEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a customer account failed on save");

            var CustomerContactModel = CustomerContactProfile.GetCustomerContactModelFromEntity(customerContactEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, CustomerContactModel.Id, null, ControllerName);
            var linkedResource = customerContactEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCustomerAccount", new { id = customerContactEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCustomerContact")]
        public async Task<IActionResult> PartiallyUpdateCustomerAccount(Guid id,
            [FromBody] JsonPatchDocument<CustomerContactUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var customerContact = await _demoCrmRepository.GetCustomerContactAsync(id);
            if (customerContact == null)
                return NotFound();

            var customerContactToUpdate = CustomerContactProfile.GetCustomerContactUpdateModelFromEntity(customerContact);

            patchDoc.ApplyTo(customerContactToUpdate);

            TryValidateModel(customerContactToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            CustomerContactProfile.MapCustomerContactModelToEntity(customerContactToUpdate, customerContact);

            _demoCrmRepository.UpdateCustomerContact(customerContact);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching customer contact" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateCustomerContact")]
        public async Task<IActionResult> FullyUpdateCustomerAccount(Guid id, 
            [FromBody] CustomerContactUpdate customerContactUpdate)
        {
            if (customerContactUpdate == null)
                return BadRequest();

            var customerContactEntity = await _demoCrmRepository.GetCustomerContactAsync(id);
            if (customerContactEntity == null)
                return NotFound();

            TryValidateModel(customerContactEntity);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            CustomerContactProfile.MapCustomerContactModelToEntity(customerContactUpdate, customerContactEntity);

            // apply update
            _demoCrmRepository.UpdateCustomerContact(customerContactEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating customer contact with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCustomerContact")]
        public async Task<IActionResult> DeleteCustomerContact(Guid id)
        {
            var customerContactToDelete = await _demoCrmRepository.GetCustomerContactAsync(id);
            if (customerContactToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteCustomerContact(customerContactToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting a customer contact with Id {id} failed on save.");

            _logger.LogInformation(100, $"A customer contact with Id {id} has been deleted.");

            return NoContent();
        }
    }
}