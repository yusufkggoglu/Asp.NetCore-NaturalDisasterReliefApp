using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace Services.Aid.ActionFilters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var modelErrors = new List<string>();
                foreach (var modelState in context.ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }
                var response= Response<NoContent>.Fail(modelErrors, 404);

                context.Result = new UnprocessableEntityObjectResult(response);
            }
        }
    }
}
