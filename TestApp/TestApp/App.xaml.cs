using System;
using System.Threading.Tasks;
using TestApp.Services.Navigation;
using TestApp.ViewModels.Base;
using TestApp.Views;
using Xamarin.Forms;

namespace TestApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            InitApp();

            MainPage = new TrainingPlansSummariesView();
            InitNavigation();
        }

        private void InitApp()
        {
            //_settingsService = ViewModelLocator.Resolve<ISettingsService>();
            //if (!_settingsService.UseMocks)
            //    ViewModelLocator.UpdateDependencies(_settingsService.UseMocks); 

            Services.AppSession.AppSession.CurrentUserId = 1;   // RIGM todo!



            // Use Mock
            ViewModelLocator.UpdateDependencies(true);

            // Unhandled exceptions management - This does not fire when Exception is thrwon inside Tasks - Actually this seems never working...
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                Exception ex = (Exception)args.ExceptionObject;
                Console.WriteLine(ex);
                System.Diagnostics.Debugger.Break();
            };
            // Unhandled exceptions management - This is always fired, even if the Exception is handled by the raising code block
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) => {
                Console.WriteLine(args.Exception);

                if(!(args.Exception is TaskCanceledException))
                    System.Diagnostics.Debugger.Break();
            };
        }

        private async Task InitNavigation()
        {
            try
            {
                INavigationService navigationService = ViewModelLocator.Resolve<INavigationService>();
                await navigationService.InitializeAsync();
            }
            catch
            {
                System.Diagnostics.Debugger.Break();
                throw;
            }
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
