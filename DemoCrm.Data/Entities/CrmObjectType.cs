using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DemoCrm.Data.Entities
{
    [Table("CrmObjectTypes")]
    public class CrmObjectType : DemoCrmEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
    }
}
