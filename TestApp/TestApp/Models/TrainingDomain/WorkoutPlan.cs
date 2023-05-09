using System.Collections.Generic;

namespace TestApp.Models.TrainingDomain
{
    public class WorkoutPlan
    {

        public string WorkoutName { get; set; }

        public IList<ScheduledWorkout> PlannedWorkouts { get; set; }


        public WorkoutPlan(string workoutName)
        {
            WorkoutName = workoutName;
            PlannedWorkouts = new List<ScheduledWorkout>();
        }


        /// <summary>
        /// Add a workout to the plan.
        /// </summary>
        /// <param name="workoutId">The workout ID</param>
        /// <param name="weekId">The week Id</param>
        /// <param name="weekProgressiveNumber">The week progressive number</param>
        public void ScheduleWorkout(uint workoutId, uint weekId, uint weekProgressiveNumber)
            => PlannedWorkouts.Add(new ScheduledWorkout() { Id = workoutId, TrainingWeekId = weekId, WeekProgressiveNumber = weekProgressiveNumber, });

        /// <summary>
        /// Mark the workout as not planned over the specific training week.
        /// </summary>
        /// <param name="weekId">The week Id</param>
        /// <param name="weekProgressiveNumber">The week progressive number</param>
        public void WorkoutNotPlanned(uint weekId, uint weekProgressiveNumber)
            => PlannedWorkouts.Add(ScheduledWorkout.WorkoutNotPlanned(weekId, weekProgressiveNumber));
    }

}
