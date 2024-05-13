using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace NaturalDisasters.IdentityServer.ActionFilters
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelState in context.ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }

                var response = new Response<object>
                {
                    StatusCode = 400,
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(response) 
                {
                    StatusCode = 400 
                };
            }
        }
    }
}
