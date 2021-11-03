using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Mvp.Feature.Forms.Models
{
    public class ApplicationModel
    {
        public TextField Title { get; set; }
        public RichTextField BodyText { get; set; }
        public ImageField FeaturedImage { get; set; }
        public HyperLinkField PromoLink { get; set; }
        public DateField ExampleDate { get; set; }
    }
}
