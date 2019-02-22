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
    }
}
