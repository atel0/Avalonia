using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia.Diagnostics.Converters
{
    internal class BooleanToBoldConverter : IValueConverter
    {
        public static BooleanToBoldConverter Instance { get; } = new BooleanToBoldConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
            {
                return FontWeight.Bold;
            }

            return FontWeight.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
