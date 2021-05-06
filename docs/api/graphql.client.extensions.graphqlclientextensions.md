[`< Back`](./)

---

# GraphQLClientExtensions

Namespace: GraphQL.Client.Extensions

Extensions for the GraphQL.Client.GraphQLClient class.

```csharp
public static class GraphQLClientExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GraphQLClientExtensions](./graphql.client.extensions.graphqlclientextensions)

## Methods

### **Get&lt;T&gt;(GraphQLClient, IQuery, CancellationToken)**

Send a GraphQL.Common.Request.GraphQLRequest via GET.

```csharp
public static Task<T> Get<T>(GraphQLClient client, IQuery query, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br>
Data Type.

#### Parameters

`client` GraphQLClient<br>
The GraphQL Client.

`query` IQuery<br>
The query.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The cancellation token.

#### Returns

Task&lt;T&gt;<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Dupe Key, missing parts or empty parts of a query

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Invalid Configuration

### **GetBatch(GraphQLClient, IQuery[], CancellationToken)**

Send a GraphQL.Common.Request.GraphQLRequest composed of a query batch via GET.

```csharp
public static Task<IReadOnlyDictionary<string, JToken>> GetBatch(GraphQLClient client, IQuery[] queries, CancellationToken cancellationToken)
```

#### Parameters

`client` GraphQLClient<br>
The GraphQL Client.

`queries` IQuery[]<br>
The query batch.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The cancellation token.

#### Returns

[Task&lt;IReadOnlyDictionary&lt;String, JToken&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Dupe Key, missing parts or empty parts of a query

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Invalid Configuration

### **Post&lt;T&gt;(GraphQLClient, IQuery, CancellationToken)**

Send a GraphQL.Common.Request.GraphQLRequest via POST.

```csharp
public static Task<T> Post<T>(GraphQLClient client, IQuery query, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br>
Data Type.

#### Parameters

`client` GraphQLClient<br>
The GraphQL Client.

`query` IQuery<br>
The query.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The cancellation token.

#### Returns

Task&lt;T&gt;<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Dupe Key, missing parts or empty parts of a query

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Invalid Configuration

### **PostBatch(GraphQLClient, IQuery[], CancellationToken)**

Send a GraphQL.Common.Request.GraphQLRequest composed of a query batch via POST.

```csharp
public static Task<IReadOnlyDictionary<string, JToken>> PostBatch(GraphQLClient client, IQuery[] queries, CancellationToken cancellationToken)
```

#### Parameters

`client` GraphQLClient<br>
The GraphQL Client.

`queries` IQuery[]<br>
The query batch.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The cancellation token.

#### Returns

[Task&lt;IReadOnlyDictionary&lt;String, JToken&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Dupe Key, missing parts or empty parts of a query

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Invalid Configuration

---

[`< Back`](./)
