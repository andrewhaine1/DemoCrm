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
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private IDemoCrmRepository _demoCrmRepository;
        private ILogger<DepartmentsController> _logger;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        private const string ControllerName = "Department";
        private const string ControllerNamePlural = "Departments";

        public DepartmentsController(IDemoCrmRepository demoCrmRepository,
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

        [HttpGet(Name = "GetDepartments")]
        [HttpHead]
        public async Task<IActionResult> GetCompanies([FromQuery] CrmResourceParameters crmResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            // Check if mapping exists for orderBy parameter passed in via URL
            if (!_propertyMappingService.ValidMappingExistsFor<Department, Data.Entities.Department>
                (crmResourceParameters.OrderBy))
                return BadRequest();

            if (!_typeHelperService.TypeHasProperties<Company>(crmResourceParameters.Fields))
                return BadRequest();

            // Get all companies 
            var departments = await _demoCrmRepository.GetDepartmentsAsync(crmResourceParameters);

            var departmentModels = DepartmentProfile.GetDepartmentModelsFromEntities(departments);

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = departments.TotalCount,
                    pageSize = departments.PageSize,
                    currentPage = departments.CurrentPage,
                    totalPages = departments.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CrmUriHelpers.CreateActionLinksExpandoList(_urlHelper, crmResourceParameters,
                    departments.HasNext, departments.HasPrevious, ControllerNamePlural);

                var shapedCompanies = departmentModels.ShapeData(crmResourceParameters.Fields);


                var shapedDepartmentsWithLinks = shapedCompanies.Select(crmUser =>
                {
                    var departmentsAsDictionary = crmUser as IDictionary<string, object>;
                    var crmUserLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper,
                        (Guid)departmentsAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                    departmentsAsDictionary.Add("links", crmUserLinks);

                    return departmentsAsDictionary;
                });

                var linkedCollectionsResource = new
                {
                    value = shapedDepartmentsWithLinks,
                    links
                };

                return Ok(linkedCollectionsResource);
            }
            else
            {
                var prevousPageLink = departments.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

                var nextPageLink = departments.HasNext ?
                    CrmUriHelpers.CreateCrmObjectRecourceUri(_urlHelper, crmResourceParameters,
                    ResourceUriType.NextPage, ControllerName) : null;

                var paginationMetadata = new
                {
                    totalCount = departments.TotalCount,
                    pageSize = departments.PageSize,
                    currentPage = departments.CurrentPage,
                    totalPages = departments.TotalPages,
                    prevousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(departmentModels.ShapeData(crmResourceParameters.Fields));
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> GetDepartment(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<Department>(fields))
                return BadRequest();

            // Get CrmUser using Guid Id. This may need to be changed to use th Oauth Id instead
            var departmentEntity = await _demoCrmRepository.GetDepartmentAsync(id);

            // If the user is not found return 404.
            if (departmentEntity == null)
                return NotFound();

            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var vndcompanyModel = DepartmentProfile.GetDepartmentModelFromEntity(departmentEntity);

                //var links = CreateActionLinksExpando(id, fields);
                var vndLinks = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, id, fields, ControllerName);
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

            var departmentModel = DepartmentProfile.GetDepartmentModelFromEntity(departmentEntity);

            return Ok(departmentModel.ShapeData(fields));
        }

        [HttpPost(Name = "CreateDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentCreate department)
        {
            if (department == null)
                return BadRequest();

            if (await _demoCrmRepository.DepartmentNameExitsAsync(department.Name))
            {
                ModelState.AddModelError(nameof(department),
                    $"A department with the name {department.Name} already exists.");
                return Conflict(ModelState);
            }

            //department.ManagerId = (department.ManagerId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            //    ? null : department.ManagerId;

            // Check if entity with foriegn key exists before inserting as it will cause an exception and
            // result in an error 500.
            if (!await _demoCrmRepository.CompanyExistsAsync(department.CompanyId))
            {
                ModelState.AddModelError(nameof(department.CompanyId),
                    $"Foreign key error: A company with Id '{department.CompanyId}' does not exist.");
            }

            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            var departmentEntity = DepartmentProfile.GetDepartmentEntityFromCreateModel(department);
            _demoCrmRepository.AddDepartment(departmentEntity);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception("Creating a department failed on save");

            var departmentModel = DepartmentProfile.GetDepartmentModelFromEntity(departmentEntity);

            var links = CrmUriHelpers.CreateActionLinksExpando(_urlHelper, departmentEntity.Id, null, ControllerName);
            var linkedResource = departmentEntity.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetDepartment", new { id = departmentEntity.Id }, linkedResource);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateDepartment")]
        public async Task<IActionResult> PartiallyUpdateDepartment(Guid id,
            [FromBody] JsonPatchDocument<DepartmentUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var department = await _demoCrmRepository.GetDepartmentAsync(id);
            if (department == null)
                return NotFound();

            var departmentToUpdate = DepartmentProfile.GetDepartmentUpdateModelFromEntity(department);

            patchDoc.ApplyTo(departmentToUpdate);

            TryValidateModel(departmentToUpdate);

            // Validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            DepartmentProfile.MapDepartmentUpdateModelToEntity(departmentToUpdate, department);

            _demoCrmRepository.UpdateDepartment(department);


            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Patching company with id {id} failed on save");

            return NoContent();
        }

        [HttpPut("{id}", Name = "FullyUpdateDepartment")]
        public async Task<IActionResult> FullyUpdateDepartment(Guid id, [FromBody] DepartmentUpdate departmentUpdate)
        {
            if (departmentUpdate == null)
                return BadRequest();

            var departmentEntity = await _demoCrmRepository.GetDepartmentAsync(id);
            if (departmentEntity == null)
                return NotFound();

            TryValidateModel(departmentUpdate);

            // Add validation
            if (!ModelState.IsValid)
                return new Data.Helpers.UnprocessableEntityObjectResult(ModelState);

            // map
            DepartmentProfile.MapDepartmentUpdateModelToEntity(departmentUpdate, departmentEntity);

            // apply update
            _demoCrmRepository.UpdateDepartment(departmentEntity);

            // map back to entity
            // ...

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Updating department with Id {id} failed on save.");

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var departmentToDelete = await _demoCrmRepository.GetDepartmentAsync(id);
            if (departmentToDelete == null)
                return NotFound();

            _demoCrmRepository.DeleteDepartment(departmentToDelete);

            if (!await _demoCrmRepository.SaveChangesAsync())
                throw new Exception($"Deleting department with Id {id} failed on save.");

            _logger.LogInformation(100, $"A Department with Id {id} has been deleted.");

            return NoContent();
        }
    }
}