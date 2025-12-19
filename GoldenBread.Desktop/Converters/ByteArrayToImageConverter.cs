using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        private static readonly Lazy<Bitmap> Placeholder = new(() =>
        {
            var uri = new Uri("avares://GoldenBread.Desktop/Assets/product-placeholder.png");
            return new Bitmap(AssetLoader.Open(uri));
        });

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not byte[] bytes || bytes.Length == 0)
                return Placeholder.Value;

            try
            {
                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }
            catch
            {
                return Placeholder.Value;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
