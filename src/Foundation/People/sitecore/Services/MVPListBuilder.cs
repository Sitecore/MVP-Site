using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using Mvp.Foundation.People.Shared.Models;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Abstractions;

namespace Mvp.Foundation.People.Services
{
	public class MVPListBuilder : IMVPListBuilder
	{
		private readonly BaseLinkManager linkManager;

		public MVPListBuilder(BaseLinkManager linkManager)
		{
			this.linkManager = linkManager;
		}
		public MVPYear GetMvpYearList(string year, Item contextItem, Rendering rendering)
		{

			MVPYear yearList = new MVPYear();
			yearList.Name = year;
			yearList.Countries = new List<MVPCountry>();
			List<MVPPerson> People = new List<MVPPerson>();
			var mvpRepoItem = contextItem.Database.GetItem(Constants.Folders.MVPRepoFolder);
			var awards = mvpRepoItem.Axes.GetDescendants().Where(x => x.DescendsFrom(Constants.Templates.PersonAward) && x.Fields["__Semantics"].Value.ToLower().Contains("{4F63290E-7FC5-412A-96A4-301A86B774F1}".ToLower())).ToList();
			foreach (var personAward in awards)
			{
				if (!People.Any(p => p.ID.Equals(personAward.Parent.ID.ToString())))
				{
					MVPPerson mvpPerson = BuildMVPPerson(personAward, personAward.Parent);
					People.Add(mvpPerson);
				}				
			}
			if(People.Count > 0)
			{
				List<string> countries = People.SelectMany(p => p.Awards).Select(a => a.Country).Distinct().ToList();
				foreach(var country in countries)
				{
					MVPCountry mvpCountry = new MVPCountry();
					mvpCountry.Name = country;
					List<MVPCategory> mvpCategories = new List<MVPCategory>();
					List<string> categories = People.Where(p=>p.Awards.Any(a=>a.Country.Equals(country, System.StringComparison.InvariantCultureIgnoreCase)))
						.SelectMany(p => p.Awards).SelectMany(a => a.AwardsTypes).Distinct().ToList();
					foreach(var category in categories)
					{
						MVPCategory mvpCategory = new MVPCategory();
						mvpCategory.Name = category;
						mvpCategory.People = People.Where(p => p.Awards.Any(a => a.Country.Equals(country, System.StringComparison.InvariantCultureIgnoreCase) && a.AwardsTypes.Contains(category))).ToList();
						mvpCategories.Add(mvpCategory);
					}
					mvpCountry.Categories = mvpCategories;
					yearList.Countries.Add(mvpCountry);
				}
			}
			return yearList;
		}

		private MVPPerson BuildMVPPerson(Item personItem, Item awardItem)
		{
			MVPPerson person = new MVPPerson();
			person.ID = personItem.ID.ToString();
			person.Name = personItem[Constants.FieldNames.FirstName] + " " + personItem[Constants.FieldNames.LastName];
			person.Introduction = personItem[Constants.FieldNames.Introduction];
			person.ProfileUrl = linkManager.GetItemUrl(personItem);
			person.Name = personItem[Constants.FieldNames.FirstName];
			person.Awards = new List<MVPPersonAward>();
			person.Awards.Add(BuildMVPPerson(awardItem));
			return person;
		}

		private MVPPersonAward BuildMVPPerson(Item awardItem)
		{
			MVPPersonAward personAward = new MVPPersonAward();
			personAward.ID = awardItem.ID.ToString();
			personAward.AwardsTypes = new List<string>();
			var tags = ((MultilistField)awardItem.Fields["__Semantics"]).GetItems();
			foreach (var tag in tags)
			{
				if (tag.ParentID.Equals(Constants.Folders.AwardsTagFolder))
				{
					personAward.AwardsTypes.Add(tag.Name);
				}
				if (tag.ParentID.Equals(Constants.Folders.CountriesTagFolder))
				{
					personAward.Country = tag.Name;
				}
				if (tag.ParentID.Equals(Constants.Folders.YearsTagFolder))
				{
					personAward.Year = tag.Name;
				}
			}
			return personAward;
		}
	}
}