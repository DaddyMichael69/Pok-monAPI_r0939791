using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pokémon.ViewModel;

namespace Pokémon.Model
{
    // pokemon species endpoint
    public class Pokedex
    {
        public Pokedex()
        {
            PokedexLoaded = new ObservableCollection<Pokemon>();
        }

        public int Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public ObservableCollection<PokedexResults> Results { get; set; }
        public ObservableCollection<Pokemon> PokedexLoaded { get; set; }
    }



    public class PokedexResults
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
