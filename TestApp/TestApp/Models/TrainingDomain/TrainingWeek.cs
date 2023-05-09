using System.Collections.ObjectModel;
using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingWeek : BaseEntityModel
    {
        #region Backing fields
        private uint? _progressiveNumber;
        private uint? _weekTypeId;
        private ObservableCollection<WorkoutTemplate> _workouts;
        #endregion

        public uint? ProgressiveNumber
        {
            get => _progressiveNumber;
            set => Set(ref _progressiveNumber, value);
        }
        public uint? WeekTypeId 
        { 
            get => _weekTypeId; 
            set => Set(ref _weekTypeId, value); }

        public ObservableCollection<WorkoutTemplate> Workouts 
        {
            get => _workouts;
            set => Set(ref _workouts, value);
        }
    }
}
