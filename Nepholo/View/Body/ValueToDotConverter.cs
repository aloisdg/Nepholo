using System;
using System.Globalization;
using System.Windows.Data;

namespace Nepholo.View.Body
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class ValueToDotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? String.Empty : ".";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
