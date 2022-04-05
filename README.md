# GraphQL Client Extensions

![logo](https://raw.githubusercontent.com/charlesdevandiere/graphql-client-extensions/master/logo.png)

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model.

[![Build Status](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_apis/build/status/charlesdevandiere.graphql-client-extensions?branchName=master)](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_build/latest?definitionId=1&branchName=master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/charlesdevandiere/charlesdevandiere/1/master)
[![Nuget](https://img.shields.io/nuget/v/GraphQL.Client.Extensions.svg?color=blue&logo=nuget)](https://www.nuget.org/packages/GraphQL.Client.Extensions)
[![Downloads](https://img.shields.io/nuget/dt/GraphQL.Client.Extensions.svg?logo=nuget)](https://www.nuget.org/packages/GraphQL.Client.Extensions)

Uses [GraphQL.Query.Builder](https://github.com/charlesdevandiere/graphql-query-builder-dotnet) for query building.

See complete documentation [here](https://charlesdevandiere.github.io/graphql-client-extensions/)

See sample [here](https://github.com/charlesdevandiere/graphql-client-extensions/tree/master/sample/Pokedex)

## Install

```console
dotnet add package GraphQL.Client.Extensions
```

## Usage

```csharp
// create the query with GraphQL.Query.Builder
var query = new Query<Human>("humans") // set the name of the query
    .AddArguments(new { id = "uE78f5hq" }) // add query arguments
    .AddField(h => h.FirstName) // add firstName field
    .AddField(h => h.LastName) // add lastName field
    .AddField( // add a sub-object field
        h => h.HomePlanet, // set the name of the field
        sq => sq /// build the sub-query
            .AddField(p => p.Name)
    )
    .AddField<human>( // add a sub-list field
        h => h.Friends,
        sq => sq
            .AddField(f => f.FirstName)
            .AddField(f => f.LastName)
    );
// this corresponds to :
// humans (id: "uE78f5hq") {
//   FirstName
//   LastName
//   HomePlanet {
//     Name
//   }
//   Friends {
//     FirstName
//     LastName
//   }
// }

using (var client = new GraphQLClient("<url>"))
{
    // run the query
    Human human = await client.Get<Human>(query);
}
```

## Dependencies

- [Dawn.Guard](https://www.nuget.org/packages/Dawn.Guard/) (>= 1.12.0)
- [Newtonsoft.Json](https://www.nuget.org/packages/GraphQL.Client/) (>= 12.0.3)
- [GraphQL.Client](https://www.nuget.org/packages/GraphQL.Client/) (>= 1.0.3)
- [GraphQL.Query.Builder](https://www.nuget.org/packages/GraphQL.Query.Builder/) (>= 1.6.0)
