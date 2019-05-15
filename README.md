# GraphQL Client Extensions

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model, inspired by [Getit](https://github.com/Revmaker/Getit)

[![Build Status](https://charlesdevandiere.visualstudio.com/charlesdevandiere/_apis/build/status/charlesdevandiere.graphql-client-extensions?branchName=master)](https://charlesdevandiere.visualstudio.com/charlesdevandiere/_build/latest?definitionId=1&branchName=master)

## Install

```batch
dotnet add package GraphQL.Client.Extensions
```

## Usage

```csharp
// create the query
var query = new Query<Human>()
    .Name("humans")
    .Select(h => h.FirstName)
    .Select(h => h.LastName)
    .SubSelect(h => h.Planet, new Query<Planet>()
        .Select(p => p.Name))
    .Where("id", "uE78f5hq");
// this corresponds to :
// humans (id: "uE78f5hq") {
//   firstName
//   lastName
//   planet {
//     name
//   }
// }

using (var client = new GraphQLClient())
{
    // run the query
    var human = client.Get<Human>(query);
}
```
