using Sitecore.LayoutService.Client.Response.Model.Fields;
using System;
using System.Collections.Generic;
using System.Text;


namespace Mvp.Feature.BasicContent.Models
{
    public class ImageTeaser
    {
        public TextField TeaserTitle { get; set; }
        public RichTextField TeaserText { get; set; }
        public HyperLinkField TeaserLink { get; set; }
        public ImageField TeaserImage { get; set; }
    }
}
