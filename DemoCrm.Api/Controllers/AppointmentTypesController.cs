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
    [Route("api/appointmenttypes")]
    [ApiController]
    public class AppointmentTypesController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "Appointment";
        private const string ControllerNamePlural = "Appointments";

        public AppointmentTypesController(IDemoCrmRepository demoCrmRepository,
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


        [HttpGet(Name = "GetAppointmentTypes")]
        [HttpHead]
        public async Task<IActionResult> GetAppointmentTypes([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<AppointmentType, Data.Entities.AppointmentType>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<AppointmentType>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var appointmenTypeEntities = await _demoCrmRepository.GetAppointmentTypesAsync(crmResourceParameters);

            var appointmentTypeModels = AppointmentTypeProfile.GetAppointmentTypeModelsFromEntities(appointmenTypeEntities);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = appointmenTypeEntities.TotalCount,
                    pageSize = appointmenTypeEntities.PageSize,
                    currentPage = appointmenTypeEntities.CurrentPage,
                    totalPages = appointmenTypeEntities.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    appointmenTypeEntities.HasNext, appointmenTypeEntities.HasPrevious, ControllerNamePlural);

                var shapedAppointmentTypes = appointmenTypeEntities.ShapeData(crmResourceParameters.Fields);


                var shapedAppointmentTypesWithLinks = appointmenTypeEntities.Select(position =>
                {
                    var appoinmentTypesAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)appoinmentTypesAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    appoinmentTypesAsDictionary.Add("links", positionLinks);

                    return appoinmentTypesAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedAppointmentTypesWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = appointmenTypeEntities.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = appointmenTypeEntities.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = appointmenTypeEntities.TotalCount,
                    pageSize = appointmenTypeEntities.PageSize,
                    currentPage = appointmenTypeEntities.CurrentPage,
                    totalPages = appointmenTypeEntities.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(appointmentTypeModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetAppointmentType")]
        public async Task<IActionResult> GetAppointmentType(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<AppointmentType>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var appointmentTypeEntity = await _demoCrmRepository.GetAppointmentTypeAsync(id);

            // If the user is not found return 404.
            if (appointmentTypeEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndAppointmentTypeModel = AppointmentTypeProfile.GetAppointmentTypeModelFromEntity(appointmentTypeEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndAppointmentTypeModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndAppointmentTypeModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }
            var appointmentModel = AppointmentTypeProfile.GetAppointmentTypeModelFromEntity(appointmentTypeEntity);
            return Ok(appointmentModel.ShapeData(fields));
        }


        [HttpPost(Name = "CreateAppointmentType")]
        public async Task<IActionResult> AddAppointmentType([FromBody] AppointmentTypeCreate appointmentTypeCreate)
        {
            if (appointmentTypeCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var appointmentEntity = AppointmentTypeProfile.GetAppointmentTypeEntityFromCreateModel(appointmentTypeCreate);
            _demoCrmRepository.AddAppointmentType(appointmentEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating an appointment failed on save");

            var appointmentModel = AppointmentTypeProfile.GetAppointmentTypeModelFromEntity(appointmentEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, appointmentModel.Id, null, ControllerName);
            var linkedResource = appointmentEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCustomerAccount", new { id = appointmentEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateAppointmentType")]
        public async Task<IActionResult> PartiallyUpdateAppointmentType(Guid id,
            [FromBody] JsonPatchDocument<AppointmentTypeUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var appointmentType = await _demoCrmRepository.GetAppointmentTypeAsync(id);
            if (appointmentType == null)
                return NotFound();

            var appointmentTypeToUpdate = AppointmentTypeProfile.GetAppointmentTypeUpdateModelFromEntity(appointmentType);

            patchDoc.ApplyTo(appointmentTypeToUpdate);

            TryValidateModel(appointmentTypeToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            AppointmentTypeProfile.MapAppointmentTypeModelToEntity(appointmentTypeToUpdate, appointmentType);

            _demoCrmRepository.UpdateAppointmentType(appointmentType);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching appointment type" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateAppointmentType")]
        public async Task<IActionResult> FullyUpdateAppointmentType(Guid id,
            [FromBody] AppointmentTypeUpdate appointmentTypeUpdate)
        {
            if (appointmentTypeUpdate == null)
                return BadRequest();

            var appointmentTypeEntity = await _demoCrmRepository.GetAppointmentTypeAsync(id);
            if (appointmentTypeEntity == null)
                return NotFound();

            TryValidateModel(appointmentTypeEntity);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            AppointmentTypeProfile.MapAppointmentTypeModelToEntity(appointmentTypeUpdate, appointmentTypeEntity);

            // apply update
            _demoCrmRepository.UpdateAppointmentType(appointmentTypeEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating appointment type with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteAppointmentType")]
        public async Task<IActionResult> DeleteAppointmentType(Guid id)
        {
            var AppointmentTypeToDelete = await _demoCrmRepository.GetAppointmentTypeAsync(id);
            if (AppointmentTypeToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteAppointmentType(AppointmentTypeToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting an appointment type with Id {id} failed on save.");

            _logger.LogInformation(100, $"An appointment type with Id {id} has been deleted.");

            return NoContent();
        }
    }
}