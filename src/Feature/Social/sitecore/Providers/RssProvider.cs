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
            this.RssUrl = rssUrl;
        }

        public IList<FeedItem> GetFeedItems()
        {
            // Setting default cache interval of 60
            return GetFeedItems(60, 100);
        }

        public IList<FeedItem> GetFeedItems(int cacheInterval, int count)
        {
            string key = $"{this.CacheKey}_{count}_{cacheInterval}";
            string key_backup = $"{this.CacheKey}_{count}_{cacheInterval}_backup";
            IList<FeedItem> source = HttpContext.Current.Cache[key] as IList<FeedItem>;

            if (source == null)
            {
                source = (IList<FeedItem>)(HttpContext.Current.Cache[key] as List<FeedItem>);
                if (source == null)
                {
                    source = this.LoadFeed();
                    if (source.Any<FeedItem>())
                    {
                        HttpContext.Current.Cache.Add(key, source, null, DateTime.UtcNow.AddMinutes((double)cacheInterval), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        HttpContext.Current.Cache.Add(key_backup, source, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (HttpContext.Current.Cache[key_backup] != null)
                    {
                        source = HttpContext.Current.Cache[key_backup] as IList<FeedItem>;
                        Sitecore.Diagnostics.Log.Warn("There was an problem with loading RSS feed items so getting them from backup cache", this);
                    }
                }
            }
            return source.Take<FeedItem>(count).ToList<FeedItem>();
        }

        private List<FeedItem> LoadFeed()
        {
            // Load the actual RSS feed
            XmlReader reader = XmlReader.Create(this.RssUrl);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            var list = new List<FeedItem>();
            foreach (SyndicationItem item in feed.Items)
            {
                var feedItem = new FeedItem()
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Url = item.Links.FirstOrDefault().Uri.ToString(),
                    Timestamp = item.PublishDate.DateTime
                };
                list.Add(feedItem);
            }
            return list;
        }
    }

}