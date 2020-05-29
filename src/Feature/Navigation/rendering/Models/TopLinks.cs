using Sitecore.AspNet.RenderingEngine.Binding.Attributes;

namespace Mvp.Feature.Navigation.Models
{
    public class TopLinks
    {
        [SitecoreComponentField]
        public TopLink[] Links { get; set; }
    }
}