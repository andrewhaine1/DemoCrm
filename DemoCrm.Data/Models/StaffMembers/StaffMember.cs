using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class StaffMember
    {
        public string Name { get; set; }

        public Guid Position { get; set; }

        public string Email { get; set; }

        public string Phone{ get; set; }

        public bool Manager { get; set; }
    }
}
