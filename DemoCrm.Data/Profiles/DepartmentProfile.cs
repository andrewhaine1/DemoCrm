using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class DepartmentProfile
    {
        public static Models.Department GetDepartmentModelFromEntity(Entities.Department department)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Department, Models.Department>()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src =>
                src.Company.Name))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => 
                $"{src.Manager.FirstName} {src.Manager.LastName}")))
                .CreateMapper().Map<Models.Department>(department);
        }

        public static Models.DepartmentUpdate GetDepartmentUpdateModelFromEntity(Entities.Department department)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Department, Models.DepartmentUpdate>())
                .CreateMapper().Map<Models.DepartmentUpdate>(department);
        }

        public static IEnumerable<Models.Department> GetDepartmentModelsFromEntities(IEnumerable<Entities.Department> departments)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Department, Models.Department>()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src =>
                src.Company.Name))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src =>
                $"{src.Manager.FirstName} {src.Manager.LastName}")))
                .CreateMapper().Map<IEnumerable<Models.Department>>(departments);
        }

        public static Entities.Department GetDepartmentEntityFromModel(Models.Department department)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.Department, Entities.Department>())
                .CreateMapper().Map<Entities.Department>(department);
        }

        public static Entities.Department GetDepartmentEntityFromCreateModel(Models.DepartmentCreate departmentCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.DepartmentCreate, Entities.Department>())
                .CreateMapper().Map<Entities.Department>(departmentCreate);
        }

        public static void MapDepartmentUpdateModelToEntity(Models.DepartmentUpdate departmentUpdate, Entities.Department department)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.DepartmentUpdate, Entities.Department>())
                .CreateMapper();

            mapper.Map(departmentUpdate, department);
        }
    }
}
