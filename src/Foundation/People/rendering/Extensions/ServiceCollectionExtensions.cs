using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Foundation.People.Infrastructure;
using Mvp.Foundation.People.Services;

namespace Mvp.Foundation.People.Extensions
{
	public static class ServiceCollectionExtensions
	{
        public static void AddFoundationPeople(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<GraphQLRequestBuilder>();
            serviceCollection.AddHttpClient<GraphQLClientFactory>();
            serviceCollection.AddSingleton<IGraphQLProvider, GraphQLProvider>();
            serviceCollection.AddSingleton<IGraphQLPeopleService, GraphQLPeopleService>();
            
        }
    }
}
