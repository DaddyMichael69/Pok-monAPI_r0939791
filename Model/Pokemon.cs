using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Media;
using static Pokémon.Model.AllAPI;

namespace Pokémon.Model
{
    public class Pokemon
    {
        /*PROPERTIES*/
        public List<Ability> abilities { get; set; }
        public int base_experience { get; set; }
        public List<Form> forms { get; set; }
        public List<GameIndex> game_indices { get; set; }
        public int height { get; set; }
        public List<HeldItem> held_items { get; set; }
        public int id { get; set; }
        public bool is_default { get; set; }
        public string location_area_encounters { get; set; }
        public List<Move> moves { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public List<object> past_types { get; set; }
        public Species species { get; set; }
        public Sprites sprites { get; set; }
        public ObservableCollection<Stat> stats { get; set; }
        public ObservableCollection<PokeType> types { get; set; }
        public int weight { get; set; }
    }

    public class PokeType
    {
        public int slot { get; set; }
        public PokeType2 type { get; set; }
    }

    public class PokeType2 : PokeType
    {
        /*PROPS*/
        public string name { get; set; }
        public string url { get; set; }
    }


    public class Sprites
    {
        /*PROPERTIES*/
        public ImageSource back_default { get; set; }
        public ImageSource back_female { get; set; }
        public ImageSource back_shiny { get; set; }
        public ImageSource back_shiny_female { get; set; }
        public ImageSource front_default { get; set; }
        public ImageSource front_female { get; set; }
        public ImageSource front_shiny { get; set; }
        public ImageSource front_shiny_female { get; set; }
        public Other other { get; set; }
        public Versions versions { get; set; }
    }

    public class Stat
    {
        public int base_stat { get; set; }
        public int effort { get; set; }
        public Stat2 stat { get; set; }
    }

    public class Stat2
    {
        public string name { get; set; }
        public string url { get; set; }
    }

}
