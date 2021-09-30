using GraphQL;
using System;
using System.IO;
using System.Reflection;

namespace Mvp.Foundation.People.Infrastructure
{
    public class GraphQLRequestBuilder
    {

        public GraphQLRequest BuildQuery(string query, string operationName, dynamic? variables)
        {
            return new GraphQLRequest
            {
                Query = query,
                OperationName = operationName,
                Variables = variables
            };
        }

        public GraphQLRequest BuildQuery(GraphQLFiles queryFile, dynamic? variables)
        {
            return BuildQuery(GetOperationResource(queryFile), "MVPSearch", variables);
        }


        protected string GetOperationResource(GraphQLFiles queryFile)
        {
            return _query;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.GraphQL.{queryFile}.graphql";
            if (assembly.GetManifestResourceInfo(resourceName) == null)
            {
                throw new Exception($"Unknown GraphQL resource: {resourceName} -- is the file embedded?");
            }
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"An error occurred with GraphQL resource {resourceName}"));
            return reader.ReadToEnd();
        }
        private string _query = @"query MVPSearch(
   $language: String!
  $rootItem: String!
  $pageSize: Int
  $cursorValueToGetItemsAfter: String!
  $facetOn: [String!]
  $fieldsEqual: [ItemSearchFieldQuery]
  $query: String

) {
  search(
    rootItem: $rootItem
    language: $language
    latestVersion:true
    first: $pageSize
    after: $cursorValueToGetItemsAfter
     fieldsEqual: $fieldsEqual
    facetOn: $facetOn
     keyword: $query
    
  ) {
    facets {
      name
      values {
        value
        count
      }
    }
 
    results {
      items {
        item {
          ... on Person {
            firstName {
              value
            }
            lastName {
              value
            }
            email {
              value
            }
            introduction {
              value
            }
            url
            country{targetItem{name}}
            
          }
        }
      }
      totalCount
      pageInfo {
        startCursor
        endCursor
        hasNextPage
        hasPreviousPage
      }
    }
  }
}";
    }

    [Flags]
    public enum GraphQLFiles
    {
        None = 0,
        PeopleSearchAdvanced = 1
    }
}
