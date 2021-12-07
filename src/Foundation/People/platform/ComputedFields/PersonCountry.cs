using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace Mvp.Foundation.People.ComputedFields
{
	public class PersonCountry : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item != null && item.TemplateID.Equals(Constants.Templates.Person))
            {
                var countryField = ((Sitecore.Data.Fields.ReferenceField)item.Fields[Constants.FieldNames.Country]);
                if(countryField!=null && countryField.TargetItem!=null && countryField.TargetItem.TemplateID.Equals(Constants.Templates.Country))
				{
                    return countryField.TargetItem["Name"];

                }
            }
            return null;
        }

        public string FieldName { get; set; }

        public string ReturnType { get; set; }
    }
}