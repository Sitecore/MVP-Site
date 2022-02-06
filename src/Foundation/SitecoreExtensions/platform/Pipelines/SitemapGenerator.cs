#region Namespaces
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.IO;
using System.Linq;
using System.Web;
using X.Web.Sitemap;
#endregion

#region Main Code
namespace Mvp.Foundation.SitecoreExtensions.Pipelines
{
    public class SitemapGenerator : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!string.IsNullOrWhiteSpace(args.LocalPath) && args.LocalPath.Contains("healthz") || string.IsNullOrWhiteSpace(args.LocalPath)) return;

            if (HttpContext.Current.Cache["SitemapSaved"] != null) return;

            if (string.IsNullOrWhiteSpace(Context.Site.StartPath)) return;

            try
            {
                // Homepage of the Website.
                var homepage = Context.Database.GetItem(Context.Site.StartPath);
                if (homepage==null) return;

                var sitemap = new Sitemap();

                //Create node of Homepage in Sitemap.
                var config = AppendLanguage();

                if (!ExcludeItemFromSitemap(homepage))
                {
                    sitemap.Add(new Url
                    {
                        ChangeFrequency = ChangeFrequency.Daily,
                        Location = GetAbsoluteLink(LinkManager.GetItemUrl(homepage, new ItemUrlBuilderOptions() { LanguageEmbedding = config == 2 ? LanguageEmbedding.Always : (config == 1 ? LanguageEmbedding.AsNeeded : LanguageEmbedding.Never) })),
                        Priority = 0.5,
                        TimeStamp = homepage.Statistics.Updated
                    });
                }

                // Get all decendants of Homepage to create full Sitemap.
                var childrens = homepage.Axes.GetDescendants();
                //Remove the items whose templateid is in exclude list
                var finalcollection = childrens.Where(x => !ExcludeItemFromSitemap(x)).ToList();

                sitemap.AddRange(finalcollection.Select(childItem => new Url
                {
                    Location = GetAbsoluteLink(LinkManager.GetItemUrl(childItem, new ItemUrlBuilderOptions() { LanguageEmbedding = (config == 2 ? LanguageEmbedding.Always : (config == 1 ? LanguageEmbedding.AsNeeded : LanguageEmbedding.Never)) })),
                    TimeStamp = childItem.Statistics.Updated
                }));

                var dockerPath = string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Mvp.DockerSitemapPath")) ? "c:\\solution\\src\\Project\\MvpSite\\rendering\\" : Sitecore.Configuration.Settings.GetSetting("Mvp.DockerSitemapPath");

                sitemap.SaveAsync($"{dockerPath}sitemap.xml");
                SaveRobotTxtFile(dockerPath);

                HttpContext.Current.Cache["SitemapSaved"]=1;
            }
            catch (Exception ex)
            {
                Log.Error("Error creating Sitemap.xml.", ex, this);
            }
        }

        private string RobotTxtContent
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Mvp.RobotsTxtItemId"))) return null;

                var robotcontentitem = Context.Database.GetItem(new Sitecore.Data.ID(Sitecore.Configuration.Settings.GetSetting("Mvp.RobotsTxtItemId")));

                if (robotcontentitem == null) return null;

                return robotcontentitem["Content"];
            }
        }

        private void SaveRobotTxtFile(string filepath)
        {
            string robottxtcontent = RobotTxtContent;

            if (string.IsNullOrWhiteSpace(robottxtcontent))
            {
                robottxtcontent += "User-agent: *";
                robottxtcontent += "\nAllow: /";
            }

            robottxtcontent += $"\nSitemap: {GetAbsoluteLink("/sitemap.xml")}";

            // Write the specified text to a new file named "robots.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, "robots.txt")))
            {
                outputFile.WriteAsync(robottxtcontent);
            }
        }

        ///
        /// Crete Absolute url as per the site
        /// 
        private static string GetAbsoluteLink(string relativeUrl)
        {
            var site = Sitecore.Configuration.Factory.GetSite("mvp-site");            
            return site.SiteInfo.Scheme  + "://" + site.SiteInfo.HostName.Replace("-cd",string.Empty) + relativeUrl;
        }

        ///
        /// Append language or not in URL to return language specific sitemap.xml
        /// 
        private static int AppendLanguage()
        {
            return string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Mvp.LanguageEmbedForSitemap")) ? 0 : System.Convert.ToInt32((Sitecore.Configuration.Settings.GetSetting("Mvp.LanguageEmbedForSitemap")));
        }

        ///
        /// This method will get a list of excluding template ids and will check if the passed item is in
        /// 
        private static bool ExcludeItemFromSitemap(Item objItem)
        {
            //Check if the item is having any version
            if (objItem.Versions.Count > 0)
            {
                var excludeItems = Sitecore.Configuration.Settings.GetSetting("Mvp.ExcludeSitecoreItemsByTemplatesInSitemap");
                if (string.IsNullOrWhiteSpace(excludeItems)) return false;

                var collection = excludeItems.Split(',').ToList();
                return collection.Contains(objItem.TemplateID.ToString());
            }
            return true;
        }
    }
}
#endregion
