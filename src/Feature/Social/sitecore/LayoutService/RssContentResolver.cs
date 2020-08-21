using Mvp.Feature.Social.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Mvp.Feature.Social.LayoutService
{
    public class RssContentResolver : RenderingContentsResolver
    {
        private readonly IRssBuilder RssBuilder;

        public RssContentResolver(IRssBuilder _rssBuilder)
        {
            this.RssBuilder = _rssBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            JObject obj = (JObject)base.ResolveContents(rendering, renderingConfig);
            
            obj["feedItems"] = RssBuilder.GetFeedItemsAsJson(this.GetContextItem(rendering, renderingConfig));

            return obj;
        }
    }
}