using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog;
using Services.Aid.ActionFilters;
using Services.Aid.Extensions;
using Services.Aid.Logging;
using Services.Aid.Services;
using Services.Aid.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Aid
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   //logging
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            NLog.GlobalDiagnosticsContext.Set("LogDirectory", logPath);
            services.AddSingleton<ILoggerService, LoggerManager>();
            services.AddSingleton<LogFilterAttribute>();

            services.AddScoped<ValidationFilterAttribute>();
            services.Configure<ApiBehaviorOptions>(options =>options.SuppressModelStateInvalidFilter = true);

            services.AddLogging();
            //caching
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(expirationOpt =>
            {
                expirationOpt.MaxAge = 70;
                expirationOpt.CacheLocation = CacheLocation.Private;
            },
                validationOpt =>
                {
                    validationOpt.MustRevalidate = false;
                }
            );
            //rate limit
            services.AddMemoryCache();
                    var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint = "*",
                    Limit = 100,  
			        Period ="1m",
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddHttpContextAccessor();
            //other record
            services.AddScoped<IBasisAidService, BasisAidService>();
            services.AddScoped<IHumaneAidService, HumaneAidService>();
            services.AddAutoMapper(typeof(Startup));
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            services.AddSingleton<IDatabaseSettings>(sp =>
            {
                return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            });
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.Authority = Configuration["IdentityServerURL"];
                opt.Audience = "resource_aid";
                opt.RequireHttpsMetadata = false;
            });
            services.AddControllers(config =>
            {
                config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });
                config.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));

            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
                // Diðer roller için benzer politikalar ekleyebilirsiniz.
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Services.Aid", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Services.Aid v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseIpRateLimiting();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerService>();
            app.ConfigureExceptionHandler(logger);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
