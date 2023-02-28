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
        public int PokeBalls { get; set; }
        public ItemInfo Pokeballs2 { get; set; }
        public DispatcherTimer ResetPokeballTimer { get; set; }     // to do set timer on pokeballs



        /*CONSTRUCTOR*/
        public Player()
        {
            PlayerPokemonList = new ObservableCollection<Pokemon>();
            Pokeballs2 = new ItemInfo();
            PokeBalls = 30000;
        }

        /*METHODS*/
        // check if the player has any pokeballs
        public bool CheckPokeballCount()
        {

            if (PokeBalls > 0)
            {

                return true;
            }
            else
            {
                ResetPokeballsTimer();
                return false;
            }
        }
        
        //deplete pokéballs
        public void DepletePokeBalls() 
        {
            PokeBalls--;
        }

        public void CatchPokemon(Pokemon pokemon) 
        {
            PlayerPokemonList.Add(pokemon);
        }

        public void ResetPokeballsTimer() 
        {
            
        }


    }
}
