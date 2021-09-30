using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Shared.Models
{
	public class MVPPerson
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Introduction { get; set; }
		public string ProfileUrl { get; set; }
		public List<MVPPersonAward> Awards { get; set; }
	}
}
