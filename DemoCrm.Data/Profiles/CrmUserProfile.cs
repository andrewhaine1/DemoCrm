using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class CrmUserProfile
    {
        /// <summary>
        /// Gets a mapped instance of Models.CrmUser from an instance of Entities.CrmUser
        /// </summary>
        /// <param name="crmUserEntity">An instance of Entities.CrmUser</param>
        /// <returns>A mapped instance of Models.CrmUser</returns>
        public static Models.CrmUser GetCrmUserModelFromEntity(Entities.CrmUser crmUserEntity)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CrmUser, Models.CrmUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
                src.Id))
                //.ForMember(dest => dest.OauthId, opt => opt.MapFrom(src =>
                //src.OauthId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src.ObjectType.Name))).CreateMapper().Map<Models.CrmUser>(crmUserEntity);
        }

        /// <summary>
        /// Gets an collection of mapped instances of Models.CrmUser from a collection of instances of Entities.CrmUser
        /// </summary>
        /// <param name="crmUserEnties">A collection of Entities.CrmUser</param>
        /// <returns>A collection of mapped instances of Models.CrmUser</returns>
        public static IEnumerable<Models.CrmUser> GetCrmUserModelsFromEntities(IEnumerable<Entities.CrmUser> crmUserEnties)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CrmUser, Models.CrmUser>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src.ObjectType.Name))).CreateMapper().Map<IEnumerable<Models.CrmUser>>(crmUserEnties);
        }

        /// <summary>
        /// Gets a mapped instance of Entities.CrmUser from an instance of Models.CrmUserCreate
        /// </summary>
        /// <param name="crmUserCreateModel">A n instance of Models.CrmUserCreate</param>
        /// <returns>A mapped instance of Entities.CrmUser from an instance of Models.CrmUserCreate</returns>
        public static Entities.CrmUser GetCrmUserEntityFromCreateModel(Models.CrmUserCreate crmUserCreateModel)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CrmUserCreate, Entities.CrmUser>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src =>
                src.Phone))).CreateMapper().Map<Entities.CrmUser>(crmUserCreateModel);
        }

        /// <summary>
        /// Gets a collection of mapped instances of Entities.CrmUser from a collection of Models.CrmUserCreate instances.
        /// </summary>
        /// <param name="crmUserCreateModels">IEnumerable of Models.CrmUserCreate.</param>
        /// <returns>IEnumerable of Entities.CrmUser.</returns>
        public static IEnumerable<Entities.CrmUser> GetCrmUserEntitiesFromCreateModels(IEnumerable<Models.CrmUserCreate> crmUserCreateModels)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CrmUserCreate, Entities.CrmUser>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src =>
                src.Phone))).CreateMapper().Map<IEnumerable<Entities.CrmUser>>(crmUserCreateModels);
        }

        /// <summary>
        /// Gets a mapped instance of Entities.CrmUser from an instance of Models.CrmUserUpdate
        /// </summary>
        /// <param name="crmUserUpdateModel">An instance of Models.CrmUserUpdate</param>
        /// <returns>An instance of Entities.CrmUser</returns>
        public static Entities.CrmUser GetCrmUserEntityFromUpdateModel(Models.CrmUserUpdate crmUserUpdateModel)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.CrmUserUpdate, Entities.CrmUser>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src =>
                src.Phone))).CreateMapper().Map<Entities.CrmUser>(crmUserUpdateModel);
        }

        /// <summary>
        /// Gets a mapped instance of Models.CrmUserUpdate from an instance of the CrmUser Entity
        /// </summary>
        /// <param name="crmUserEntity">An instance of Entities.CrmUser</param>
        /// <returns>A mapped instance of Models.CrmUserUpdate</returns>
        public static Models.CrmUserUpdate GetCrmUserUpdateModelFromEntity(Entities.CrmUser crmUserEntity)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.CrmUser, Models.CrmUserUpdate>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber))).CreateMapper().Map<Models.CrmUserUpdate>(crmUserEntity);
        }
    }
}
