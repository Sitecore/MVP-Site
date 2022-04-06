using Sitecore.Data.Items;
using System.Linq;

namespace Mvp.Feature.Navigation.Services
{
    public class ItemTools : IItemTools
    {
        public Item GetNavigationRootItem(Item contextItem)
        {
            return contextItem.DescendsFrom(Templates.NavigationRootItem.TemplateId)
              ? contextItem
              : contextItem.Axes.GetAncestors().LastOrDefault(x => x.DescendsFrom(Templates.NavigationRootItem.TemplateId));
        }
    }
}