using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using GraphQL.Query.Builder;
using Newtonsoft.Json.Linq;

namespace GraphQL.Client.Extensions;

/// <summary>
/// Extensions for the <see cref="GraphQLClient" /> class.
/// </summary>
public static class GraphQLClientExtensions
{
    /// <summary>Send a <see cref="GraphQLRequest" /> via GET.</summary>
    /// <typeparam name="T">Data Type.</typeparam>
    /// <param name="client">The GraphQL Client.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
    /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
    public static async Task<T> Get<T>(this GraphQLClient client, IQuery query, CancellationToken cancellationToken = default)
        where T : class
    {
        RequiredArgument.NotNull(query, nameof(query));

        GraphQLRequest request = CreateGraphQLResquest(query);
        GraphQLResponse response = await client.GetAsync(request, cancellationToken);

        return ParseResponse<T>(query, response);
    }

    /// <summary>Send a <see cref="GraphQLRequest" /> composed of a query batch via GET.</summary>
    /// <param name="client">The GraphQL Client.</param>
    /// <param name="queries">The query batch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
    /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
    public static async Task<IReadOnlyDictionary<string, JToken>> GetBatch(this GraphQLClient client, IQuery[] queries, CancellationToken cancellationToken = default)
    {
        RequiredArgument.NotNullOrEmpty(queries, nameof(queries));

        GraphQLRequest request = CreateGraphQLResquest(queries);
        GraphQLResponse response = await client.GetAsync(request, cancellationToken);

        return ParseResponse(queries, response);
    }

    /// <summary>Send a <see cref="GraphQLRequest" /> via POST.</summary>
    /// <typeparam name="T">Data Type.</typeparam>
    /// <param name="client">The GraphQL Client.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
    /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
    public static async Task<T> Post<T>(this GraphQLClient client, IQuery query, CancellationToken cancellationToken = default)
        where T : class
    {
        RequiredArgument.NotNull(query, nameof(query));

        GraphQLRequest request = CreateGraphQLResquest(query);
        GraphQLResponse response = await client.PostAsync(request, cancellationToken);

        return ParseResponse<T>(query, response);
    }

    /// <summary>Send a <see cref="GraphQLRequest" /> composed of a query batch via POST.</summary>
    /// <param name="client">The GraphQL Client.</param>
    /// <param name="queries">The query batch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">Dupe Key, missing parts or empty parts of a query</exception>
    /// <exception cref="ArgumentNullException">Invalid Configuration</exception>
    public static async Task<IReadOnlyDictionary<string, JToken>> PostBatch(this GraphQLClient client, IQuery[] queries, CancellationToken cancellationToken = default)
    {
        RequiredArgument.NotNullOrEmpty(queries, nameof(queries));

        GraphQLRequest request = CreateGraphQLResquest(queries);
        GraphQLResponse response = await client.PostAsync(request, cancellationToken);

        return ParseResponse(queries, response);
    }

    private static T ParseResponse<T>(IQuery query, GraphQLResponse response)
        where T : class
    {
        RequiredArgument.NotNull(query, nameof(query));
        RequiredArgument.NotNull(response, nameof(response));

        CheckResponse(response);

        string resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.Name : query.AliasName;

        JToken value = response.Data.GetValue(resultName);

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

    private static IReadOnlyDictionary<string, JToken> ParseResponse(IQuery[] queries, GraphQLResponse response)
    {
        RequiredArgument.NotNullOrEmpty(queries, nameof(queries));
        RequiredArgument.NotNull(response, nameof(response));

        CheckResponse(response);

        Dictionary<string, JToken> result = new ();

        foreach (IQuery query in queries)
        {
            string resultName = string.IsNullOrWhiteSpace(query.AliasName) ? query.Name : query.AliasName;
            JToken value = response.Data.GetValue(resultName);
            result.Add(resultName, value);
        }

        return result;
    }

    private static void CheckResponse(GraphQLResponse response)
    {
        RequiredArgument.NotNull(response, nameof(response));

        if (response.Errors != null)
        {
            throw new GraphQLClientException(response.Errors);
        }
    }

    private static GraphQLRequest CreateGraphQLResquest(params IQuery[] queries)
    {
        RequiredArgument.NotNullOrEmpty(queries, nameof(queries));

        return new GraphQLRequest { Query = "{" + string.Concat(queries.Select(q => q.Build())) + "}" };
    }
}
