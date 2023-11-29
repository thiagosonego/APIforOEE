using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OEE.Application.Commands;
using OEE.Domain.Interfaces;
using OEE.Extensions;
using OEE.Infrastructure.Dapper.ConnectionFactory;
using OEE.Infrastructure.Dapper.Queries;
using OEE.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace OEE.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(ActualOEE))));
            services.AddScoped<ICalendarsRepository, CalendarsRepository>();
            services.AddScoped<IDowntimeRepository, DowntimeRepository>();
            services.AddScoped<IOEERepository, OEERepository>();
            services.AddScoped<ICalendarsQueries, CalendarsQueries>();
            services.AddScoped<IDowntimeQueries, DowntimeQueries>();
            services.AddScoped<IOEEQueries, OEEQueries>();
            services.AddCustomSwagger();
            services.AddLocalization(opt => opt.ResourcesPath = "Resources");

            services.AddScoped<IConnectionFactory, ConnectionFactory>(
                        options =>
                            new ConnectionFactory(
                                Configuration.GetConnectionString("MESDbContext")
                            )
                        );

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"), new CultureInfo("fr")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
                app.UsePathBase(pathBase);

            app.UseCors("CorsPolicy");

            AddCultureInfos(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            UseSwagger(app, pathBase);
        }

        private static void AddCultureInfos(IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("fr"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }

        private static void UseSwagger(IApplicationBuilder app, string pathBase)
        {
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                        "API to update Andon of OEE for CCGL");
                    c.OAuthClientId("API to update Andon of OEE for CCGL");
                    c.OAuthAppName("API to update Andon of OEE for CCGL");
                });
        }
    }
}