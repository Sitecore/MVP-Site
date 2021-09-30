using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Shared.Models
{
	public class MVPCategory
	{
		public string Name { get; set; }
		public List<MVPPerson> People { get; set; }
	}
}
