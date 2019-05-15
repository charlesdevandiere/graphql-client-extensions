# GraphQL Client Extensions

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model, inspired by [Getit](https://github.com/Revmaker/Getit)

## Install

Run this command with dotnet CLI:

```batch
dotnet add package GraphQL.Client.Extensions
```

## Usage

### Create a query

There is tow main ways to create queries. Using strings or using expressions.

#### Using strings

```csharp
var query = new Query()
    .Name("humans") // set the name of the query
    .Select("fisrtName", "lastName") // add a list of fields
    .Select(new Query() // add a sub query
        .Name("homePlanet")
        .Select("name")
    )
    .Where("id", "uE78f5hq"); // add filter arguments
```

This corresponds to :

```txt
humans (id: "uE78f5hq") {
  firstName
  lastName
  homePlanet {
    name
  }
}
```

#### Using expressions

Entities definition

```csharp
class Human
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Planet HomePlanet { get; set; }
}

class Planet
{
    public string Name { get; set; }
}
```

Creation of the query

```csharp
var query = new Query<Human>()
    .Name("humans") // set the name of the query
    .Select(h => h.FirstName) // add firstName field
    .Select(h => h.LastName) // add lastName field
    .SubSelect( // add a sub query
        h => h.HomePlanet, // set the name of the sub query with the property name
        new Query<Planet>()
            .Select(p => p.Name)
    )
    .Where("id", "uE78f5hq"); // add filter arguments
```

This corresponds to :

```txt
humans (id: "uE78f5hq") {
  FirstName
  LastName
  HomePlanet {
    Name
  }
}
```

By default, the ```Select``` method use the property name as field name.
You can use custom formater or JsonPropertyAttribute to change this behavior.

```csharp
class Human
{
    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("homePlanet")]
    public Planet HomePlanet { get; set; }
}
```

```csharp
var query = new Query(options: new QueryOptions
            {
                Formater = QueryFormaters.CamelCaseFormater
            });
```

Formater's type is ```Func<string, string>```

### Run the query

You can run the query using tow GraphQLCLient extension methods:

* ```Get<T>(IQuery query)```
* ```Post<T>(IQuery query)```

Example:

```csharp
using (var client = new GraphQLClient())
{
    var human = client.Get<Human>(query);
}
```