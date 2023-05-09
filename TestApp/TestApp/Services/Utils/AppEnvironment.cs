using System;
using System.Collections.Generic;
using System.Linq;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;

namespace TestApp.Services.Utils
{


    //RIGM: todo
    [Obsolete("Use domain when integrating the project")]
    public static class AppEnvironment
    {

        public enum BackendCommunicationLatency
        {
            /// <summary>
            /// Immediate backend response
            /// </summary>
            Instantaneous = 0,
            /// <summary>
            /// Almost immediate backend response
            /// </summary>
            Low = 50,
            /// <summary>
            /// Mild delay in the backend response, hardly perceivable by the user
            /// </summary>
            Normal = 100,
            /// <summary>
            /// Substantial delay in the backend response, the user is aware of that
            /// </summary>
            High = 500,
            /// <summary>
            /// Excessive delay in the backend response (1 second)
            /// </summary>
            VeryHigh = 1000,
        }


        public const int TrainingPlanVariant = 1;
        public const int TrainingPlanReceived = 2;
        public const int HashtagsMaxLength = 30;
        public const int TrainingHashtagsMaxNumber = 10;
        public const int WsAmrapValue = -1;
        public const int WsFullRestValue = int.MaxValue;
        public const int LiftingTempoStringLength = 4;
        //public const int WsRestNotSetValue = -1;

        // To be used on views
        public readonly static IntensityTechnique DefaultLinkingIntensityTechnique = NativeIntensityTechniques.Find("SS");
        public readonly static string TrainingHashtagsMaxNumberString = TrainingHashtagsMaxNumber.ToString();
        public readonly static string HashtagsMaxLengthString = HashtagsMaxLength.ToString();
        public readonly static uint DropsetId = NativeIntensityTechniques.Find("DS").Id.Value;
        public readonly static uint DefaultLinkingIntensityTechniqueId = DefaultLinkingIntensityTechnique.Id.Value;


        
        //RIGM: The following might store the data fetched from the DB, populated at startup...
        public static IEnumerable<ISimpleTagElement> TrainingProficiencies => NativeProficiencies.GetAll();
        public static IEnumerable<ISimpleTagElement> TrainingPhases => NativePhases.GetAll();
        public static IEnumerable<TrainingEffortType> TrainingEfforts => Efforts.GetAll();
        public static IEnumerable<IntensityTechnique> IntensityTechniques => NativeIntensityTechniques.GetAll();





        #region Hiding DB enumerations...
        public static class NativeProficiencies
        {

            public static TrainingProficiency Beginner = new TrainingProficiency { Id = 1, Body = "Beginner" };
            public static TrainingProficiency Intermediate = new TrainingProficiency { Id = 2, Body = "Intermediate" };
            public static TrainingProficiency Advanced = new TrainingProficiency { Id = 3, Body = "Advanced" };
            public static TrainingProficiency Pro = new TrainingProficiency { Id = 4, Body = "Pro" };

            public static IEnumerable<TrainingProficiency> GetAll() => new List<TrainingProficiency> { Beginner, Intermediate, Advanced, Pro };
        }

        public static class NativePhases
        {

            public static TrainingPhase Conditioning = new TrainingPhase { Id = 1, Body = "Conditioning" };
            public static TrainingPhase Recomp = new TrainingPhase { Id = 2, Body = "Recomp" };
            public static TrainingPhase Cut = new TrainingPhase { Id = 3, Body = "Cut" };
            public static TrainingPhase Bulk = new TrainingPhase { Id = 4, Body = "Bulk" };
            public static TrainingPhase Strength = new TrainingPhase { Id = 5, Body = "Strength" };
            public static TrainingPhase Peak = new TrainingPhase { Id = 6, Body = "Peak" };

            public static IEnumerable<TrainingPhase> GetAll() => new List<TrainingPhase> { Conditioning, Recomp, Cut, Bulk, Strength, Peak, };
        }
        public static class Efforts
        {

            public static TrainingEffortType IntensityPercentage = new TrainingEffortType { Id = 1, Name = "Intensity Percentage", Abbreviation = "%" };
            public static TrainingEffortType RM = new TrainingEffortType { Id = 2, Name = "Repetitions Max", Abbreviation = "RM" };
            public static TrainingEffortType PRE = new TrainingEffortType { Id = 3, Name = "RPE", Abbreviation = "RPE" };
            
            public static IEnumerable<TrainingEffortType> GetAll() => new List<TrainingEffortType> { IntensityPercentage, RM, PRE, };
            public static TrainingEffortType Find(int id) => GetAll().Single(x => x.Id == id);
            public static TrainingEffortType Find(uint id) => GetAll().Single(x => x.Id == id);
            public static TrainingEffortType Find(string abbreviation) => GetAll().Single(x => x.Abbreviation == abbreviation);
        }

        public static class NativeIntensityTechniques
        {
            public static IntensityTechnique RP = new IntensityTechnique { Id = 1, Name = "Rest pause", Abbreviation = "RP", IsLinking = true, };
            public static IntensityTechnique DS = new IntensityTechnique { Id = 2, Name = "Drop set", Abbreviation = "DS", IsLinking = true, };
            public static IntensityTechnique SS = new IntensityTechnique { Id = 3, Name = "Superset", Abbreviation = "SS", IsLinking = true, };
            public static IntensityTechnique Slow = new IntensityTechnique { Id = 4, Name = "Super slow", Abbreviation = "Slow", IsLinking = false, };
            public static IntensityTechnique Neg = new IntensityTechnique { Id = 4, Name = "Negatives", Abbreviation = "Neg", IsLinking = false, };

            public static IEnumerable<IntensityTechnique> GetAll() => new List<IntensityTechnique> { RP, DS, SS, Slow, Neg };

            public static IntensityTechnique Find(string abbreviation) => GetAll().Single(x => x.Abbreviation == abbreviation);
            public static IntensityTechnique FindOrDefault(string abbreviation) => GetAll().SingleOrDefault(x => x.Abbreviation == abbreviation);
            public static IntensityTechnique Find(int id) => GetAll().Single(x => x.Id == id);
            public static IntensityTechnique Find(uint id) => GetAll().Single(x => x.Id == id);
        }

        #endregion

        public static class WeekTypesIds
        {
            public static uint Rest = 1;
            public static uint Generic = 2;
            public static uint Peak = 3;
            public static uint Overreach = 4;

            public static IEnumerable<uint> GetAll() => new List<uint> { Rest, Generic, Peak, Overreach };
        }
    }
}
