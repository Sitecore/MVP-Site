using Mvp.Feature.Navigation.Models;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvp.Feature.Navigation.Services
{
    public class SocialLinksBuilder : ISocialLinksBuilder
    {
        public IList<SocialLink> GetSocialLinks(Item footerRootItem)
        {
            var allSocialLinks = ((MultilistField)footerRootItem.Fields[Templates.SocialMediaLinks.Fields.SocialMediaLinks]).GetItems().Where(i => i.TemplateID == Templates.SocialMediaLink.TemplateId);
            var socialLinks = new List<SocialLink>();
            foreach(var socialLink in allSocialLinks)
            {
                var linkField = (LinkField)socialLink.Fields[Templates.HasLink.Fields.Link];
                var socialLinkSimplified = new SocialLink { Icon = socialLink.Fields[Templates.LinkIcon.Fields.Icon].Value, Title = linkField.Title, Url = linkField.Url };
                socialLinks.Add(socialLinkSimplified);
            }

            return socialLinks;
        }
    }
}