using System.Windows.Input;
using TestApp.Services.Navigation;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels.Popups.Common
{
    public class DestructiveActionPopupViewModel : BasePopupViewModel
    {

        public const string RememberMeOptionText = "Remember my answer";


        //private INavigationService _navigationService;


        #region Backing Fields
        private bool _enableRememberMe = false;
        private bool _rememberMe = false;
        #endregion


        public bool EnableRememberMe
        {
            get => _enableRememberMe;
            set
            {
                _enableRememberMe = value;
                RaisePropertyChanged();
            }
        }
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                RaisePropertyChanged();
            }
        }


        #region Commands

        public override ICommand OnConfirmCommand => new Command(async () =>
        {
            if (MainCommand == null || !MainCommand.CanExecute(default))
                return;

            if (MainCommand is ICommandAsync asyncCommand)
                await asyncCommand.ExecuteAsync();
            else
                MainCommand.Execute(default);

            if (RememberMe)
                ;   //RIGM: TODO

            await _navigationService.ClosePopup();
        });
        #endregion



        public DestructiveActionPopupViewModel(INavigationService navigationService) :base(navigationService)
        {

        }

    }
}
