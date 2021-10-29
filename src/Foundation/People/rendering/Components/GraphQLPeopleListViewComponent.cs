using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Mvp.Foundation.People.Services;
using Sitecore.AspNet.RenderingEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvp.Foundation.People.Components
{
	public class GraphQLPeopleListViewComponent : ViewComponent
	{
        private readonly IGraphQLPeopleService _graphQLPeopleService;

        public GraphQLPeopleListViewComponent(IGraphQLPeopleService graphQLProductsService)
        {
            _graphQLPeopleService = graphQLProductsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var searchParams = _graphQLPeopleService.CreateSearchParams();
            searchParams.Language = GetLanguage();
            searchParams.RootItemId = "{64F31E3A-2040-4E69-B9A7-6830CBE669D2}";
            searchParams.IsInEditingMode = this.HttpContext.GetSitecoreRenderingContext().Response?.Content?.Sitecore?.Context?.IsEditing;
            searchParams.PageSize = 20;
            searchParams.CursorValueToGetItemsAfter = GetCursorIndex(searchParams.PageSize);
            searchParams.Facets = GetFacetsFromUrl();
            searchParams.CacheKey = this.HttpContext.Request.GetEncodedPathAndQuery();
            searchParams.Query = GetQueryKeyword();
            var results = await _graphQLPeopleService.Search(searchParams);

            return View(results);
        }

     
        private List<KeyValuePair<string, string[]>>? GetFacetsFromUrl()
        {
            List<KeyValuePair<string, string[]>> filterFacets = new List<KeyValuePair<string, string[]>>();
            var fffacetsList = this.HttpContext.Request.Query.Where(kvp => kvp.Key.StartsWith(Constants.QueryParameters.FacetPrefix) && !string.IsNullOrWhiteSpace(kvp.Value));
            foreach(KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> fcFilter in fffacetsList)
			{
                filterFacets.Add(new KeyValuePair<string, string[]>( fcFilter.Key.Replace(Constants.QueryParameters.FacetPrefix, ""),  fcFilter.Value.ToString().Split('|')));

            }
            return filterFacets;

        }

        private string GetQueryKeyword()
		{
            if (this.HttpContext.Request.Query.ContainsKey(Constants.QueryParameters.Query))
                return Request.Query[Constants.QueryParameters.Query];
            return "";
        }

        

        private int? GetCursorIndex(int? pageSize)
        {
            int? cursorIndex = 0;

            if (this.HttpContext.Request.Query.ContainsKey(Constants.QueryParameters.Page))
                cursorIndex = pageSize * int.Parse( Request.Query[Constants.QueryParameters.Page]) - pageSize;

            return cursorIndex;
        }

        private string GetLanguage()
        {
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            return currentCulture.RequestCulture.Culture.Name;

        }
    }
}
