using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace Mvp.Foundation.People.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFoundationPeople(this RenderingEngineOptions options)
        {
            options.AddViewComponent("GraphQLPeopleList", "GraphQLPeopleList");
            return options;
        }
    }
}