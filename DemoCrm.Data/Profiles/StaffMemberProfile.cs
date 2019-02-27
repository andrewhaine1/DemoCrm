using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class StaffMemberProfile
    {
        public static Models.StaffMember GetStaffMemberModelFromEntity(Entities.StaffMember staffMember)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffMember, Models.StaffMember>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src =>
                src.StaffPosition.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber)))
                .CreateMapper().Map<Models.StaffMember>(staffMember);
        }

        public static IEnumerable<Models.StaffMember> GetStaffMemberModelsFromEntities(IEnumerable<Entities.StaffMember> staffMembers)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffMember, Models.StaffMember>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src =>
                src.StaffPosition.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                src.PhoneNumber)))
                .CreateMapper().Map<IEnumerable<Models.StaffMember>>(staffMembers);
        }

        public static Entities.StaffMember GetStaffMemberEntityFromModel(Models.StaffMember staffMember)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffMember, Entities.StaffMember>())
                .CreateMapper().Map<Entities.StaffMember>(staffMember);
        }

        public static Models.StaffMemberUpdate GetStaffMemberUpdateModelFromEntity(Entities.StaffMember staffMember)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffMember, Models.StaffMember>())
                .CreateMapper().Map<Models.StaffMemberUpdate>(staffMember);
        }

        public static Entities.StaffMember GetStaffMemberEntityFromCreateModel(Models.StaffMemberCreate staffMemberCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffMemberCreate, Entities.StaffMember>())
                .CreateMapper().Map<Entities.StaffMember>(staffMemberCreate);
        }

        public static void MapStaffMemberUpdateModelToEntity(Models.StaffMemberUpdate staffMemberUpdate,
            Entities.StaffMember staffMember)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffMemberUpdate, Entities.StaffMember>())
                .CreateMapper();

            mapper.Map(staffMemberUpdate, staffMember);
        }
    }
}
