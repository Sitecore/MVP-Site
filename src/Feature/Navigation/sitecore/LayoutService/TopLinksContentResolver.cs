using Mvp.Feature.Navigation.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class TopLinksContentResolver : RenderingContentsResolver
    {
        private readonly ITopLinksBuilder topLinksBuilder;

        public TopLinksContentResolver(ITopLinksBuilder topLinksBuilder)
        {
            this.topLinksBuilder = topLinksBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var topLinks = topLinksBuilder.GetTopLinks(GetContextItem(rendering, renderingConfig), rendering);
            return new
            {
                links = topLinks
            };
        }
    }
}