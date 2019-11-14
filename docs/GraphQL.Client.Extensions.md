## `GraphQLClientExtensions`

Extensions for the `GraphQL.Client.GraphQLClient` class.
```csharp
public static class GraphQL.Client.Extensions.GraphQLClientExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Task<T>` | Get(this `GraphQLClient` gqlClient, `IQuery` query, `CancellationToken` cancellationToken = null) | Send a `GraphQL.Common.Request.GraphQLRequest` via GET. | 
| `Task<IReadOnlyDictionary<String, JToken>>` | GetBatch(this `GraphQLClient` gqlClient, `IQuery[]` queries, `CancellationToken` cancellationToken = null) | Send a `GraphQL.Common.Request.GraphQLRequest` composed of a query batch via GET. | 
| `Task<T>` | Post(this `GraphQLClient` gqlClient, `IQuery` query, `CancellationToken` cancellationToken = null) | Send a `GraphQL.Common.Request.GraphQLRequest` via POST. | 
| `Task<IReadOnlyDictionary<String, JToken>>` | PostBatch(this `GraphQLClient` gqlClient, `IQuery[]` queries, `CancellationToken` cancellationToken = null) | Send a `GraphQL.Common.Request.GraphQLRequest` composed of a query batch via POST. | 


## `IQuery`

Query interface
```csharp
public interface GraphQL.Client.Extensions.IQuery

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AliasName | Gets the query alias name. | 
| `String` | Name | Gets the query name. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Build() | Builds the query. | 


## `IQuery<TSource>`

Query of TSource interface
```csharp
public interface GraphQL.Client.Extensions.IQuery<TSource>
    : IQuery

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<String, Object>` | ArgumentsMap | Gets the arguments map. | 
| `List<Object>` | SelectList | Gets the select list. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IQuery<TSource>` | AddArgument(`String` key, `Object` value) | Adds a new argument to the query. | 
| `IQuery<TSource>` | AddArguments(`Dictionary<String, Object>` arguments) | Adds arguments to the query. | 
| `IQuery<TSource>` | AddArguments(`TArguments` arguments) | Adds arguments to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, TProperty>>` selector) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`String` field) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, TSubSource>>` selector, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, IEnumerable<TSubSource>>>` selector, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`String` field, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | Alias(`String` alias) | Sets the query alias name. | 


## `IQueryStringBuilder`

Builds a GraphQL query from the Query Object. For parameters it  support simple parameters, ENUMs, Lists, and Objects.  For selections fields it supports sub-selects with params as above.    Most all structures can be recursive, and are unwound as needed
```csharp
public interface GraphQL.Client.Extensions.IQueryStringBuilder

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Build(`IQuery<TSource>` query) | Build the entire query into a string. This will take  the query object and build a graphql query from it. This  returns the query, but not the outer block. This is done so  you can use the output to batch the queries | 
| `void` | Clear() | Clear the QueryStringBuilder and all that entails | 


## `Query<TSource>`

The Query Class is a simple class to build out graphQL  style queries. It will build the parameters and field lists  similar in a way you would use a SQL query builder to assemble  a query. This will maintain the response for the query
```csharp
public class GraphQL.Client.Extensions.Query<TSource>
    : IQuery<TSource>, IQuery

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AliasName | Gets the alias name. | 
| `Dictionary<String, Object>` | ArgumentsMap | Gets the arguments map. | 
| `String` | Name | Gets the query name. | 
| `List<Object>` | SelectList | Gets the select list. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IQuery<TSource>` | AddArgument(`String` key, `Object` value) | Adds a new argument to the query. | 
| `IQuery<TSource>` | AddArguments(`Dictionary<String, Object>` arguments) | Adds arguments to the query. | 
| `IQuery<TSource>` | AddArguments(`TArguments` arguments) | Adds arguments to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, TProperty>>` selector) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`String` field) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, TSubSource>>` selector, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`Expression<Func<TSource, IEnumerable<TSubSource>>>` selector, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | AddField(`String` field, `Func<IQuery<TSubSource>, IQuery<TSubSource>>` build) | Adds a field to the query. | 
| `IQuery<TSource>` | Alias(`String` alias) | Sets the query alias name. | 
| `String` | Build() | Builds the query. | 


## `QueryFormaters`

Query formater class
```csharp
public static class GraphQL.Client.Extensions.QueryFormaters

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Func<String, String>` | CamelCaseFormater | Camel case formater | 


## `QueryOptions`

Query options class
```csharp
public class GraphQL.Client.Extensions.QueryOptions

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Func<String, String>` | Formater | Gets or sets the formater | 
| `Func<IQueryStringBuilder>` | QueryStringBuilderFactory | Gets or sets the query string builder factory | 


## `QueryStringBuilder`

Builds a GraphQL query from the Query Object. For parameters it  support simple parameters, ENUMs, Lists, and Objects.  For selections fields it supports sub-selects with params as above.    Most all structures can be recursive, and are unwound as needed
```csharp
public class GraphQL.Client.Extensions.QueryStringBuilder
    : IQueryStringBuilder

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `StringBuilder` | QueryString | The query string builder. | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddFields(`IQuery<TSource>` query) | Adds fields to the query sting. This will use the SelectList  structure from the query to build the graphql select list. This  will recurse as needed to pick up sub-selects that can contain  parameter lists. | 
| `void` | AddParams(`IQuery<TSource>` query) | This take all parameter data  and builds the string. This will look in the query and  use the WhereMap for the list of data. The data can be  most any type as long as it's one that we support. Will  resolve nested structures | 
| `String` | Build(`IQuery<TSource>` query) | Build the entire query into a string. This will take  the query object and build a graphql query from it. This  returns the query, but not the outer block. This is done so  you can use the output to batch the queries | 
| `String` | BuildQueryParam(`Object` value) | Recurse an object which could be a primitive or more  complex structure. This will return a string of the value  at the current level. Recursion terminates when at a terminal  (primitive). | 
| `void` | Clear() | Clear the QueryStringBuilder and all that entails | 


