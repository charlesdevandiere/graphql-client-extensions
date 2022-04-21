using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Client.Extensions;
using GraphQL.Query.Builder;
using GraphQL.Query.Builder.Formatter.NewtonsoftJson;
using Newtonsoft.Json.Linq;
using Shared.Models;

namespace Pokedex
{
    class PokemonService
    {
        private readonly string graphqlPokemonUrl;

        private readonly QueryOptions options = new()
        {
            Formatter = NewtonsoftJsonPropertyNameFormatter.Format
        };

        public PokemonService(string graphqlPokemonUrl)
        {
            this.graphqlPokemonUrl = graphqlPokemonUrl;
        }

        private IQuery pokemonQuery(string name) =>
            new Query<Pokemon>("pokemon", this.options)
                .Alias(name)
                .AddArguments(new { name })
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

        /// <summary>Returns a Pokemon.</summary>
        /// <param name="name">The Pokemon name.</param>
        public async Task<Pokemon> GetPokemon(string name)
        {
            var query = pokemonQuery(name);

            using var client = new GraphQLClient(this.graphqlPokemonUrl);
            var pokemon = await client.Get<Pokemon>(query);

            return pokemon;
        }

        /// <summary>Returns a Pokemon batch</summary>
        /// <param name="names">The Pokemons names</param>
        public async Task<IEnumerable<Pokemon>> GetPokemonBatch(string[] names)
        {
            IQuery[] queries = names.Select(name => pokemonQuery(name)).ToArray();

            using var client = new GraphQLClient(this.graphqlPokemonUrl);
            IReadOnlyDictionary<string, JToken> batch = await client.GetBatch(queries);

            return batch.Values.Select(jToken => jToken.ToObject<Pokemon>());
        }

        /// <summary>Returns all Pokemons</summary>
        public async Task<IEnumerable<Pokemon>> GetAllPokemons()
        {
            var query = new Query<Pokemon>("pokemons", this.options)
                .AddArguments(new { first = 100 })
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
                .AddField(p => p.Types);

            using var client = new GraphQLClient(this.graphqlPokemonUrl);
            var pokemons = await client.Get<IEnumerable<Pokemon>>(query);

            return pokemons;
        }
    }
}
