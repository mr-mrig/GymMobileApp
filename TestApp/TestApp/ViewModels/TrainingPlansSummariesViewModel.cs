using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;
using TestApp.Services.AppSession;
using TestApp.Services.Mocking;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using static TestApp.Services.Mocking.MockingService;

namespace TestApp.ViewModels
{
    public class TrainingPlansSummariesViewModel : BaseViewModel
    {


        private readonly ITrainingPlanService _trainingPlanService;
        private readonly INavigationService _navigationService;

        private ICommand _openPlanCommand;

        public string CurrentUser
        {
            get => "Current User:" + AppSession.CurrentUserId.ToString();
        }



        private ObservableCollection<TrainingPlanSummary> _trainingPlansSummaries;

        public ObservableCollection<TrainingPlanSummary> TrainingPlanSummaries
        {
            get => _trainingPlansSummaries;
            set
            {
                _trainingPlansSummaries = value;
                RaisePropertyChanged();
            }
        }

        public ICommand OpenPlanCommand => _openPlanCommand ?? (_openPlanCommand = new Command(async (p) => 
        { 
            await OpenPlanAsync(p as TrainingPlanSummary); 
        }));


        //private IDebouncerService _debouncer = new DebouncerService();

        //public ICommand TrainingWeekChangedCommand => new Command(async x =>
        //{
        //    await _debouncer.DebounceAsync(1000, async e => await MyFun(int.Parse(x.ToString())));
        //});

        //public async Task MyFun(int x)
        //{
        //    System.Diagnostics.Debug.WriteLine(x);
        //    await Task.Yield();
        //}

        #region Ctors

        public TrainingPlansSummariesViewModel(ITrainingPlanService trainingPlanService, INavigationService navigationService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        #endregion




        public override async Task InitializeAsync(object navigationData)
        {
            //if(navigationData is ObservableCollection<ExpandableCardViewModel> trainingPlansSummaries)
            //{
            //    TrainingPlanSummaryViewModel = trainingPlansSummaries;
            //}


            if (TrainingPlanSummaries == null)
                TrainingPlanSummaries = new ObservableCollection<TrainingPlanSummary>();

            //var authToken = _settingsService.AuthAccessToken;
            //var userInfo = await _userService.GetUserInfoAsync(authToken);

            //IEnumerable<TrainingPlanSummary> plans = await _trainingPlanService.GetTrainingPlansSummariesAsync(userInfo.UserId, authToken);

            // Get Training Plans
            IEnumerable<TrainingPlanSummary> plans = await _trainingPlanService.GetTrainingPlansSummariesAsync(1);

            if (plans != null)
                AddTrainingPlans(plans);


            // Subscribes/Unsubscribes ....


            await base.InitializeAsync(navigationData);
        }


        #region Private Methods

        /// <summary>
        /// Add the specified training plans to the VM
        /// </summary>
        /// <param name="plans">The Models to be displayed</param>
        private void AddTrainingPlans(IEnumerable<TrainingPlanSummary> plans)
        {
            foreach (TrainingPlanSummary plan in plans)
                AddTrainingPlan(plan);
        }

        /// <summary>
        /// Add the specified training plan to the VM
        /// </summary>
        /// <param name="plans">The Model to be displayed</param>
        private void AddTrainingPlan(TrainingPlanSummary plan)
        {
            TrainingPlanSummaries.Add(plan);
        }

        /// <summary>
        /// Open the Training Plan page from the selected summary
        /// </summary>
        /// <param name="planSummary">The summary of the selected training plan</param>
        /// <returns>The navigation Task</returns>
        private async Task OpenPlanAsync(TrainingPlanSummary planSummary)
        {
            if(planSummary != null)
            {
                //TrainingPlan plan = new TrainingPlan
                //{
                //    Details = new TrainingPlanDetails
                //    {
                //        Name = "Test Plan Name",
                //    }
                //};

                // Open the page
                await _navigationService.NavigateToAsync<TrainingPlanMainViewModel>(planSummary);
            }
        }
        #endregion
    }
}
