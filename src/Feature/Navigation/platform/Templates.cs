using Sitecore.Data;

namespace Mvp.Feature.Navigation
{
  public static class Templates
  {
    public static class TopLink
    {
      public static readonly ID TemplateId = ID.Parse("{E4986869-E4B1-4990-8669-52ABF69CD9CF}");
    }

    public static class TopLinksCollection
    {
      public static readonly ID TemplateId = ID.Parse("{E454A4E3-F69A-4D46-8601-EDA00669D470}");
    }

    public static class TopLinksFolder
    {
      public static readonly ID TemplateId = ID.Parse("{DA4E3EC4-5133-42D7-A5A0-36BE30DCF187}");
    }

    public static class HasLink
    {
      public static readonly ID TemplateId = ID.Parse("{2957694E-7790-4A69-8707-898D0CA71E8B}");

      public struct Fields
      {
        public static readonly ID Link = new ID("{AFF6FD57-80BD-4B53-ADB9-164B922D5763}");
      }
    }

    public static class Navigation
    {
      public static readonly ID TemplateId = ID.Parse("{2FA21400-EB0C-4339-ADC0-71FB2D762256}");

      public struct Fields
      {
        public static readonly ID MenuTitle = new ID("{B1ECE12F-B2C9-44E6-92A3-C5DFD6514B59}");
        public static readonly ID IncludeInMenu = new ID("{EFC0A3C7-6993-47D2-BA85-10B44961916E}");
      }
    }

    public static class NavigationRootItem
    {
      public static readonly ID TemplateId = ID.Parse("{AD1CB5AB-CB35-4107-BD11-793053B31A38}");

      public struct Fields
      {
        public static readonly ID LogoSvgPath = new ID("{8E50D26E-5046-490D-8D62-113D2821AD38}");
      }
    }
  }
}