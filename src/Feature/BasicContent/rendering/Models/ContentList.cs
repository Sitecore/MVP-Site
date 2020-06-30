using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using System.Security.Policy;

namespace Mvp.Feature.BasicContent.Models
{
    public class ContentList
    {
        [SitecoreComponentField(Name = "Title")]
        public string Title { get; set; }

        [SitecoreComponentField]
        public Mvp.Feature.BasicContent.Shared.Models.ContentListItem[] Items { get; set; }

        [SitecoreComponentField] 
        public string listType { get; set; }
        public int limit { get; set; }
    }
}