using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Mvp.Foundation.People.Infrastructure
{
    public class GraphQLClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public GraphQLClientFactory(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        private IGraphQLClient LiveClient { get; set; }

        private IGraphQLClient EditClient { get; set; }


        public IGraphQLClient CreateLiveClient()
        {
            return LiveClient ??= CreateGraphQLClient("Foundation:PeopleGraphQL:UrlLive");
        }

        public IGraphQLClient CreateEditClient()
        {
            return EditClient ??= CreateGraphQLClient("Foundation:PeopleGraphQL:UrlEdit");
        }

        private IGraphQLClient CreateGraphQLClient(string configurationKeyUrlLiveOrEditMode)
        {
            GraphQLHttpClientOptions graphQLHttpClientOptions = new GraphQLHttpClientOptions()
            {
                EndPoint = new Uri(
                    $"{_configuration.GetValue<string>(configurationKeyUrlLiveOrEditMode)}?sc_apikey={_configuration.GetValue<string>("Sitecore:ApiKey")}"),
            };

            return new GraphQLHttpClient(graphQLHttpClientOptions, new NewtonsoftJsonSerializer(), _httpClient);
        }


    }
}
