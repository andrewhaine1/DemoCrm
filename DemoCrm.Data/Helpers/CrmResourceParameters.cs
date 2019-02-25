using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Helpers
{
    public class CrmResourceParameters
    {
        const int maxPageSize = 20;
        public int PageNumber { get; set; } = 1;

        private int _pagesize = 10;

        public int PageSize
        {
            get
            {
                return _pagesize;
            }

            set
            {
                _pagesize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Type { get; set; }

        public string Fields { get; set; }
    }
}
