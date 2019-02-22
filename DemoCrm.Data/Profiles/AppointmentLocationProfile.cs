using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentLocationProfile
    {
        public static Models.AppointmentLocation GetStaffPositionModelFromEntity(Entities.AppointmentLocation 
            appointmentLocation)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentLocation, Models.AppointmentLocation>())
                .CreateMapper().Map<Models.AppointmentLocation>(appointmentLocation);
        }

        public static IEnumerable<Models.AppointmentLocation> GetStaffPositionModelsFromEntities(IEnumerable<Entities.AppointmentLocation>
             appointmentLocations)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentLocation, Models.AppointmentLocation>())
                .CreateMapper().Map<IEnumerable<Models.AppointmentLocation>>(appointmentLocations);
        }

        public static Entities.AppointmentLocation GetAppointmentLocationEntityFromModel(Models.AppointmentLocation 
            appointmentLocation)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentLocation, Entities.AppointmentLocation>())
                .CreateMapper().Map<Entities.AppointmentLocation>(appointmentLocation);
        }
    }
}
