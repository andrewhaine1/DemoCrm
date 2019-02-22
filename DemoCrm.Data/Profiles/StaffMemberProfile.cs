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
                configure.CreateMap<Entities.StaffMember, Models.StaffMember>())
                .CreateMapper().Map<Models.StaffMember>(staffMember);
        }

        public static IEnumerable<Models.StaffMember> GetStaffMemberModelsFromEntities(IEnumerable<Entities.StaffMember> staffMembers)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffMember, Models.StaffMember>())
                .CreateMapper().Map<IEnumerable<Models.StaffMember>>(staffMembers);
        }

        public static Entities.Department GetStaffMemberEntityFromModel(Models.StaffMember staffMember)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.Department, Entities.Department>())
                .CreateMapper().Map<Entities.Department>(staffMember);
        }
    }
}
