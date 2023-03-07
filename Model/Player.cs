using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Pokémon.Model.ItemData;

namespace Pokémon.Model
{
    public class Player
    {
        /*PROPERTIES*/
        public ObservableCollection<Pokemon> PlayerPokemonList { get; set; }
        public ObservableCollection<ItemInfo> PokeballList { get; set; }
        public ItemInfo Pokeball { get; set; }



        /*CONSTRUCTOR*/
        public Player()
        {
            PlayerPokemonList = new ObservableCollection<Pokemon>();
            PokeballList= new ObservableCollection<ItemInfo>();
            Pokeball = new ItemInfo();
        }

        /*METHODS*/
        // catch pokemon
        public void CatchPokemon(Pokemon pokemon) 
        {
            pokemon.increaseCaughtCount();      //increase caught counter
            PlayerPokemonList.Add(pokemon);     // add pokemon to player pokelist
        }

        // count pokemon
        public void CountPokemon(Pokemon parPokemon)
        {
            foreach (Pokemon pokemon  in PlayerPokemonList)
            {
                if (pokemon == parPokemon)
                {
                    pokemon.CaughtCount++;
                }
                else
                {
                    pokemon.CaughtCount = 1;
                }
            }
        }

        // check if the player has any pokeballs
        public bool CheckPokeballCount()
        {
            if (PokeballList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        //deplete pokéballs
        public void DepletePokeBalls() 
        {
            PokeballList.RemoveAt(0);
        }


        // add 3 balls to pokeballList
        public void RefillPokeballList(ItemInfo pokeballs) 
        {
            if (pokeballs != null)
            {
                for (int i = 0; i < 300; i++)
                {
                    PokeballList.Add(pokeballs);
                }
            }

        }

    }
}
