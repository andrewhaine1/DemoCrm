using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentLocationProfile
    {
        public static Models.AppointmentLocation GetAppointmentLocationFromEntity(Entities.AppointmentLocation 
            appointmentLocation)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentLocation, Models.AppointmentLocation>())
                .CreateMapper().Map<Models.AppointmentLocation>(appointmentLocation);
        }

        public static IEnumerable<Models.AppointmentLocation> GetAppointmentLocationModelsFromEntities(IEnumerable<Entities.AppointmentLocation>
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

        public static Models.AppointmentLocationUpdate GetAppointmentLocationUpdateModelFromEntity(
            Entities.AppointmentLocation appointmentLocation)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentLocation, Models.AppointmentLocationUpdate>())
                .CreateMapper().Map<Models.AppointmentLocationUpdate>(appointmentLocation);
        }

        public static Entities.AppointmentLocation GetAppointmentLocationEntityFromCreateModel(
            Models.AppointmentLocationCreate appointmentLocationCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentLocationCreate, Entities.AppointmentLocation>())
                .CreateMapper().Map<Entities.AppointmentLocation>(appointmentLocationCreate);
        }

        public static void MapAppointmentLocationModelToEntity(Models.AppointmentLocationUpdate appointmentLocationUpdate,
            Entities.AppointmentLocation appointmentLocation)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentLocationUpdate, Entities.AppointmentLocation>())
                .CreateMapper();

            mapper.Map(appointmentLocationUpdate, appointmentLocation);
        }
    }
}
