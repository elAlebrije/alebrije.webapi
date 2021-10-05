using System.Collections.Generic;
using System.Linq;
using Alebrije.WebApi.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Alebrije.WebApi.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("MicroServiceConfig");
            var name = settings["Name"];
            var myArraySection = settings.GetSection("AllowedVersions").Get<List<string>>();
            var versions = myArraySection.AsEnumerable();

            services.AddSwaggerGen(c =>
            {
                foreach (var ver in versions)
                {
                    c.SwaggerDoc($"v{ver}", new OpenApiInfo
                    {
                        Title = name,
                        Version = $"v{ver}"
                    });
                }
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }
    }
}