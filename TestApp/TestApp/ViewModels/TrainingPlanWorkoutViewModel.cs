using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils.Extensions;
using TestApp.ViewModels.Base;
using TestApp.ViewModels.Base.Commands;
using TestApp.ViewModels.Popups.Common;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class TrainingPlanWorkoutViewModel : BaseViewModel
    {


        private ITrainingPlanService _trainingPlanService;
        private INavigationService _navigationService;
        private SmartObservableCollection<GroupedWorkoutViewModels> _weeklyWorkouts;


        #region Backing Fields
        private bool _isEditing = false;
        private bool _fullLoad;
        private string _workoutName;
        private ICommand _stopEditingCommand;
        private ICommand _startEditingCommand;
        private ICommand _openWorkUnitCommand;
        private ICommand _addWorkUnitCommand;
        private ICommand _deleteWorkoutPlanCommand;
        #endregion


        /// <summary>
        /// Whether the user is editing the Workout
        /// </summary>        
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///  The name of the training day
        /// </summary>
        public string WorkoutName
        {
            get => _workoutName;
            set
            {
                _workoutName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The ViewModels of the workouts grouped by Week
        /// </summary> 
        public SmartObservableCollection<GroupedWorkoutViewModels> WeeklyWorkouts 
        { 
            get => _weeklyWorkouts; 
            set
            {
                _weeklyWorkouts = value;
                RaisePropertyChanged();
            }
        }

        #region Commands

        //public ICommand OnWorkUnitMoreActionCommand => new Command(x => SwitchToEditMode((x as WorkUnitTemplate).Id.Value)); 

        //public ICommand CancelCommand => new Command(x => ClearSelection()); 

        //public ICommand DeleteWorkUnitCommand => new Command(async x => await DeleteWorkUnits());

        public ICommand StopEditingCommand => _stopEditingCommand ?? (_stopEditingCommand = new Command(() => IsEditing = false));
        public ICommand StartEditingCommand => _startEditingCommand ?? (_startEditingCommand = new Command(() => IsEditing = true));
        public ICommand OpenWorkUnitCommand => _openWorkUnitCommand ?? (_openWorkUnitCommand = new Command(async x => await OpenWorkUnitDetail(x as WorkUnitTemplate)));
        public ICommand DeleteWorkoutPlanCommand => _deleteWorkoutPlanCommand ?? (_deleteWorkoutPlanCommand = new Command(async x => await DeleteWorkoutPlan()));
        public ICommand AddWorkUnitCommand => _addWorkUnitCommand ?? (_addWorkUnitCommand = new Command(async x => await AddWorkUnit(x as WorkUnitTemplate)));

        #endregion


        #region Ctors

        public TrainingPlanWorkoutViewModel(ITrainingPlanService trainingPlanService, INavigationService navigationService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion


        #region Commands Methods

        private async Task AddWorkUnit(WorkUnitTemplate beforeThisOne)
        {
            WorkUnitTemplate draftWorkUnit = new WorkUnitTemplate { ProgressiveNumber = beforeThisOne.ProgressiveNumber };

            IList<WorkUnitDetailViewModel> workUnitsPlan = WeeklyWorkouts.SelectMany(x => x.Select(wo => new WorkUnitDetailViewModel(_navigationService)
            {
                WeekId = x.WeekId,
                IsEditing = IsEditing,
                WeekProgressiveNumber = x.WeekProgressiveNumber,
                WorkoutId = wo.Id.Value,
                WorkUnit = draftWorkUnit,

            })).ToList();

            await _navigationService.NavigateToAsync<WorkUnitMainViewModel>(
                new WorkUnitMainViewModel(_trainingPlanService, _navigationService)
                {
                    WorkoutName = WorkoutName,
                    PlannedWorkUnits = workUnitsPlan,
                    SelectedWorkUnit = workUnitsPlan[0],
                    Excercise = null,
                    Note = null,
                    NoteId = null,
                    IsEditing = true,
                    IsNewWorkUnit = true,
                });
        }

        private async Task OpenWorkUnitDetail(WorkUnitTemplate workUnit)
        {
            Debug.WriteLine("OPENING " + workUnit.ToString());

            IEnumerable<WorkUnitDetailViewModel> workUnitsPlan = WeeklyWorkouts.SelectMany(x => x.Select(wo => new WorkUnitDetailViewModel(_navigationService)
            {
                WeekId = x.WeekId,
                IsEditing = IsEditing,
                WeekProgressiveNumber = x.WeekProgressiveNumber,
                WorkoutId = wo.Id.Value,
                WorkUnit = new WorkUnitTemplate(
                    wo.WorkUnits.SingleOrDefault(wu => wu.ProgressiveNumber == workUnit.ProgressiveNumber))   //RIGM: this implies that WUs cannot be deleted in a single WO
            }));
            
            // This Command cannot be moved to the child VM as we need the whole plan to be passed as a navigation parameter
            await _navigationService.NavigateToAsync<WorkUnitMainViewModel>(
                new WorkUnitMainViewModel(_trainingPlanService, _navigationService)
                {
                    WorkoutName = WorkoutName,
                    PlannedWorkUnits = workUnitsPlan.ToList(),
                    SelectedWorkUnit = workUnitsPlan.SingleOrDefault(x => x.WorkUnit == workUnit),
                    IsEditing = IsEditing,
                    Excercise = workUnit.Excercise,
                    Note = workUnit.Note,       //RIGM: should one note per WU be allowed? If so, then this and the following line should be changed
                    NoteId = workUnit.NoteId,
                });
        }


        private async Task DeleteWorkoutPlan()
        {
            var popupViewModel = new DestructiveActionPopupViewModel(_navigationService)
            {
                TitleText = "WARNING",
                MessageText = "Do you really want to delete the whole workout?",
                CancelActionText = "No, go back",
                MainActionText = "Yes, delete it!",
                MainCommand = new Command(() =>
                {
                    MessagingCenter.Send(this as BaseViewModel, MessageKeys.WorkoutDeleted,
                        WeeklyWorkouts.SelectMany(x => x.Workouts).Select(x => x.Id.Value));
                }),
            };
            await _navigationService.OpenPopup<DestructiveActionPopupViewModel>(popupViewModel);
        }

        //private void ClearSelection()
        //{
        //    IsEditing = false;
        //    // Clear checked workunits
        //    foreach (WorkUnitViewModel wu in FindSelectedWorkUnits())
        //        wu.IsSelected = false;
        //}
        #endregion




        /// <summary>
        /// Refresh the ViewModel when Training Weeks are removed
        /// </summary>
        /// <param name="removedWeekIds">The removed Training Weeks IDs</param>
        private void RefreshRemovedTrainingWeeks(IEnumerable<uint> removedWeekIds)
        {
            WeeklyWorkouts.RemoveAll(x => removedWeekIds.Contains(x.WeekId));
        }

        /// <summary>
        /// Refresh the ViewModel when Training Weeks are added
        /// </summary>
        /// <param name="newWeekIds">The added Training Weeks IDs</param>
        /// <returns>The Task object</returns>
        private async Task RefreshNewTrainingWeeks(IEnumerable<uint> newWeekIds)
        {
            IList<WorkoutTemplate> newWorkouts = await _trainingPlanService.GetWorkoutTemplatesByWeekAsync(newWeekIds, WorkoutName);

            foreach(uint weekId in newWeekIds)
            {
                WeeklyWorkouts.Add(new GroupedWorkoutViewModels(
                    weekId,
                    (uint)WeeklyWorkouts.Count,
                    newWorkouts.Where(x => x.WeekId == weekId).Select(wo => new WorkoutViewModel(
                        wo.Id,
                        wo.WorkUnits,
                        _trainingPlanService)
                    ))
                );
            }
        }


        /// <summary>
        /// <para>Delete all the Work Units of the workout having the specified Progressive Number</para>
        /// <para></para>
        /// <para>The current architecture forces that if any WU is deleted, then it must be deleted for allt eh Days of the same WO
        /// If different excercise are reuired, then the user has to plan other workout days</para>
        /// </summary>
        /// <param name="progressiveNumber">The WU progressive number</param>
        private async Task DeleteWorkUnits(WorkoutViewModel sender, uint progressiveNumber)
        {
            IEnumerable<WorkoutViewModel> workoutViewModels = WeeklyWorkouts.SelectMany(x => x.Workouts);

            // Destructive operation: ask user confirm
            var popupViewModel = new DestructiveActionPopupViewModel(_navigationService)
            {
                MessageText = $"This excercise will be removed in all the planned workouts of {WorkoutName}.{Environment.NewLine} Do you really want to continue?",
                CancelActionText = "No, go back",
                MainActionText = "Yes, delete it!",
                EnableRememberMe = true,
                MainCommand = new CommandAsync(async () =>
                {
                    // Let's be optimistic...
                    foreach (var vm in workoutViewModels)
                        vm.DeleteWorkUnit(progressiveNumber);

                    // Perform the Command
                    if (!await _trainingPlanService.DeleteWorkUnitTemplates(workoutViewModels.Select(x => x.Id.Value), progressiveNumber))
                    {
                        // RIGM: notify/rollback
                    }
                }),
            };
            await _navigationService.OpenPopup<DestructiveActionPopupViewModel>(popupViewModel);
        }


        private void RefreshWorkUnit(WorkUnitMainViewModel workUnitViewModel, uint workUnitId)
        {
            WorkUnitTemplate updated = workUnitViewModel.PlannedWorkUnits.Single(x => x.WorkUnit.Id == workUnitId).WorkUnit;
            WorkUnitTemplate toChange = WeeklyWorkouts.SelectMany(x => x.Workouts).SelectMany(x => x.WorkUnits).Single(wu => wu.Id == workUnitId);
            toChange = updated;

            WeeklyWorkouts = new SmartObservableCollection<GroupedWorkoutViewModels>(WeeklyWorkouts);
            RaisePropertyChanged(nameof(WeeklyWorkouts));
        }



        private void SubscribeToMessages()
        {
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksAdded);
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksRemoved);
            MessagingCenter.Unsubscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitDeleted);
            MessagingCenter.Unsubscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitChanged);

            MessagingCenter.Subscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksAdded, async (sender, args) 
                => await RefreshNewTrainingWeeks(args as IEnumerable<uint>));

            MessagingCenter.Subscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksRemoved, (sender, args) 
                => RefreshRemovedTrainingWeeks(args as IEnumerable<uint>));

            MessagingCenter.Subscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitDeleted, async (sender, args) 
                => await DeleteWorkUnits(sender as WorkoutViewModel, uint.Parse(args.ToString())));

            MessagingCenter.Subscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitChanged, (sender, args) 
                => RefreshWorkUnit(sender as WorkUnitMainViewModel, uint.Parse(args.ToString())));

        }



        #region Helpers methods

        /// <summary>
        /// Build the Work Units ViewModels from the specified Workout model
        /// </summary>
        /// <param name="workout">The workout</param>
        /// <returns>The Work Units ViewModels as an ObservableCollection</returns>
        private IList<WorkUnitTemplate> BuildWorkUnitsViewModels(WorkoutTemplate workout)

            => new SmartObservableCollection<WorkUnitTemplate>(workout?.WorkUnits
                ?? new SmartObservableCollection<WorkUnitTemplate>());

        #endregion


        public override async Task InitializeAsync(object navigationData)
        {
            // First-time init: displaying the workout name in the tab title and storing the IDs to be fetched later
            if (WorkoutName == null && navigationData is WorkoutPlan cachedWorkoutsPlan)
            {
                _fullLoad = true;
                WorkoutName = cachedWorkoutsPlan.WorkoutName;
                WeeklyWorkouts = new SmartObservableCollection<GroupedWorkoutViewModels>(
                    cachedWorkoutsPlan.PlannedWorkouts.GroupBy(x => x.TrainingWeekId).Select(x => new GroupedWorkoutViewModels(
                        x.Key,
                        x.FirstOrDefault()?.WeekProgressiveNumber ?? 1,
                        x.Select(wo => new WorkoutViewModel(wo.Id.Value, null, _trainingPlanService)
                    ))));
            }
            else
            {
                // Second-time init: when the page is actually loaded the first time
                if (_fullLoad && navigationData is IEnumerable<WorkoutTemplate> cachedData)
                {
                    IList<WorkoutTemplate> plannedWorkouts = await _trainingPlanService.GetWorkoutTemplatesAsync(WeeklyWorkouts.SelectMany(x => x.Select(wo => wo.Id.Value)));
                    _fullLoad = false;

                    foreach (var workoutVm in WeeklyWorkouts.SelectMany(x => x.Workouts))
                        workoutVm.WorkUnits =
                            BuildWorkUnitsViewModels(plannedWorkouts.SingleOrDefault(x => x.Id == workoutVm.Id));
                }
                // Subscribe only when the tab is selected
                SubscribeToMessages();
            }
            IsEditing = false;

            //RIGM: if performance issues the second load might be joint to the first one, 
            //so the data is being loaded while the user is still seeing the first page
            //Leaving separate loads is more efficient as the actual data is fetched only if really needed

            await base.InitializeAsync(navigationData);
        }

        public override void Dispose()
        {
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksAdded);
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<uint>>(this, MessageKeys.TrainingWeeksRemoved);
            MessagingCenter.Unsubscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitDeleted);
            MessagingCenter.Unsubscribe<BaseViewModel, uint>(this, MessageKeys.WorkUnitChanged);
            base.Dispose();
        }
    }
}
