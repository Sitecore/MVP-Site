using Mvp.Feature.BasicContent.Models;
using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace Mvp.Feature.BasicContent.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFeatureBasicContent(this RenderingEngineOptions options)
        {
            options.AddModelBoundView<AnnouncementBar>("AnnouncementBar");
            return options;
        }
    }
}