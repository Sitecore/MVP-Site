using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Hero.Models
{
    public class HeroBase
    {
        public TextField HeroTitle { get; set; }
        public TextField HeroSubtitle { get; set; }
        public TextField HeroDescription { get; set; }
        public HyperLinkField HeroLink { get; set; }
        public ImageField HeroImage { get; set; }
    }
}
