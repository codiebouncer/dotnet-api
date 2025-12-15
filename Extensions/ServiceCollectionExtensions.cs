using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PropMan.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register all Repositories that follow the pattern
            var repoTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Repository") && t.IsClass && !t.IsAbstract);

            foreach (var repoType in repoTypes)
            {
                var interfaceType = repoType.GetInterface($"I{repoType.Name}");
                if (interfaceType != null)
                    services.AddScoped(interfaceType, repoType);
            }

            // Register all Services that follow the pattern
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service") && t.IsClass && !t.IsAbstract);

            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterface($"I{serviceType.Name}");
                if (interfaceType != null)
                    services.AddScoped(interfaceType, serviceType);
            }

            return services;
        }
    }
}
