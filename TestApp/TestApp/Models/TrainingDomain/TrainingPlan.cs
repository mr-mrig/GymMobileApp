using System.Collections.Generic;
using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingPlan : BaseEntityModel
    {

        #region Backing fields
        private TrainingPlanDetails _details;
        private IList<TrainingWeek> _trainingWeeks;
        #endregion



        public TrainingPlanDetails Details 
        {
            get => _details;
            set => Set(ref _details, value);
        }

        public IList<TrainingWeek> TrainingWeeks 
        { 
            get => _trainingWeeks;
            set => Set(ref _trainingWeeks, value);
        }

        public object Schedules { get; set; }


    }
}
