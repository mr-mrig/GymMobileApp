using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Views.Popups.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertPopupView : Rg.Plugins.Popup.Pages.PopupPage
    {


        public const int AlertPopoutDelay = 3000;


        private bool _isClosed;


        public AlertPopupView()
        {
            InitializeComponent();
            _isClosed = false;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            ShowAndWaitAsync();
        }
        private async void ShowAndWaitAsync()
        {
            await PopupPanel.ScaleTo(1.4, 200, easing: Easing.Linear);
            await PopupPanel.ScaleTo(1, 100, easing: Easing.Linear);
            await Task.Delay(AlertPopoutDelay);
            if(!_isClosed)
                await PopupNavigation.Instance.RemovePageAsync(this);
        }


        private async void ClosePopupAsync(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.RemovePageAsync(this);
            _isClosed = true;
        }
    }
}