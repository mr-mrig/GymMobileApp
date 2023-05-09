using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models.TrainingDomain;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using System;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils.Extensions;
using TestApp.Services.Utils;

namespace TestApp.ViewModels
{
    public class WorkoutViewModel : BaseViewModel
    {

        #region Backing Fields
        private IList<WorkUnitTemplate> _workUnits;
        private uint? _id;
        private ICommand _linkWorkUnitCommand;
        private ICommand _deleteWorkUnitCommand;
        #endregion

        private ITrainingPlanService _trainingPlanService;


        /// <summary>
        /// The Workout ID
        /// </summary>
        public uint? Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The model of the WorkUnits belonging to the Workout
        /// </summary>
        public IList<WorkUnitTemplate> WorkUnits
        {
            get => _workUnits;
            set
            {
                _workUnits = value;
                RaisePropertyChanged();
            }
        }

        #region Commands
        public ICommand LinkWorkUnitCommand 
            => _linkWorkUnitCommand ?? (_linkWorkUnitCommand = new Command(async x => await LinkWorkUnits(x as WorkUnitTemplate)));
        public ICommand DeleteWorkUnitCommand 
            => _deleteWorkUnitCommand ?? (_deleteWorkUnitCommand = new Command(x => NotifyDeleteWorkUnit((x as WorkUnitTemplate).ProgressiveNumber)));

        #endregion



        #region Ctors
        public WorkoutViewModel(uint? id, IEnumerable<WorkUnitTemplate> workUnits, ITrainingPlanService trainingPlanService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentException(nameof(trainingPlanService));

            Id = id;
            WorkUnits = new SmartObservableCollection<WorkUnitTemplate>(workUnits ?? new List<WorkUnitTemplate>());
        }
        #endregion


        public void DeleteWorkUnit(uint progressiveNumber)
        {
            WorkUnitTemplate workUnitVm = FindWorkUnitByProgressiveNumber(progressiveNumber);

            if (WorkUnits.Remove(workUnitVm))
            {
                // Sort progressive numbers again
                ForceWorkUnitsProgressiveNumbers();
                // Fix the link to the deleted WU, if any in place
                // NOTE: after the delete, the following WU has now the Progressive Number the removed one had before!
                UnlinkWorkUnit(workUnitVm.ProgressiveNumber.Value);
            }
        }


        public void AddWorkUnit(WorkUnitTemplate toAdd)
        {
            WorkUnits.Insert((int)toAdd.ProgressiveNumber, toAdd);
            ForceWorkUnitsProgressiveNumbers();

            RaisePropertyChanged(nameof(WorkUnits));
        }


        private void NotifyDeleteWorkUnit(uint? progressiveNumber)
        {
            if (progressiveNumber.HasValue)
                MessagingCenter.Send(this as BaseViewModel, MessageKeys.WorkUnitDeleted, progressiveNumber.Value);
        }


        private void ForceWorkUnitsProgressiveNumbers()
        {
            //RIGM: ensure WorkUnits are always sorted
            for (uint i = 0; i < WorkUnits.Count; i++)
            {
                WorkUnitTemplate wu = WorkUnits[(int)i];
                wu.ProgressiveNumber = i;
            }
        }

        private void UnlinkWorkUnit(uint toUnlinkProgressiveNumber)
        {
            if(toUnlinkProgressiveNumber < WorkUnits.Count - 1)
            {
                WorkUnitTemplate nextOne = WorkUnits[(int)toUnlinkProgressiveNumber];

                if (nextOne?.IntensityTechnique != null && nextOne.IntensityTechnique.IsLinking)
                    nextOne.IntensityTechnique = null;

                RaisePropertyChanged(nameof(WorkUnits));
            }
        }

        private void LinkWorkUnit(uint toLinkProgressiveNumber, IntensityTechnique technique)
        {
            if (toLinkProgressiveNumber > 0)
                WorkUnits[(int)toLinkProgressiveNumber].IntensityTechnique = technique;

            RaisePropertyChanged(nameof(WorkUnits));
        }

        private async Task LinkWorkUnits(WorkUnitTemplate toBeLinkedToPrevious)
        {
            if(toBeLinkedToPrevious.ProgressiveNumber > 0)
            {
                if (toBeLinkedToPrevious.IntensityTechnique == null)
                {
                    IntensityTechnique it = AppEnvironment.DefaultLinkingIntensityTechnique;
                    await _trainingPlanService.LinkToPreviousWorkUnit(Id.Value, toBeLinkedToPrevious.ProgressiveNumber.Value, it.Id.Value);
                    LinkWorkUnit(toBeLinkedToPrevious.ProgressiveNumber.Value, it);
                }
                else
                {
                    await _trainingPlanService.UnlinkFromPreviousWorkUnit(Id.Value, toBeLinkedToPrevious.ProgressiveNumber.Value);
                    // Do not propagate...
                    //UnlinkWorkUnit(toBeLinkedToPrevious.ProgressiveNumber.Value);
                }
                RaisePropertyChanged(nameof(WorkUnits));
            }
        }


        /// <summary>
        /// Get the ViewModel of the WorkUnit with the specified ProgressiveNumber
        /// </summary>
        /// <param name="progressiveNumber">The progressive number</param>
        /// <returns>The WorkUnitDetailViewModel object</returns>
        private WorkUnitTemplate FindWorkUnitByProgressiveNumber(uint progressiveNumber)
            => WorkUnits.SingleOrDefault(x => x.ProgressiveNumber == progressiveNumber);
    }





    public class GroupedWorkoutViewModels : ObservableCollection<WorkoutViewModel>
    {
        public uint WeekId { get; set; }
        public uint WeekProgressiveNumber { get; set; }
        public ObservableCollection<WorkoutViewModel> Workouts => this;

        public GroupedWorkoutViewModels(uint weekId, uint weekProgressiveNumber, IEnumerable<WorkoutViewModel> items)
        {
            WeekId = weekId;
            WeekProgressiveNumber = weekProgressiveNumber;

            foreach (WorkoutViewModel item in items)
                Items.Add(item);
        }
    }
}
