using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace Mvp.Foundation.People.ComputedFields
{
	public class PersonAward : BaseComputedField, IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item != null && item.TemplateID.Equals(Constants.Templates.Person))
            {
                return  base.GetAwardsTags(item, TagType.AwardType);
            }
            return null;
        }

        public string FieldName { get; set; }

        public string ReturnType { get; set; }
    }
}