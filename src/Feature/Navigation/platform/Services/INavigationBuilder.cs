using System.Collections.Generic;
using Mvp.Feature.Navigation.Models;
using Sitecore.Data.Items;

namespace Mvp.Feature.Navigation.Services
{
  public interface INavigationBuilder
  {
    IList<Link> GetNavigationLinks(Item contextItem);
    Item GetNavigationRootItem(Item contextItem);
  }
}