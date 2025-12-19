using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Converters
{
    public class TrendColorConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values[0] is decimal trend)
            {
                if (trend > 0)
                    return Avalonia.Media.Color.FromRgb(34, 197, 94); // Зеленый
                else if (trend < 0)
                    return Avalonia.Media.Color.FromRgb(239, 68, 68); // Красный
                else
                    return Avalonia.Media.Color.FromRgb(156, 163, 175); // Серый
            }

            return Avalonia.Media.Color.FromRgb(156, 163, 175);
        }
    }
}
