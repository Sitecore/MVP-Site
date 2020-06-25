using Mvp.Feature.Hero.Models;
using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace Mvp.Feature.Hero.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFeatureHero(this RenderingEngineOptions options)
        {
            options.AddModelBoundView<HeroBig>("HeroBig")
                   .AddModelBoundView<HeroMedium>("HeroMedium");
            
            return options;
        }
    }
}