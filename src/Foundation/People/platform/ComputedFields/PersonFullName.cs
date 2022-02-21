using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using System.Globalization;

namespace Mvp.Foundation.People.ComputedFields
{
	public class PersonFullName : IComputedIndexField
	{
        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item != null && item.TemplateID.Equals(Constants.Templates.Person))
            {
                var personFullNameSimple = item["First Name"] + " " + item["Last Name"];
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(personFullNameSimple);
            }
            return false;
        }

        public string FieldName { get; set; }

        public string ReturnType { get; set; }
    }
}