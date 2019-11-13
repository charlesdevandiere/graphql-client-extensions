using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new PokemonService("https://graphql-pokemon.now.sh");

            IEnumerable<Pokemon> pokemons;

            if (args != null && args.Length > 0)
            {
                if (args.Length == 1)
                {
                    pokemons = new List<Pokemon> { await service.GetPokemon(args[0]) };
                }
                else
                {
                    pokemons = await service.GetPokemonBatch(args);
                }
            }
            else
            {
                pokemons = await service.GetAllPokemons();
            }

            foreach (var pokemon in pokemons)
            {
                Console.WriteLine(pokemon);
            }
        }

    }
}
