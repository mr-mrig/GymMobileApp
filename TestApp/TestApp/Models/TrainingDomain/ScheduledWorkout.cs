namespace TestApp.Models.TrainingDomain
{
    public class ScheduledWorkout
    {

        public uint? Id { get; set; }
        public uint TrainingWeekId { get; set; }
        public uint WeekProgressiveNumber { get; set; }


        #region Do we need these?
        //public uint? WeekTypeId { get; set; }
        //public int? WeekdayId { get; set; }
        //public uint? ProgressiveNumber { get; set; }
        #endregion



        /// <summary>
        /// Factory method for creating workouts which have not been planned in a specific week.
        /// </summary>
        /// <param name="workoutName">The workout name</param>
        /// <param name="weekId">The week Id</param>
        /// <param name="weekProgressiveNumber">The week progressive number</param>
        /// <returns></returns>
        public static ScheduledWorkout WorkoutNotPlanned(uint weekId, uint weekProgressiveNumber)
            => new ScheduledWorkout
            {
                Id = null,
                TrainingWeekId = weekId,
                WeekProgressiveNumber = weekProgressiveNumber,
            };
    }
}
