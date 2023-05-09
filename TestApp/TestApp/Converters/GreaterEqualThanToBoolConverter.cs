using System;
using System.Globalization;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class GreaterEqualThanToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float fValue)
                return fValue >= float.Parse(parameter.ToString());

            if (value is int ivalue)
                return ivalue >= int.Parse(parameter.ToString());

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
