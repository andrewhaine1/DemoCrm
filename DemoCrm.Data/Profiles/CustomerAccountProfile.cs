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
                configure.CreateMap<Entities.CustomerAccount, Models.CustomerAccount>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                src.CompanyName))
                .ForMember(dest => dest.Registration, opt => opt.MapFrom(src =>
                src.RegistrationNumber))
                .ForMember(dest => dest.Vat, opt => opt.MapFrom(src =>
                src.VatNumber))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber)))
                .CreateMapper().Map<Models.CustomerAccount>(customerAccount);
        }

        public static IEnumerable<Models.CustomerAccount> GetCustomerAccountModelsFromEntities(
            IEnumerable<Entities.CustomerAccount> customerAccounts)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerAccount, Models.CustomerAccount>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                src.CompanyName))
                .ForMember(dest => dest.Registration, opt => opt.MapFrom(src =>
                src.RegistrationNumber))
                .ForMember(dest => dest.Vat, opt => opt.MapFrom(src =>
                src.VatNumber))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber)))
                .CreateMapper().Map<IEnumerable<Models.CustomerAccount>>(customerAccounts);
        }

        public static Entities.CustomerAccount GetCustomerAccountEntityFromModel(Models.CustomerAccount customerAccount)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerAccount, Entities.CustomerAccount>())
                .CreateMapper().Map<Entities.CustomerAccount>(customerAccount);
        }

        public static Models.CustomerAccountUpdate GetCustomerAccountUpdateModelFromEntity(Entities.CustomerAccount customerAccount)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CustomerAccount, Models.CustomerAccountUpdate>())
                .CreateMapper().Map<Models.CustomerAccountUpdate>(customerAccount);
        }

        public static Entities.CustomerAccount GetCustomerAccountEntityFromCreateModel(Models.CustomerAccountCreate customerAccountCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CustomerAccountCreate, Entities.CustomerAccount>())
                .CreateMapper().Map<Entities.CustomerAccount>(customerAccountCreate);
        }

        public static void MapCustomerAccountUpdateModelToEntity(Models.CustomerAccountUpdate customerAccountUpdate,
            Entities.CustomerAccount customerAccount)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffMemberUpdate, Entities.StaffMember>())
                .CreateMapper();

            mapper.Map(customerAccountUpdate, customerAccount);
        }
    }
}
