using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentProfile
    {
        public static Models.Appointment GetCustomerContactModelFromEntity(Entities.Appointment
             appointment)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Appointment, Models.Appointment>())
                .CreateMapper().Map<Models.Appointment>(appointment);
        }

        public static IEnumerable<Models.Appointment> GetAppointmentModelsFromEntities(
            IEnumerable<Entities.Appointment> appointments)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Appointment, Models.Appointment>())
                .CreateMapper().Map<IEnumerable<Models.Appointment>>(appointments);
        }

        public static Entities.Appointment GetCustomerContactEntityFromModel(Models.Appointment appointment)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.Appointment, Entities.Appointment>())
                .CreateMapper().Map<Entities.Appointment>(appointment);
        }

        public static Models.AppointmentUpdate GetAppointmentUpdateModelFromEntity(Entities.Appointment appointment)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Appointment, Models.AppointmentUpdate>())
                .CreateMapper().Map<Models.AppointmentUpdate>(appointment);
        }

        public static Entities.Appointment GetAppointmentEntityFromCreateModel(Models.AppointmentCreate appointmentCreate)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentCreate, Entities.Appointment>())
                .CreateMapper().Map<Entities.Appointment>(appointmentCreate);
        }

        public static void MapAppointmentModelToEntity(Models.AppointmentUpdate appointmentUpdate,
            Entities.Appointment appointment)
        {
            var mapper = new MapperConfiguration(configure =>
                configure.CreateMap<Models.AppointmentUpdate, Entities.Appointment>())
                .CreateMapper();

            mapper.Map(appointmentUpdate, appointment);
        }
    }
}
