using Sitecore.Data.Items;

namespace Mvp.Feature.Navigation.Services
{
    public interface IItemTools
    {
        Item GetNavigationRootItem(Item contextItem);
    }
}