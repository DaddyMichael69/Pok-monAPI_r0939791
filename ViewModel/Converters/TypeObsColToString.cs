using Pokémon.Model;
using Pokémon.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pokémon.ViewModel.Converters
{
    public class TypeObsColToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<PokeType> pokeType)
            {
                return string.Join(Environment.NewLine, pokeType.Select(s => $"{s.type.name}"));
            };

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
