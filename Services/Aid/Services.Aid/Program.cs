using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Services.Aid.Scheduler;
using Services.Aid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Aid
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var basisAidService = services.GetRequiredService<IBasisAidService>();
                var humaneAidService = services.GetRequiredService<IHumaneAidService>();

                var scheduler = new AidScheduler(basisAidService, humaneAidService);
                scheduler.Start();
            }


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(opt =>
                {
                    opt.ClearProviders();
                    opt.AddConsole();
                    opt.AddDebug();
                    opt.SetMinimumLevel(LogLevel.Error);
                }).UseNLog();

    }
}
