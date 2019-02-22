using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DemoCrm.Data.Entities
{
    public abstract class DemoCrmEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
