using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Mvp.Feature.Forms.Models
{
    public class TermsAndConditionsModel
    {
        public TextField WelcomeText { get; set; }
        public RichTextField ProcessDescription { get; set; }
        public CheckboxField PrivacyPolicyAgreement { get; set; }
    }
}
