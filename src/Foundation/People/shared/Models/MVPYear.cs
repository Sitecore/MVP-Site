using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Shared.Models
{
	public class MVPYear
	{
		public string Name { get; set; }

		public List<MVPCountry> Countries { get; set; }
	}
}
