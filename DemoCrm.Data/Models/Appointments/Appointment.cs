using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public DateTime Time { get; set; }

        public string Contact { get; set; }

        public string Type { get; set; }

        public string Location { get; set; }

        private string address;
        public string Address { get { return address; } set { address = value ?? "None set"; } }

        public string Customer { get; set; }

        public string Attendee { get; set; }

        public bool Completed { get; set; }
    }
}
