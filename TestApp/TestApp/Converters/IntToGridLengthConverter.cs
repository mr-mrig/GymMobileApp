using System;
using System.Globalization;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class IntToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int parameterValue)
                return new GridLength(parameterValue);
            else
                return GridLength.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
