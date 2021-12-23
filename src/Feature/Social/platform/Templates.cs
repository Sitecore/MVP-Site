using Sitecore.Data;

namespace Mvp.Feature.Social
{
  public static class Templates
  {
    public static class RssFeed
    {
      public static readonly ID TemplateId = ID.Parse("{074C7B05-5600-4F3E-BA0C-DB359D42ABE6}");
      public struct Fields
      {
        public static readonly ID Title = new ID("{1126EA11-FBCF-460E-98EE-D731F63E9F75}");
        public static readonly ID Description = new ID("{61FF3997-3033-40EB-BF77-B497D1B9430D}");
        public static readonly ID RssUrl = new ID("{69913AE6-614C-4CB6-BB3B-353700E53CCF}");
      }
    }
  }
}