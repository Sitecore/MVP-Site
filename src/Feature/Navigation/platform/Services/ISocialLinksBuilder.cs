using Mvp.Feature.Navigation.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;

namespace Mvp.Feature.Navigation.Services
{
    public interface ISocialLinksBuilder
    {
        IList<SocialLink> GetSocialLinks(Item footerRootItem);
    }
}