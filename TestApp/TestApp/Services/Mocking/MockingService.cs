using System.Collections.Generic;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;

namespace TestApp.Services.Mocking
{
    public class MockingService
    {


        public const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";



        public static class ExcerciseLibrary
        {
            public static Excercise BenchPress = new Excercise { Id = 1, Name = "Bench press", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Chest.Id, };
            public static Excercise DbBenchPress = new Excercise { Id = 2, Name = "Bench press - DB", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Chest.Id, };
            public static Excercise CableFlyes = new Excercise { Id = 3, Name = "Cable flyes", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Chest.Id, };

            public static Excercise LateralRaises = new Excercise { Id = 4, Name = "Lateral raises", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Delts.Id, };
            public static Excercise Military = new Excercise { Id = 5, Name = "Military press", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Delts.Id, };
            public static Excercise Ohp = new Excercise { Id = 6, Name = "Over hand press - DB", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Delts.Id, };

            public static Excercise Squat = new Excercise { Id = 7, Name = "Squat", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Quads.Id, };
            public static Excercise WideGobletSquatSteps = new Excercise { Id = 8, Name = "Wide goblet squat from steps", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Quads.Id, };
            public static Excercise SingleLegExtensionsButtRaised = new Excercise { Id = 9, Name = "Single leg extensions - Butt raised", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Quads.Id, };

            public static Excercise Rdl = new Excercise { Id = 10, Name = "Romanian deadlift", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Hams.Id, };
            public static Excercise DbRdlDeficit = new Excercise { Id = 11, Name = "Romanian deadlift from deficit - DB", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Hams.Id, };
            public static Excercise SeatedLegCurl = new Excercise { Id = 12, Name = "Seated leg curl", Description = LoremIpsum, MuscleGroupId = MuscleLibrary.Hams.Id, };


            public static IEnumerable<Excercise> GetAll()
                => new List<Excercise>
                {
                    BenchPress, DbBenchPress, CableFlyes, LateralRaises, Military, Ohp, Squat, WideGobletSquatSteps, SingleLegExtensionsButtRaised, Rdl, DbRdlDeficit, SeatedLegCurl,
                };
        }

        public static class MuscleLibrary
        {
            public static MuscleGroup Chest = new MuscleGroup { Id = 1, Name = "Chest", Abbreviation = "Chest", };
            public static MuscleGroup Delts = new MuscleGroup { Id = 2, Name = "Shoulders", Abbreviation = "Delts", };
            public static MuscleGroup Quads = new MuscleGroup { Id = 3, Name = "Quadriceps", Abbreviation = "Quads", };
            public static MuscleGroup Hams = new MuscleGroup { Id = 4, Name = "Hamstring", Abbreviation = "Hams", };
        }

        public static class IntensityTechniqueLibrary
        {
            public static IntensityTechnique RP = new IntensityTechnique { Id = 1, Name = "Rest pause", Abbreviation = "RP", IsLinking = true, };
            public static IntensityTechnique DS = new IntensityTechnique { Id = 2, Name = "Drop set", Abbreviation = "DS", IsLinking = true, };
            public static IntensityTechnique SS = new IntensityTechnique { Id = 3, Name = "Superset", Abbreviation = "SS", IsLinking = true, };
            public static IntensityTechnique Slow = new IntensityTechnique { Id = 4, Name = "Super slow", Abbreviation = "Slow", IsLinking = false, };
            public static IntensityTechnique Neg = new IntensityTechnique { Id = 5, Name = "Negatives", Abbreviation = "Neg", IsLinking = false, };

            public static IEnumerable<IntensityTechnique> GetAll()
                => new List<IntensityTechnique> { RP, DS, SS, Slow, Neg };
        }

        public static class EffortLibrary
        {

            public static TrainingEffortType IntensityPercentage = new TrainingEffortType { Id = 1, Name = "Intensity Percentage", Abbreviation = "%" };
            public static TrainingEffortType RM = new TrainingEffortType { Id = 2, Name = "Repetitions Max", Abbreviation = "RM" };
            public static TrainingEffortType RPE = new TrainingEffortType { Id = 3, Name = "RPE", Abbreviation = "RPE" };

            public static IEnumerable<TrainingEffortType> GetAll() => new List<TrainingEffortType> { IntensityPercentage, RM, RPE, };
        }

        public static class Proficiency
        {

            public static TrainingProficiency Beginner = new TrainingProficiency { Id = 1, Body = "Beginner" };
            public static TrainingProficiency Intermediate = new TrainingProficiency { Id = 2, Body = "Intermediate" };
            public static TrainingProficiency Advanced = new TrainingProficiency { Id = 3, Body = "Advanced" };
            public static TrainingProficiency Pro = new TrainingProficiency { Id = 4, Body = "Pro" };

            public static IEnumerable<TrainingProficiency> GetAll() => new List<TrainingProficiency> { Beginner, Intermediate, Advanced, Pro };
        }

        public static class Phase
        {

            public static TrainingPhase Conditioning = new TrainingPhase { Id = 1, Body = "Conditioning" };
            public static TrainingPhase Recomp = new TrainingPhase { Id = 2, Body = "Recomp" };
            public static TrainingPhase Cut = new TrainingPhase { Id = 3, Body = "Cut" };
            public static TrainingPhase Bulk = new TrainingPhase { Id = 4, Body = "Bulk" };
            public static TrainingPhase Strength = new TrainingPhase { Id = 5, Body = "Strength" };
            public static TrainingPhase Private1 = new TrainingPhase { Id = 6, Body = "Personal" };
            public static TrainingPhase Private2 = new TrainingPhase { Id = 7, Body = "AnotherPersonalPhase" };

            public static IEnumerable<TrainingPhase> GetAllPublic() => new List<TrainingPhase> { Conditioning, Recomp, Cut, Bulk, Strength };
            public static IEnumerable<TrainingPhase> GetAllPrivate() => new List<TrainingPhase> { Private1, Private2 };
        }

        public static class HashtagFactory
        {

            public static Hashtag Fitness = new Hashtag { Id = 1510, Body = "Fitness" };
            public static Hashtag Healthy = new Hashtag { Id = 1511, Body = "Healthy" };
            public static Hashtag Bulk = new Hashtag { Id = 1512, Body = "Bulk" };
            public static Hashtag VeryLong = new Hashtag { Id = 1513, Body = "ThisIsTheLongestHashtagYouCouldImagine" };
            public static Hashtag MaxLength = new Hashtag { Id = 1514, Body = "TheMaximumLengthMightBe30Chars" };
            public static Hashtag MediumLength = new Hashtag { Id = 1515, Body = "ThisIsMediumLength" };

            /// <summary>
            /// Build a hastag having the specified Id and "Hashtag#id#" as body
            /// </summary>
            /// <param name="id">The Id</param>
            /// <returns>The Hashtag mock object</returns>
            public static Hashtag BuildHashtag(uint id) => new Hashtag { Id = id, Body = "Hashtag" + id.ToString() };
        }
    }

}
