using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class CustomerAccount
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Registration { get; set; }

        public string Vat { get; set; }

        public string Phone { get; set; }
    }
}
