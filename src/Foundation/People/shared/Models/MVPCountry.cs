using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Shared.Models
{
	public class MVPCountry
	{
		public string Name { get; set; }
		public List<MVPCategory> Categories { get; set; }
	}
}
