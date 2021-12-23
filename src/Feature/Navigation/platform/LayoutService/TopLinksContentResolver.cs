using Mvp.Feature.Navigation.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class TopLinksContentResolver : RenderingContentsResolver
    {
        private readonly ITopLinksBuilder _topLinksBuilder;

        public TopLinksContentResolver(ITopLinksBuilder topLinksBuilder)
        {
            _topLinksBuilder = topLinksBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var topLinks = _topLinksBuilder.GetTopLinks(GetContextItem(rendering, renderingConfig), rendering);
            return new
            {
                links = topLinks
            };
        }
    }
}