# GraphQL Client Extensions

![logo](https://raw.githubusercontent.com/charlesdevandiere/graphql-client-extensions/master/logo.png)

Extensions for [GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) to build graphQL queries from a C# model, inspired by [Getit](https://github.com/Revmaker/Getit)

## Install

Run this command with dotnet CLI:

```batch
dotnet add package GraphQL.Client.Extensions
```

## Usage

### Create a query

The query building is based on the object which returns.

#### Entities definition

In a first time, you need to create POCOs.

```csharp
class Human
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Planet HomePlanet { get; set; }
    public IEnumerable<Human> Friends { get; set; }
}

class Planet
{
    public string Name { get; set; }
}
```

#### Creation of the query

After that, you can write a query like this :

```csharp
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
            .AddField(h => h.FirstName)
            .AddField(h => h.LastName)
    );
```

This corresponds to :

```txt
humans (id: "uE78f5hq") {
  FirstName
  LastName
  HomePlanet {
    Name
  }
  Friends {
    FirstName
    LastName
  }
}
```

By default, the `AddField()` method use the property name as field name.
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

    [JsonProperty("friends")]
    public IEnumerable<Human> Friends { get; set; }
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

You can run the query using two GraphQLCLient extension methods:

* ```Get<T>(IQuery query)```
* ```Post<T>(IQuery query)```

Example:

```csharp
using (var client = new GraphQLClient())
{
    Human human = client.Get<Human>(query);
}
```
