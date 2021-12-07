using GraphQL.Types;
using Sitecore.Services.GraphQL.Schemas;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mvp.Foundation.People.ComputedFields;

namespace Mvp.Foundation.People.Extensions
{
	public class CustomSchemaExtender: SchemaExtender
	{
		public CustomSchemaExtender()
		{
			ExtendTypes<ObjectGraphType<Item>>(type =>
			{
				// add a new field to the field object type
				// note the resolve method's Source property is the Field so you can get at its data
				type.Field<StringGraphType>("mvpYear",
					description: "MVP Person year",
					resolve: context => GetLatestMvpYear(context.Source));
			});
			ExtendTypes<ObjectGraphType<Item>>(type =>
			{
				// add a new field to the field object type
				// note the resolve method's Source property is the Field so you can get at its data
				type.Field<StringGraphType>("mvpCategory",
					description: "MVP Person category",
					resolve: context => GetLatestMvpCategory(context.Source));
			});
		}

		private string GetLatestMvpYear(Sitecore.Data.Items.Item personItem)
		{
			var awards = GetAwardsTags(personItem);
			if(awards.Any())
			{
				return awards.First().Key.ToString();
			}
			return "";
		}

		private string GetLatestMvpCategory(Sitecore.Data.Items.Item personItem)
		{
			var awards = GetAwardsTags(personItem);
			if (awards.Any())
			{
				return awards.First().Value;
			}
			return "";
		}
		private Dictionary<int,string> GetAwardsTags(Sitecore.Data.Items.Item personItem)
		{
			Dictionary<int, string> mvpTags = new Dictionary<int, string>();
			if (personItem != null && personItem.TemplateID.Equals(Constants.Templates.Person))
			{
				var awards = ((MultilistField)personItem.Fields[Constants.FieldNames.Awards]).GetItems();
				foreach (var award in awards)
				{
					string mvpaward = "";
					int mvpyear = 0;
					if (award.TemplateID.Equals(Constants.Templates.YearCategory))
					{
						Int32.TryParse( award.Parent.Name, out mvpyear);

						var mvptype = ((ReferenceField)award.Fields[Constants.FieldNames.Type]).TargetItem;
						if (mvptype != null && mvptype.TemplateID.Equals(Constants.Templates.MVPType))
						{
							mvpaward = mvptype.Name;
						}
					}
					if (!string.IsNullOrEmpty(mvpaward) && mvpyear > 0)
					{
						if(mvpTags.ContainsKey(mvpyear))
						{
							mvpTags[mvpyear] += ", " + mvpaward;
						}
						else
						{
							mvpTags.Add(mvpyear, mvpaward);
						}
					}
				}
			}
			return mvpTags.OrderByDescending(mvp=>mvp.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
		}
	}
}
