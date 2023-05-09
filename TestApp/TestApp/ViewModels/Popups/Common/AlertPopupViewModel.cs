using System;
using System.Collections.Generic;
using System.Text;
using TestApp.ViewModels.Base;

namespace TestApp.ViewModels.Popups.Common
{
    public enum AlertPopupType
    {
        Info,
        Warning,
        Error,
        Success,
    }

    public class AlertPopupViewModel : BaseViewModel
    {

        #region Backing Fields
        protected AlertPopupType _alertType;
        protected string _alertMessage;
        #endregion


        public AlertPopupType AlertType
        {
            get => _alertType;
            set
            {
                _alertType = value;
                RaisePropertyChanged();
            }
        }

        public string AlertMessage
        {
            get => _alertMessage;
            set
            {
                _alertMessage = value;
                RaisePropertyChanged();
            }
        }



        public AlertPopupViewModel()
        {
            _alertType = AlertPopupType.Info;
        }



        void ClosePopup()
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
