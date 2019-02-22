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
                configure.CreateMap<Entities.Department, Models.Department>())
                .CreateMapper().Map<Models.Department>(department);
        }

        public static IEnumerable<Models.Department> GetDepartmentModelsFromEntities(IEnumerable<Entities.Department> departments)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Department, Models.Department>())
                .CreateMapper().Map<IEnumerable<Models.Department>>(departments);
        }

        public static Entities.Department GetDepartmentEntityFromModel(Models.Department department)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.Department, Entities.Department>())
                .CreateMapper().Map<Entities.Department>(department);
        }
    }
}
