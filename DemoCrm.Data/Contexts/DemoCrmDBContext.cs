using DemoCrm.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Contexts
{
    public class DemoCrmDBContext : DbContext
    {
        public DbSet<CrmUser> CrmUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<StaffPosition> StaffPositions { get; set; }
        public DbSet<BusinessLead> BusinessLeads { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentLocation> AppointmentLocations { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<CrmObjectType> CrmObjectTypes { get; set; }

        public DemoCrmDBContext(DbContextOptions<DemoCrmDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CrmUser>(entity => {
                entity.HasIndex(u => u.OauthId).IsUnique();
            });
        }
    }
}
