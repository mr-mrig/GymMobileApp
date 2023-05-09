using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils.Extensions;
using TestApp.ViewModels.Base;
using TestApp.ViewModels.Base.Commands;
using TestApp.ViewModels.Popups;
using TestApp.ViewModels.Popups.Common;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class WorkUnitMainViewModel : BaseViewModel
    {


        /// <summary>
        /// Placeholder text when no note has been attached. Used by the XAML also.
        /// </summary>
        public const string NewNoteText = "You can write your note here...";


        private ITrainingPlanService _trainingPlanService;
        private INavigationService _navigationService;


        /// <summary>
        /// The WorkUnits as stored in the DB. 
        /// These should be compared whenever the user asks for a save, so only real changes are saved again
        /// </summary>
        private ICollection<WorkUnitTemplate> _savedWorkUnits;


        #region Backing Fields
        private bool _isNewWorkUnit = false;
        private bool _isEditing = false;
        private bool _hasPendingChanges = false;
        private Excercise _excercise;
        private uint? _noteId;
        private string _note;
        private string _workoutName;
        private WorkUnitDetailViewModel _selectedWorkUnit;
        private IList<WorkUnitDetailViewModel> _plannedWorkUnits;
        private ICommandAsync _copyFromPreviousWorkoutCommand;
        private ICommandAsync _undoChangesCommand;
        private ICommandAsync _saveChangesCommand;
        private ICommandAsync _startEditingCommand;
        private ICommandAsync _copyAllFromThisCommand;
        private ICommandAsync _addWorkingSetCommand;
        private ICommandAsync _openNoteDetailCommand;
        #endregion


        /// <summary>
        /// Whether the user is in editing mode (beacuse of modifying or adding)
        /// </summary>        
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                RaisePropertyChanged();
                NotifyEditingChanged();
                NotifyChildren(value);
            }
        }

        /// <summary>
        /// Whether the user is adding a new Work Unit.
        /// Please notice that setting this will force <see cref="IsEditing"/> to the same value.
        /// </summary>        
        public bool IsNewWorkUnit
        {
            get => _isNewWorkUnit;
            set
            {
                _isNewWorkUnit = value;
                RaisePropertyChanged();
                if(value)
                    IsEditing = true;     // Forcing 
            }
        }

        /// <summary>
        /// Whether the user changed something in the WU. Yet, this does not mean that the final WU is different from the original one...
        /// </summary>        
        public bool HasPendingChanges => PlannedWorkUnits.Any(x => x.HasPendingChanges);
        //public bool HasPendingChanges
        //{
        //    get => _hasPendingChanges;
        //    set
        //    {
        //        _hasPendingChanges = value;
        //        RaisePropertyChanged();
        //    }
        //}

        /// <summary>
        /// The Workout name
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
        /// The Work unit note ID - this is the same for the whole plan
        /// </summary>        
        public uint? NoteId
        {
            get => _noteId;
            set
            {
                _noteId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Work unit note - this is the same for the whole plan
        /// </summary>        
        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Excercise to be performed  - this is the same for the whole plan
        /// </summary>        
        public Excercise Excercise
        {
            get => _excercise;
            set
            {
                _excercise = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The model of the selected WorkUnit
        /// </summary>
        public WorkUnitDetailViewModel SelectedWorkUnit
        {
            get => _selectedWorkUnit;
            set
            {
                _selectedWorkUnit = value;
                RaisePropertyChanged();
                CopyFromPreviousWorkoutCommand?.ChangeCanExecute();
            }
        }

        //RIGM: to optimize space we could remove the Excercise and the Note from the WorkUnitDetailViewModels  as they are all the same
        /// <summary>
        /// The plan of the Work Unit over the training weeks
        /// </summary>
        public IList<WorkUnitDetailViewModel> PlannedWorkUnits
        {
            get => _plannedWorkUnits;
            set
            {
                _plannedWorkUnits = value;
                RaisePropertyChanged();
            }
        }


        #region Commands

        public ICommandAsync UndoChangesCommand => _undoChangesCommand ?? (_undoChangesCommand = new CommandAsync(
            async () => await UndoChanges(), 
            x => IsEditing));
        public ICommandAsync SaveChangesCommand => _saveChangesCommand ?? (_saveChangesCommand = new BlockingCommandAsync(
            async () => await SaveChanges(),
            x => IsEditing));
        public ICommandAsync StartEditingCommand => _startEditingCommand ?? (_startEditingCommand = new CommandAsync(
            async () => await StartEditing(),
            x => !IsEditing));
        public ICommandAsync CopyFromPreviousWorkoutCommand => _copyFromPreviousWorkoutCommand ?? (_copyFromPreviousWorkoutCommand = new ThrottledCommandAsync(
            execute: async () => await CopyFromPreviousWorkout(),
            throttlePeriod: 200,
            canExecute: x => IsEditing && PlannedWorkUnits.IndexOf(SelectedWorkUnit) > 0));
        public ICommandAsync CopyAllFromThisCommand => _copyAllFromThisCommand ?? (_copyAllFromThisCommand = new ThrottledCommandAsync(
            execute: () => CopyAllFromThisWorkout(),
            throttlePeriod: 200,
            canExecute: x => IsEditing));
        public ICommandAsync AddWorkingSetCommand => _addWorkingSetCommand ?? (_addWorkingSetCommand = new CommandAsync(
            async () => await AddWorkingSet(), 
            x => IsEditing));
        public ICommandAsync OpenNoteDetailCommand => _openNoteDetailCommand ?? (_openNoteDetailCommand = new CommandAsync(
            async () => await OpenNoteDetail()));

        #endregion



        #region Ctors
        public WorkUnitMainViewModel(ITrainingPlanService trainingPlanService, INavigationService navigationService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion


        #region Commands Methods

        private async Task AddWorkingSet()
        {
            SelectedWorkUnit.AddWorkingSet();
            await Task.Yield();
        }

        private async Task OpenNoteDetail()
        {
            var popupViewModel = new BaseEditingPopupViewModel(_navigationService)
            {
                TitleText = "Excercise note",
                InputText = Note,
                PlaceholderText = NewNoteText,
                MainCommand = new CommandAsync<string>(async x => await SaveNoteAsync(x)),
            };
            await _navigationService.OpenPopup<BaseEditingPopupViewModel>(popupViewModel);
        }

        private async Task SaveNoteAsync(string newNote)
        {
            bool ok = true;
            string parsedNote = newNote.Trim();

            if (parsedNote == Note)
                return;

            if (string.IsNullOrEmpty(parsedNote))
                ; //ok = _trainingPlanService.ClearNote(PlannedWorkUnits.Select(x => x.WorkUnit.Id));
            else
            {
                ;
                //IEnumerable<uint> noteIds = _trainingPlanService.AddNote(PlannedWorkUnits.Select(x => x.WorkUnit.Id), parsedNote);

                //if (noteIds.Count() != PlannedWorkUnits.Count)
                //    ok = false;
            }
            if(!ok)
            {
                var alertViewModel = new AlertPopupViewModel()
                {
                    AlertMessage = "",    
                    AlertType = AlertPopupType.Info,
                };
                await _navigationService.OpenPopup<AlertPopupViewModel>(alertViewModel);

                //RIGM: todo rollback?
            }
        }

        private Task CopyFromPreviousWorkout()
        {
            int toReplaceIndex = PlannedWorkUnits.IndexOf(SelectedWorkUnit);

            PlannedWorkUnits[toReplaceIndex].CopyFrom(PlannedWorkUnits[toReplaceIndex - 1].WorkUnit);
            SelectedWorkUnit = PlannedWorkUnits[toReplaceIndex];

            return Task.FromResult(true);
        }
        private Task CopyAllFromThisWorkout()
        {
            foreach (var vm in PlannedWorkUnits)
                vm.CopyFrom(SelectedWorkUnit.WorkUnit);

            return Task.FromResult(true);
        }

        private async Task StartEditing()
        {
            IsEditing = true;
            await Task.Yield();
        }

        private async Task SaveChanges()
        {
            bool ok = true;

            if (!HasPendingChanges)
            {
                IsEditing = false;
                return;
            }

            IEnumerable<WorkUnitDetailViewModel> changedViewModels = PlannedWorkUnits.Where(x => x.HasPendingChanges);

            //RIGM: by saving as below the user can insert new ITs sepcifying no Description etc. Should this be allowed? If not, how do we le the user add new ITs?
            List<IntensityTechnique> addedTechniques = (await SaveNewIntensityTechniques(changedViewModels
                .SelectMany(wm => wm.WorkUnit.WorkingSets)
                .SelectMany(ws => ws.IntensityTechniques ?? new ObservableCollection<IntensityTechnique>())
                .Where(it => it.Id == null)))
                .ToList();

            if (addedTechniques == null)
                ok = false;
            else
                AssignNewIntensityTechniques(changedViewModels.SelectMany(vm => vm.WorkUnit.WorkingSets), addedTechniques);

            //RIGM: We should update the AppEnvironment so we don't have to fetch again...



            // Save the changes to the Wroking Sets
            foreach (WorkUnitDetailViewModel workUnitVm in changedViewModels)
            {
                List<uint> workoutIds = (await _trainingPlanService.SetWorkUnitTemplate(workUnitVm.WorkoutId, workUnitVm.WorkUnit)).ToList();

                if (workoutIds.Count == workUnitVm.WorkUnit.WorkingSets.Count)
                {
                    foreach (WorkingSetTemplate ws in workUnitVm.WorkUnit.WorkingSets)
                        ws.Id = workoutIds[(int)ws.ProgressiveNumber];

                    workUnitVm.HasPendingChanges = false;

                    // Notify changes. Please notice that this is mandatory as we deliberately got rid of ExtendedObservableCollections
                    MessagingCenter.Send(this as BaseViewModel, MessageKeys.WorkUnitChanged, workUnitVm.WorkUnit.Id.Value);
                }
                else
                {
                    // Something went wrong
                    ok = false;
                    break;
                }
            }
            if (ok)
            {
                StoreLastSavedWorkUnits();
                IsEditing = false;
            }
            else
                ; //RIGM: notify error / rollback
        }

        /// <summary>
        /// Modify the Working Sets by replacing the new Intensity Techniques (which have null Id) with the ones took from the input list.
        /// PLease notice that the equality is checked comparing by the <see cref="IntensityTechnique.Abbreviation"/> property.
        /// </summary>
        /// <param name="workingSets">Thw Working Sets to be processed</param>
        /// <param name="newTechniques">The Intensity Techniques which have just been created</param>
        private void AssignNewIntensityTechniques(IEnumerable<WorkingSetTemplate> workingSets, IEnumerable<IntensityTechnique> newTechniques)
        {
            if (newTechniques.Count() > 0)
            {
                foreach (WorkingSetTemplate ws in workingSets)
                {
                    // Replace the ITs with the updated ones
                    ws.IntensityTechniques = new ObservableCollection<IntensityTechnique>(ws.IntensityTechniques.Select(it =>
                    {
                        if (it.Id == null)
                            it.Id = newTechniques.Single(x => x.Abbreviation == it.Abbreviation).Id;

                        return it;
                    }));
                }
            }
        }

        /// <summary>
        /// Insert the Intensity Techniques that have just been created (IE: having null Ids)
        /// </summary>
        /// <param name="intensityTechniques">The Intensity Techniques to be saved which must have null Id</param>
        /// <returns>The created Intenisty Techniques list, with the DB generated Ids</returns>
        private async Task<IEnumerable<IntensityTechnique>> SaveNewIntensityTechniques(IEnumerable<IntensityTechnique> intensityTechniques)
        {
            if(intensityTechniques == null || intensityTechniques.Count() == 0)
                return new List<IntensityTechnique>();

            List<uint> itIds = (await _trainingPlanService.InsertIntensityTechniques(intensityTechniques)).ToList();

            if (itIds.Count != intensityTechniques.Count())
                return null;

            IEnumerable<IntensityTechnique> savedIntensityTechniques = intensityTechniques.Select((it, i) => new IntensityTechnique
            {
                Id = itIds[i],
                Abbreviation = it.Abbreviation,
            });

            return savedIntensityTechniques;
        }

        private async Task UndoChanges()
        {
            if(HasPendingChanges)
            {
                LoadLastSavedWorkUnits();

                foreach (var vm in PlannedWorkUnits.Where(x => x.HasPendingChanges))
                    vm.HasPendingChanges = false;

                await Task.Yield();
            }
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Notify the child VMs that their IsEditing mode should change
        /// </summary>
        /// <param name="isEditing">The new IsEditing value</param>
        private void NotifyChildren(bool isEditing)
        {
            SelectedWorkUnit.IsEditing = isEditing;

            foreach (var vm in PlannedWorkUnits)
                vm.IsEditing = isEditing;
        }

        /// <summary>
        /// All commands which CanExecute is bounded to the IsEditing flag should be notified here
        /// </summary>
        private void NotifyEditingChanged()
        {
            CopyFromPreviousWorkoutCommand.ChangeCanExecute();
            CopyAllFromThisCommand.ChangeCanExecute();
            AddWorkingSetCommand.ChangeCanExecute();
            SaveChangesCommand.ChangeCanExecute();
            UndoChangesCommand.ChangeCanExecute();
            StartEditingCommand.ChangeCanExecute();
        }
        /// <summary>
        /// All commands which CanExecute is bounded to the HasPendingChanges flag should be notified here
        /// </summary>
        private void NotifyPendingChangesChanged()
        {
            UndoChangesCommand?.ChangeCanExecute();
            SaveChangesCommand?.ChangeCanExecute();
        }

        /// <summary>
        /// Stores a deep copy of the Work Units in the _savedWorkUnits buffer
        /// </summary>
        private void StoreLastSavedWorkUnits()
        {
            _savedWorkUnits = new List<WorkUnitTemplate>();

            foreach (var workUnitVm in PlannedWorkUnits)
                _savedWorkUnits.Add(new WorkUnitTemplate(workUnitVm.WorkUnit));
        }

        /// <summary>
        /// Loads the Work Units from the _savedWorkUnits buffer into the PlannedWorkUnits - performs a deep copy
        /// </summary>
        private void LoadLastSavedWorkUnits()
        {
            int selectedIndex = PlannedWorkUnits.IndexOf(SelectedWorkUnit);

            foreach(var vm in PlannedWorkUnits)
                vm.CopyFrom(_savedWorkUnits.Single(x => vm.WorkUnit == x));

            SelectedWorkUnit = PlannedWorkUnits[selectedIndex];
        }

        /// <summary>
        /// Check whether the user has wrote a note
        /// </summary>
        /// <returns>True if any note has been wrote</returns>
        private bool AnyNoteAttached() => Note == NewNoteText;  // Cannot check by NoteId as it might be NULL for new Notes
        #endregion

        public override Task InitializeAsync(object navigationData)
        {
            if(navigationData is WorkUnitMainViewModel vm)
            {
                WorkoutName = vm.WorkoutName;
                PlannedWorkUnits = new SmartObservableCollection<WorkUnitDetailViewModel>(vm.PlannedWorkUnits);
                SelectedWorkUnit = PlannedWorkUnits.Single(x => x.WorkoutId == vm.SelectedWorkUnit.WorkoutId);     // Need to do this way so we keep the references! IE: just copying from SelectedWorkUnit makes the selected VM a different object from the PlannedWorkUnits[selectedIndex]
                Excercise = vm.Excercise;
                Note = vm.Note; 
                NoteId = vm.NoteId;
                IsEditing = vm.IsEditing;
                IsNewWorkUnit = vm.IsNewWorkUnit;

                // Listen to events
                //foreach (var wu in PlannedWorkUnits)
                //    wu.PropertyChanged += (o, e) =>
                //    {
                //        if (e.PropertyName == nameof(wu.HasPendingChanges))
                //            HasPendingChanges = HasPendingChanges || wu.HasPendingChanges;  //RIGM: Does not work when HasPendingChanges changes to false!
                //    };

                if (!IsNewWorkUnit)
                    StoreLastSavedWorkUnits();
            }

            return base.InitializeAsync(navigationData);
        }


        public override void Dispose()
        {
            SelectedWorkUnit.Dispose();

            foreach (var vm in PlannedWorkUnits)
                vm.Dispose();

            base.Dispose();
        }
    }
}
