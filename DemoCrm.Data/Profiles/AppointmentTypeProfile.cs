using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentTypeProfile
    {
        public static Models.AppointmentType GetCustomerContactModelFromEntity(Entities.AppointmentType
            appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentType>())
                .CreateMapper().Map<Models.AppointmentType>(appointmentType);
        }

        public static IEnumerable<Models.AppointmentType> GetAppointmentModelsFromEntities(
            IEnumerable<Entities.AppointmentType> appointmentTypes)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentType>())
                .CreateMapper().Map<IEnumerable<Models.AppointmentType>>(appointmentTypes);
        }

        public static Entities.AppointmentType GetCustomerContactEntityFromModel(Models.AppointmentType appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentType, Entities.AppointmentType>())
                .CreateMapper().Map<Entities.AppointmentType>(appointmentType);
        }

        public static Models.AppointmentType GetAppointmentTypeUpdateModelFromEntity(Entities.AppointmentType appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.AppointmentType, Models.AppointmentTypeUpdate>())
                .CreateMapper().Map<Models.AppointmentType>(appointmentType);
        }

        public static Entities.AppointmentType GetAppointmentTypeEntityFromCreateModel(Models.AppointmentType appointmentType)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentTypeCreate, Entities.AppointmentType>())
                .CreateMapper().Map<Entities.AppointmentType>(appointmentType);
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
