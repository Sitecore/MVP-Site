using Mvp.Feature.Social.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Social.Shared.Models
{
    public class RssControl
    {
        public IEnumerable<FeedItem> FeedItems { get; set; }
    }
}
