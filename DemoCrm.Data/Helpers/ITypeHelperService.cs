using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCrm.Data.Helpers
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
