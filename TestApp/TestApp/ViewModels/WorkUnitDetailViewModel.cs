using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Models.TrainingDomain;
using TestApp.Services.DomainPresenters;
using TestApp.Services.Navigation;
using TestApp.Services.Utils.Extensions;
using TestApp.ViewModels.Base;
using TestApp.ViewModels.Base.Commands;
using TestApp.ViewModels.Popups.Common;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class WorkUnitDetailViewModel : BaseViewModel
    {


        private INavigationService _navigationService;


        #region Backing Fields
        private uint _weekId;
        private uint _weekProgressiveNumber;
        private uint _workoutId;
        private bool _isEditing;
        private bool _hasPendingChanges;
        private WorkUnitTemplate _workUnit;
        private ICollection<WorkingSetViewModel> _formattedWorkingSets;
        private ICommandAsync<WorkingSetViewModel> _deleteWorkingSetCommand;
        #endregion



        /// <summary>
        /// Whether the user is editing
        /// </summary>        
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                RaisePropertyChanged();
                NotifyEditingChanged();
            }
        }


        /// <summary>
        /// Whether the user changed something in the WU. Yet, this does not mean that the final WU is different from the original one...
        /// </summary>        
        public bool HasPendingChanges
        {
            get => _hasPendingChanges;
            set
            {
                _hasPendingChanges = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The parent Workout ID
        /// </summary>        
        public uint WorkoutId
        {
            get => _workoutId;
            set
            {
                _workoutId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The ID of the week the parent Workout has been planned for
        /// </summary>        
        public uint WeekId
        {
            get => _weekId;
            set
            {
                _weekId = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The progressive number of the week the parent Workout has been planned for
        /// </summary>        
        public uint WeekProgressiveNumber
        {
            get => _weekProgressiveNumber;
            set
            {
                _weekProgressiveNumber = value;
                RaisePropertyChanged();
            }
        }

        ///// <summary>
        ///// The Work Unit
        ///// </summary>
        public WorkUnitTemplate WorkUnit
        {
            get => _workUnit;
            set
            {
                if (_workUnit != null)
                    WorkUnit.PropertyChanged -= WorkUnit_PropertyChanged;

                _workUnit = value;
                RaisePropertyChanged();
                FormattedWorkingSets = ToFormattedWorkingSets(WorkUnit);

                if (value != null)
                    WorkUnit.PropertyChanged += WorkUnit_PropertyChanged;
            }
        }

        public ICollection<WorkingSetViewModel> FormattedWorkingSets
        {
            get => _formattedWorkingSets;
            set
            {
                _formattedWorkingSets = value;
                RaisePropertyChanged();
            }
        }



        #region Commands

        public ICommandAsync<WorkingSetViewModel> DeleteWorkingSetCommand => (_deleteWorkingSetCommand ?? new CommandAsync<WorkingSetViewModel>(
            x => DeleteWorkingSet((x as WorkingSetViewModel).ProgressiveNumber),
            x => IsEditing && WorkUnit.WorkingSets.Count > 1));

        #endregion


        #region Ctors
        public WorkUnitDetailViewModel(INavigationService navigationService)
        {
            _hasPendingChanges = false;
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion



        #region Commands Methods

        private Task DeleteWorkingSet(uint progressiveNumber)
        {
            // If unlinked WS just remove it, otherwise remove the whole cluster
            do
            {
                WorkUnit.RemoveWorkingSet((int)progressiveNumber);
            }
            while (progressiveNumber < WorkUnit.WorkingSets.Count && WorkUnit.WorkingSets[(int)progressiveNumber].IsDropset());

            HasPendingChanges = true;

            return Task.FromResult(true);
        }
        #endregion





        /// <summary>
        /// Map the specified Work Unit to a list of Working Sets formatted in such a way that the linked WS are grouped into the same entry
        /// </summary>
        /// <param name="workUnit">The model</param>
        /// <returns>The collection of formatted ViewModels</returns>
        private ICollection<WorkingSetViewModel> ToFormattedWorkingSets(WorkUnitTemplate workUnit)
        {
            // Group the WS into WS linked clusters - Dropset supported so far
            List<WorkUnitTemplate> clustered = new List<WorkUnitTemplate>();
            WorkUnitTemplate cluster = null;

            foreach(WorkingSetTemplate ws in workUnit.WorkingSets ?? new List<WorkingSetTemplate>())
            {
                if (ws.IsDropset())
                    cluster.WorkingSets.Add(ws);
                else
                {
                    // The first WS is never a cluster even if it is the first one of a cluster
                    cluster = new WorkUnitTemplate
                    {
                        WorkingSets = new List<WorkingSetTemplate> { ws },
                    };
                    clustered.Add(cluster);
                }
            }

            // Map the clusters into their WS representation
            List<WorkingSetViewModel> workingSets = new List<WorkingSetViewModel>();
            int wsCounter = 0;

            foreach (WorkUnitTemplate clusterAsWorkUnit in clustered)
            {
                ITrainingPresenter presenter = new WorkingSetPresenter(clusterAsWorkUnit);

                // Please notice that clustered sets have formatted representation has non consecutive Progressive Numbers: 2x10+10 -> WS[0].PNum = 0, WS[1].PNum = 2
                uint progressiveNumber = wsCounter == 0 ? 0 :
                    clustered[wsCounter - 1].WorkingSets.Last().ProgressiveNumber.Value + 1;

                WorkingSetViewModel ws = new WorkingSetViewModel
                {
                    ProgressiveNumber = progressiveNumber,
                    ClusteredSetsCounter = clusterAsWorkUnit.WorkingSets.Count,
                    FormattedRepetitions = presenter.ToFormattedRepetitions(),
                    Rest = presenter.ToFormattedRest(),
                    LiftingTempo = presenter.ToFormattedTempo(),
                    Effort = presenter.ToFormattedEffort(),
                    IntensityTechniques = presenter.ToFormattedIntensityTechniques(),
                };
                // Event handlers
                //ws.PropertyChanged += FormattedWorkingSet_PropertyChanged;    // We don't want to raise an event until the user has finished editing!
                ws.RepetitionsChangedCommand = new Command(x => ChangeRepetitions(x as WorkingSetViewModel), x => IsEditing);
                ws.RestChangedCommand = new Command(x => ChangeRest(x as WorkingSetViewModel), x => IsEditing);
                ws.LiftingTempoChangedCommand = new Command(x => ChangeLiftingTempo(x as WorkingSetViewModel), x => IsEditing);
                ws.EffortChangedCommand = new Command(x => ChangeEffort(x as WorkingSetViewModel), x => IsEditing);
                ws.OpenIntensityTechniquesManagerCommand = new Command(async x => await OpenIntensityTechniquesManager((x as WorkingSetViewModel).IntensityTechniques), x => IsEditing);
                workingSets.Add(ws);
                wsCounter++;
            }
            return workingSets;
        }

        /// <summary>
        /// Map the working set formatted as per view to the corresponding model.
        /// This can return a list of WS as the VM might represent a concise description of multiple WSs, IE: '3x12'
        /// Please notice that if the input cannot be parsed then null is returned.
        /// </summary>
        /// <param name="formattedWorkingSets">The VM of the working sets as provided by the View</param>
        /// <returns>The collection of formatted WorkingSet Models or NULL if the input could not be parsed</returns>
        private WorkUnitTemplate ToModel(WorkingSetViewModel formattedWorkingSet)
        {
            ITrainingPresenter presenter = new WorkUnitPresenter();
            try
            {
                presenter.ToModel(
                    formattedWorkingSet.FormattedRepetitions,
                    formattedWorkingSet.Rest,
                    formattedWorkingSet.LiftingTempo,
                    formattedWorkingSet.Effort,
                    formattedWorkingSet.IntensityTechniques);
            }
            catch (Exception exc)
            {
                return null;
            }
            // Fix progressive numbers
            presenter.WorkUnit.WorkingSets = presenter.WorkUnit.WorkingSets.Select((x, i) =>
            {
               x.ProgressiveNumber = (uint)(formattedWorkingSet.ProgressiveNumber + i);
               return new WorkingSetTemplate(x);

            }).ToList();

            return presenter.WorkUnit;
        }



        #region Public Methods
        public void AddWorkingSet()
        {
            HasPendingChanges = true;
            WorkUnit.DupicateLastWorkingSet();
        }

        /// <summary>
        /// Perform the "value copy" from another instance IE: the WSs value copies are performed
        /// </summary>
        /// <param name="another">The other instance to copy from</param>
        public void CopyFrom(WorkUnitTemplate another)
        {
            WorkUnit.CopyFrom(another);
            HasPendingChanges = true;
        }
        #endregion




        private void WorkUnit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Keep the model and the view aligned
            if (e.PropertyName == nameof(WorkUnit.WorkingSets))
                FormattedWorkingSets = ToFormattedWorkingSets(WorkUnit);
        }

        private void NotifyEditingChanged()
        {
            DeleteWorkingSetCommand.ChangeCanExecute();
        }

        private void UpdateWorkingSets(IEnumerable<WorkingSetTemplate> original, IList<WorkingSetTemplate> updated)
        {
            IEnumerator<WorkingSetTemplate> originalIterator = original.GetEnumerator();
            IEnumerator<WorkingSetTemplate> updatedIterator = updated.GetEnumerator();

            // Start changing the WSs sequentially
            while (originalIterator.MoveNext() && updatedIterator.MoveNext())
                //originalIterator.Current.Repetitions = updatedIterator.Current.Repetitions;
                originalIterator.Current.CopyFrom(updatedIterator.Current);

            // Then, if the updated list has more elements it means that there have been additions
            while (updatedIterator.MoveNext())
                WorkUnit.AddWorkingSet(updatedIterator.Current);

            // Conversely, if the original list has more items, there have been deletions
            if (original.Count() > updated.Count())
            {
                do
                {
                    WorkUnit.RemoveWorkingSet(originalIterator.Current);
                }
                while (originalIterator.MoveNext());
            }
        }



        #region Workaround for 'Operation is not valid due to the current state of the object' when calling RelastiveSource in 'WorkUnitDetailView' Commands
        private void ChangeRepetitions(WorkingSetViewModel workingSetViewModel)
        {
            IList<WorkingSetTemplate> changed = ToModel(workingSetViewModel)?.WorkingSets;

            if(changed == null)
            {
                //RIGM: todo
                return;
            }
            IEnumerable<WorkingSetTemplate> toChange = WorkUnit.WorkingSets
                .Skip((int)workingSetViewModel.ProgressiveNumber)
                .Take(workingSetViewModel.ClusteredSetsCounter);

            //RIGM: this is very basic!
            if (!toChange.Select(x => x.Repetitions).SequenceEqual(changed.Select(x => x.Repetitions)))
            {
                HasPendingChanges = true;
                UpdateWorkingSets(toChange, changed);
            }
        }

        private void ChangeRest(WorkingSetViewModel workingSetViewModel)
        {
            IList<WorkingSetTemplate> changed = ToModel(workingSetViewModel)?.WorkingSets;

            if (changed == null)
            {
                //RIGM: todo
                return;
            }
            IEnumerable<WorkingSetTemplate> toChange = WorkUnit.WorkingSets
                .Skip((int)workingSetViewModel.ProgressiveNumber)
                .Take(workingSetViewModel.ClusteredSetsCounter);

            //RIGM: this is very basic!
            if (!toChange.Select(x => x.Rest).SequenceEqual(changed.Select(x => x.Rest)))
            {
                HasPendingChanges = true;
                UpdateWorkingSets(toChange, changed);
            }
        }

        private void ChangeLiftingTempo(WorkingSetViewModel workingSetViewModel)
        {
            IList<WorkingSetTemplate> changed = ToModel(workingSetViewModel)?.WorkingSets;

            if (changed == null)
            {
                //RIGM: todo
                return;
            }
            IEnumerable<WorkingSetTemplate> toChange = WorkUnit.WorkingSets
                .Skip((int)workingSetViewModel.ProgressiveNumber)
                .Take(workingSetViewModel.ClusteredSetsCounter);

            //RIGM: this is very basic!
            if (!toChange.Select(x => x.LiftingTempo).SequenceEqual(changed.Select(x => x.LiftingTempo)))
            {
                HasPendingChanges = true;
                UpdateWorkingSets(toChange, changed);
            }
        }

        private void ChangeEffort(WorkingSetViewModel workingSetViewModel)
        {
            IList<WorkingSetTemplate> changed = ToModel(workingSetViewModel)?.WorkingSets;

            if (changed == null)
            {
                //RIGM: todo
                return;
            }
            IEnumerable<WorkingSetTemplate> toChange = WorkUnit.WorkingSets
                .Skip((int)workingSetViewModel.ProgressiveNumber)
                .Take(workingSetViewModel.ClusteredSetsCounter);

            //RIGM: this is very basic!
            if (!toChange.Select(x => x.Effort).SequenceEqual(changed.Select(x => x.Effort)))
            {
                HasPendingChanges = true;
                UpdateWorkingSets(toChange, changed);
            }
        }

        private async Task OpenIntensityTechniquesManager(string formattedIntensityTechniques)
        {
            var popupViewModel = new DestructiveActionPopupViewModel(_navigationService)
            {
                TitleText = "Intensity Techniques Manager",
                MessageText = "Current intensity techniques: " + formattedIntensityTechniques + Environment.NewLine + ". TODO!",
                CancelActionText = "No, go back",
                MainActionText = "Yes, delete it!",
                MainCommand = null,
            };
            await _navigationService.OpenPopup<DestructiveActionPopupViewModel>(popupViewModel);
        }
        #endregion


        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        public override void Dispose()
        {
            WorkUnit.WorkingSets.Clear();   // Disposing handlers
            WorkUnit.PropertyChanged -= WorkUnit_PropertyChanged;

            base.Dispose();
        }


        #region Needed by HorizontalScrollableViewSelector
        public override string ToString() => $"Excercise {WorkUnit.ProgressiveNumber + 1} {Environment.NewLine}Week {WeekProgressiveNumber + 1}";

        public override bool Equals(object obj)
        {
            if (obj is WorkUnitDetailViewModel other)
                return WorkUnit.Id == other.WorkUnit.Id;

            return false;
        }

        public override int GetHashCode() => WorkUnit.Id.GetHashCode();
        #endregion


    }
}
