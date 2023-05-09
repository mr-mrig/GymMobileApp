using TestApp.Views.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkUnitDetailView : ContentView
    {
        public WorkUnitDetailView()
        {
            InitializeComponent();
        }
    }
}