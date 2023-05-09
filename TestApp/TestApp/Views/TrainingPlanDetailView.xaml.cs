using TestApp.Views.Base;
using Xamarin.Forms.Xaml;

namespace TestApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingPlanDetailView : BaseView
    {
        public TrainingPlanDetailView()
        {
            InitializeComponent();
        }
    }
}