using System;
using System.Collections.Generic;
using System.Linq;
using Alebrije.WebApi.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alebrije.WebApi.Extensions
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssemblies(this IServiceCollection services, IConfiguration configuration)
        {

            var settings = configuration.GetSection("MicroServiceConfig");
            var name = settings["Name"];

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName != null
                                   && (assembly.FullName.Contains("Alebrije.") || assembly.FullName.Contains(name)))
                .AsEnumerable();

            foreach (var assembly in assemblies)
            {
                var installers = assembly.ExportedTypes
                    .Where(et => typeof(IInstaller).IsAssignableFrom(et) && !et.IsInterface && !et.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IInstaller>()
                    .ToList();
                InstallServices(services, configuration, installers);
            }
        }

        private static void InstallServices(IServiceCollection services, IConfiguration configuration,
            IEnumerable<IInstaller> installers)
        {
            foreach (var installer in installers)
            {
                installer.InstallServices(services, configuration);
            }
        }
    }
}