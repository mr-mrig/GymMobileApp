using System.Collections.Generic;
using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingEffortType : BaseEnumeration
    {

        /// <summary>
        /// Minimum allowed RPE Value: all values below this will be saturated
        /// </summary>
        public const int MinimumRPE = 5;



        public string Name { get; set; }
        public string Abbreviation { get; set; }


        public static TrainingEffortType IntensityPercentage = new TrainingEffortType { Id = 1, Name = "Intensity Percentage", Abbreviation = "%" };
        public static TrainingEffortType RM = new TrainingEffortType { Id = 2, Name = "Repetitions Max", Abbreviation = "RM" };
        public static TrainingEffortType RPE = new TrainingEffortType { Id = 3, Name = "RPE", Abbreviation = "RPE" };

        public static IEnumerable<TrainingEffortType> GetAll() => new List<TrainingEffortType> { IntensityPercentage, RM, RPE, };


        public TrainingEffortType()
        {

        }

        public TrainingEffortType(uint id)
        {
            TrainingEffortType @this = null;

            if (id == IntensityPercentage.Id)
                @this = IntensityPercentage;

            else if (id == RM.Id)
                @this = RM;

            else if (id == RPE.Id)
                @this = RPE;

            Id = @this.Id;
            Name = @this.Name;
            Abbreviation = @this.Abbreviation;
        }

        public string ToFormattedString()
        {
            if (this == IntensityPercentage)
                return Abbreviation;
            else
                return " " + Abbreviation;
        }

    }
}
