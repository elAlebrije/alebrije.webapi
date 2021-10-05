using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Alebrije.WebApi.Extensions;
using Alebrije.WebApi.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Alebrije.WebApi.Infrastructure
{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration { get; }

        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected virtual void ConfigureModule(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssemblies(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ConfigureSwagger(app);
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            status = report.Status.ToString(),
                            errors = report.Entries.Select(e => new
                            {
                                key = e.Key,
                                value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            })
                        });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureModule(app, env);
        }

        private void ConfigureSwagger(IApplicationBuilder app)
        {
            var microServiceConfig = new VersionSettings();
            Configuration.Bind(nameof(microServiceConfig), microServiceConfig);

            app.UseSwagger();
            foreach (var ver in microServiceConfig.AllowedVersions)
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{ver}/swagger.json", $"{microServiceConfig.Name} v{ver}"));
            }
        }
    }
}
