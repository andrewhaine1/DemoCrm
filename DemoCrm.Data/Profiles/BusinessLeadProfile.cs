using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class BusinessLeadProfile
    {
        public static Models.BusinessLead GetBusinessLeadModelFromEntity(Entities.BusinessLead businessLead)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.BusinessLead, Models.BusinessLead>())
                .CreateMapper().Map<Models.BusinessLead>(businessLead);
        }

        public static IEnumerable<Models.BusinessLead> GetBusinessLeadModelsFromEntities(IEnumerable<Entities.BusinessLead> businessLeads)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.BusinessLead, Models.BusinessLead>())
                .CreateMapper().Map<IEnumerable<Models.BusinessLead>>(businessLeads);
        }

        public static Entities.BusinessLead GetBusinessLeadEntityFromModel(Models.BusinessLead businessLead)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.BusinessLead, Entities.BusinessLead>())
                .CreateMapper().Map<Entities.BusinessLead>(businessLead);
        }

        public static Models.BusinessLeadUpdate GetBusinessLeadUpdateModelFromEntity(Entities.BusinessLead businessLead)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.BusinessLead, Models.BusinessLead>())
                .CreateMapper().Map<Models.BusinessLeadUpdate>(businessLead);
        }

        public static Entities.BusinessLead GetBusinessLeadEntityFromCreateModel(Models.BusinessLeadCreate businessLeadCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.BusinessLeadCreate, Entities.BusinessLead>())
                .CreateMapper().Map<Entities.BusinessLead>(businessLeadCreate);
        }

        public static void MapBusinessLeadUpdateModelToEntity(Models.BusinessLeadUpdate businessLeadUpdate,
            Entities.BusinessLead businessLead)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffMemberUpdate, Entities.StaffMember>())
                .CreateMapper();

            mapper.Map(businessLeadUpdate, businessLead);
        }
    }
}
