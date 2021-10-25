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
				type.Field<StringGraphType>("mvpAwards",
					description: "MVP Person Awards",
					resolve: context => GetAwardsTags(context.Source));
			});
		}
		private string GetAwardsTags(Sitecore.Data.Items.Item personItem)
		{
			List<string> mvpTags = new List<string>();
			if (personItem != null && personItem.TemplateID.Equals(Constants.Templates.Person))
			{
				var awards = ((MultilistField)personItem.Fields[Constants.FieldNames.Awards]).GetItems();
				foreach (var award in awards)
				{
					string mvpaward = "";
					string mvpyear = "";
					if (award.TemplateID.Equals(Constants.Templates.YearCategory))
					{
						mvpyear = award.Parent.Name;

						var mvptype = ((ReferenceField)award.Fields[Constants.FieldNames.Type]).TargetItem;
						if (mvptype != null && mvptype.TemplateID.Equals(Constants.Templates.MVPType))
						{
							mvpaward = mvptype.Name;
						}
					}
					if (!string.IsNullOrEmpty(mvpaward) && !string.IsNullOrEmpty(mvpyear))
					{
						mvpTags.Add(mvpaward + " " + mvpyear);
					}
				}
			}
			if (mvpTags.Any())
			{
				return string.Join(", ", mvpTags.Distinct().ToArray());
			}
			return "";
		}
	}
}
