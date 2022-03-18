using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.People.Models
{
	

	public class SearchParams
	{
		public string Language { get; set; }
		public string RootItemId { get; set; }
		public int? PageSize { get; set; }

		public int? CursorValueToGetItemsAfter { get; set; }

		public bool? IsInEditingMode { get; set; }

		public IList<(KeyValuePair<string, string>, IDictionary<string, string>)>? FilterFacets { get; set; }
		public List<KeyValuePair<string, string[]>>? Facets { get; set; }

		public IList<string> FacetOn { get; set; }
		public string Query { get; set; }

		public string CacheKey { get; set; }
	}

	public class PeopleSearchResults
	{
		public IEnumerable<Person> People { get; set; }
		public List<Facet> Facets { get; set; }
		public int TotalCount { get; set; }
		public int StartCursor { get; set; }
		public int EndCursor { get; set; }
		public bool HasNextPage { get; set; }
		public bool HasPreviousPage { get; set; }
		public int? PageSize { get; set; }
		public IList<(KeyValuePair<string, string>, IDictionary<string, string>)>? FilterFacets { get; set; }
		public int CurrentPage { get; set; }
		public string keyword { get; set; }
		public int LastPage
		{
			get
			{
				if(TotalCount == 0 || !PageSize.HasValue || PageSize.Value == 0)
				{
					return 1;
				}
				return (int)Math.Ceiling(((decimal)TotalCount) / ((decimal)PageSize.Value));
			}
		}
		public int PagerStart { get
			{
				if(CurrentPage>3)
				{
					return CurrentPage - 2;
				}
				else
				{
					return  1;
				}
			}
		}
		public int PagerEnd { get
			{ 
				if(!HasNextPage)
				{
					return CurrentPage;
				}
				
				if(CurrentPage+ 2 >= LastPage)
				{
					return LastPage;
				}
				if (CurrentPage < 2 && LastPage > 5)
				{
					return CurrentPage + 4;
				}
				if (CurrentPage <3 && LastPage > 5)
				{
					return CurrentPage + 3;
				}
				if (CurrentPage <3 && LastPage <=5)
				{
					return LastPage;
				}
				return CurrentPage + 2;
			} 
		}
	}

	public class PeopleSearch
	{
		public List<Facet> facets { get; set; }
		public Results results { get; set; }
	}

	public class Response
	{
		public PeopleSearch Search { get; set; }
	}


	public class Value
	{
		public string value { get; set; }
		public bool isChecked { get; set; }
		public int count { get; set; }
	}

	public class Facet
	{
		public string name { get; set; }
		public string DisplayName { get; set; }
		public List<Value> values { get; set; }

	}

	public class FirstName
	{
		public string value { get; set; }
	}

	public class LastName
	{
		public string value { get; set; }
	}

	public class Email
	{
		public string value { get; set; }
	}

	public class Introduction
	{
		public string value { get; set; }
	}

	public class TargetItem
	{
		public string name { get; set; }
	}

	public class Country
	{
		public TargetItem targetItem { get; set; }
	}

	public class Person
	{
		public FirstName firstName { get; set; }
		public LastName lastName { get; set; }
		public Email email { get; set; }
		public Introduction introduction { get; set; }
		public string url { get; set; }
		public Country country { get; set; }
		public string mvpYear { get; set; }
		public string mvpCount { get; set; }
		public string mvpCategory { get; set; }
	}


	public class SearchItem
	{
		public Person item { get; set; }
	}

	public class PageInfo
	{
		public string startCursor { get; set; }
		public string endCursor { get; set; }
		public bool hasNextPage { get; set; }
		public bool hasPreviousPage { get; set; }
	}

	public class Results
	{
		public List<SearchItem> items { get; set; }
		public int totalCount { get; set; }
		public PageInfo pageInfo { get; set; }
	}

	public class FieldFilter
	{
		public string name { get; set; }
		public string value { get; set; }
	}
}
