using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestApp.Models.Base;
using TestApp.Models.Common;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingPlanDetails : BaseModel
    {


        #region Backing fields
        private string _name;
        private string _note;
        private uint _ownerId;
        private bool _isBookmarked;
        private uint _userPlanId;
        private int? _trainingWeeksCounter;
        private uint? _noteId;
        private ParentPlanRelation _parentPlan;
        private ObservableCollection<ISimpleTagElement> _hashtags;
        private ObservableCollection<ISimpleTagElement> _targetProficiencies;
        private ObservableCollection<ISimpleTagElement> _targetPhases;
        private ObservableCollection<ISimpleTagElement> _musclesFocuses;
        #endregion

        public string Name 
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public uint OwnerId 
        { 
            get => _ownerId; 
            set => Set(ref _ownerId, value); 
        }

        public bool IsBookmarked 
        { 
            get => _isBookmarked; 
            set => Set(ref _isBookmarked, value); 
        }

        public uint UserPlanId 
        { 
            get => _userPlanId; 
            set => Set(ref _userPlanId, value); 
        }

        public uint? NoteId 
        { 
            get => _noteId; 
            set => Set(ref _noteId, value); 
        }

        public int? TrainingWeeksCounter
        { 
            get => _trainingWeeksCounter; 
            set => Set(ref _trainingWeeksCounter, value); 
        }

        public string Note
        {
            get => _note;
            set => Set(ref _note, value);
        }

        public ParentPlanRelation ParentPlan 
        { 
            get => _parentPlan; 
            set => Set(ref _parentPlan, value); 
        }
        public ObservableCollection<ISimpleTagElement> Hashtags 
        { 
            get => _hashtags; 
            set => Set(ref _hashtags, value);
        }
        public ObservableCollection<ISimpleTagElement> TargetProficiencies 
        { 
            get => _targetProficiencies; 
            set => Set(ref _targetProficiencies, value); 
        }
        public ObservableCollection<ISimpleTagElement> TargetPhases 
        { 
            get => _targetPhases; 
            set => Set(ref _targetPhases, value); 
        }
        public ObservableCollection<ISimpleTagElement> MusclesFocuses 
        { 
            get => _musclesFocuses; 
            set => Set(ref _musclesFocuses, value); 
        }

    }
}