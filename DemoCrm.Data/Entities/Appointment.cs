using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("Appointments")]
    public class Appointment : DemoCrmEntity
    {
        [Required]
        [MaxLength(150)]
        public string Subject { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        [MaxLength(150)]
        public string ContactPersonFullName { get; set; }

        [ForeignKey("AppointmentLocationId")]
        public AppointmentLocation AppointmentLocation { get; set; }

        [Required]
        public Guid AppointmentLocationId { get; set; }

        [ForeignKey("AppointmentTypeId")]
        public AppointmentType AppointmentType { get; set; }

        [Required]
        public Guid AppointmentTypeId { get; set; }

        [MaxLength(500)]
        public string AppointmentAddress { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [ForeignKey("StaffMemberId")]
        public StaffMember StaffMember { get; set; }

        [Required]
        public Guid StaffMemberId { get; set; }

        [ForeignKey("CustomerAccountId")]
        public CustomerAccount CustomerAccount { get; set; }

        [Required]
        public Guid CustomerAccountId { get; set; }
    }
}
