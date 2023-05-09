using System;
using System.Windows.Input;
using TestApp.Services.Navigation;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels.Popups.Common
{
    public class BasePopupViewModel : BaseViewModel
    {

        public const string PopupDefaultCancelActionText = "Cancel";
        public const string PopupDefaultOkActionText = "Ok";

        public static Color DefaultColor = (Color)App.Current.Resources["PopupColor"];      // This is not readonly!


        protected INavigationService _navigationService;


        #region Backing Fields
        private Color _popupColor = DefaultColor;
        private string _cancelActionText = PopupDefaultCancelActionText;
        private string _mainActionText = PopupDefaultOkActionText;
        private string _titleText;
        private string _messageText;
        private ICommand _mainCommand;
        private ICommand _cancelCommand;
        #endregion


        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                RaisePropertyChanged();
            }
        }
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                RaisePropertyChanged();
            }
        }

        public Color PopupColor
        {
            get => _popupColor;
            set
            {
                _popupColor = value;
                RaisePropertyChanged();
            }
        }

        public string MainActionText
        {
            get => _mainActionText;
            set
            {
                _mainActionText = value;
                RaisePropertyChanged();
            }
        }
        public string CancelActionText
        {
            get => _cancelActionText;
            set
            {
                _cancelActionText = value;
                RaisePropertyChanged();
            }
        }
        public ICommand CancelCommand
        {
            get => _cancelCommand;
            set
            {
                _cancelCommand = value;
                RaisePropertyChanged();
            }
        }
        public ICommand MainCommand
        {
            get => _mainCommand;
            set
            {
                _mainCommand = value;
                RaisePropertyChanged();
            }
        }


        #region Commands

        public virtual ICommand OnCancelCommand => new Command(async x =>
             {
                 if (CancelCommand != null)
                     if (CancelCommand.CanExecute(x))
                         CancelCommand.Execute(x);

                 await _navigationService.ClosePopup();
             });

        public virtual ICommand OnConfirmCommand => new Command(async () =>
        {
            if (MainCommand == null || !MainCommand.CanExecute(default))
                return;

            if (MainCommand is ICommandAsync asyncCommand)
                await asyncCommand.ExecuteAsync();
            else
                MainCommand.Execute(default);

            await _navigationService.ClosePopup();
        });
        #endregion



        public BasePopupViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

    }
}
