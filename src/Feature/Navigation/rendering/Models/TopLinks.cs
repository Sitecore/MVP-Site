using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace Mvp.Feature.Navigation.Models
{
    public class TopLinks
    {
        [SitecoreComponentField]
        public Link[] Links { get; set; }
    }
}