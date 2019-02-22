using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class CustomerContactProfile
    {
        public static Models.CustomerContact GetCustomerContactModelFromEntity(Entities.CustomerContact 
            customerContact)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerContact, Models.CustomerContact>())
                .CreateMapper().Map<Models.CustomerContact>(customerContact);
        }

        public static IEnumerable<Models.CustomerContact> GetCustomerContactModelsFromEntities(
            IEnumerable<Entities.CustomerContact> customerContacts)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerContact, Models.CustomerContact>())
                .CreateMapper().Map<IEnumerable<Models.CustomerContact>>(customerContacts);
        }

        public static Entities.CustomerContact GetCustomerContactEntityFromModel(Models.CustomerContact customerContact)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerContact, Entities.CustomerContact>())
                .CreateMapper().Map<Entities.CustomerContact>(customerContact);
        }
    }
}
