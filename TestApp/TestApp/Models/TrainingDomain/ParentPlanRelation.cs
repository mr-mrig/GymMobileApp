using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class ParentPlanRelation : BaseModel
    {

        #region Backing fields
        private uint? _parentId;
        private string _parentName;
        private uint? _relationTypeId;
        #endregion


        public uint? ParentId 
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }
        public string ParentName 
        { 
            get => _parentName; 
            set => Set(ref _parentName, value); 
        }
        public uint? RelationTypeId 
        {
            get => _relationTypeId; 
            set => Set(ref _relationTypeId, value); 
        }
    }
}