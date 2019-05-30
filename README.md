# GraphQL Client Extensions

![logo](https://raw.githubusercontent.com/charlesdevandiere/graphql-client-extensions/master/logo.png)

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model, inspired by [Getit](https://github.com/Revmaker/Getit)

[![Build Status](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_apis/build/status/charlesdevandiere.graphql-client-extensions?branchName=master)](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_build/latest?definitionId=1&branchName=master)

[![Nuget](https://img.shields.io/nuget/v/GraphQL.Client.Extensions.svg?color=blue&logo=nuget)](https://www.nuget.org/packages/GraphQL.Client.Extensions)

See complete documentation [here](https://charlesdevandiere.github.io/graphql-client-extensions/)

## Install

```batch
> dotnet add package GraphQL.Client.Extensions
```

## Usage

```csharp
// create the query
var query = new Query<Human>("humans")
    .Select(h => h.FirstName)
    .Select(h => h.LastName)
    .SubSelect(h => h.HomePlanet, subQuery => subQuery
        .Select(p => p.Name))
    .Where("id", "uE78f5hq");
// this corresponds to :
// humans (id: "uE78f5hq") {
//   firstName
//   lastName
//   homePlanet {
//     name
//   }
// }

using (var client = new GraphQLClient())
{
    // run the query
    var human = client.Get<Human>(query);
}
```

## Credits

Rocket by Gregor Cresnar from the Noun Project
