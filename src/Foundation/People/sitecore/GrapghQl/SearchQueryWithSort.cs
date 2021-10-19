using GraphQL.Types;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Services.GraphQL.Content.GraphTypes.ContentSearch;
using Sitecore.Services.GraphQL.GraphTypes.Connections;
using Sitecore.Services.GraphQL.Content.GraphTypes;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace Mvp.Foundation.People.GrapghQl
{
	public class SearchQueryWithSort: Sitecore.Services.GraphQL.Content.Queries.SearchQuery
	{
		public SearchQueryWithSort()
        {
            this.Arguments.Add(new QueryArgument<StringGraphType>()
            {
                Name = "sortBy",
                Description = "Sort by a field"
            });

            this.Arguments.Add(new QueryArgument<BooleanGraphType>()
            {
                Name = "sortDesc",
                Description = "Specify Desc vs ascending",
                DefaultValue = false
            });
        }


        protected override ContentSearchResults Resolve(ResolveFieldContext context)
        {
            var sortBy = context.GetArgument<string>("sortBy");
            bool? sortDesc = context.GetArgument<bool>("sortDesc");
            string inputPathOrIdOrShortId = context.GetArgument<string>("rootItem");
            ID rootId = (ID)null;
            Item result1;
            if (!string.IsNullOrWhiteSpace(inputPathOrIdOrShortId) && Sitecore.Services.GraphQL.Content.GraphTypes.IdHelper.TryResolveItem(this.Database, inputPathOrIdOrShortId, out result1))
                rootId = result1.ID;
            string keywordArg = context.GetArgument<string>("keyword");
            Language result2;
            if (!Language.TryParse(context.GetArgument<string>("language") ?? Sitecore.Context.Language.Name ?? LanguageManager.DefaultLanguage.Name, out result2))
                result2 = (Language)null;
            bool flag = context.GetArgument<bool?>("version").HasValue ? context.GetArgument<bool?>("version").Value : false;
            string name1 = context.GetArgument<string>("index") ?? "sitecore_" + this.Database.Name.ToLowerInvariant() + "_index";
            IEnumerable<Dictionary<string, object>> fieldsEqual = Enumerable.OfType<Dictionary<string, object>>(context.GetArgument<object[]>("fieldsEqual", new object[0]));

            IEnumerable<string> facetOnStrings = (IEnumerable<string>)((object)context.GetArgument<IEnumerable<string>>("facetOn") ?? (object)new string[0]);
            Dictionary < string,List<string>> facetFilters = new Dictionary<string, List<string>>();
            foreach(string factOn in facetOnStrings)
			{
                IEnumerable<Dictionary<string, object>> facetValueFilters = fieldsEqual.Where(f => f["name"].ToString().Equals(factOn));
                if(facetValueFilters!=null && facetValueFilters.Any())
				{
                    if(facetFilters.ContainsKey(factOn))
					{
                        facetFilters[factOn].AddRange(facetValueFilters.Select(fvf=>fvf["value"].ToString()).ToList());

                    }
                    else
					{
                        facetFilters.Add(factOn, facetValueFilters.Select(fvf => fvf["value"].ToString()).ToList());
                    }
				}
            }
            fieldsEqual = fieldsEqual.RemoveWhere(f => facetFilters.ContainsKey(f["name"].ToString()));


            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex(name1).CreateSearchContext())
            {
                IQueryable<ContentSearchResult> queryable = searchContext.GetQueryable<ContentSearchResult>();
                if (rootId != (ID)null)
                    queryable = queryable.Where<ContentSearchResult>((Expression<Func<ContentSearchResult, bool>>)(result => result.AncestorIDs.Contains<ID>(rootId)));
                if (!string.IsNullOrWhiteSpace(keywordArg))
                    queryable = queryable.Where<ContentSearchResult>((Expression<Func<ContentSearchResult, bool>>)(result => result.Content.Contains(keywordArg)));
                if (result2 != (Language)null)
                {
                    string resultLanguage = result2.Name;
                    queryable = queryable.Where<ContentSearchResult>((Expression<Func<ContentSearchResult, bool>>)(result => result.Language == resultLanguage));
                }
                if (flag)
                    queryable = queryable.Where<ContentSearchResult>((Expression<Func<ContentSearchResult, bool>>)(result => result.IsLatestVersion));
                foreach (Dictionary<string, object> dictionary in fieldsEqual)
                {
                    string name = dictionary["name"].ToString();
                    string value = dictionary["value"].ToString();
                    queryable = queryable.Where<ContentSearchResult>((Expression<Func<ContentSearchResult, bool>>)(result => result[name].Equals(value)));
                }
                if(facetFilters!=null && facetFilters.Any())
				{
                    foreach(var facetFilter in facetFilters)
					{
                        var facetPredicate = PredicateBuilder.True<ContentSearchResult>();
                        foreach (var value in facetFilter.Value)
                        {
                            facetPredicate = facetPredicate.Or(x => x[facetFilter.Key] == value);
                        }
                        queryable = queryable.Where(facetPredicate);
                    }
				}
                foreach (string str in facetOnStrings)
                {
                    string facet = str;
                    queryable = queryable.FacetOn<ContentSearchResult, string>((Expression<Func<ContentSearchResult, string>>)(result => result[facet]));
                }
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortDesc.HasValue && sortDesc == true)
                    {
                        queryable = queryable.OrderByDescending(result => result[sortBy]);
                    }
                    else
                    {
                        queryable = queryable.OrderBy(result => result[sortBy]);
                    }
                }
                int? nullable = context.GetArgument<int?>("after");
                return new ContentSearchResults(queryable.ApplyEnumerableConnectionArguments<ContentSearchResult, object>((ResolveFieldContext<object>)context).GetResults<ContentSearchResult>(), nullable ?? 0);
            }
        }
    }
}