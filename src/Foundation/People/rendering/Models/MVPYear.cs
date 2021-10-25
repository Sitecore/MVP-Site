using Mvp.Foundation.People.Shared.Models;
using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Models
{
	public class MVPYear
	{
		[SitecoreComponentField]
		public string Name { get; set; }
		[SitecoreComponentField]

		public List<MVPCountry> Countries { get; set; }

		
	}
}
