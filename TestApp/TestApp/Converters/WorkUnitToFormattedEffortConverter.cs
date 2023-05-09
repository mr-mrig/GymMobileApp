using System;
using System.Globalization;
using TestApp.Models.TrainingDomain;
using TestApp.Services.DomainPresenters;
using Xamarin.Forms;

namespace TestApp.Converters
{
    public class WorkUnitToFormattedEffortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WorkUnitTemplate wu)
            {
                ITrainingPresenter presenter = new WorkUnitPresenter()
                {
                    WorkUnit = wu,
                };
                return presenter.ToFormattedEffort();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
