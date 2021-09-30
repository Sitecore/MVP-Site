using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Mvp.Foundation.People.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvp.Foundation.People.Services
{
	public class GraphQLPeopleService : IGraphQLPeopleService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IConfiguration _configuration;
		private readonly IGraphQLProvider _graphQLProvider;


		public GraphQLPeopleService(IMemoryCache memoryCache, IConfiguration configuration, IGraphQLProvider graphQLProvider)
		{
			_memoryCache = memoryCache;
			_configuration = configuration;
			_graphQLProvider = graphQLProvider;
		}

		public SearchParams CreateSearchParams()
		{
			return new SearchParams()
			{
				FacetOn = new List<string>() { "personaward", "personyear", "personcountry" }
			};

		}


		public async Task<PeopleSearchResults> Search(SearchParams searchParams)
		{
			var fieldsEqualsList = new List<FieldFilter>();
			

			//Facets from URL
			if (searchParams.Facets != null && searchParams.Facets.Any())
				fieldsEqualsList.AddRange(AddFieldsEqualParams(searchParams.Facets.ToArray()));

			List<FieldFilter> fieldFilters = new List<FieldFilter>();
			fieldFilters.Add(new FieldFilter() { name = "_templatename", value = "Person" });

			fieldsEqualsList.AddRange(fieldFilters);
			return await ProductSearchResults();

			async Task<PeopleSearchResults> ProductSearchResults()
			{
				GraphQL.GraphQLResponse<Response> response = await _graphQLProvider.SendQueryAsync<Response>(searchParams.IsInEditingMode, GraphQLFiles.PeopleSearchAdvanced, new
				{
					language = searchParams.Language,
					rootItem = new Guid(searchParams.RootItemId).ToString("N"),
					pageSize = searchParams.PageSize,
					cursorValueToGetItemsAfter = searchParams.CursorValueToGetItemsAfter?.ToString(),
					query = searchParams.Query,
					fieldsEqual = fieldsEqualsList,
					facetOn = searchParams.FacetOn
				});

				return new PeopleSearchResults
				{
					People = response.Data.Search.results.items.Select(x => x.item),
					Facets = response.Data.Search.facets,
					TotalCount = response.Data.Search.results.totalCount,
					StartCursor = int.Parse(response.Data.Search.results.pageInfo.startCursor),
					EndCursor = int.Parse(response.Data.Search.results.pageInfo.endCursor),
					HasNextPage = response.Data.Search.results.pageInfo.hasNextPage,
					HasPreviousPage = response.Data.Search.results.pageInfo.hasPreviousPage,
					PageSize = searchParams.PageSize != 0 ? searchParams.PageSize : null,
					FilterFacets = searchParams.FilterFacets,
					CurrentPage = !searchParams.PageSize.HasValue
						? 1
						: Convert.ToInt32(Math.Ceiling(int.Parse(response.Data.Search.results.pageInfo.endCursor) /
													   Convert.ToDouble(searchParams.PageSize)))
				};
			}

		}


		private List<FieldFilter> AddFieldsEqualParams(KeyValuePair<string, string>[] filter)
		{
			List<FieldFilter> fieldFilters = new List<FieldFilter>();

			foreach (KeyValuePair<string, string> keyValue in filter)
			{
				FieldFilter ff = new FieldFilter();
				ff.name = keyValue.Key;
				ff.value = keyValue.Value;
				fieldFilters.Add(ff);
			}
			return fieldFilters;

		}
	}

	public class SearchParams
	{
		public string Language { get; set; }
		public string RootItemId { get; set; }
		public int? PageSize { get; set; }

		public int? CursorValueToGetItemsAfter { get; set; }

		public bool? IsInEditingMode { get; set; }

		public IList<(KeyValuePair<string, string>, IDictionary<string, string>)>? FilterFacets { get; set; }
		public IList<KeyValuePair<string, string>>? Facets { get; set; }

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
		public int count { get; set; }
	}

	public class Facet
	{
		public string name { get; set; }
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
