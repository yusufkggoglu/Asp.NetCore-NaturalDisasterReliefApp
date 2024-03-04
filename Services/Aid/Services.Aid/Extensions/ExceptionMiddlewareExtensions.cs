using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Aid.Logging;
using Services.Aid.Models;
using Shared.Dtos;
using System.Runtime.CompilerServices;

namespace Services.Aid.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerService logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "applicaton/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        //context.Response.StatusCode = contextFeature.Error switch
                        //{
                        //    NotFoundException => StatusCodes.Status404NotFound,
                        //    _ => StatusCodes.Status500InternalServerError
                        //};

                        logger.LogError($"Something went wrong : {contextFeature.Error.Message}");
                        var response = Response<NoContent>.Fail(contextFeature.Error.Message, 404).ToString();
                        await context.Response.WriteAsync(response);
                    }
                });
            });
        }
    }
}
