using System.Collections.Generic;
using System.Linq;
using Mvp.Feature.Navigation.Models;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.Services
{
  public class NavigationBuilder : INavigationBuilder
  {
    private readonly BaseLinkManager _linkManager;

    public NavigationBuilder(BaseLinkManager linkManager)
    {
      _linkManager = linkManager;
    }

    

    public IList<Link> GetNavigationLinks(Item item)
    {
      if (item == null)
        return new List<Link>();

      var rootItem = item.DescendsFrom(Templates.NavigationRootItem.TemplateId) ? item: GetNavigationRootItem(item);
      return rootItem.Children.Where(x =>
          x.DescendsFrom(Templates.Navigation.TemplateId) && MainUtil.GetBool(x[Templates.Navigation.Fields.IncludeInMenu], false))
        .Select(x => new Link { Title = x[Templates.Navigation.Fields.MenuTitle], Url = _linkManager.GetItemUrl(x) })
        .ToList();
    }

    public Item GetNavigationRootItem(Item item)
    {
      return item == null ? null : item.DescendsFrom(Templates.NavigationRootItem.TemplateId)
        ? item
        : item.Axes.GetAncestors().LastOrDefault(x => x.DescendsFrom(Templates.NavigationRootItem.TemplateId)) ?? item;
    }
  }
}