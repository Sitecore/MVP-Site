using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Mvp.Environment.Sitecore.Healthcheck;
using Sitecore.DependencyInjection;
using Sitecore.HealthCheck.DependencyInjection;

namespace Mvp.Environment.Sitecore.Dependencies
{
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks()
              .AddCheck<ApplicationInitializationCheck>(
                  "Application Initialization",
                  HealthStatus.Unhealthy, new[] { "ready" }
              );
        }
    }
}
