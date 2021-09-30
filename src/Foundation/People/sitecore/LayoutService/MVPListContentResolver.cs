using Mvp.Foundation.People.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Foundation.People.LayoutService
{
    public class MVPListContentResolver : RenderingContentsResolver
    {
        private readonly IMVPListBuilder _mvpListBuilder;

        public MVPListContentResolver(IMVPListBuilder mvpListBuilder)
        {
            this._mvpListBuilder = mvpListBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var contextItem = GetContextItem(rendering, renderingConfig);
            return _mvpListBuilder.GetMvpYearList("2021", contextItem, rendering);
        }
    }
}