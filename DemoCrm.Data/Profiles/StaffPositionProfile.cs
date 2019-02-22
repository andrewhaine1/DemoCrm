using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class StaffPositionProfile
    {
        public static Models.StaffPosition GetStaffPositionModelFromEntity(Entities.StaffPosition staffPosition)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffPosition, Models.StaffPosition>())
                .CreateMapper().Map<Models.StaffPosition>(staffPosition);
        }

        public static IEnumerable<Models.StaffPosition> GetStaffPositionModelsFromEntities(IEnumerable<Entities.StaffPosition>
             staffPositions)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.StaffPosition, Models.StaffPosition>())
                .CreateMapper().Map<IEnumerable<Models.StaffPosition>>(staffPositions);
        }

        public static Entities.StaffPosition GetStaffPositionEntityFromModel(Models.StaffPosition staffPosition)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.StaffPosition, Entities.StaffPosition>())
                .CreateMapper().Map<Entities.StaffPosition>(staffPosition);
        }
    }
}
