using Mvp.Feature.Navigation.Models;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using Sitecore;

namespace Mvp.Feature.Navigation.Services
{
    public class NavigationBuilder : INavigationBuilder
    {
        private readonly BaseLinkManager _linkManager;

        public NavigationBuilder(BaseLinkManager linkManager)
        {
            _linkManager = linkManager;
        }

        public IList<Link> GetNavigationLinks(Item contextItem, Rendering rendering)
        {
            var homeItem = GetHomePage(contextItem);
            return homeItem.Children.Where(x => x.DescendsFrom(Templates.Navigation.TemplateId) && MainUtil.GetBool(x[Templates.Navigation.Fields.IncludeInMenu], false))
                                    .Select(x => new Link { Title = x[Templates.Navigation.Fields.MenuTitle], Url = _linkManager.GetItemUrl(x) })
                                    .ToList();
        }

        private Item GetHomePage(Item contextItem)
        {
            return contextItem.DescendsFrom(Templates.NavigationRootItem.TemplateId) 
                   ? contextItem
                   : contextItem.Axes.GetAncestors().LastOrDefault(x => x.DescendsFrom(Templates.NavigationRootItem.TemplateId));
        }
    }
}