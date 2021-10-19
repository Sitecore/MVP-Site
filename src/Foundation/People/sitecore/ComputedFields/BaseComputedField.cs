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
				var awards = ((MultilistField)personItem.Fields[Constants.FieldNames.Awards]).GetItems();
				foreach(var award in awards)
				{
					if(award.TemplateID.Equals(Constants.Templates.YearCategory))
					{
						if (tagType == TagType.Year)
						{
							mvpTags.Add(award.Parent.Name);
						}
						else
						{
							var mvptype = ((ReferenceField)award.Fields[Constants.FieldNames.Type]).TargetItem;
							if (mvptype != null && mvptype.TemplateID.Equals(Constants.Templates.MVPType))
							{
								mvpTags.Add(mvptype.Name);
							}
						}
					}
				}
			}
			return mvpTags.Distinct().ToList() ;
		}

	}
	public enum TagType
	{
		Year=0, AwardType = 1
	}
}