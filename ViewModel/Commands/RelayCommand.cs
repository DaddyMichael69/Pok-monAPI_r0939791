using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Pokémon.ViewModel;

namespace Pokémon.Commands
{
    public class RelayCommand : ICommand
    {

        //deze Action property wordt uitgevoerd wanneer
        //we een commando koppelen
        private Action<object> _action;



        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;


        //deze methode zal beslissen of het commando mag uitgevoerd worden
        //(click-event van de knop)
        //als ButtonCommand uitgevoerd wordt
        //commandparameter vanuit GUI-> hello world -> wordt hier object? parameter
        public bool CanExecute(object? parameter)
        {
            //throw new NotImplementedException();
            return true;
        }


        //deze methode bevat de eigenlijke logica
        //als de CanExecute-methode een true zal retourneren
        //dan zal de execute methode uitgevoerd worden

        //indien de commandparameter ingevuld is in de GUI --> hello world
        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}
