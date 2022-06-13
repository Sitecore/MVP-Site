using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sitecore.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mvp.Environment.Sitecore.Healthcheck
{
    public class ApplicationInitializationCheck : IHealthCheck
    {
        public ApplicationInitializationCheck() { }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var machineName = "CM"; /*Environment.GetEnvironmentVariable("Sitecore_InstanceName") ?? Environment.MachineName;*/
            if ((string)HttpContext.Current.Cache.Get("APPINIT") == "1")
            {
                Log.Warn($"Warmup is in progress of {machineName}", this);
                return HealthCheckResult.Unhealthy($"Warmup is in progress of {machineName}");
            }

            Log.Info($"Warmup is complete for {machineName}", this);

            return HealthCheckResult.Healthy();
        }
    }
}