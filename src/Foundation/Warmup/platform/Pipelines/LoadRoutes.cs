using System.Web.Http;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Mvp.Foundation.Warmup.Pipelines
{
    public class LoadRoutes
    {
        public void Process(PipelineArgs args)
        {
            // Registration
            RouteTable.Routes.MapHttpRoute("warmup", "healthz/warm", new { action = "Warm", controller = "Warmup" });
        }
    }
}