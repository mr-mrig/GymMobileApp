using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;

namespace TestApp.Services.TrainingDomain
{
    public class TrainingPlanService : ITrainingPlanService
    {

        #region Queries

        public async Task<IEnumerable<TrainingPlanSummary>> GetTrainingPlansSummariesAsync(uint userId)
        {
            throw new NotImplementedException();
        }        
        
        public async Task<IEnumerable<TrainingWeek>> GetWorkoutsPlanAsync(uint trainingPlanId)
        {
            throw new NotImplementedException();
        }

        public async Task<TrainingPlanDetails> GetTrainingPlanDetailsAsync(uint userTrainingPlanId)
        {
            throw new NotImplementedException();
        }

        public async Task<TrainingPlanDetails> GetTrainingPlanFullDetailsAsync(uint userTrainingPlanId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetTopRatedHashtagsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetFavouriteHashtagsAsync(uint userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetTopRatedPhasesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetFavouritePhasesAsync(uint userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<WorkoutTemplate>> GetWorkoutTemplatesAsync(IEnumerable<uint> workoutsIds)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<WorkoutTemplate>> GetWorkoutTemplatesByWeekAsync(IEnumerable<uint> weekIds, string workoutName)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Commands

        public async Task<bool> DeleteWorkUnitTemplates(IEnumerable<uint> workoutIds, uint workUnitProgressiveNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LinkToPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber, uint intensityTechniqueId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<uint>> AddTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToAdd)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<uint>> RemoveTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToRemove)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RenameTrainingPlanAsync(uint userTrainingPlanId, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement> hashtags)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement> hashtags)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetTrainingPlanBookmarkedAsync(uint userTrainingPlanId, bool isBookmarked)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UnlinkFromPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<uint>> SetWorkUnitTemplate(uint workoutId, WorkUnitTemplate workUnits)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<uint>> InsertIntensityTechniques(IEnumerable<IntensityTechnique> intensityTechniques)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
