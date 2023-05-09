using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils.Extensions;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class TrainingPlanMainViewModel : BaseViewModel
    {


        #region Private Fields

        private TrainingPlan _trainingPlan;
        private IList<BaseViewModel> _childViewModels;
        private BaseViewModel _selectedChildViewModel;
        private ITrainingPlanService _trainingPlanService;
        private ITrainingTagService _taggingService;
        private readonly INavigationService _navigationService;
        #endregion



        public IList<BaseViewModel> ChildViewModels
        {
            get => _childViewModels;
            set
            {
                _childViewModels = value;
                RaisePropertyChanged();
            }
        }

        public BaseViewModel SelectedChildViewModel
        {
            get => _selectedChildViewModel;
            set
            {
                if (_selectedChildViewModel is TrainingPlanWorkoutViewModel vm)
                    DeselectWorkoutPage(vm);

                _selectedChildViewModel = value;
                RaisePropertyChanged();

                if(_selectedChildViewModel is TrainingPlanWorkoutViewModel)
                    SelectWorkoutPage(_selectedChildViewModel, TrainingPlan.TrainingWeeks.SelectMany(x => x.Workouts));
            }
        }

        /// <summary>
        /// The Model
        /// </summary>
        public TrainingPlan TrainingPlan 
        {
            get => _trainingPlan;
            set
            {
                _trainingPlan = value;
                RaisePropertyChanged();
            }
        }



        #region Ctors

        public TrainingPlanMainViewModel(ITrainingPlanService trainingPlanService, ITrainingTagService taggingService, INavigationService navigationService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _taggingService = taggingService ?? throw new ArgumentNullException(nameof(taggingService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion


        private void AddViewModel(BaseViewModel vm, object dataContext)
        {
            ChildViewModels.Add(vm);
            InitChildPage(vm, dataContext);
        }

        /// <summary>
        /// First time initialization for the tab pages.
        /// </summary>
        /// <param name="vm">The VM the page is bound to</param>
        /// <param name="dataContext">The initialization data which has already ben fetched</param>
        private void InitChildPage(BaseViewModel vm, object dataContext)
        {
            if (vm is TrainingPlanWorkoutViewModel)
            {
                WorkoutPlan plan = null;

                // If input is the workout name, get the workout plan from it, otherwise dataContext is already the plan
                if (dataContext is string)
                    plan = GetWorkoutPlanFromCachedData(dataContext as string);

                vm.InitializeAsync(plan ?? dataContext as WorkoutPlan).Wait();
            }
            else
                vm.InitializeAsync(dataContext).Wait();
        }

        /// <summary>
        /// Get the plan of the workout having the name specified.
        /// </summary>
        /// <param name="workoutName">The workout name</param>
        /// <returns>The Workout Plan</returns>
        private WorkoutPlan GetWorkoutPlanFromCachedData(string workoutName)
        {
            WorkoutPlan plan = new WorkoutPlan(workoutName);

            foreach (TrainingWeek week in TrainingPlan.TrainingWeeks.OrderBy(x => x.ProgressiveNumber))
            {
                IEnumerable<WorkoutTemplate> weeklyWorkouts = week.Workouts.Where(x => x.Name == workoutName); // Support for WOs scheduled more then once than week

                if (weeklyWorkouts == null)
                    plan.WorkoutNotPlanned(week.Id.Value, week.ProgressiveNumber.Value);
                else
                {
                    foreach (WorkoutTemplate wo in weeklyWorkouts)
                        plan.ScheduleWorkout(wo.Id.Value, week.Id.Value, week.ProgressiveNumber.Value);
                }
            }
            return plan;
        }

        /// <summary>
        /// Get the plan of all the workouts.
        /// </summary>
        /// <returns>The Workout Plan for all the workouts of the training plan</returns>
        private IEnumerable<WorkoutPlan> GetWorkoutPlanFromCachedData()
        {
            // Map the Weeks/Workouts to the actual Workout list
            List<WorkoutPlan> workoutsPlans = new List<WorkoutPlan>();
            IEnumerable<string> workoutsNames = TrainingPlan.TrainingWeeks
                .SelectMany(x => x.Workouts)
                .GroupBy(x => x.Name)
                .Select(x => x.Key);

            // Get the plan for each workout, including missing weeks
            foreach (string workoutName in workoutsNames)
                workoutsPlans.Add(GetWorkoutPlanFromCachedData(workoutName));

            return workoutsPlans;
        }

        /// <summary>
        /// Perform the final cleanup before the page is unloaded
        /// </summary>
        /// <param name="vm"></param>
        private void DeselectWorkoutPage(TrainingPlanWorkoutViewModel vm)
        {
            vm.Dispose();
        }

        /// <summary>
        /// Initializes the selected Workout tab page after it has been selected.
        /// This is different from the first-time initialization which just loads the Workout name and the IDs
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="vm"></param>
        private void SelectWorkoutPage(BaseViewModel vm, object dataContext)
        {
            vm.InitializeAsync(dataContext);
        }

        /// <summary>
        /// Refresh the Workout list removing the specified ones
        /// </summary>
        /// <param name="idsToRemove">The IDs  to remove</param>
        private void OnWorkoutRemoved(IEnumerable<uint> idsToRemove)
        {
            string workoutName = TrainingPlan.TrainingWeeks.SelectMany(x => x.Workouts).First(x => idsToRemove.Contains(x.Id.Value)).Name;

            foreach (TrainingWeek week in TrainingPlan.TrainingWeeks)
                week.Workouts.RemoveAll(x => idsToRemove.Contains(x.Id.Value));

            // Remove the tab and switch to the first one
            ChildViewModels.Remove(ChildViewModels.Single(x => (x is TrainingPlanWorkoutViewModel vm) && vm.WorkoutName == workoutName));
            SelectedChildViewModel = ChildViewModels[0];
        }

        /// <summary>
        /// Refresh the Workout list adding the specified one
        /// </summary>
        /// <param name="workoutName">The workout name</param>
        private void OnWorkoutAdded(string workoutName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializing function.
        /// <para>The ViewModel is supposed to work with data coming from different sources, so the same Page can be used in different contexts, provided that at least the ID is received.</para>
        /// <para>The source navigation data supported so far is: </para>
        ///     <para>- <see cref="TrainingPlanSummary"/> </para>
        /// </summary>
        /// <param name="navigationData">A TrainingPlanSummary instance, loaded with all the available pre-fetched data (ID is mandatory)</param>
        /// <returns>The initialization Task</returns>
        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is TrainingPlanSummary srcSummary)
            {
                TrainingPlan = new TrainingPlan
                {
                    Id = srcSummary.PlanId,

                    Details = new TrainingPlanDetails
                    {
                        Name = srcSummary.PlanName,
                        OwnerId = srcSummary.OwnerId,
                        UserPlanId = srcSummary.PlanUserLibraryId,
                        IsBookmarked = srcSummary.IsBookmarked,
                        TrainingWeeksCounter = srcSummary.WeeksNumber.HasValue ? srcSummary.WeeksNumber.Value : 0,
                        Hashtags = new ObservableCollection<ISimpleTagElement>(srcSummary.Hashtags),
                        TargetPhases = new ObservableCollection<ISimpleTagElement>(srcSummary.TargetPhases),
                        TargetProficiencies = new ObservableCollection<ISimpleTagElement>(srcSummary.TargetProficiencies),
                    },
                };
            }
            // Add other source types here, one per different source page ...



            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.WorkoutDeleted);
            MessagingCenter.Subscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.WorkoutDeleted, (sender, args) => OnWorkoutRemoved(args as IEnumerable<uint>));
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.WorkoutAdded);
            MessagingCenter.Subscribe<BaseViewModel, string>(this, MessageKeys.WorkoutAdded, (sender, args) => OnWorkoutAdded(args as string));

            // Init the View Models
            // WARNING: if not read-only Views then it might not be correct to fetch data only once...
            ChildViewModels = new SmartObservableCollection<BaseViewModel>();
            AddViewModel(new TrainingPlanDetailViewModel(_trainingPlanService, _taggingService, _navigationService), TrainingPlan);


            // Load the workouts planning over the weeks
            TrainingPlan.TrainingWeeks = (await _trainingPlanService.GetWorkoutsPlanAsync(TrainingPlan.Id.Value)).ToList();

            foreach (WorkoutPlan workout in GetWorkoutPlanFromCachedData())
                AddViewModel(new TrainingPlanWorkoutViewModel(_trainingPlanService, _navigationService), workout);

            (_trainingPlanService as TrainingPlanMockService).RefreshPlan(TrainingPlan); //RIGM: to be removed!

            await base.InitializeAsync(navigationData);
        }


        public override void Dispose()
        {
            Debug.WriteLine("Disposing" + GetType().Name);

            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.WorkoutDeleted);
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.WorkoutAdded);
            base.Dispose();
        }
    }
}
