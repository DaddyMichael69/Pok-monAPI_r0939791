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
                
        private Action<object> _action;


        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;


        public bool CanExecute(object? parameter)
        {
            return true;
        }


        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}
