using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using TestApp.ViewModels;
using TestApp.ViewModels.Base;
using TestApp.Views.Popups.Common;
using Xamarin.Forms;

namespace TestApp.Services.Navigation
{
    public class NavigationService : INavigationService, IDisposable
    {

        private readonly Page _activityIndicatorPopup = new LoadingPopup();


        //private readonly ISettingsService _settingsService;

        public BaseViewModel PreviousViewModel
        {
            get
            {
                NavigationPage mainPage = Application.Current.MainPage as NavigationPage;
                return mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext as BaseViewModel;
            }
        }




        #region ctors

        public NavigationService()
        {
            MessagingCenter.Unsubscribe<object>(this, MessageKeys.OpenActivityIndicatorPopup);
            MessagingCenter.Subscribe<object>(this, MessageKeys.OpenActivityIndicatorPopup, async (sender) 
                => await PopupNavigation.Instance.PushAsync(_activityIndicatorPopup as PopupPage));

            MessagingCenter.Unsubscribe<object>(this, MessageKeys.CloseActivityIndicatorPopup);
            MessagingCenter.Subscribe<object>(this, MessageKeys.CloseActivityIndicatorPopup, async (sender)
                => await PopupNavigation.Instance.PopAllAsync());      //RIGM: popping all the popups! This might be unacceptable in some cases, however we have found no other way without coupling the specific popup with the activity indicator...
        }

        //public NavigationService(ISettingsService settingsService) : this()
        //{
        //    _settingsService = settingsService;
        //}

        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            MessagingCenter.Unsubscribe<object>(this, MessageKeys.OpenActivityIndicatorPopup);
            MessagingCenter.Unsubscribe<object>(this, MessageKeys.CloseActivityIndicatorPopup);
        }
        #endregion

        #region INavigationService Implementation

        public Task InitializeAsync()
        {
            //if (string.IsNullOrEmpty(_settingsService.AuthAccessToken))
            //    return NavigateToAsync<LoginViewModel>();
            //else
            //    return NavigateToAsync<MainViewModel>();

            return NavigateToAsync<TrainingPlansSummariesViewModel>();
        }

        public async Task OpenPopup<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            try
            {
                PopupPage page = CreatePopupPage(typeof(TViewModel), parameter);
                page.BindingContext = parameter;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public Task NavigateToAsync<TViewModel>(bool dispose = false) where TViewModel : BaseViewModel
        {
            return InternalNavigateToAsync(typeof(TViewModel), null, dispose);
        }

        public Task NavigateToAsync<TViewModel>(object parameter, bool dispose = false) where TViewModel : BaseViewModel
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter, dispose);
        }


        public Task RemoveLastFromBackStackAsync()
        {
            Page mainPage = Application.Current.MainPage as NavigationPage;

            if (mainPage != null)
            {
                Page toRemove = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2];

                (toRemove.BindingContext as BaseViewModel).Dispose();
                mainPage.Navigation.RemovePage(toRemove);
            }

            return Task.FromResult(true);
        }


        public Task RemoveBackStackAsync()
        {
            var mainPage = Application.Current.MainPage as NavigationPage;

            if (mainPage != null)
            {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        public async Task GoBackAsync(bool dispose = true)
        {
            Page mainPage = Application.Current.MainPage as NavigationPage;
            Page poppedPage;

            if (mainPage != null)
            {
                poppedPage = await mainPage.Navigation.PopAsync();

                if (dispose && poppedPage != null)
                    (poppedPage.BindingContext as BaseViewModel).Dispose();
            }
        }

        public async Task ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }
        #endregion


        #region Private Methods

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter, bool dispose)
        {
            try
            {
                Page page = CreatePage(viewModelType, parameter);

                //if (page is LoginView)
                //{
                //    Application.Current.MainPage = new NavigationPage(page);
                //}
                //else
                //{
                //    var navigationPage = Application.Current.MainPage as NavigationPage;
                //    if (navigationPage != null)
                //    {
                //        await navigationPage.PushAsync(page);
                //    }
                //    else
                //    {
                //        Application.Current.MainPage = new NavigationPage(page);
                //    }
                //}

                //await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);

                if (Application.Current.MainPage is NavigationPage navigationPage)
                    await navigationPage.PushAsync(page);
                else
                    Application.Current.MainPage = new NavigationPage(page);

                if (dispose)
                    await RemoveLastFromBackStackAsync();

                await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            string viewName = viewModelType.FullName.Replace("Model", string.Empty);
            string viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;

            return Type.GetType(string.Format(
                CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName));
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
                throw new InvalidOperationException($"Cannot locate page type for {viewModelType}");

            return Activator.CreateInstance(pageType) as Page;
        }

        private PopupPage CreatePopupPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
                throw new InvalidOperationException($"Cannot locate page type for {viewModelType}");

            return Activator.CreateInstance(pageType) as PopupPage;
        }

        public Task RemoveBackStackAsync(bool dispose = true)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
