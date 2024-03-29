using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Query.Builder;
using GraphQL.Query.Builder.Formatter.NewtonsoftJson;
using Newtonsoft.Json.Linq;
using Shared.Models;
using Xunit;

namespace GraphQL.Client.Extensions.IntegrationTests;

public class GraphQLClientExtensionsTests
{
    const string URL = "https://graphql-pokemon2.vercel.app/";
    // The official URL https://graphql-pokemon.now.sh is actualy down.

    private readonly QueryOptions options = new()
    {
        Formatter = NewtonsoftJsonPropertyNameFormatter.Format
    };

    [Fact]
    public async Task TestGet()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("pokemon", this.options)
            .AddArguments(new { name = "pikachu" })
            .AddField(p => p.Id)
            .AddField(p => p.Number)
            .AddField(p => p.Name)
            .AddField(p => p.Height, hq => hq
                .AddField(h => h.Minimum)
                .AddField(h => h.Maximum)
            )
            .AddField(p => p.Weight, wq => wq
                .AddField(w => w.Minimum)
                .AddField(w => w.Maximum)
            )
            .AddField(p => p.Types)
            .AddField(p => p.Attacks, aq => aq
                .AddField<Attack>(a => a.Fast, fq => fq
                    .AddField(f => f.Name)
                    .AddField(f => f.Type)
                    .AddField(f => f.Damage)
                )
                .AddField<Attack>(a => a.Special, sq => sq
                    .AddField(f => f.Name)
                    .AddField(f => f.Type)
                    .AddField(f => f.Damage)
                )
            );

        using GraphQLClient client = new(URL);

        Pokemon pikachu = await client.Get<Pokemon>(query);

        Assert.NotNull(pikachu);
        Assert.Equal("UG9rZW1vbjowMjU=", pikachu.Id);
        Assert.Equal("025", pikachu.Number);
        Assert.Equal("Pikachu", pikachu.Name);
        Assert.Equal("0.35m", pikachu.Height.Minimum);
        Assert.Equal("0.45m", pikachu.Height.Maximum);
        Assert.Equal("5.25kg", pikachu.Weight.Minimum);
        Assert.Equal("6.75kg", pikachu.Weight.Maximum);
        Assert.Single(pikachu.Types);
        Assert.Equal("Electric", pikachu.Types[0]);
        Assert.Equal(2, pikachu.Attacks.Fast.Length);
        Assert.Equal("Quick Attack", pikachu.Attacks.Fast[0].Name);
        Assert.Equal("Normal", pikachu.Attacks.Fast[0].Type);
        Assert.Equal(10, pikachu.Attacks.Fast[0].Damage);
        Assert.Equal("Thunder Shock", pikachu.Attacks.Fast[1].Name);
        Assert.Equal("Electric", pikachu.Attacks.Fast[1].Type);
        Assert.Equal(5, pikachu.Attacks.Fast[1].Damage);
        Assert.Equal(3, pikachu.Attacks.Special.Length);
        Assert.Equal("Discharge", pikachu.Attacks.Special[0].Name);
        Assert.Equal("Electric", pikachu.Attacks.Special[0].Type);
        Assert.Equal(35, pikachu.Attacks.Special[0].Damage);
        Assert.Equal("Thunder", pikachu.Attacks.Special[1].Name);
        Assert.Equal("Electric", pikachu.Attacks.Special[1].Type);
        Assert.Equal(100, pikachu.Attacks.Special[1].Damage);
        Assert.Equal("Thunderbolt", pikachu.Attacks.Special[2].Name);
        Assert.Equal("Electric", pikachu.Attacks.Special[2].Type);
        Assert.Equal(55, pikachu.Attacks.Special[2].Damage);
    }

    [Fact]
    public async Task TestGetList()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("pokemons", this.options)
            .AddArguments(new { first = 10 })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        IEnumerable<Pokemon> pokemons = await client.Get<IEnumerable<Pokemon>>(query);

        Assert.NotNull(pokemons);
        Assert.Equal(10, pokemons.Count());
        Assert.All(pokemons, pokemon =>
        {
            Assert.NotNull(pokemon);
            Assert.NotEmpty(pokemon.Name);
        });
    }

    [Fact]
    public async Task TestGetBatch()
    {
        IQuery<Pokemon> query(string name) => new Query<Pokemon>("pokemon", this.options)
            .Alias(name)
            .AddArguments(new { name })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        IReadOnlyDictionary<string, JToken> batch = await client.GetBatch(new IQuery[] { query("Pikachu"), query("Bulbasaur") });

        Pokemon pikachu = batch["Pikachu"].ToObject<Pokemon>();
        Assert.NotNull(pikachu);
        Assert.Equal("Pikachu", pikachu.Name);

        Pokemon bulbasaur = batch["Bulbasaur"].ToObject<Pokemon>();
        Assert.NotNull(bulbasaur);
        Assert.Equal("Bulbasaur", bulbasaur.Name);
    }

    [Fact]
    public async Task TestPost()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("pokemon", this.options)
            .AddArguments(new { name = "pikachu" })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        Pokemon pikachu = await client.Post<Pokemon>(query);

        Assert.NotNull(pikachu);
        Assert.Equal("Pikachu", pikachu.Name);
    }

    [Fact]
    public async Task TestPostBatch()
    {
        IQuery<Pokemon> query(string name) => new Query<Pokemon>("pokemon", this.options)
            .Alias(name)
            .AddArguments(new { name })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        IReadOnlyDictionary<string, JToken> batch = await client.PostBatch(new IQuery[] { query("Pikachu"), query("Bulbasaur") });

        Pokemon pikachu = batch["Pikachu"].ToObject<Pokemon>();
        Assert.NotNull(pikachu);
        Assert.Equal("Pikachu", pikachu.Name);

        Pokemon bulbasaur = batch["Bulbasaur"].ToObject<Pokemon>();
        Assert.NotNull(bulbasaur);
        Assert.Equal("Bulbasaur", bulbasaur.Name);
    }

    [Fact]
    public async Task TestStringResult()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("pokemon", this.options)
            .AddArguments(new { name = "pikachu" })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        string json = await client.Get<string>(query);

        JToken jToken = JToken.Parse(json);

        Assert.Single(jToken);
        Assert.Equal("Pikachu", jToken["name"]);
    }

    [Fact]
    public async Task TestJTokenResult()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("pokemon", this.options)
            .AddArguments(new { name = "pikachu" })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        JToken jToken = await client.Get<JToken>(query);

        Assert.Single(jToken);
        Assert.Equal("Pikachu", jToken["name"]);
    }

    [Fact]
    public async Task TestError()
    {
        IQuery<Pokemon> query = new Query<Pokemon>("wrongQueryName", this.options)
            .AddArguments(new { name = "pikachu" })
            .AddField(p => p.Name);

        using GraphQLClient client = new(URL);

        GraphQLClientException exception = await Assert.ThrowsAsync<GraphQLClientException>(async () => await client.Post<Pokemon>(query));
        Assert.Equal(
            $"The GraphQL request returns errors.{Environment.NewLine}Cannot query field \"wrongQueryName\" on type \"Query\".",
            exception.Message);
    }
}
