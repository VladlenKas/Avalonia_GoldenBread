using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Converters
{
    public class TrendColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is decimal trend)
            {
                if (trend > 0)
                    return new SolidColorBrush(Color.FromRgb(34, 197, 94)); // Зеленый
                else if (trend < 0)
                    return new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Красный
                else
                    return new SolidColorBrush(Color.FromRgb(156, 163, 175)); // Серый
            }

            return new SolidColorBrush(Color.FromRgb(156, 163, 175));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
