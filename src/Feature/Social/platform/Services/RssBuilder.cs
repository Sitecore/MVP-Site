using Sitecore.Data.Items;
using System.Diagnostics;
using Sitecore.Mvc.Presentation;
using Mvp.Feature.Social.Models;
using System;
using System.Collections.Generic;
using Mvp.Feature.Social.Providers;
using Newtonsoft.Json.Linq;

namespace Mvp.Feature.Social.Services
{
    public class RssBuilder : IRssBuilder
    {
        public RssControl GetRssControl(Item contextItem, Rendering rendering)
        {
            Debug.Assert(contextItem != null);

            var datasource = GetDatasourceItem(contextItem, rendering);

            return new RssControl
            {
                FeedItems = GetFeedItems(datasource)
            };
        }

        public IList<FeedItem> GetFeedItems(Item datasourceItem)
        {
            var rssUrl = datasourceItem.Fields[Templates.RssFeed.Fields.RssUrl].Value;

            var rssProvider = new RssProvider(rssUrl);
            return rssProvider.GetFeedItems(10, 10);
        }

        public JArray GetFeedItemsAsJson(Item datasourceItem)
        {
            var feedItems = GetFeedItems(datasourceItem);
            var rssUrl = datasourceItem.Fields[Templates.RssFeed.Fields.RssUrl].Value;

            var rssProvider = new RssProvider(rssUrl);

            var itemsArray = new JArray();

            foreach (var item in rssProvider.GetFeedItems())
            {
                var feedItem = new JObject()
                {
                    ["Title"] = item.Title,
                    ["Description"] = item.Description,
                    ["Url"] = item.Url,
                    ["Timestamp"] = item.Timestamp
                };
                itemsArray.Add(feedItem);
            }

            return itemsArray;
        }

        public Item GetDatasourceItem(Item contextItem, Rendering rendering)
           {
            var dataSourceItem = contextItem?.Database?.GetItem(rendering.DataSource);
            if (dataSourceItem == null)
            {
                throw new NullReferenceException();
            }
            return dataSourceItem;
        }

    }
}