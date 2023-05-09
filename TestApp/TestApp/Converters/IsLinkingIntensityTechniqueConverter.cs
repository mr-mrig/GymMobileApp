using System;
using System.Globalization;
using TestApp.Models.TrainingDomain;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class IsLinkingIntensityTechniqueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IntensityTechnique it)
                return it != null && it.IsLinking;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
