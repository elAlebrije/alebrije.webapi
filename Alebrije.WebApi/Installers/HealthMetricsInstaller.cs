using Alebrije.WebApi.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alebrije.WebApi.Installers
{
    public class HealthMetricsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMetrics();
            services.AddHealthChecks();
        }

    }
}