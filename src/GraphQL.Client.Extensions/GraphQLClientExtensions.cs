using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json.Linq;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="GraphQLClient" /> class.
    /// </summary>
    public static class GraphQLClientExtensions
    {
        /// <summary>Send a <see cref="GraphQLRequest" /> via GET.</summary>
        /// <typeparam name="T">Data Type.</typeparam>
        /// <param name="gqlClient">The GraphQL Client.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<T> Get<T>(this GraphQLClient gqlClient, IQuery query, CancellationToken cancellationToken = default)
            where T : class
        {
            GraphQLRequest gqlRequest = CreateGraphQLResquest(query);
            GraphQLResponse gqlResponse = await gqlClient.GetAsync(gqlRequest, cancellationToken);

            return ParseResponse<T>(query, gqlRequest, gqlResponse);
        }

        /// <summary>Send a <see cref="GraphQLRequest" /> composed of a query batch via GET.</summary>
        /// <param name="gqlClient">The GraphQL Client.</param>
        /// <param name="queries">The query batch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<IReadOnlyDictionary<string, JToken>> GetBatch(this GraphQLClient gqlClient, IQuery[] queries, CancellationToken cancellationToken = default)
        {
            GraphQLRequest gqlRequest = CreateGraphQLResquest(queries);
            GraphQLResponse gqlResponse = await gqlClient.GetAsync(gqlRequest, cancellationToken);

            return ParseResponse(queries, gqlResponse);
        }

        /// <summary>Send a <see cref="GraphQLRequest" /> via POST.</summary>
        /// <typeparam name="T">Data Type.</typeparam>
        /// <param name="gqlClient">The GraphQL Client.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<T> Post<T>(this GraphQLClient gqlClient, IQuery query, CancellationToken cancellationToken = default)
            where T : class
        {
            GraphQLRequest gqlQuery = CreateGraphQLResquest(query);
            GraphQLResponse gqlResp = await gqlClient.PostAsync(gqlQuery, cancellationToken);

            return ParseResponse<T>(query, gqlQuery, gqlResp);
        }

        /// <summary>Send a <see cref="GraphQLRequest" /> composed of a query batch via POST.</summary>
        /// <param name="gqlClient">The GraphQL Client.</param>
        /// <param name="queries">The query batch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<IReadOnlyDictionary<string, JToken>> PostBatch(this GraphQLClient gqlClient, IQuery[] queries, CancellationToken cancellationToken = default)
        {
            GraphQLRequest gqlRequest = CreateGraphQLResquest(queries);
            GraphQLResponse gqlResponse = await gqlClient.PostAsync(gqlRequest, cancellationToken);

            return ParseResponse(queries, gqlResponse);
        }

        private static T ParseResponse<T>(IQuery query, GraphQLRequest gqlQuery, GraphQLResponse gqlResp)
            where T : class
        {
            CheckResponse(gqlResp);

            string resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.Name : query.AliasName;

            JToken value = gqlResp.Data.GetValue(resultName);

            if (typeof(T) == typeof(string))
            {
                return value.ToString() as T;
            }

            if (typeof(T) == typeof(JToken))
            {
                return value as T;
            }

            return value.ToObject<T>();
        }

        private static IReadOnlyDictionary<string, JToken> ParseResponse(IQuery[] queries, GraphQLResponse gqlResp)
        {
            CheckResponse(gqlResp);

            var result = new Dictionary<string, JToken>();

            foreach (IQuery query in queries)
            {
                string resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.Name : query.AliasName;
                JToken value = gqlResp.Data.GetValue(resultName);
                result.Add(resultName, value);
            }

            return result;
        }

        private static void CheckResponse(GraphQLResponse response)
        {
            if (response == null) {
                throw new InvalidOperationException("Param \"response\" cannot be null.");
            }

            if (response.Errors != null)
            {
                throw new GraphQLClientException(response.Errors);
            }
        }

        private static GraphQLRequest CreateGraphQLResquest(params IQuery[] queries)
        {
            return new GraphQLRequest { Query = "{" + string.Concat(queries.Select(q => q.Build())) + "}" };
        }
    }
}
