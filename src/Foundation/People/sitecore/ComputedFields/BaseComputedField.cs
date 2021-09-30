using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Foundation.People.ComputedFields
{
	public class BaseComputedField
	{
		public List<string> GetAwardsTags(Sitecore.Data.Items.Item personItem, TagType tagType)
		{
			List<string> mvpTags = new List<string>();
			if (personItem != null && personItem.TemplateID.Equals(Constants.Templates.Person))
			{
				var awards = personItem.Children.Where(i => i.TemplateID.Equals(Constants.Templates.PersonAward)).ToList();
				foreach(var award in awards)
				{
					var tags = ((MultilistField)award.Fields["__Semantics"]).GetItems();
					foreach (var tag in tags)
					{
						if (tagType== TagType.AwardType && tag.ParentID.Equals(Constants.Folders.AwardsTagFolder))
						{
							mvpTags.Add(tag.Name);
						}
						if (tagType == TagType.Year && tag.ParentID.Equals(Constants.Folders.YearsTagFolder))
						{
							mvpTags.Add(tag.Name);
						}
					}
				}
			}
			return mvpTags.Distinct().ToList() ;
		}

		public string GetPersonCountry(Sitecore.Data.Items.Item personItem)
		{
			string country = "";
			if (personItem != null && personItem.TemplateID.Equals(Constants.Templates.Person))
			{
				var tags = ((MultilistField)personItem.Fields["__Semantics"]).GetItems();
				foreach (var tag in tags)
				{
					if (tag.ParentID.Equals(Constants.Folders.CountriesTagFolder))
					{
						country = tag.Name;
						break;
					}
				}
			}
			return country;
		}

	}
	public enum TagType
	{
		Year=0, AwardType = 1
	}
}