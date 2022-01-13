using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Mvc.ItemResolving;
using Sitecore.LayoutService.Mvc.Pipelines.RequestBegin;
using Sitecore.LayoutService.Mvc.Routing;
using Sitecore.Mvc.Pipelines.Request.RequestBegin;
using System.Net;

namespace Mvp.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomContextItemResolver : ContextItemResolver
    {
        public CustomContextItemResolver(IItemResolver itemResolver, IRouteMapper routeMapper) : base(itemResolver, routeMapper)
        {
        }

        public override void Process(RequestBeginArgs args)
        {
            base.Process(args);
            if (!Settings.AliasesActive)
            {
                return;
                // if aliases aren't active, we really shouldn't confuse whoever turned them off 
            }

            var database = Context.Database;
            if (database == null)
            {
                return;
                // similarly, if we don't have a database, we probably shouldn't try to do anything
            }

            if (Context.Item == null)
            {
                string path = this.GetPath(args);
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                ID targetID = database.Aliases.GetTargetID(path);
                if (targetID.IsNull)
                {
                    Context.Item = Context.Database.GetItem(string.Concat(Context.Site.StartPath,Settings.GetSetting("Mvp.404PageName")));
                    args.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    string message = string.Concat("An alias for \"", path, "\" exists, but points to a non-existing item.");
                    Log.Info(message, this);
                    return;
                }
                Item item = database.GetItem(targetID);
                if (item != null)
                {
                    Context.Item = item;
                }
            }
        }
    }
}