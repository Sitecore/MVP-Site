#region Namespaces
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using X.Web.Sitemap;
using X.Web.Sitemap.Extensions;
#endregion

#region Main Code
namespace Mvp.Foundation.SitecoreExtensions.Pipelines
{
    public class SitemapGenerator : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.LocalPath) && args.LocalPath.Contains("healthz") || string.IsNullOrWhiteSpace(args.LocalPath)) return;
            //This check will verify if the physical path of the request exists or not.
            if (!System.IO.File.Exists(args.HttpContext.Request.PhysicalPath) &&
                !System.IO.Directory.Exists(args.HttpContext.Request.PhysicalPath))
            {
                Assert.ArgumentNotNull(args, "args");
                try
                {
                    // Homepage of the Website.
                    // Start path will give homepage including Multisite.
                    var homepage = Context.Database.GetItem(Context.Site.StartPath);
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

                    var dockerPath = string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Mvp.DockerSitemapPath")) ? "c:\\solution\\src\\Project\\MvpSite\\rendering\\sitemap.xml" : Sitecore.Configuration.Settings.GetSetting("Mvp.DockerSitemapPath");

                    sitemap.SaveAsync(dockerPath);
                }
                catch (Exception ex)
                {
                    Log.Error("Error creating Sitemap.xml.", ex, this);
                }
            }
        }

        ///

        /// Crete Absolute url as per the site
        /// 

        ///
        ///
        private static string GetAbsoluteLink(string relativeUrl)
        {
            var site = Sitecore.Configuration.Factory.GetSite("mvp-site");            
            return site.SiteInfo.Scheme  + "://" + site.SiteInfo.HostName.Replace("-cd",string.Empty) + relativeUrl;
        }

        ///

        /// Append language or not in URL to return language specific sitemap.xml
        /// 

        ///
        private static int AppendLanguage()
        {
            return string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Mvp.LanguageEmbedForSitemap")) ? 0 : System.Convert.ToInt32((Sitecore.Configuration.Settings.GetSetting("Mvp.LanguageEmbedForSitemap")));
        }

        ///

        /// This method will get a list of excluding template ids and will check if the passed item is in
        /// 

        ///
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
