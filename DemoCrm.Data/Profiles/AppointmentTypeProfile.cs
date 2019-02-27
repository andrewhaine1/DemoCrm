using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentTypeProfile
    {
        public static Models.AppointmentType GetAppointmentTypeModelFromEntity(Entities.AppointmentType
            appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentType>())
                .CreateMapper().Map<Models.AppointmentType>(appointmentType);
        }

        public static IEnumerable<Models.AppointmentType> GetAppointmentTypeModelsFromEntities(
            IEnumerable<Entities.AppointmentType> appointmentTypes)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentType>())
                .CreateMapper().Map<IEnumerable<Models.AppointmentType>>(appointmentTypes);
        }

        public static Entities.AppointmentType GetAppointmentTypeEntityFromModel(Models.AppointmentType appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentType, Entities.AppointmentType>())
                .CreateMapper().Map<Entities.AppointmentType>(appointmentType);
        }

        public static Models.AppointmentTypeUpdate GetAppointmentTypeUpdateModelFromEntity(Entities.AppointmentType appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentTypeUpdate>())
                .CreateMapper().Map<Models.AppointmentTypeUpdate>(appointmentType);
        }

        public static Entities.AppointmentType GetAppointmentTypeEntityFromCreateModel(
            Models.AppointmentTypeCreate appointmentTypeCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentTypeCreate, Entities.AppointmentType>())
                .CreateMapper().Map<Entities.AppointmentType>(appointmentTypeCreate);
        }

        public static void MapAppointmentTypeModelToEntity(Models.AppointmentTypeUpdate appointmentTypeUpdate,
            Entities.AppointmentType appointmentType)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentType, Entities.AppointmentType>())
                .CreateMapper();

            mapper.Map(appointmentTypeUpdate, appointmentType);
        }
    }
}
