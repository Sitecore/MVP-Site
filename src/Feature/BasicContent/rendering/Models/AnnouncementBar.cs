using Sitecore.LayoutService.Client.Response.Model.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.BasicContent.Models
{
    public class AnnouncementBar
    {
        public RichTextField AnnouncementText { get; set; }
        public HyperLinkField AnnouncementLink { get; set; }
    }
}
