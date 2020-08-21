using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Diagnostics;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Mvp.Feature.Social.Models;
using System;
using System.Collections.Generic;
using Mvp.Feature.Social.Providers;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Web;

namespace Mvp.Feature.Social.Services
{
    public class RssBuilder : IRssBuilder
    {
        public RssControl GetRssControl(Item contextItem, Rendering rendering)
        {
            Debug.Assert(contextItem != null);

            Item datasource = GetDatasourceItem(contextItem, rendering);

            return new RssControl
            {
                FeedItems = GetFeedItems(datasource)
            };
        }

        public IList<FeedItem> GetFeedItems(Item datasourceItem)
        {
            var rssUrl = datasourceItem.Fields[Constants.FieldNames.RssUrl].Value;

            RssProvider rssProvider = new RssProvider(rssUrl);
            return rssProvider.GetFeedItems(10, 10);
        }

        public JArray GetFeedItemsAsJson(Item datasourceItem)
        {
            var feedItems = this.GetFeedItems(datasourceItem);
            var rssUrl = datasourceItem.Fields[Constants.FieldNames.RssUrl].Value;

            RssProvider rssProvider = new RssProvider(rssUrl);

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