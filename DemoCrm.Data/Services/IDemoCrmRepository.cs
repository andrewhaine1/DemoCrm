using DemoCrm.Data.Entities;
using DemoCrm.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoCrm.Data.Services
{
    public interface IDemoCrmRepository
    {
        /*----------------------------------------------- CRM Object Types -------------------------------------------*/

        #region CrmObjectTypes
        Task<IEnumerable<CrmObjectType>> GetCrmObjectTypesAsync();

        Task<CrmObjectType> GetCrmObjectTypeAsync(int id);

        Task<CrmObjectType> GetCrmObjectTypeIdAsync(string name);

        /// <summary>
        /// Add new CrmObjectType
        /// </summary>
        /// <param name="crmUser"></param>
        /// <returns>Task</returns>
        void AddObjectType(CrmObjectType crmObjectType);

        Task<bool> ObjectTypeExistsAsync(int id);

        /// <summary>
        /// Updates a CrmObjectType.
        /// </summary>
        /// <param name="crmUser">The CrmUser to update.</param>
        void UpdateCrmObjectType(CrmObjectType crmObjectType);

        /// <summary>
        /// Delete CrmObjectType.
        /// </summary>
        /// <param name="crmUser">An instance of CrmUser of the user to be deleted.</param>
        void DeleteObjectType(CrmObjectType crmObjectType);

        /// <summary>
        /// Checks if a CRM Object Type exists.
        /// </summary>
        /// <param name="id">The Id issued by SQL Server.</param>
        /// <returns>A bool value indicating whether the Id is present in the database or not.</returns>
        Task<bool> CrmObjectTypeExitsAsync(string name);
        #endregion

        /*----------------------------------------------- CRM Users -------------------------------------------*/
        
        #region CrmUsers
        /// <summary>
        /// Gets all CRM Users
        /// </summary>
        /// <returns>A list of CrmUser</returns>
        Task<IEnumerable<CrmUser>> GetCrmUsersAsync();

        /// <summary>
        /// Gets one CRM user
        /// </summary>
        /// <param name="id">The Guid Id of the CRM User</param>
        /// <returns>A CrmUser</returns>
        Task<CrmUser> GetCrmUserAsync(Guid id);

        /// <summary>
        /// Gets one CRM user Id.
        /// </summary>
        /// <param name="id">The Oauth Id of the CRM User received from Identity server.</param>
        /// <returns>The CrmUser Id.</returns>
        Task<Guid> GetCrmUserIdAsync(Guid oauthId);

        Task<PagedList<CrmUser>> GetCrmUsersAsync(CrmResourceParameters crmUsersPageSizeParameters);

        /// <summary>
        /// Gets a collection of CRM users
        /// </summary>
        /// <param name="userIds">IEnumerable of user Id's (GUID).</param>
        /// <returns>IEnumerable of CRM users.</returns>
        Task<IEnumerable<CrmUser>> GetCrmUserCollectionAsync(IEnumerable<Guid> userIds);

        /// <summary>
        /// Checks if a CRM user exists using the Oauth Id.
        /// </summary>
        /// <param name="oauthId">The Oauth Id as issued by identity server.</param>
        /// <returns>A bool value indicating whethere the Oauth Id is present in the database or not.</returns>
        Task<bool> CrmUserExitsAsync(Guid oauthId);

        /// <summary>
        /// Checks if a CRM user exists using the Id.
        /// </summary>
        /// <param name="id">The Id issued by SQL Server.</param>
        /// <returns>A bool value indicating whethere the Id is present in the database or not.</returns>
        Task<bool> CrmUserIdExits(Guid id);

        /// <summary>
        /// Add new CRM User
        /// </summary>
        /// <param name="crmUser"></param>
        /// <returns>Task</returns>
        void AddCrmUser(CrmUser crmUser);

        /// <summary>
        /// Updates a Crm User Entity.
        /// </summary>
        /// <param name="crmUser">The CrmUser to update.</param>
        void UpdateCrmUser(CrmUser crmUser);
       
        /// <summary>
        /// Delete CRM user.
        /// </summary>
        /// <param name="crmUser">An instance of CrmUser of the user to be deleted.</param>
        void DeleteCrmUser(CrmUser crmUser);
        #endregion

        /*----------------------------------------------- Companies -------------------------------------------*/

        #region Companies
        Task<PagedList<Company>> GetCompaniesAsync(CrmResourceParameters crmResourceParameters);

        Task<Company> GetCompanyAsync(Guid id);

        Task<Company> GetCompanyByUserIdAsync(Guid userId);

        Task<bool> CompanyNameExistsAsync(string name);

        Task<bool> CompanyExistsAsync(Guid id);

        void AddCompany(Company company);

        void UpdateCompany(Company company);

        void DeleteCompany(Company company);
        #endregion

        /*----------------------------------------------- Departments -------------------------------------------*/

        #region Departments
        Task<PagedList<Department>> GetDepartmentsAsync(CrmResourceParameters crmResourceParameters);

        Task<Department> GetDepartmentAsync(Guid id);

        Task<IEnumerable<Department>> GetDepartmentsForCompanyAsync(Guid companyId);

        Task<bool> DepartmentExitsAsync(Guid Id);

        Task<bool> DepartmentNameExitsAsync(string name);

        void AddDepartment(Department department);

        void UpdateDepartment(Department department);

        void DeleteDepartment(Department department);
        #endregion

        /*----------------------------------------------- Staff Members -------------------------------------------*/

        #region Staff Members
        Task<PagedList<StaffMember>> GetStaffMembersAsync(CrmResourceParameters crmResourceParameters);

        Task<StaffMember> GetStaffMemberAsync(Guid id);

        Task<bool> StaffMemberEmailExitsAsync(string email);

        Task<bool> StaffMemberExitsAsync(Guid Id);

        void AddStaffMember(StaffMember staffMember);

        void UpdateStaffMember(StaffMember staffMember);

        void DeleteStaffMember(StaffMember staffMember);
        #endregion

        /*----------------------------------------------- Staff Positions -------------------------------------------*/

        #region Staff Positions
        Task<PagedList<StaffPosition>> GetStaffPositionsAsync(CrmResourceParameters crmResourceParameters);

        Task<StaffPosition> GetStaffPositionAsync(Guid id);

        Task<bool> StaffPositionNameExitsAsync(string name);

        Task<bool> StaffPositionExitsAsync(Guid Id);

        void AddStaffPosition(StaffPosition staffPosition);

        void UpdateStaffPosition(StaffPosition staffPosition);

        void DeleteStaffPosition(StaffPosition staffPosition);
        #endregion

        /*----------------------------------------------- Business Leads -------------------------------------------*/

        #region Business Leads
        Task<PagedList<BusinessLead>> GetBusinessLeadsAsync(CrmResourceParameters crmResourceParameters);

        Task<BusinessLead> GetBusinessLeadAsync(Guid id);

        Task<bool> BusinessLeadExitsAsync(Guid Id);

        void AddBusinessLead(BusinessLead businessLead);

        void UpdateBusinessLead(BusinessLead businessLead);

        void DeleteBusinessLead(BusinessLead businessLead);
        #endregion

        /*----------------------------------------------- Customer Accounts -------------------------------------------*/
        
        #region Customer Accounts
        Task<PagedList<CustomerAccount>> GetCustomerAccountsAsync(CrmResourceParameters crmResourceParameters);

        Task<CustomerAccount> GetCustomerAccountAsync(Guid id);

        Task<bool> CustomerAccountExitsAsync(Guid Id);

        void AddCustomerAccount(CustomerAccount customerAccount);

        void UpdateCustomerAccount(CustomerAccount customerAccount);

        void DeleteCustomerAccount(CustomerAccount customerAccount);
        #endregion

        /*----------------------------------------------- Customer Contacts -------------------------------------------*/

        #region Customer Contacts
        Task<PagedList<CustomerContact>> GetCustomerContactsAsync(CrmResourceParameters crmResourceParameters);

        Task<CustomerContact> GetCustomerContactAsync(Guid id);

        Task<bool> CustomerContactExitsAsync(Guid Id);

        void AddCustomerContact(CustomerContact customerContact);

        void UpdateCustomerContact(CustomerContact customerContact);

        void DeleteCustomerContact(CustomerContact customerContact);
        #endregion

        /*----------------------------------------------- Appointments -------------------------------------------*/

        #region Appointments
        Task<PagedList<Appointment>> GetAppointmentsAsync(CrmResourceParameters crmResourceParameters);

        Task<Appointment> GetAppointmentAsync(Guid id);

        Task<bool> AppointmentExitsAsync(Guid Id);

        void AddAppointment(Appointment appointment);

        void UpdateAppointment(Appointment appointment);

        void DeleteAppointment(Appointment appointment);
        #endregion

        /*----------------------------------------------- Appointment Types -------------------------------------------*/

        #region Appointmen Types
        Task<PagedList<AppointmentType>> GetAppointmentTypesAsync(CrmResourceParameters crmResourceParameters);

        Task<AppointmentType> GetAppointmentTypeAsync(Guid id);

        Task<bool> AppointmentTypeExitsAsync(Guid Id);

        void AddAppointmentType(AppointmentType appointmentType);

        void UpdateAppointmentType(AppointmentType appointmentType);

        void DeleteAppointmentType(AppointmentType appointmentType);
        #endregion

        /*----------------------------------------------- Appointment Locations -------------------------------------------*/
        #region Appointmen Locations
        Task<PagedList<AppointmentLocation>> GetAppointmentLocationsAsync(CrmResourceParameters crmResourceParameters);

        Task<AppointmentLocation> GetAppointmentLocationAsync(Guid id);

        Task<bool> AppointmentLocationExitsAsync(Guid Id);

        void AddAppointmentLocation(AppointmentLocation appointmentLocation);

        void UpdateAppointmentLocation(AppointmentLocation appointmentLocation);

        void DeleteAppointmentLocation(AppointmentLocation appointmentLocation);
        #endregion


        #region Save
        /// <summary>
        /// Saves changes to the databse.
        /// </summary>
        /// <returns>Task of type bool to indacate whether change has happened successfullty.</returns>
        Task<bool> SaveChangesAsync();

        /// <summary>
        /// Saves changes to the databse.
        /// </summary>
        /// <returns>Task of type bool to indacate whether change has happened successfullty.</returns>
        bool SaveChanges();
        #endregion
    }
}
