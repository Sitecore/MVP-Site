using Mvp.Feature.Social.Models;
using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace Mvp.Feature.Social.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFeatureSocial(this RenderingEngineOptions options)
        {
            options.AddModelBoundView<Rss>("Rss");

            return options;
        }
    }
}