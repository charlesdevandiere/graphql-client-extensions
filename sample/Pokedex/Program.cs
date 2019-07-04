using GraphQL.Client;
using GraphQL.Client.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args)
                {
                    var pokemon = await GetPokemon(arg);
                    Console.WriteLine(pokemon);
                }
            }
            else
            {
                var pokemons = await GetPokemons();
                foreach (var pokemon in pokemons)
                {
                    Console.WriteLine(pokemon);
                }
            }
        }

        private static async Task<Pokemon> GetPokemon(string name)
        {
            var query = new Query<Pokemon>("pokemon")
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

            using (var client = new GraphQLClient("https://graphql-pokemon.now.sh"))
            {
                var pokemon = await client.Get<Pokemon>(query);

                return pokemon;
            }
        }

        private static async Task<Pokemon[]> GetPokemons()
        {
            var query = new Query<Pokemon>("pokemons")
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

            using (var client = new GraphQLClient("https://graphql-pokemon.now.sh"))
            {
                var pokemons = await client.Get<Pokemon[]>(query);

                return pokemons;
            }
        }
    }
}
