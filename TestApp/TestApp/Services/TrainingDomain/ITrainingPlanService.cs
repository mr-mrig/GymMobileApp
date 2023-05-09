using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;

namespace TestApp.Services.TrainingDomain
{
    public interface ITrainingPlanService
    {


        #region Queries

        Task<IEnumerable<TrainingPlanSummary>> GetTrainingPlansSummariesAsync(uint userId);
        Task<TrainingPlanDetails> GetTrainingPlanDetailsAsync(uint userTrainingPlanId);
        Task<TrainingPlanDetails> GetTrainingPlanFullDetailsAsync(uint userTrainingPlanId);
        Task<IEnumerable<TrainingWeek>> GetWorkoutsPlanAsync(uint trainingPlanId);
        Task<IEnumerable<ISimpleTagElement>> GetTopRatedHashtagsAsync();
        Task<IEnumerable<ISimpleTagElement>> GetFavouriteHashtagsAsync(uint userId);
        Task<IEnumerable<ISimpleTagElement>> GetTopRatedPhasesAsync();
        Task<IEnumerable<ISimpleTagElement>> GetFavouritePhasesAsync(uint userId);
        Task<IList<WorkoutTemplate>> GetWorkoutTemplatesAsync(IEnumerable<uint> workoutsIds);
        Task<IList<WorkoutTemplate>> GetWorkoutTemplatesByWeekAsync(IEnumerable<uint> weekIds, string workoutName);
        #endregion

        #region Commands

        //Task<bool> DeleteWorkUnitTemplate(uint workoutId, uint workUnitProgressiveNumber);
        Task<IEnumerable<uint>> SetWorkUnitTemplate(uint workoutId, WorkUnitTemplate workUnit);
        Task<bool> DeleteWorkUnitTemplates(IEnumerable<uint> workoutIds, uint workUnitProgressiveNumber);
        Task<bool> LinkToPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber, uint intensityTechniqueId);
        Task<bool> UnlinkFromPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber);
        Task<bool> SetTrainingPlanBookmarkedAsync(uint userTrainingPlanId, bool isBookmarked);
        Task<IEnumerable<uint>> AddTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToAdd);
        Task<IEnumerable<uint>> RemoveTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToRemove);
        Task<bool> RenameTrainingPlanAsync(uint userTrainingPlanId, string name);
        Task<bool> AddTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement>hashtags);
        Task<bool> RemoveTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement>hashtags);
        Task<bool> AddTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency);
        Task<bool> RemoveTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency);
        Task<bool> AddTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase);
        Task<bool> RemoveTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase);
        Task<IEnumerable<uint>> InsertIntensityTechniques(IEnumerable<IntensityTechnique> intensityTechniques);
        #endregion
    }
}
