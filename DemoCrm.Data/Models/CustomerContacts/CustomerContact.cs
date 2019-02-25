using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class CustomerContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
