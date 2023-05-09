using System.Diagnostics;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.Views.Base
{
    public class BaseView : ContentPage
    {


        /// <summary>
        /// The bound View Model is disposed when the Back Button is pressed, unless the VM explicitly says not to do it.
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            BaseViewModel viewModel = BindingContext as BaseViewModel;

            if (viewModel.ShouldDisposeOnBack)
            {
                viewModel.Dispose();
                Debug.WriteLine($"{viewModel.GetType().Name} disposed!");
            }

            return base.OnBackButtonPressed();
        }

    }
}