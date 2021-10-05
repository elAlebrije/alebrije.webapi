using Alebrije.WebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Alebrije.WebApi.Installers
{
    public class ConfigurationInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(o =>
            {
                o.AllowSynchronousIO = true;
            });
            services.AddControllers()
                .AddNewtonsoftJson(nsj =>
                {
                    nsj.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    nsj.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }
    }
}