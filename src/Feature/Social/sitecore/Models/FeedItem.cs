using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Social.Models
{
    public class FeedItem
    {
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
