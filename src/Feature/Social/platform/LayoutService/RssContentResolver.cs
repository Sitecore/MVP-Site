using Mvp.Feature.Social.Services;
using Newtonsoft.Json.Linq;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Social.LayoutService
{
  public class RssContentResolver : RenderingContentsResolver
  {
    private readonly IRssBuilder _rssBuilder;

    public RssContentResolver(IRssBuilder rssBuilder)
    {
      _rssBuilder = rssBuilder;
    }

    public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
    {
      var obj = (JObject)base.ResolveContents(rendering, renderingConfig);

      obj["feedItems"] = _rssBuilder.GetFeedItemsAsJson(GetContextItem(rendering, renderingConfig));

      return obj;
    }
  }
}