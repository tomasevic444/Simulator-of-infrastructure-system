using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkService.Models
{
    public class BarHeightToCanvasTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return 350 - height; // Adjust based on your scale and starting point
            }
            return 350; // Default value
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}