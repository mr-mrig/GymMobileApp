using System.Collections.ObjectModel;
using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class WorkoutTemplate : BaseEntityModel
    {

        #region Backing fields
        private string _name;
        private int? _weekdayId;
        private uint _weekId;

        private ObservableCollection<WorkUnitTemplate> _workUnits;
        #endregion


        public string Name 
        { 
            get => _name; 
            set => Set(ref _name, value); 
        }
        public int? WeekdayId
        {
            get => _weekdayId;
            set => Set(ref _weekdayId, value);
        }
        public uint WeekId 
        { 
            get => _weekId; 
            set => Set(ref _weekId,value);
        }

        public ObservableCollection<WorkUnitTemplate> WorkUnits
        {
            get => _workUnits;
            set => Set(ref _workUnits, value);
        }
    }
}