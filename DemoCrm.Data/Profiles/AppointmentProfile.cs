using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Profiles
{
    public static class AppointmentProfile
    {
        public static Models.Appointment GetAppointmentModelFromEntity(Entities.Appointment
             appointment)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Appointment, Models.Appointment>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src =>
                src.ContactPersonFullName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src.AppointmentType.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                src.AppointmentLocation.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => 
                src.AppointmentAddress))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src =>
                src.CustomerAccount.CompanyName))
                .ForMember(dest => dest.Attendee, opt => opt.MapFrom(src =>
                $"{src.StaffMember.FirstName} {src.StaffMember.LastName}")))
                .CreateMapper().Map<Models.Appointment>(appointment);
        }

        public static IEnumerable<Models.Appointment> GetAppointmentModelsFromEntities(
            IEnumerable<Entities.Appointment> appointments)
        {
            return new MapperConfiguration(configure =>
                configure.CreateMap<Entities.Appointment, Models.Appointment>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src =>
                src.ContactPersonFullName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src.AppointmentType.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                src.AppointmentLocation.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                src.AppointmentAddress))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src =>
                src.CustomerAccount.CompanyName))
                .ForMember(dest => dest.Attendee, opt => opt.MapFrom(src =>
                $"{src.StaffMember.FirstName} {src.StaffMember.LastName}")))
                .CreateMapper().Map<IEnumerable<Models.Appointment>>(appointments);
        }

        public static Entities.Appointment GetAppointmentEntityFromModel(Models.Appointment appointment)
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
