using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class AppointmentBase
    {
        public string Subject { get; set; }

        public DateTime Time { get; set; }

        public string ContactPersonFullName { get; set; }

        public Guid AppointmentLocationId { get; set; }

        public Guid AppointmentTypeId { get; set; }

        public string AppointmentAddress { get; set; }

        public bool IsCompleted { get; set; }

        public Guid StaffMemberId { get; set; }

        public Guid CustomerAccountId { get; set; }
    }
}
