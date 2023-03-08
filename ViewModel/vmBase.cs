using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokémon.ViewModel
{
    public class vmBase : INotifyPropertyChanged
    {


        /*EVENT HANDLERS*/
        public event PropertyChangedEventHandler PropertyChanged;

        /*EVENT METHODS*/
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


    }
}
