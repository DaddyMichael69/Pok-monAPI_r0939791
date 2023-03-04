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
using System.Timers;
using Newtonsoft.Json.Linq;
using Pokémon.ViewModel.Services;

////
/// TO DO:      -- foutafhandeling
///             -- pokedex --> show how many times a pokemon 's been caught
///             --         --> json file saving player data
///             -- Show amount of pokeballs
///             -- timer   --> @ 5 min 
///                        --> start after throw 3


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
        private ErrorLogService _errorLogService;
        private List<ErrorLogService> _errorLogServiceList;

        private Pokemon _pokemon;                           // objects
        private Pokemon _selectedPokemon;
        private PokemonSpecies _pokemonSpecies;
        private Pokedex _pokedex;
        private Player _player;
        private DispatcherTimer _PokeballTimer;
        private TimeSpan _timeLeft;

                                                            // bools
        private bool _hasPokeballs;
        private bool _isActiveTimer;
        private bool _hasCaughtPokemon;

        private ICommand _searchCommand;                    // commands
        private ICommand _randomSearchCommand;
        private ICommand _browsePokedex;
        private ICommand _catchCommand;
        private ICommand _clearSaveDataCommand;


        

        /*CONSTRUCTOR*/
        public vmMainWindow(Uri parBaseUri)
        {
            _baseUri = parBaseUri;
            _apiService = new APIService(_baseUri);
            _errorLogServiceList = new List<ErrorLogService>();

            _pokemon = new Pokemon();
            _pokemonSpecies = new PokemonSpecies();
            _pokedex = new Pokedex();
            LoadPokemonFromAPI("https://pokeapi.co/api/v2/pokemon/");
            _player = new Player();
            LoadPokeballs();
            LoadPlayerPokemonList();

            _searchCommand = new RelayCommand(new Action<object>(SearchPokemon));
            _randomSearchCommand = new RelayCommand(new Action<object>(RandomSearchPokemon));
            _browsePokedex = new RelayCommand(new Action<object>(NextPreviousPokedex));
            _catchCommand = new RelayCommand(new Action<object>(CatchPokemonCommand));
            _clearSaveDataCommand = new RelayCommand(new Action<object>(ErasePlayerPokemonList));

        }


        /*PROPERTIES*/
        public Pokemon Pokemon
        {
            get => _pokemon;
            set { _pokemon = value;
                OnPropertyChanged("Pokemon");
            }
        }
        public Pokemon SelectedPokemon 
        {
            get { return _selectedPokemon; }
            set {
                if (_selectedPokemon != value)
                {
                    _selectedPokemon = value;
                    OnPropertyChanged("SelectedPokemon");
                    OnPropertyChanged("PokemonCaughtCount");
                }
            }
        }
        public int PokemonCaughtCount
        {
            get => _selectedPokemon.CaughtCount;
            set {
                _selectedPokemon.CaughtCount = value;
                OnPropertyChanged("PokemonCaughtCount");
            }
        }
        public PokemonSpecies PokemonSpecies 
        { 
            get => _pokemonSpecies; 
            set => _pokemonSpecies = value; 
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
        public Player Player                            
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged("Player");
            }
        }
        public ObservableCollection<Pokemon> PlayerPokemonList
        {
            get => _player.PlayerPokemonList;
            set
            {
                _player.PlayerPokemonList = value;
                OnPropertyChanged("PlayerPokemonList");
            }
        }
        public ObservableCollection<ItemInfo> PlayerPokeballList
        {
            get => _player.PokeballList;
            set
            {
                _player.PokeballList = value;
                OnPropertyChanged("PlayerPokeballList");
            }
        }
        public ItemInfo PlayerPokeball 
        {
            get => _player.Pokeball;
            set
            {
                _player.Pokeball = value;
                OnPropertyChanged("PlayerPokeballs");
            }
        }
        public ImageSource PokeballSprite
        {
            get => _player.Pokeball.sprites.@default;
            set
            {
                _player.Pokeball.sprites.@default = value;
                OnPropertyChanged("PokeballSprite");
            }
        }
        public DispatcherTimer PokeballTimer
        {
            get => _PokeballTimer;
            set
            {
                _PokeballTimer = value;
                OnPropertyChanged("Pokeballtimer");
            }
        }     
        public TimeSpan TimeLeft 
        { 
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                OnPropertyChanged("TimeLeft");
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
        public ICommand ClearSaveDataCommand 
        { 
            get => _clearSaveDataCommand; 
            set => _clearSaveDataCommand = value; 
        }


        /*EVENTMETHODS*/
        // event handler method:
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            if (TimeLeft <= TimeSpan.Zero)
            {
                _PokeballTimer.Stop();
                _isActiveTimer = false;
                LoadPokeballs();
            }
        }



        /*WORKMETHODS*/
        #region searchtab
        // Search pokemon @ API
        public void SearchPokemon(object commandParameter)
        {
            try
            {
                // extract pokemon
                _response = _apiService.APICall("pokemon", commandParameter.ToString());
                Pokemon pokemon = _apiService.ConvertAPIResponse<Pokemon>(_response);

                // extract pokemon species info
                _response = _apiService.APICall(pokemon.species.url, "");
                PokemonSpecies pokemonSpecies = _apiService.ConvertAPIResponse<PokemonSpecies>(_response);

                // filter out generation I species
                if (pokemonSpecies.generation.name == "generation-i")
                {
                    PokemonSpecies = pokemonSpecies;
                    Pokemon = pokemon;
                }
                else { MessageBox.Show("Generation I only (1-151)"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching for the pokémon...");    // msg 2 user

                //make object from error
                ErrorLogService error = new ErrorLogService()
                {
                    TimeStamp = DateTime.Now,
                    ErrorMsg = ex.Message,
                    Stacktrace = ex.StackTrace
                };

                _errorLogServiceList.Add(error);    // add to error list
                _apiService.AddToJson(_errorLogServiceList, _apiService.JsonErrorLog);  // add to json file
            }

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
                    Player.CatchPokemon(pokemon);                   // add pokemon to player pokemon list
                    MessageBox.Show("Caught " + pokemon.name);      // msg 2 user
                    SavePlayerPokemonList();
                    Pokemon = null;                                 // reset gui
                }

                else { 
                    MessageBox.Show("you have " + Player.PokeballList.Count + " left");
                }
            }

            else
            {
                SetPokeballTimer();
                MessageBox.Show("you have " + Player.PokeballList.Count + " left");
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

            // boolean return
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
        public void LoadPokeballs()              
        {
            _response = _apiService.APICall("item/4", "");            // pokemons oproepen
            PlayerPokeball = _apiService.ConvertAPIResponse<ItemData.ItemInfo>(_response);     // deserialize pokemon list
            Player.RefillPokeballList(PlayerPokeball);             // refill pokeballs
        }

        // set pokeballs timer                          // TO DO --> zet op 5 min
        public void SetPokeballTimer()
        {
            if (!_isActiveTimer)
            {
                _PokeballTimer = new DispatcherTimer();

                _PokeballTimer.Interval = TimeSpan.FromSeconds(1);
                _PokeballTimer.Tick += OnTimerTick;     // activate event method

                _timeLeft = TimeSpan.FromSeconds(60);

                _PokeballTimer.Start();                // start timer

                _isActiveTimer = true;                // set timer bool active
            }
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


        #region Player
        // Load player data
        public void LoadPlayerPokemonList() 
        {
            string jsonPlayerData = _apiService.ReadFromJson();
            _player.PlayerPokemonList = JsonConvert.DeserializeObject<ObservableCollection<Pokemon>>(jsonPlayerData);
        }
        // Save player data
        public void SavePlayerPokemonList() 
        {
            _apiService.WriteToJson(_player.PlayerPokemonList, _apiService.JsonPlayerDataFilePath);
        }
        // Erase player data
        public void ErasePlayerPokemonList(object CommandParameter) 
        {
            _apiService.ResetJson();
        }
        #endregion
    }
}




