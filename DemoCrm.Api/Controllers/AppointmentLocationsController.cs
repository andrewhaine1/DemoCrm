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
    [Route("api/appointmentlocations")]
    [ApiController]
    public class AppointmentLocationsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "Appointment";
        private const string ControllerNamePlural = "Appointments";

        public AppointmentLocationsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetAppointmentLocations")]
        [HttpHead]
        public async Task<IActionResult> GetAppointmentLocations([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<AppointmentLocation, Data.Entities.AppointmentLocation>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<AppointmentLocation>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var appointmenLocationsEntities = await _demoCrmRepository.GetAppointmentLocationsAsync(crmResourceParameters);

            var appointmentLocationModels = AppointmentLocationProfile.GetAppointmentLocationModelsFromEntities(appointmenLocationsEntities);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = appointmenLocationsEntities.TotalCount,
                    pageSize = appointmenLocationsEntities.PageSize,
                    currentPage = appointmenLocationsEntities.CurrentPage,
                    totalPages = appointmenLocationsEntities.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    appointmenLocationsEntities.HasNext, appointmenLocationsEntities.HasPrevious, ControllerNamePlural);

                var shapedAppointmentLocations = appointmenLocationsEntities.ShapeData(crmResourceParameters.Fields);


                var shapedAppointmentLocationWithLinks = appointmenLocationsEntities.Select(position =>
                {
                    var appoinmentLocationsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)appoinmentLocationsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    appoinmentLocationsAsDictionary.Add("links", positionLinks);

                    return appoinmentLocationsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedAppointmentLocationWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = appointmenLocationsEntities.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = appointmenLocationsEntities.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = appointmenLocationsEntities.TotalCount,
                    pageSize = appointmenLocationsEntities.PageSize,
                    currentPage = appointmenLocationsEntities.CurrentPage,
                    totalPages = appointmenLocationsEntities.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(appointmentLocationModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetAppointmentLocation")]
        public async Task<IActionResult> GetAppointmentLocation(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<AppointmentLocation>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var appointmentLocationEntity = await _demoCrmRepository.GetAppointmentLocationAsync(id);

            // If the user is not found return 404.
            if (appointmentLocationEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndAppointmentLocationModel = AppointmentLocationProfile.
                    GetAppointmentLocationFromEntity(appointmentLocationEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndAppointmentLocationModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndAppointmentLocationModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }
            var appointmentLocationModel = AppointmentLocationProfile.GetAppointmentLocationFromEntity(appointmentLocationEntity);
            return Ok(appointmentLocationModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateAppointmentLocation")]
        public async Task<IActionResult> AddAppointmentLocation([FromBody] AppointmentLocationCreate appointmentLocationCreate)
        {
            if (appointmentLocationCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var appointmentEntity = AppointmentLocationProfile.GetAppointmentLocationEntityFromCreateModel(appointmentLocationCreate);
            _demoCrmRepository.AddAppointmentLocation(appointmentEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating an appointment failed on save");

            var appointmentLocationModel = AppointmentLocationProfile.GetAppointmentLocationFromEntity(appointmentEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, appointmentLocationModel.Id, null, ControllerName);
            var linkedResource = appointmentEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetAppointmentLocation", new { id = appointmentEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateAppointmentLocation")]
        public async Task<IActionResult> PartiallyUpdateAppointmentType(Guid id,
            [FromBody] JsonPatchDocument<AppointmentLocationUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var appointmentLocation = await _demoCrmRepository.GetAppointmentLocationAsync(id);
            if (appointmentLocation == null)
                return NotFound();

            var appointmentLocationToUpdate = AppointmentLocationProfile
                .GetAppointmentLocationUpdateModelFromEntity(appointmentLocation);

            patchDoc.ApplyTo(appointmentLocationToUpdate);

            TryValidateModel(appointmentLocationToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            AppointmentLocationProfile.MapAppointmentLocationModelToEntity(appointmentLocationToUpdate, appointmentLocation);

            _demoCrmRepository.UpdateAppointmentLocation(appointmentLocation);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching appointment location" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateAppointmentLocation")]
        public async Task<IActionResult> FullyUpdateAppointmentType(Guid id,
            [FromBody] AppointmentLocationUpdate appointmentLocationUpdate)
        {
            if (appointmentLocationUpdate == null)
                return BadRequest();

            var appointmentLocationEntity = await _demoCrmRepository.GetAppointmentLocationAsync(id);
            if (appointmentLocationEntity == null)
                return NotFound();

            TryValidateModel(appointmentLocationEntity);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            AppointmentLocationProfile.MapAppointmentLocationModelToEntity(appointmentLocationUpdate, appointmentLocationEntity);

            // apply update
            _demoCrmRepository.UpdateAppointmentLocation(appointmentLocationEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating appointment location with Id {id} failed on save.");

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteAppointmentLocation")]
        public async Task<IActionResult> DeleteAppointmentLocation(Guid id)
        {
            var AppointmentLocationToDelete = await _demoCrmRepository.GetAppointmentLocationAsync(id);
            if (AppointmentLocationToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteAppointmentLocation(AppointmentLocationToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting an appointment location with Id {id} failed on save.");

            _logger.LogInformation(100, $"An appointment location with Id {id} has been deleted.");

            return NoContent();
        }
    }
}