using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class Appointment
    {
        public string Subject { get; set; }

        public DateTime Time { get; set; }

        public string Contact { get; set; }

        public string Address { get; set; }

        public bool Completed { get; set; }
    }
}
