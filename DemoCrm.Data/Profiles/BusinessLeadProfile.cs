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

        public static Entities.BusinessLead GetStaffMemberEntityFromModel(Models.BusinessLead businessLead)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.BusinessLead, Entities.BusinessLead>())
                .CreateMapper().Map<Entities.BusinessLead>(businessLead);
        }
    }
}
