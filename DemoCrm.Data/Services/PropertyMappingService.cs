using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Data.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _crmUserPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
               { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
               { "Email", new PropertyMappingValue(new List<string>() { "Email" }) },
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
           };

        private Dictionary<string, PropertyMappingValue> _crmCompanyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) },
                { "Phone", new PropertyMappingValue(new List<string>() { "PhoneNumber" }) }
            };

        private Dictionary<string, PropertyMappingValue> _crmDepartmentPropertyMapping =
            new Dictionary<string, PropertyMappingValue>
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" }) }
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
