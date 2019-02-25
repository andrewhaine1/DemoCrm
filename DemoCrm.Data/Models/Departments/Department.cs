using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Models
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }

        private string manager;
        public string Manager
        {
            get { return manager ?? "Not Set"; }
            set { manager = value; }
        }
    }
}
