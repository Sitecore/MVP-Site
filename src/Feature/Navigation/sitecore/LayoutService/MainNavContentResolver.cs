using Mvp.Feature.Navigation.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class MainNavContentResolver : RenderingContentsResolver
    {
        private readonly INavigationBuilder navigationBuilder;

        public MainNavContentResolver(INavigationBuilder navigationBuilder)
        {
            this.navigationBuilder = navigationBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var contextItem = GetContextItem(rendering, renderingConfig);
            return new
            {
                Links = navigationBuilder.GetNavigationLinks(contextItem, rendering)
            };
        }
    }
}