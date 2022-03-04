using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace Mvp.Feature.Navigation.Models
{
    public class Footer
    {
        [SitecoreComponentField]
        public string CopyrightText { get; set; }
        
        [SitecoreComponentField]
        public SocialLink[] SocialLinks { get; set; }
    }
}