using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class CustomerAccountProfile
    {
        public static Models.CustomerAccount GetCustomerAccountModelFromEntity(Entities.CustomerAccount customerAccount)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerAccount, Models.CustomerAccount>())
                .CreateMapper().Map<Models.CustomerAccount>(customerAccount);
        }

        public static IEnumerable<Models.CustomerAccount> GetCustomerAccountModelsFromEntities(
            IEnumerable<Entities.CustomerAccount> customerAccounts)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerAccount, Models.CustomerAccount>())
                .CreateMapper().Map<IEnumerable<Models.CustomerAccount>>(customerAccounts);
        }

        public static Entities.CustomerAccount GetCustomerAccountEntityFromModel(Models.CustomerAccount customerAccount)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerAccount, Entities.CustomerAccount>())
                .CreateMapper().Map<Entities.CustomerAccount>(customerAccount);
        }
    }
}
