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

        public static Models.CustomerContactUpdate GetCustomerContactUpdateModelFromEntity(Entities.CustomerContact customerContact)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerContact, Models.CustomerContactUpdate>())
                .CreateMapper().Map<Models.CustomerContactUpdate>(customerContact);
        }

        public static Entities.CustomerContact GetCustomerContactEntityFromCreateModel(Models.CustomerContactCreate customerContactCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerContactCreate, Entities.CustomerContact>())
                .CreateMapper().Map<Entities.CustomerContact>(customerContactCreate);
        }

        public static void MapCustomerContactModelToEntity(Models.CustomerContactUpdate customerContactUpdate,
            Entities.CustomerContact customerContact)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerContactUpdate, Entities.CustomerContact>())
                .CreateMapper();

            mapper.Map(customerContactUpdate, customerContact);
        }
    }
}
