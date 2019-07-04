using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="GraphQLClient" /> class.
    /// </summary>
    public static class GraphQLClientExtensions
    {
        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match. This handles server connection here!
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.</typeparam>
        /// <param name="gqlClient"></param>
        /// <param name="query"></param>
        /// <param name="resultName">Override of the Name/Alias of the query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The type of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<T> Get<T>(this GraphQLClient gqlClient, IQuery query, string resultName = null, CancellationToken cancellationToken = default)
            where T : class
        {
            GraphQLRequest gqlQuery = CreateGraphQLResquest(query);

            // make the call to the server, this will toss on any non 200 response

            GraphQLResponse gqlResp = await gqlClient.GetAsync(gqlQuery, cancellationToken);

            return ParseResponse<T>(query, ref resultName, gqlQuery, gqlResp);
        }

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match. This handles server connection here!
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always.
        /// </typeparam>
        /// <param name="gqlClient"></param>
        /// <param name="query"></param>
        /// <param name="resultName">Override of the Name/Alias of the query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The type of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
        /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
        public static async Task<T> Post<T>(this GraphQLClient gqlClient, IQuery query, string resultName = null, CancellationToken cancellationToken = default)
            where T : class
        {
            GraphQLRequest gqlQuery = CreateGraphQLResquest(query);

            // make the call to the server, this will toss on any non 200 response

            GraphQLResponse gqlResp = await gqlClient.PostAsync(gqlQuery, cancellationToken);

            return ParseResponse<T>(query, ref resultName, gqlQuery, gqlResp);
        }

        private static T ParseResponse<T>(IQuery query, ref string resultName, GraphQLRequest gqlQuery, GraphQLResponse gqlResp)
            where T : class
        {
            // check for no results, this is an odd case but should be caught

            // Any mising/empty data or response errors (GQL) will cause an exception!
            //
            // NOTE: GQL can return VALID data for a partial set of queries and errors
            // for others all in the same response set. Our case here is that ANY errors cause
            // a report of failure.

            CheckResult(gqlQuery, gqlResp);

            // If the given type was a string, ship the raw JSON string

            if (typeof(T) == typeof(string))
            {
                return gqlResp.Data.ToString();
            }

            // If the smart user passes in a JObject, get it that way instead of fixed T type
            // Your (T) must match the structure of the JSON being returned or expect an exception

            if (typeof(T) == typeof(JObject))
            {
                return JsonConvert.DeserializeObject<JObject>(gqlResp.Data.ToString());
            }

            // Now we need to get the results name. This is EITHER the Name, or the Alias
            // name. If Alias was set then use it. If the user does specify it in
            // the Get call it's an override. This might be needed with raw query

            if (resultName == null)
            {
                resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.Name : query.AliasName;
            }

            // Let the client do the mapping , all sorts of things can throw at this point!
            // caller should check for exceptions, Generally invalid mapping into the type

            return gqlResp.GetDataFieldAs<T>(resultName);
        }

        private static void CheckResult(GraphQLRequest request, GraphQLResponse response)
        {
            if (response?.Data == null || response?.Errors != null)
            {
                string errorMessage = "The GraphQL request return errors.";
                var errorData = new Dictionary<object, object>();
                errorData.Add("request", request.ToString());

                if (response?.Errors != null && response.Errors.Length > 0)
                {
                    errorMessage += string.Join(" ", response.Errors.Select(error => error.Message));
                    errorData.Add("errors", response.Errors);
                }

                Exception exception = new Exception(errorMessage);
                foreach (KeyValuePair<object, object> data in errorData)
                {
                    exception.Data.Add(data.Key, data.Value);
                }

                throw exception;
            }
        }

        private static GraphQLRequest CreateGraphQLResquest(IQuery query)
        {
            return new GraphQLRequest { Query = "{" + query.Build() + "}" };
        }
    }
}
