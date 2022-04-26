using System;
using System.Collections.Generic;
using Pokedex;
using Shared.Models;

PokemonService service = new("https://graphql-pokemon2.vercel.app/");
// The official URL https://graphql-pokemon.now.sh is actualy down.

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

foreach (Pokemon pokemon in pokemons)
{
    Console.WriteLine(pokemon);
}
