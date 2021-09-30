using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using Mvp.Foundation.People.Shared.Models;

namespace Mvp.Foundation.People.Services
{
	public interface IMVPListBuilder
	{
		MVPYear GetMvpYearList(string year, Item contextItem, Rendering rendering);
	}
}