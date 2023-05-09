using TestApp.ViewModels;
using Xamarin.Forms;

namespace TestApp.Views.DataTemplateSelectors
{
    public class TrainingPlanChildTabSelector : DataTemplateSelector
    {

        public DataTemplate DetailTemplate { get; set; }
        public DataTemplate WorkoutTemplate { get; set; }
        //public DataTemplate ScheduleTemplate { get; set; }
        //public DataTemplate OverviewTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item is TrainingPlanDetailViewModel)
                    return DetailTemplate;

            if (item is TrainingPlanWorkoutViewModel)
                return WorkoutTemplate;

            // TODO other types
            return null;
        }
    }
}
