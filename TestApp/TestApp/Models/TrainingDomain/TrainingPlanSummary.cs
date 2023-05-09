using System;
using System.Collections.Generic;
using System.Linq;
using TestApp.Models.Common;
using TestApp.Services.Mocking;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingPlanSummary
    {


        public uint PlanId { get; set; }
        public uint PlanUserLibraryId { get; set; }
        public uint OwnerId { get; set; }
        public string PlanName { get; set; }
        public bool IsBookmarked { get; set; }
        public uint? ParentPlanId { get; set; }
        public ICollection<Hashtag> Hashtags { get; set; }
        public ICollection<TrainingProficiency> TargetProficiencies { get; set; }
        public ICollection<TrainingPhase> TargetPhases { get; set; }
        public float? AvgWorkingSets { get; set; }
        public float? AvgIntensityPercentage { get; set; }
        public int? WeeksNumber { get; set; }
        public int? MinWorkoutsPerWeek { get; set; }
        public int? MaxWorkoutsPerWeek { get; set; }
        public DateTime? LastWorkoutTimestamp { get; set; }

        /// <summary>
        /// Get the descriptive string for the basic Training Plan summary info
        /// Should be done via multi-binding which is not supported by XF so far
        /// </summary>
        public string SummaryInfo
        {
            get
            {
                string weeksNumberString = string.Empty;
                string workoutsNumberString;

                if (WeeksNumber.HasValue)
                    weeksNumberString = $"{WeeksNumber.ToString()} Weeks,  ";

                if (!MinWorkoutsPerWeek.HasValue || !MaxWorkoutsPerWeek.HasValue)
                    return weeksNumberString;

                if(MinWorkoutsPerWeek == MaxWorkoutsPerWeek)
                    workoutsNumberString = $"{MinWorkoutsPerWeek.ToString()} Weekly Workouts";
                else
                    workoutsNumberString = $"{MinWorkoutsPerWeek.ToString()}-{MaxWorkoutsPerWeek.ToString()} Weekly Workouts";

                return weeksNumberString + workoutsNumberString;
            }
        }

        /// <summary>
        /// Get the descriptive string for the Training Plan targets, namely the proficiencies and the phases.
        /// Should be done via multi-binding which is not supported by XF so far
        /// If performances become an issue, please have a look to <see cref="SimplifiedTargetsInfo"/>
        /// </summary>
        public string TargetsInfo
        {
            get
            {
                string phasesString = string.Empty;
                string proficienciesString = string.Empty;

                if (TargetProficiencies.Count > 0)
                {
                    if (TargetProficiencies.Count == MockingService.Proficiency.GetAll().Count())
                        proficienciesString = "All Levels,  ";
                    else
                    {
                        IEnumerable<string> orderedProficiencies = TargetProficiencies.OrderBy(x => x.Id).Select(x => x.Body);
                        proficienciesString = string.Join(",  ", orderedProficiencies) + ",  ";
                    }
                }

                if (TargetPhases.Count > 0)
                    phasesString = string.Join(",  ", TargetPhases.Select(x => x.Body));

                return (proficienciesString + phasesString).TrimEnd(',');
            }
        }

        /// <summary>
        /// Get the descriptive string for the Training Plan targets, namely the proficiencies and the phases.
        /// This is the simplier version of <see cref="TargetsInfo"/>, as it just displays the targets as they are.
        /// </summary>
        public string SimplifiedTargetsInfo
        {
            get
            {
                string phasesString = string.Empty;
                string proficienciesString = string.Empty;

                if (TargetProficiencies.Count > 0)
                    proficienciesString = string.Join(",  ", TargetProficiencies.Select(x => x.Body)) + ",  ";

                if (TargetPhases.Count > 0)
                    phasesString = string.Join(",  ", TargetPhases.Select(x => x.Body));

                return (proficienciesString + phasesString).TrimEnd(',');
            }
        }
    }
}
