using Mvp.Feature.Social.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mvp.Feature.Social.Services
{
    public interface IRssBuilder
    {
        RssControl GetRssControl(Item contextItem, Rendering rendering);

        Item GetDatasourceItem(Item contextItem, Rendering rendering);

        JArray GetFeedItemsAsJson(Item datasourceItem);
    }
}