using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Nepholo.View
{
    [ValueConversion(typeof(string), typeof(string))]
    public class EmailToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            return !String.IsNullOrWhiteSpace(s)
                ? Gravimage.Gravimage.Get(s)
                : "http://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&f=y";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
