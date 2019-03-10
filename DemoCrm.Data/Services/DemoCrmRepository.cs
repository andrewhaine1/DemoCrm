using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoCrm.Data.Contexts;
using DemoCrm.Data.Entities;
using DemoCrm.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DemoCrm.Data.Services
{
    public class DemoCrmRepository : IDemoCrmRepository, IDisposable
    {
        private DemoCrmDBContext _context;
        private IPropertyMappingService _propertyMappingService;

        public DemoCrmRepository(DemoCrmDBContext demoCrmDBContext, IPropertyMappingService propertyMappingService)
        {
            _context = demoCrmDBContext;
            _propertyMappingService = propertyMappingService;
        }

        /*----------------------------------------------- CRM Object Types -------------------------------------------*/

        #region CRM Object Types
        public async Task<IEnumerable<CrmObjectType>> GetCrmObjectTypesAsync()
        {
            return await _context.CrmObjectTypes.ToListAsync();
        }

        public async Task<CrmObjectType> GetCrmObjectTypeIdAsync(string name)
        {
            return await _context.CrmObjectTypes.Where(t => t.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<CrmObjectType> GetCrmObjectTypeAsync(int id)
        {
            return await _context.CrmObjectTypes.Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        public void AddObjectType(CrmObjectType crmObjectType)
        {
            _context.CrmObjectTypes.Add(crmObjectType);
        }

        public async Task<bool> ObjectTypeExistsAsync(int id)
        {
            return await _context.CrmObjectTypes.AnyAsync(o => o.Id == id);
        }

        public void UpdateCrmObjectType(CrmObjectType crmObjectType)
        {
            _context.CrmObjectTypes.Update(crmObjectType);
        }

        public async Task<bool> CrmObjectTypeExitsAsync(string name)
        {
            return await _context.CrmObjectTypes.AnyAsync(t => t.Name == name);
        }

        public void DeleteObjectType(CrmObjectType crmObjectType)
        {
            _context.CrmObjectTypes.Remove(crmObjectType);
        }
        #endregion

        /*----------------------------------------------- CRM Users -------------------------------------------*/

        #region CRM Users
        public async Task<IEnumerable<CrmUser>> GetCrmUserCollectionAsync(IEnumerable<Guid> userIds)
        {
            return await _context.CrmUsers.Where(u => userIds.Contains(u.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CrmUser>> GetCrmUsersAsync()
        {
            return await _context.CrmUsers.ToListAsync();
        }

        public async Task<PagedList<CrmUser>> GetCrmUsersAsync(CrmResourceParameters crmResourceParameters)
        {
            //var collectionBeforePaging = _context.CrmUsers
            //    .Include(t => t.ObjectType)
            //    .OrderBy(u => u.FirstName)
            //    .ThenBy(u => u.LastName).AsQueryable();

            var collectionBeforePaging = _context.CrmUsers
                .Include(t => t.ObjectType)
                .ApplySort(crmResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<Models.CrmUser, Entities.CrmUser>());

            if (! string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(u => u.ObjectType.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.ObjectType.Name.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<CrmUser>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<CrmUser> GetCrmUserAsync(Guid id)
        {
            return await _context.CrmUsers
                .Include(t => t.ObjectType)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Guid> GetCrmUserIdAsync(Guid oauthId)
        {
            return await _context.CrmUsers
                .Where(c => c.OauthId == oauthId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public void AddCrmUser(CrmUser crmUser)
        {
            if (crmUser == null)
                throw new ArgumentNullException(nameof(crmUser));

            _context.Add(crmUser);
        }

        public async Task<bool> CrmUserExitsAsync(Guid oauthId)
        {
            return await _context.CrmUsers.AnyAsync(u => u.OauthId == oauthId);
        }

        public async Task<bool> CrmUserIdExits(Guid id)
        {
            return await _context.CrmUsers.AnyAsync(u => u.Id == id);
        }

        public void UpdateCrmUser(CrmUser crmUser)
        {
            _context.CrmUsers.Update(crmUser);
        }

        public void DeleteCrmUser(CrmUser crmUser)
        {
            _context.Remove(crmUser);
        }
        #endregion

        /*----------------------------------------------- Companies -------------------------------------------*/

        #region Companies
        public async Task<PagedList<Company>> GetCompaniesAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.Companies
                .Include(c => c.ObjectType)
                .Include(c => c.CrmUser)
                .ApplySort(crmResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<Models.Company, Company>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(u => u.ObjectType.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.ObjectType.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<Company>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid id)
        {
            return await _context.Companies
                .Include(c => c.ObjectType)
                .Include(c => c.CrmUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Company> GetCompanyByUserIdAsync(Guid userId)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CrmUserId == userId);
        }

        public async Task<bool> CompanyExistsAsync(Guid id)
        {
            return await _context.Companies.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CompanyNameExistsAsync(string name)
        {
            return await _context.Companies.AnyAsync(c => c.Name == name);
        }

        public void AddCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            _context.Companies.Add(company);
        }

        public void UpdateCompany(Company company)
        {
            _context.Companies.Update(company);
        }

        public void DeleteCompany(Company company)
        {
            _context.Companies.Remove(company);
        }
        #endregion

        /*----------------------------------------------- Departments -------------------------------------------*/

        #region Departments
        public async Task<PagedList<Department>> GetDepartmentsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.Departments
                .Include(d => d.Company)
                .Include(d => d.Manager)
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.Department, Department>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<Department>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsForCompanyAsync(Guid companyId)
        {
            return await _context.Departments
                .Where(d => d.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentAsync(Guid id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> DepartmentNameExitsAsync(string name)
        {
            return await _context.Departments.AnyAsync(d => d.Name == name);
        }

        public async Task<bool> DepartmentExitsAsync(Guid Id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == Id);
        }

        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
        }

        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
        }

        public void DeleteDepartment(Department department)
        {
            _context.Departments.Remove(department);
        }
        #endregion

        /*----------------------------------------------- Staff Members -------------------------------------------*/

        #region Departments
        public async Task<PagedList<StaffMember>> GetStaffMembersAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.StaffMembers
                .Include(s => s.StaffPosition)
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.StaffMember, StaffMember>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<StaffMember>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<StaffMember> GetStaffMemberAsync(Guid id)
        {
            return await _context.StaffMembers
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> StaffMemberEmailExitsAsync(string email)
        {
            return await _context.StaffMembers.AnyAsync(d => d.Email == email);
        }

        public async Task<bool> StaffMemberExitsAsync(Guid Id)
        {
            return await _context.StaffMembers.AnyAsync(d => d.Id == Id);
        }

        public void AddStaffMember(StaffMember staffMember)
        {
            _context.StaffMembers.Add(staffMember);
        }

        public void UpdateStaffMember(StaffMember staffMember)
        {
            _context.StaffMembers.Update(staffMember);
        }

        public void DeleteStaffMember(StaffMember staffMember)
        {
            _context.StaffMembers.Remove(staffMember);
        }
        #endregion

        /*----------------------------------------------- Staff Positions -------------------------------------------*/

        #region StaffPositions
        public async Task<PagedList<StaffPosition>> GetStaffPositionsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.StaffPositions
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.StaffPosition, StaffPosition>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<StaffPosition>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<StaffPosition> GetStaffPositionAsync(Guid id)
        {
            return await _context.StaffPositions
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> StaffPositionNameExitsAsync(string name)
        {
            return await _context.StaffPositions.AnyAsync(d => d.Name == name);
        }

        public async Task<bool> StaffPositionExitsAsync(Guid Id)
        {
            return await _context.StaffPositions.AnyAsync(d => d.Id == Id);
        }

        public void AddStaffPosition(StaffPosition staffPosition)
        {
            _context.StaffPositions.Add(staffPosition);
        }

        public void UpdateStaffPosition(StaffPosition staffPosition)
        {
            _context.StaffPositions.Update(staffPosition);
        }

        public void DeleteStaffPosition(StaffPosition staffPosition)
        {
            _context.StaffPositions.Remove(staffPosition);
        }
        #endregion

        /*----------------------------------------------- Business Leads -------------------------------------------*/

        #region BusinessLeads
        public async Task<PagedList<BusinessLead>> GetBusinessLeadsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.BusinessLeads
                .Include(b => b.LeadManager)
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.StaffPosition, StaffPosition>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<BusinessLead>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<BusinessLead> GetBusinessLeadAsync(Guid id)
        {
            return await _context.BusinessLeads
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> BusinessLeadExitsAsync(Guid Id)
        {
            return await _context.BusinessLeads.AnyAsync(d => d.Id == Id);
        }

        public void AddBusinessLead(BusinessLead businessLead)
        {
            _context.BusinessLeads.Add(businessLead);
        }

        public void UpdateBusinessLead(BusinessLead businessLead)
        {
            _context.BusinessLeads.Update(businessLead);
        }

        public void DeleteBusinessLead(BusinessLead businessLead)
        {
            _context.BusinessLeads.Remove(businessLead);
        }
        #endregion

        /*----------------------------------------------- Customer Accounts -------------------------------------------*/

        #region CustomerAccounts
        public async Task<PagedList<CustomerAccount>> GetCustomerAccountsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.CustomerAccounts
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.CustomerAccount, CustomerAccount>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.CompanyName == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.CompanyName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<CustomerAccount>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<CustomerAccount> GetCustomerAccountAsync(Guid id)
        {
            return await _context.CustomerAccounts
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CustomerAccountExitsAsync(Guid Id)
        {
            return await _context.CustomerAccounts.AnyAsync(d => d.Id == Id);
        }

        public void AddCustomerAccount(CustomerAccount customerAccount)
        {
            _context.CustomerAccounts.Add(customerAccount);
        }

        public void UpdateCustomerAccount(CustomerAccount customerAccount)
        {
            _context.CustomerAccounts.Update(customerAccount);
        }

        public void DeleteCustomerAccount(CustomerAccount customerAccount)
        {
            _context.CustomerAccounts.Remove(customerAccount);
        }
        #endregion

        /*----------------------------------------------- Customer Contacts -------------------------------------------*/

        #region CustomerContacts
        public async Task<PagedList<CustomerContact>> GetCustomerContactsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.CustomerContacts
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.CustomerContact, CustomerContact>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<CustomerContact>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<CustomerContact> GetCustomerContactAsync(Guid id)
        {
            return await _context.CustomerContacts
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CustomerContactExitsAsync(Guid Id)
        {
            return await _context.CustomerContacts.AnyAsync(d => d.Id == Id);
        }

        public void AddCustomerContact(CustomerContact customerContact)
        {
            _context.CustomerContacts.Add(customerContact);
        }

        public void UpdateCustomerContact(CustomerContact customerContact)
        {
            _context.CustomerContacts.Update(customerContact);
        }

        public void DeleteCustomerContact(CustomerContact customerContact)
        {
            _context.CustomerContacts.Remove(customerContact);
        }
        #endregion

        /*----------------------------------------------- Appointments -------------------------------------------*/

        #region Appointments
        public async Task<PagedList<Appointment>> GetAppointmentsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.Appointments
                .Include(a =>a.AppointmentType)
                .Include(a => a.AppointmentLocation)
                .Include(a => a.CustomerAccount)
                .Include(a => a.StaffMember)
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.Appointment, Appointment>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.ContactPersonFullName == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.ContactPersonFullName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<Appointment>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<Appointment> GetAppointmentAsync(Guid id)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> AppointmentExitsAsync(Guid Id)
        {
            return await _context.Appointments.AnyAsync(d => d.Id == Id);
        }

        public void AddAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        public void DeleteAppointment(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
        }
        #endregion

        /*----------------------------------------------- Appointment Types -------------------------------------------*/
        #region AppointmentTypes
        public async Task<PagedList<AppointmentType>> GetAppointmentTypesAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.AppointmentTypes
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.AppointmentType, AppointmentType>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<AppointmentType>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<AppointmentType> GetAppointmentTypeAsync(Guid id)
        {
            return await _context.AppointmentTypes
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> AppointmentTypeExitsAsync(Guid Id)
        {
            return await _context.AppointmentTypes.AnyAsync(d => d.Id == Id);
        }

        public void AddAppointmentType(AppointmentType appointmentType)
        {
            _context.AppointmentTypes.Add(appointmentType);
        }

        public void UpdateAppointmentType(AppointmentType appointmentType)
        {
            _context.AppointmentTypes.Update(appointmentType);
        }

        public void DeleteAppointmentType(AppointmentType appointmentType)
        {
            _context.AppointmentTypes.Remove(appointmentType);
        }
        #endregion
        /*----------------------------------------------- Appointment Locations -------------------------------------------*/

        #region AppointmentTypes
        public async Task<PagedList<AppointmentLocation>> GetAppointmentLocationsAsync(CrmResourceParameters crmResourceParameters)
        {
            var collectionBeforePaging = _context.AppointmentLocations
               .ApplySort(crmResourceParameters.OrderBy,
               _propertyMappingService.GetPropertyMapping<Models.AppointmentLocation, AppointmentLocation>());

            if (!string.IsNullOrEmpty(crmResourceParameters.Type))
            {
                var objectTypeClause = crmResourceParameters.Type
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name == objectTypeClause);
            }

            if (!string.IsNullOrEmpty(crmResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = crmResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(d => d.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return await PagedList<AppointmentLocation>.Create(collectionBeforePaging,
                crmResourceParameters.PageNumber,
                crmResourceParameters.PageSize);
        }

        public async Task<AppointmentLocation> GetAppointmentLocationAsync(Guid id)
        {
            return await _context.AppointmentLocations
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> AppointmentLocationExitsAsync(Guid Id)
        {
            return await _context.AppointmentLocations.AnyAsync(d => d.Id == Id);
        }

        public void AddAppointmentLocation(AppointmentLocation appointmentLocation)
        {
            _context.AppointmentLocations.Add(appointmentLocation);
        }

        public void UpdateAppointmentLocation(AppointmentLocation appointmentLocation)
        {
            _context.AppointmentLocations.Update(appointmentLocation);
        }

        public void DeleteAppointmentLocation(AppointmentLocation appointmentLocation)
        {
            _context.AppointmentLocations.Remove(appointmentLocation);
        }
        #endregion

        #region Save
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
        #endregion

        #region Disposal
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        #endregion
    }
}
