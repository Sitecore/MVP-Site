using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.BasicContent.Models
{
    public class ContentListItem
    {
        [SitecoreRouteField]
        public TextField ItemTitle { get; set; }
        
        [SitecoreRouteField] 
        public TextField ItemSubtitle { get; set; }
        
        [SitecoreRouteField] 
        public TextField ItemText { get; set; }
        
        [SitecoreRouteField] 
        public HyperLinkField ItemLink { get; set; }
    }
}
