using Mvp.Feature.Social.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Social.Models
{
    public class RssControl
    {
        public IEnumerable<FeedItem> FeedItems { get; set; }
    }
}
