using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class BusinessLead
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
