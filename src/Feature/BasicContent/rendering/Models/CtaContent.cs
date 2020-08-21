using Sitecore.LayoutService.Client.Response.Model.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.BasicContent.Models
{
    public class CtaContent
    {
        public TextField CtaTitle { get; set; }
        public RichTextField CtaText { get; set; }
        public HyperLinkField CtaLink { get; set; }
        public ImageField CtaImage{ get; set; }
    }
}
