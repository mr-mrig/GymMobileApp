using System.Windows.Input;
using TestApp.Services.Navigation;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels.Popups.Common
{

    /// <summary>
    /// A simple Popup which allows text editions through an Editor.
    /// The prompted text is then passed to the <see cref="BasePopupViewModel.MainCommand"/> as a string argument
    /// </summary>
    public class BaseEditingPopupViewModel : BasePopupViewModel
    {


        #region Backing Fields
        protected string _inputText;
        protected string _placeholderText;
        #endregion


        public string InputText 
        { 
            get => _inputText;
            set
            {
                _inputText = value;
                RaisePropertyChanged();
            }
        }
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// The command returns the prompted text, hence it is strongly suggested to bind this to a Command which handles parameters
        /// </summary>
        public override ICommand OnConfirmCommand => new Command(async () =>
        {
            if (MainCommand == null || !MainCommand.CanExecute(default))
                return;

            if (MainCommand is ICommandAsync<string> asyncCommand)
                await asyncCommand.ExecuteAsync(InputText);
            else if (MainCommand is ICommandAsync asyncParameterlessCommand)
                await asyncParameterlessCommand.ExecuteAsync();         // Why this?
            else
                MainCommand.Execute(InputText);

            await _navigationService.ClosePopup();
        });


        /// <summary>
        /// A simple Popup which allows text editions through an Editor.
        /// The prompted text is then passed to the <see cref="BasePopupViewModel.MainCommand"/> as a string argument
        /// </summary>
        /// <param name="navigationService">The navigation service suitable to manage popups</param>
        public BaseEditingPopupViewModel(INavigationService navigationService) : base(navigationService) { }

    }
}
