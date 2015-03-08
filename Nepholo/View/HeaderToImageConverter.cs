using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Nepholo.View
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null && !s.Contains(@"/"))
            {
                var uri = new Uri("http://3.bp.blogspot.com/-sY7whMXT83o/UECGK-oSdeI/AAAAAAAAABM/-OXkkmppfvE/s1600/text-file-icon.png");
                var source = new BitmapImage(uri);
                return source;
            }
            else
            {
                var uri = new Uri("http://icons.iconarchive.com/icons/media-design/hydropro-v2/512/Folder-icon.png");
                var source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
