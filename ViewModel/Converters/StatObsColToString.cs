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
    public class StatObsColToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Stat> stats)
            {
                return string.Join(Environment.NewLine, stats.Select(s => $"{s.stat.name}: {s.base_stat}"));
            };

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
