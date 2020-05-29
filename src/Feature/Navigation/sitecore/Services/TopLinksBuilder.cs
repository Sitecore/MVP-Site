using Mvp.Feature.Navigation.Models;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvp.Feature.Navigation.Services
{
    public class TopLinksBuilder : ITopLinksBuilder
    {
        public IList<TopLink> GetTopLinks(Item contextItem, Rendering rendering)
        {
            var dataSourceItem = contextItem?.Database?.GetItem(rendering.DataSource);
            if (dataSourceItem == null)
            {
                throw new NullReferenceException();
            }

            var links = new List<TopLink>();
            foreach (var child in GetValidTopLinkItems(dataSourceItem))
            {
                links.Add(new TopLink
                {
                    Title = ((LinkField)child.Fields[Constants.FieldNames.Link]).Title,
                    Url = ((LinkField)child.Fields[Constants.FieldNames.Link]).Url
                });
            }

            return links;
        }

        private IEnumerable<Item> GetValidTopLinkItems(Item dataSourceItem)
        {
            return dataSourceItem.Children.Where(x => x.TemplateID == Constants.Templates.TopLink 
                                                 && !string.IsNullOrWhiteSpace(x[Constants.FieldNames.Link]));
        }
    }
}