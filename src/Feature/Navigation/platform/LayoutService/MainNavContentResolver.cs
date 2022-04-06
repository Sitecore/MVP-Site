using Mvp.Feature.Navigation.Services;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class MainNavContentResolver : RenderingContentsResolver
    {
        private readonly INavigationBuilder _navigationBuilder;
        private readonly IItemTools _itemTools;

        public MainNavContentResolver(IItemTools itemTools, INavigationBuilder navigationBuilder)
        {
            _itemTools = itemTools;
            _navigationBuilder = navigationBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var rootItem = _itemTools.GetNavigationRootItem(GetContextItem(rendering, renderingConfig));
            Assert.IsNotNull(rootItem, "Could not locate a navigation root item.");
            return new
            {
                LogoSvgPath = rootItem[Templates.NavigationRootItem.Fields.LogoSvgPath],
                Links = _navigationBuilder.GetNavigationLinks(rootItem)
            };
        }
    }
}