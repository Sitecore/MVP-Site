using GraphQL;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mvp.Foundation.People.Infrastructure
{
    public class GraphQLProvider :  IGraphQLProvider
    {
        private readonly ILogger<IGraphQLProvider> _logger;
        private readonly GraphQLRequestBuilder _graphQLRequestBuilder;
        private readonly GraphQLClientFactory _graphQLClientFactory;
        private readonly bool _isDevelopment;

        public GraphQLProvider(ILogger<IGraphQLProvider> logger, GraphQLRequestBuilder graphQLRequestBuilder, GraphQLClientFactory graphQLClientFactory)
        {
            _logger = logger;
            _graphQLRequestBuilder = graphQLRequestBuilder ?? throw new ArgumentNullException(nameof(graphQLRequestBuilder));
            _graphQLClientFactory = graphQLClientFactory ?? throw new ArgumentNullException(nameof(graphQLClientFactory));
            _isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }


        public async Task<GraphQLResponse<TResponse>> SendQueryAsync<TResponse>(bool? isEditMode, GraphQLFiles queryFile, dynamic? variables) where TResponse : class
        {
            var client = isEditMode.HasValue && isEditMode.Value ? _graphQLClientFactory.CreateEditClient() : _graphQLClientFactory.CreateLiveClient();

            GraphQLRequest request = _graphQLRequestBuilder.BuildQuery(queryFile, variables);

            var result = await client.SendQueryAsync<TResponse>(request);

            if (_isDevelopment)
            {
                string data = JsonConvert.SerializeObject(result.Data, Formatting.Indented);
                _logger.LogDebug(JsonConvert.SerializeObject(result.Data, Formatting.Indented));
            }

            return result;
        }

    }
}
