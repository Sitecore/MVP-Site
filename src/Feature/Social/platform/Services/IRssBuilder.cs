using Mvp.Feature.Social.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Newtonsoft.Json.Linq;

namespace Mvp.Feature.Social.Services
{
    public interface IRssBuilder
    {
        JArray GetFeedItemsAsJson(Item datasourceItem);
    }
}