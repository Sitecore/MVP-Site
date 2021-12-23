using Mvp.Feature.Social.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Mvp.Feature.Social.Providers
{
    public class RssProvider
	{
		public string RssUrl { get; set; }
        public List<FeedItem> Items { get; set; }
        private string CacheKey { get; }

        public RssProvider(string rssUrl)
        {
            RssUrl = rssUrl;
        }

        public IList<FeedItem> GetFeedItems()
        {
            // Setting default cache interval of 60
            return GetFeedItems(60, 100);
        }

        public IList<FeedItem> GetFeedItems(int cacheInterval, int count)
        {
            var key = $"{CacheKey}_{count}_{cacheInterval}";
            var keyBackup = $"{CacheKey}_{count}_{cacheInterval}_backup";

            if (HttpContext.Current.Cache[key] is IList<FeedItem> source) return source.Take(count).ToList();
            source = HttpContext.Current.Cache[key] as List<FeedItem>;
            if (source != null) return source.Take(count).ToList();

            source = LoadFeed();
            if (source.Any())
            {
              HttpContext.Current.Cache.Add(key, source, null, DateTime.UtcNow.AddMinutes(cacheInterval), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
              HttpContext.Current.Cache.Add(keyBackup, source, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            else if (HttpContext.Current.Cache[keyBackup] != null)
            {
              source = HttpContext.Current.Cache[keyBackup] as IList<FeedItem>;
              Sitecore.Diagnostics.Log.Warn("There was an problem with loading RSS feed items so getting them from backup cache", this);
            }

            return source?.Take(count).ToList();
        }

        private List<FeedItem> LoadFeed()
        {
            // Load the actual RSS feed
            var reader = XmlReader.Create(RssUrl);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            var list = new List<FeedItem>();
            foreach (var item in feed.Items)
            {
                var feedItem = new FeedItem()
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Url = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Timestamp = item.PublishDate.DateTime
                };
                list.Add(feedItem);
            }
            return list;
        }
    }

}