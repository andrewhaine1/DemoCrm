using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models.CustomerAccounts
{
    public class CustomerAccountBase
    {
        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string RegistrationNumber { get; set; }

        public string VatNumber { get; set; }

        public string PhoneNumber { get; set; }

        public Guid StaffMemberId { get; set; }

        public Guid CompanyId { get; set; }
    }
}
