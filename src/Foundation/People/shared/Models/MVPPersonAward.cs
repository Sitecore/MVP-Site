using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Shared.Models
{
	public class MVPPersonAward
	{
		public string Country { get; set; }
		public string Year { get; set; }
		public string ID { get; set; }
		public List<string> AwardsTypes { get; set; }
	}
}
