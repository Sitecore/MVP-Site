using Mvp.Feature.BasicContent.Shared.Models;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.BasicContent.Services
{
    public class ContentListBuilder : IContentListBuilder
    {
        public ContentList GetContentList(Item contextItem, Rendering rendering)
        {
            Item datasource = GetDatasourceItem(contextItem, rendering);

            ContentList panel = new ContentList();
            
            // Get panel title directly from datasource item
            panel.Title = datasource.Fields[Constants.FieldNames.PanelTitle].Value;
            panel.Items = GetFeaturePanelItems(datasource);

            return panel;
        }

        public IEnumerable<ContentListItem> GetFeaturePanelItems(Item datasourceItem)
        {
            var featurePanelList = new List<ContentListItem>();
            var featurePanelItems = datasourceItem.Children.Where(x => x.TemplateID == Constants.Templates.ContentListItem);

            foreach (var item in featurePanelItems)
            {
                featurePanelList.Add(new ContentListItem()
                {
                    ItemTitle = item.Fields[Constants.FieldNames.Title].Value,
                    ItemSubtitle = item.Fields[Constants.FieldNames.Subtitle].Value,
                    ItemText = item.Fields[Constants.FieldNames.Text].Value,
                    ItemLinkUrl = ((LinkField)item.Fields[Constants.FieldNames.Link]).GetFriendlyUrl(),
                    ItemLinkDescription = ((LinkField)item.Fields[Constants.FieldNames.Link]).Text,
                    ItemLink = item.Fields[Constants.FieldNames.Link].Value
                });
            }

            return featurePanelList;
        }

        private static Item GetDatasourceItem(Item contextItem, Rendering rendering)
        {
            var dataSourceItem = contextItem?.Database?.GetItem(rendering.DataSource);
            if (dataSourceItem == null)
            {
                throw new NullReferenceException();
            }
            return dataSourceItem;
        }

    }
}