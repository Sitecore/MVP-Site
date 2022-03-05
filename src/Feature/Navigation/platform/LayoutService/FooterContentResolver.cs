using Mvp.Feature.Navigation.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace Mvp.Feature.Navigation.LayoutService
{
    public class FooterContentResolver : RenderingContentsResolver
    {
        private readonly ISocialLinksBuilder _socialLinksBuilder;
        private readonly IItemTools _itemTools;

        public FooterContentResolver(IItemTools itemTools, ISocialLinksBuilder socialLinksBuilder)
        {
            _itemTools = itemTools;
            _socialLinksBuilder = socialLinksBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var rootItem = _itemTools.GetNavigationRootItem(GetContextItem(rendering, renderingConfig));

            var footerRootItem = rootItem.Parent.Children.FirstOrDefault(x => x.Key == "shared content")?.Children.FirstOrDefault(x => x.DescendsFrom(Templates.SocialMediaLinks.TemplateId));
            if (footerRootItem != null)
            {
                var socialLinks = _socialLinksBuilder.GetSocialLinks(footerRootItem);

                return new
                {
                    CopyrightText = footerRootItem.Fields[Templates.Content.Fields.Content].Value,
                    SocialLinks = socialLinks
                };
            }
            else
            {
                return new
                {
                    CopyrightText = "",
                    SocialLinks = new List<Models.SocialLink>() { }
                };
            }
        }
    }
}