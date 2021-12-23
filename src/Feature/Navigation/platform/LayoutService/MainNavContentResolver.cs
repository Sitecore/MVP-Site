using Mvp.Feature.Navigation.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class MainNavContentResolver : RenderingContentsResolver
    {
        private readonly INavigationBuilder _navigationBuilder;

        public MainNavContentResolver(INavigationBuilder navigationBuilder)
        {
            _navigationBuilder = navigationBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var contextItem = GetContextItem(rendering, renderingConfig);
            return new
            {
                Links = _navigationBuilder.GetNavigationLinks(contextItem, rendering)
            };
        }
    }
}