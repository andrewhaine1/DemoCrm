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

        [Required]
        [ForeignKey("AppointLocation.Id")]
        public Guid AppointmentLocationId { get; set; }

        [Required]
        [ForeignKey("AppointType.Id")]
        public Guid AppointmentTypeId { get; set; }

        [MaxLength(500)]
        public string AppointmentAddress { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [ForeignKey("StaffMember.Id")]
        [Column("StaffAtendee")]
        public Guid StaffMemberId { get; set; }

        [Required]
        [ForeignKey("CustomerAccount.Id")]
        public Guid CustomerAccountId { get; set; }
    }
}
