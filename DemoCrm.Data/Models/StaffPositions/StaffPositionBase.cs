using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Models.StaffPositions
{
    public abstract class StaffPositionBase
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
    }
}
