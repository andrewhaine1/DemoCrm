using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Helpers
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));

            StatusCode = 422;
        }
    }
}
