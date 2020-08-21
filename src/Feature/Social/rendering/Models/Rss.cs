using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using Mvp.Feature.Social.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Social.Models
{
    public class Rss
    {
        [SitecoreComponentField]
        public TextField Title { get; set; }

        [SitecoreComponentField]
        public TextField Description { get; set; }

        [SitecoreComponentField(Name="feedItems")]
        public IEnumerable<FeedItem> FeedItems {get;set; }
    }
}
