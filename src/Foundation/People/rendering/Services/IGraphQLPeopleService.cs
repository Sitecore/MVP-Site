using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mvp.Foundation.People.Models;

namespace Mvp.Foundation.People.Services
{
	public interface IGraphQLPeopleService
	{
		SearchParams CreateSearchParams();
		Task<PeopleSearchResults> Search(SearchParams searchParams);
	}
}
