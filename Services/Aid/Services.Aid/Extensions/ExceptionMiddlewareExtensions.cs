using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Aid.Logging;
using Services.Aid.Models;
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
                        var response = new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString();
                        await context.Response.WriteAsync(response);
                    }
                });
            });
        }
    }
}
