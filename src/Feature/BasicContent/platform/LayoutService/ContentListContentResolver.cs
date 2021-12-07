using Mvp.Feature.BasicContent.Services;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.BasicContent.LayoutService
{
    public class ContentListContentResolver : RenderingContentsResolver
    {
        private readonly IContentListBuilder __contentListBuilder;

        public ContentListContentResolver(IContentListBuilder contentListBuilder)
        {
            this.__contentListBuilder = contentListBuilder;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var contentList = __contentListBuilder.GetContentList(GetContextItem(rendering, renderingConfig), rendering);

            return new
            {
                title = contentList.Title,
                items = contentList.Items,
                listType = GetContentListType(rendering.Parameters, Constants.RenderingParameters.ListType),
                limit = GetContentListLimit(rendering.Parameters, Constants.RenderingParameters.Limit)
            };
        }

        private string GetContentListType(RenderingParameters parameters, string listType)
        {
            return parameters[listType].ValueOrEmpty();
        }

        private int GetContentListLimit(RenderingParameters parameters, string limit)
        {
            return parameters[limit] == null ? 0 : Convert.ToInt32(parameters[limit]);
        }
    }
}