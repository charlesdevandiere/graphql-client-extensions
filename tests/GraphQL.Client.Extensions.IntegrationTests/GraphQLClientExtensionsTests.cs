using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shared.Models;
using Xunit;

namespace GraphQL.Client.Extensions.IntegrationTests
{
    public class GraphQLClientExtensionsTests
    {
        const string URL = "https://graphql-pokemon.now.sh";

        [Fact]
        public async Task TestGet()
        {
            var query = new Query<Pokemon>("pokemon")
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

            using var client = new GraphQLClient(URL);

            Pokemon pikachu = await client.Get<Pokemon>(query);

            Assert.NotNull(pikachu);
            Assert.Equal("UG9rZW1vbjowMjU=", pikachu.Id);
            Assert.Equal("025", pikachu.Number);
            Assert.Equal("Pikachu", pikachu.Name);
            Assert.Equal("0.35m", pikachu.Height.Minimum);
            Assert.Equal("0.45m", pikachu.Height.Maximum);
            Assert.Equal("5.25kg", pikachu.Weight.Minimum);
            Assert.Equal("6.75kg", pikachu.Weight.Maximum);
            Assert.Equal(1, pikachu.Types.Count());
            Assert.Equal("Electric", pikachu.Types[0]);
            Assert.Equal(2, pikachu.Attacks.Fast.Count());
            Assert.Equal("Quick Attack", pikachu.Attacks.Fast[0].Name);
            Assert.Equal("Normal", pikachu.Attacks.Fast[0].Type);
            Assert.Equal(10, pikachu.Attacks.Fast[0].Damage);
            Assert.Equal("Thunder Shock", pikachu.Attacks.Fast[1].Name);
            Assert.Equal("Electric", pikachu.Attacks.Fast[1].Type);
            Assert.Equal(5, pikachu.Attacks.Fast[1].Damage);
            Assert.Equal(3, pikachu.Attacks.Special.Count());
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
            var query = new Query<Pokemon>("pokemons")
                .AddArguments(new { first = 10 })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

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
            Func<string, IQuery<Pokemon>> query = (string name) => new Query<Pokemon>("pokemon")
                .Alias(name)
                .AddArguments(new { name })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

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
            var query = new Query<Pokemon>("pokemon")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            Pokemon pikachu = await client.Post<Pokemon>(query);

            Assert.NotNull(pikachu);
            Assert.Equal("Pikachu", pikachu.Name);
        }

        [Fact]
        public async Task TestPostBatch()
        {
            Func<string, IQuery<Pokemon>> query = (string name) => new Query<Pokemon>("pokemon")
                .Alias(name)
                .AddArguments(new { name })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

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
            var query = new Query<Pokemon>("pokemon")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            string json = await client.Get<string>(query);

            JToken jToken = JToken.Parse(json);

            Assert.Equal(jToken.Count(), 1);
            Assert.Equal(jToken["name"], "Pikachu");
        }

        [Fact]
        public async Task TestJTokenResult()
        {
            var query = new Query<Pokemon>("pokemon")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            JToken jToken = await client.Get<JToken>(query);

            Assert.Equal(jToken.Count(), 1);
            Assert.Equal(jToken["name"], "Pikachu");
        }

        [Fact]
        public async Task TestError()
        {
            var query = new Query<Pokemon>("wrongQueryName")
                .AddArguments(new { name = "pikachu" })
                .AddField(p => p.Name);

            using var client = new GraphQLClient(URL);

            GraphQLClientException exception = await Assert.ThrowsAsync<GraphQLClientException>(async () => await client.Post<Pokemon>(query));
            Assert.Equal(
                $"The GraphQL request returns errors.{Environment.NewLine}Cannot query field \"wrongQueryName\" on type \"Query\".",
                exception.Message);
        }
    }
}
