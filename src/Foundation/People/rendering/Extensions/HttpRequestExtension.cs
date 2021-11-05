using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Net;

namespace Mvp.Foundation.People.Extensions
{
	public static class HttpRequestExtension
	{
		public static string UpdateFacetsInUrl(this HttpRequest request, string key, string value, bool AddToQuery)
		{
			var url = request.Path;
			var queryKvp = request.Query.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToList(); ;
			

			if (AddToQuery)
			{
				var existingQs = queryKvp.Where(q => q.Key.Equals(key)).FirstOrDefault();
				if (existingQs.Equals(new KeyValuePair<string, string>()))
				{
					queryKvp.Add(new KeyValuePair<string, string>(key, value));
				}
				else
				{
					string newValue = existingQs.Value + "|" + value;
					queryKvp.Remove(existingQs);
					queryKvp.Add(new KeyValuePair<string, string>(key, newValue));
				}
			}
			else
			{
				var existingQs = queryKvp.Where(q => q.Key.Equals(key)).FirstOrDefault();
				if (existingQs.Equals(new KeyValuePair<string, string>()))
				{

				}
				if (!existingQs.Value.Contains("|"))
				{
					queryKvp.Remove(queryKvp.FirstOrDefault(kvp => kvp.Key.Equals(key)));
				}
				else
				{
					string newValue = "";
					if (existingQs.Value.Contains("|" + value))
						newValue = existingQs.Value.Replace("|" + value, "");
					else
						newValue = existingQs.Value.Replace( value, "");
					queryKvp.Remove(existingQs);
					queryKvp.Add(new KeyValuePair<string, string>(key, newValue));
				}
			}
			//reset pagination
			var paginationQs = queryKvp.Where(q => q.Key.Equals(Constants.QueryParameters.Page)).FirstOrDefault();
			if (!paginationQs.Equals(new KeyValuePair<string, string>()))
			{
				queryKvp.Remove(paginationQs);
			}
			string queryString = QueryString.Create(queryKvp).ToUriComponent();
			return WebUtility.UrlEncode(url + queryString);
		}

		public static string UpdatePagerInUrl(this HttpRequest request, string key, string value)
		{
			var url = request.Path;
			var queryKvp = request.Query.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToList(); ;

			var existingQs = queryKvp.Where(q => q.Key.Equals(key)).FirstOrDefault();
			if (existingQs.Equals(new KeyValuePair<string, string>()))
			{
				queryKvp.Add(new KeyValuePair<string, string>(key, value));
			}
			else
			{
				string newValue = value;
				queryKvp.Remove(existingQs);
				queryKvp.Add(new KeyValuePair<string, string>(key, newValue));
			}

			string queryString = QueryString.Create(queryKvp).ToUriComponent();
			return WebUtility.UrlEncode(url + queryString);
		}

		public static string UpdateKeywordInUrl(this HttpRequest request, string key, string value)
		{
			var url = request.Path;
			var queryKvp = request.Query.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToList(); ;
			if (key == "q")
			{
				var existingQs = queryKvp.Where(q => q.Key.Equals(key)).FirstOrDefault();
				if (existingQs.Equals(new KeyValuePair<string, string>()))
				{
					queryKvp.Add(new KeyValuePair<string, string>(key, value));
				}
				else
				{
					queryKvp.Remove(existingQs);
					queryKvp.Add(new KeyValuePair<string, string>(key, value));
				}
			}
			//reset pagination
			var paginationQs = queryKvp.Where(q => q.Key.Equals(Constants.QueryParameters.Page)).FirstOrDefault();
			if (!paginationQs.Equals(new KeyValuePair<string, string>()))
			{
				queryKvp.Remove(paginationQs);
			}
			var facetsQs = queryKvp.Where(q => q.Key.StartsWith(Constants.QueryParameters.FacetPrefix));
			foreach(var facetQs in facetsQs.ToList())
			{
				queryKvp.Remove(facetQs);
			}
			string queryString = QueryString.Create(queryKvp).ToUriComponent();
			return WebUtility.UrlEncode(url + queryString);

		}
	}
}
