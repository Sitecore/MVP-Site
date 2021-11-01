using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Forms.Models
{
	public class CreatePerson : CreateBase
	{
		[JsonProperty("First Name")]
		public string FirstName { get; set; }

		[JsonProperty("Last Name")]
		public string LastName { get; set; }

		[JsonProperty("Okta Id")]
		public string OktaId { get; set; }

		[JsonProperty("Email")]
		public string Email { get; set; }
	}
}
