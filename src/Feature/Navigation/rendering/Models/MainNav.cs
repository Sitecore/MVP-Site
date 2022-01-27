using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace Mvp.Feature.Navigation.Models
{
    public class MainNav
    {
        [SitecoreComponentField]
        public string LogoSvgPath { get; set; }

        [SitecoreComponentField]
        public Link[] Links { get; set; }
    }
}