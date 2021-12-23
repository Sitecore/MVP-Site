using System.Linq;
using Mvp.Feature.Navigation.Services;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
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
      var rootItem = GetNavigationRootItem(GetContextItem(rendering, renderingConfig));
      Assert.IsNotNull(rootItem, "Could not locate a navigation root item.");
      return new
      {
        LogoSvgPath = rootItem[Templates.NavigationRootItem.Fields.LogoSvgPath],
        Links = _navigationBuilder.GetNavigationLinks(rootItem)
      };
    }

    private Item GetNavigationRootItem(Item contextItem)
    {
      return contextItem.DescendsFrom(Templates.NavigationRootItem.TemplateId)
        ? contextItem
        : contextItem.Axes.GetAncestors().LastOrDefault(x => x.DescendsFrom(Templates.NavigationRootItem.TemplateId));
    }
  }
}