using Mvp.Feature.BasicContent.Shared.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;

namespace Mvp.Feature.BasicContent.Services
{
    public interface IContentListBuilder
    {
        ContentList GetContentList(Item contextItem, Rendering rendering);

        IEnumerable<ContentListItem> GetFeaturePanelItems(Item contextItem);
    }
}