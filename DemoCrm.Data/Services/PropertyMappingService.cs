using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Data.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _crmUserPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
               { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
               { "Email", new PropertyMappingValue(new List<string>() { "Email" }) },
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
           };

        private readonly Dictionary<string, PropertyMappingValue> _crmCompanyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) },
                { "Phone", new PropertyMappingValue(new List<string>() { "PhoneNumber" }) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmDepartmentPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmStaffMemberPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
                { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
                { "Manager", new PropertyMappingValue(new List<string>() { "IsManager"}) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmStaffPositionPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmBusinessLeadPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
                { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
                { "Company", new PropertyMappingValue(new List<string>() { "Company"}) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmCustomerAccountPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "CompanyName" }) },
                { "Address", new PropertyMappingValue(new List<string>() { "Address" }) },
                { "Registration", new PropertyMappingValue(new List<string>() { "RegistrationNumber" }) },
                { "Vat", new PropertyMappingValue(new List<string>() { "VatNumber" }) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmCustomerContactPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
                { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email" }) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
                { "Department", new PropertyMappingValue(new List<string>() { "Department" }) },
                { "Position", new PropertyMappingValue(new List<string>() { "Position" }) }
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmAppointmentPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Subject", new PropertyMappingValue(new List<string>() { "Subject" }) },
                { "Time", new PropertyMappingValue(new List<string>() { "Time" }) },
                { "Contact", new PropertyMappingValue(new List<string>() { "ContactPersonFullName" }) },
                { "Completed", new PropertyMappingValue(new List<string>() { "IsCompleted" }) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmAppointmentTypePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) },
            };

        private readonly Dictionary<string, PropertyMappingValue> _crmAppointmentLocationPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) },
            };


        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<Models.CrmUser,
                Entities.CrmUser>(_crmUserPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Company,
                Entities.Company>(_crmCompanyPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmDepartmentPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmStaffMemberPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmStaffPositionPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmBusinessLeadPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmCustomerAccountPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmCustomerContactPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmAppointmentPropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmAppointmentTypePropertyMapping));

            propertyMappings.Add(new PropertyMapping<Models.Department,
                Entities.Department>(_crmAppointmentLocationPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue>  GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
