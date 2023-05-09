using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TestApp.Models.Common;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class TagListToConcatenatedStringConverter : IValueConverter
    {
        public const string DefualtTagSeparator = ",";


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<ISimpleTagElement> list)
            {
                string separator = parameter as string ?? DefualtTagSeparator;
                return string.Join(separator, list.Select(x => x.Body));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
