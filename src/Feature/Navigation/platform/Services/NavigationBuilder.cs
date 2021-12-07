using Mvp.Feature.Navigation.Models;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace Mvp.Feature.Navigation.Services
{
    public class NavigationBuilder : INavigationBuilder
    {
        private readonly BaseLinkManager linkManager;

        public NavigationBuilder(BaseLinkManager linkManager)
        {
            this.linkManager = linkManager;
        }

        public IList<Link> GetNavigationLinks(Item contextItem, Rendering rendering)
        {
            var homeItem = GetHomePage(contextItem);
            return homeItem.Children.Where(x => x.DescendsFrom(Constants.Templates.NavigationItem) && x[Constants.FieldNames.IncludeInMenu] == Constants.FieldValues.CheckboxTrue)
                                    .Select(x => new Link { Title = x[Constants.FieldNames.MenuTitle], Url = linkManager.GetItemUrl(x) })
                                    .ToList();
        }

        private Item GetHomePage(Item contextItem)
        {
            return contextItem.DescendsFrom(Constants.Templates.HomePage) 
                   ? contextItem
                   : contextItem.Axes.GetAncestors().LastOrDefault(x => x.DescendsFrom(Constants.Templates.HomePage));
        }
    }
}