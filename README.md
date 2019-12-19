# GraphQL Client Extensions

![logo](https://raw.githubusercontent.com/charlesdevandiere/graphql-client-extensions/master/logo.png)

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model, inspired by [Getit](https://github.com/Revmaker/Getit)

[![Build Status](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_apis/build/status/charlesdevandiere.graphql-client-extensions?branchName=master)](https://dev.azure.com/charlesdevandiere/charlesdevandiere/_build/latest?definitionId=1&branchName=master)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/charlesdevandiere/charlesdevandiere/1/master)
[![Nuget](https://img.shields.io/nuget/v/GraphQL.Client.Extensions.svg?color=blue&logo=nuget)](https://www.nuget.org/packages/GraphQL.Client.Extensions)
[![Downloads](https://img.shields.io/nuget/dt/GraphQL.Client.Extensions.svg?logo=nuget)](https://www.nuget.org/packages/GraphQL.Client.Extensions)

See complete documentation [here](https://charlesdevandiere.github.io/graphql-client-extensions/)

See sample [here](sample/Pokedex)

## Install

```console
> dotnet add package GraphQL.Client.Extensions
```

## Usage

```csharp
// create the query
var query = new Query<Human>("humans") // set the name of the query
    .AddArguments(new { id = "uE78f5hq" }) // add query arguments
    .AddField(h => h.FirstName) // add firstName field
    .AddField(h => h.LastName) // add lastName field
    .AddField( // add a sub-object field
        h => h.HomePlanet, // set the name of the field
        sq => sq /// build the sub-query
            .AddField(p => p.Name)
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

## Credits

Rocket by Gregor Cresnar from the Noun Project
