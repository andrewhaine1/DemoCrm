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
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "Appointment";
        private const string ControllerNamePlural = "Appointments";

        public AppointmentsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetAppointments")]
        [HttpHead]
        public async Task<IActionResult> GetAppointments([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<Appointment, Data.Entities.Appointment>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<Appointment>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var appointmenEntities = await _demoCrmRepository.GetAppointmentsAsync(crmResourceParameters);

            var appointmentModels = AppointmentProfile.GetAppointmentModelsFromEntities(appointmenEntities);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = appointmenEntities.TotalCount,
                    pageSize = appointmenEntities.PageSize,
                    currentPage = appointmenEntities.CurrentPage,
                    totalPages = appointmenEntities.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    appointmenEntities.HasNext, appointmenEntities.HasPrevious, ControllerNamePlural);

                var shapedAppointments = appointmenEntities.ShapeData(crmResourceParameters.Fields);


                var shapedAppointsWithLinks = appointmenEntities.Select(position =>
                {
                    var appoinmentsAsDictionary = position as IDictionary<string, object>;
                    var positionLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)appoinmentsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    appoinmentsAsDictionary.Add("links", positionLinks);

                    return appoinmentsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedAppointsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = appointmenEntities.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = appointmenEntities.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = appointmenEntities.TotalCount,
                    pageSize = appointmenEntities.PageSize,
                    currentPage = appointmenEntities.CurrentPage,
                    totalPages = appointmenEntities.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));


                return Ok(appointmentModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetAppointment")]
        public async Task<IActionResult> GetAppointment(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<Appointment>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var appointmentEntity = await _demoCrmRepository.GetAppointmentAsync(id);

            // If the user is not found return 404.
            if (appointmentEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndAppointmentModel = AppointmentProfile.GetAppointmentModelFromEntity(appointmentEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
                var vndLinkedResource = vndAppointmentModel.ShapeData(fields)
                    as IDictionary<string, object>;

                vndLinkedResource.Add("links", vndLinks);

                var valueLinkResource = new
                {
                    value = vndAppointmentModel.ShapeData(fields)
                    as IDictionary<string, object>,
                    links = vndLinks
                };

                // If found return 200 with CrmUser object.
                return Ok(valueLinkResource);
            }

            var appointmentModel = AppointmentProfile.GetAppointmentModelFromEntity(appointmentEntity);
            return Ok(appointmentModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateAppointment")]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentCreate appointmentCreate)
        {
            if (appointmentCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var appointmentEntity = AppointmentProfile.GetAppointmentEntityFromCreateModel(appointmentCreate);
            _demoCrmRepository.AddAppointment(appointmentEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating an appointment failed on save");

            var appointmentModel = AppointmentProfile.GetAppointmentModelFromEntity(appointmentEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, appointmentModel.Id, null, ControllerName);
            var linkedResource = appointmentEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCustomerAccount", new { id = appointmentEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateAppointment")]
        public async Task<IActionResult> PartiallyUpdateAppointment(Guid id,
            [FromBody] JsonPatchDocument<AppointmentUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var appointment = await _demoCrmRepository.GetAppointmentAsync(id);
            if (appointment == null)
                return NotFound();

            var appointmentToUpdate = AppointmentProfile.GetAppointmentUpdateModelFromEntity(appointment);

            patchDoc.ApplyTo(appointmentToUpdate);

            TryValidateModel(appointmentToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            AppointmentProfile.MapAppointmentModelToEntity(appointmentToUpdate, appointment);

            _demoCrmRepository.UpdateAppointment(appointment);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching appointment" +
                    $" with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateAppointment")]
        public async Task<IActionResult> FullyUpdateAppointment(Guid id,
            [FromBody] AppointmentUpdate appointmentUpdate)
        {
            if (appointmentUpdate == null)
                return BadRequest();

            var appointmentEntity = await _demoCrmRepository.GetAppointmentAsync(id);
            if (appointmentEntity == null)
                return NotFound();

            TryValidateModel(appointmentEntity);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
           AppointmentProfile.MapAppointmentModelToEntity(appointmentUpdate, appointmentEntity);

            // apply update
            _demoCrmRepository.UpdateAppointment(appointmentEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating customer contact with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteAppointment")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var AppointmentToDelete = await _demoCrmRepository.GetAppointmentAsync(id);
            if (AppointmentToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteAppointment(AppointmentToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting an appointment with Id {id} failed on save.");

            _logger.LogInformation(100, $"An appointment with Id {id} has been deleted.");

            return NoContent();
        }
    }
}