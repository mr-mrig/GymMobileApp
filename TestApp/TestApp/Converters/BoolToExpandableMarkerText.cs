using System;
using System.Globalization;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class BoolToExpandableMarkerText : IValueConverter
    {

        const string DefaultExpandMarkerText = "+";
        const string DefaultCollapseMarkerText = "-";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
                return isExpanded ? DefaultCollapseMarkerText : DefaultExpandMarkerText;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
