using System;
using System.Globalization;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class IntToPreviousIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;

            if (int.TryParse(value.ToString(), out number))
                return number - 1;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
