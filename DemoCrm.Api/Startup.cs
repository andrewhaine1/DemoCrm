using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using DemoCrm.Data.Contexts;
using DemoCrm.Data.Helpers;
using DemoCrm.Data.Models;
using DemoCrm.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace DemoCrm.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
      
                //setupAction.InputFormatters.Add(new XmlSerializerInputFormatter(setupAction));
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(setupAction));

                var xmlDataContractSerializerOutputFormatter = new XmlDataContractSerializerOutputFormatter();
                xmlDataContractSerializerOutputFormatter.SerializerSettings.KnownTypes = new List<Type> { typeof(List<HypermediaLink>) };
                setupAction.OutputFormatters.Add(xmlDataContractSerializerOutputFormatter);

                var jsonInputFormatter = setupAction.InputFormatters
                .OfType<JsonInputFormatter>().FirstOrDefault();

                if (jsonInputFormatter != null)
                {
                    jsonInputFormatter.SupportedMediaTypes
                    .Add("application/vnd.onesoft.hateoas+json");
                }

                var jsonOutputFormatter = setupAction.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.onesoft.hateoas+json");
                }

            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionString = Configuration["ConnectionStrings:DemoCrmDatabase"];
            services.AddDbContext<DemoCrmDBContext>(d => d.UseSqlServer(connectionString));

            services.AddScoped<IDemoCrmRepository, DemoCrmRepository>();

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext =
                implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddHttpCacheHeaders(
                (expirationModelOptions) =>
                {
                    expirationModelOptions.MaxAge = 600;
                },
                (validationModelOptions) =>
                {
                    validationModelOptions.MustRevalidate = true;
                });

            services.AddResponseCaching();

            //services.Configure<IpRateLimitOptions>((options) =>
            //{
            //    options.GeneralRules = new List<RateLimitRule>()
            //    {
            //        new RateLimitRule()
            //        {
            //            Endpoint = "*",
            //            Limit = 10,
            //            Period = "5m"
            //        },
            //        new RateLimitRule()
            //        {
            //            Endpoint = "*",
            //            Limit = 2,
            //            Period = "10s"
            //        }
            //    };
            //});

            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global Exception Logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault occured. Try again later");
                    });
                });
            }

            app.UseHttpsRedirection();

            //app.UseIpRateLimiting();

            app.UseResponseCaching();          

            app.UseHttpCacheHeaders();
            
            app.UseMvc();
        }
    }
}
