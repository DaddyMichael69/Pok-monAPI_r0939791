using Pokémon.Model;
using Pokémon.View;
using Pokémon.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using static Pokémon.Model.ItemData;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

////
/// TO DO:      -- foutafhandeling
///             -- pokedex fix --> pokemon species ipv regular pokemon
///             -- try to show 3 pokeball objects in the stackpane


namespace Pokémon.ViewModel
{
    public class vmMainWindow : INotifyPropertyChanged
    {
        /*EVENT HANDLERS*/
        public event PropertyChangedEventHandler PropertyChanged;


        /*FIELD*/
        private Uri _baseUri;                               // API
        private HttpResponseMessage _response;
        private APIService _apiService;
        private Pokemon _pokemon;                           // objects
        private PokemonSpecies _pokemonSpecies;
        private Pokedex _pokedex;
        private Player _player;
        private DispatcherTimer _resetPokeballs;
        private bool _hasPokeballs;
        private bool _hasCaughtPokemon;

        private ICommand _searchCommand;                    // commands
        private ICommand _randomSearchCommand;
        private ICommand _browsePokedex;
        private ICommand _catchCommand;



        /*CONSTRUCTOR*/
        public vmMainWindow(Uri parBaseUri)
        {
            _baseUri = parBaseUri;
            _apiService = new APIService(_baseUri);
            _pokemon = new Pokemon();
            _pokedex = new Pokedex();
            LoadPokemonFromAPI("https://pokeapi.co/api/v2/pokemon/");
            _player = new Player();
            LoadPokeballs();

            _searchCommand = new RelayCommand(new Action<object>(SearchPokemon));
            _randomSearchCommand = new RelayCommand(new Action<object>(RandomSearchPokemon));
            _browsePokedex = new RelayCommand(new Action<object>(NextPreviousPokedex));
            _catchCommand = new RelayCommand(new Action<object>(CatchPokemonCommand));

        }


        /*PROPERTIES*/
        public Pokemon Pokemon
        {
            get => _pokemon;
            set { _pokemon = value;
                OnPropertyChanged("Pokemon");
            }
        }
        public Pokedex PokedexMain 
        { 
            get => _pokedex;
            set 
            {
                _pokedex = value;
                OnPropertyChanged("PokedexMain");
                    
            }
        }
        public ObservableCollection<Pokemon> PokedexLoaded       
        {
            get => _pokedex.PokedexLoaded;
            set 
            {
                _pokedex.PokedexLoaded = value;
                OnPropertyChanged("PokedexLoaded");
            }
        }
        public Player Player                            // TO DO Give pokeballs to the player + make player able to catch using pokeballs 
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged("Player");
            }
        }
        public ItemInfo PlayerPokeballs
        {
            get => _player.Pokeballs2;
            set
            {
                _player.Pokeballs2 = value;
                OnPropertyChanged("PlayerPokeballs");
            }
        }
        public ImageSource PokeballSprite
        {
            get => _player.Pokeballs2.sprites.@default;
            set
            {
                _player.Pokeballs2.sprites.@default = value;
                OnPropertyChanged("PokeballSprite");
            }
        }

        public ICommand SearchCommand                   
        {
            get => _searchCommand;
            set => _searchCommand = value;
        }
        public ICommand RandomSearchCommand
        {
            get => _randomSearchCommand;
            set => _randomSearchCommand = value;
        }
        public ICommand BrowsePokedex 
        { 
            get => _browsePokedex; 
            set => _browsePokedex = value; 
        }
        public ICommand CatchCommand
        { 
            get => _catchCommand; 
            set => _catchCommand = value; 
        }


        /*METHODS*/
        // event handler method:
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /*METHODS*/
        #region searchtab
        // Search pokemon @ API
        public void SearchPokemon(object commandParameter)
        {
            _response = _apiService.APICall("pokemon", commandParameter.ToString());
            Pokemon = _apiService.ConvertAPIResponse<Pokemon>(_response);
        }

        // Search RANDOM Pokemon @ API
        public void RandomSearchPokemon(object commandParameter)
        {
            Random randomPoke = new Random();
            string strRandomPoke = randomPoke.Next(1, 152).ToString();
            SearchPokemon(strRandomPoke);
        }

        // Catch Pokemon
        public void CatchPokemonCommand(object commandParameter) 
        {
            _hasPokeballs = Player.CheckPokeballCount();

            if (_hasPokeballs)
            {
                //throw a pokeball
                Player.DepletePokeBalls();

                // receive pokemon from UI
                Pokemon pokemon = (Pokemon)commandParameter;

                // convert pokemon > pokemonspecies
                _response = _apiService.APICall(pokemon.species.url, "");            
                _pokemonSpecies = _apiService.ConvertAPIResponse<PokemonSpecies>(_response);

                //calculate odds
                _hasCaughtPokemon = CaptureToss(_pokemonSpecies);

                //add pokemon if player wins toss
                if (_hasCaughtPokemon)
                {
                    Player.CatchPokemon(pokemon);
                    MessageBox.Show("Caught " + pokemon.name);
                    Pokemon = null;
                }

                else { 
                    MessageBox.Show("you have " + Player.PokeBalls.ToString() + " left");
                }

            }

        }

        // player vs pokemon catch / toss
        private bool CaptureToss(PokemonSpecies pokemonespecie) 
        {
            // CPU chances
            int iCaptureRate = ((int)_pokemonSpecies.capture_rate * 100) / 255;  // check pokemon capture rate
            
            // Player chances
            Random rndCaptureCounterRate = new Random();
            int counterCaptureRate = rndCaptureCounterRate.Next(1, 255);

            if (counterCaptureRate < iCaptureRate)
            {
                return true;
            }

            else
            {
                return false;

            }

        }

        // load pokeballs   // unfinished
        public void LoadPokeballs()                 // try to show 3 pokeball objects in the stackpanel
        {
            _response = _apiService.APICall("item/4", "");            // pokemons oproepen
            PlayerPokeballs = _apiService.ConvertAPIResponse<ItemData.ItemInfo>(_response);            // deserialize pokemon list

        } 
        #endregion

        #region pokedex
        // Load pokemon list @ POKEDEX
        public async Task LoadPokemonFromAPI(string endpoint)
        {
             _response = _apiService.APICall(endpoint, "");            // pokemons oproepen
            PokedexMain = _apiService.ConvertAPIResponse<Pokedex>(_response);            // deserialize pokemon list
            AddPokemonToPokedex(PokedexMain);  // add pokemon to pokedexLoaded list (pokedex class)
        }

        // extract every pokemon from the pokedex(data)
        public async Task AddPokemonToPokedex(Pokedex pokedex) 
        {
            // loop through pokedata and use names to summon pokemon
            foreach (PokedexResults pokeData in pokedex.Results)
            {
                // api call pokedata > pokemon
                _response = _apiService.APICall(pokeData.Url, "");
                Pokemon pokemon = _apiService.ConvertAPIResponse<Pokemon>(_response);
                
                // pokemon 1st generation filter                 
                if (pokemon.id < 152)
                {
                    PokedexLoaded.Add(pokemon);
                }
            }  
        }

        // Search pokemon @ API
        public async void NextPreviousPokedex(object commandParameter)     
        {
            // Check if the pokedex.next is not null
            if (commandParameter != null)
            {
                // load pokemon from pokedex.next
                LoadPokemonFromAPI(commandParameter.ToString());

                //property change
                OnPropertyChanged("PokedexLoaded");
            }
        }
        #endregion
    }
}




