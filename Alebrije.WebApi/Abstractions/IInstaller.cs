using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alebrije.WebApi.Abstractions
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}