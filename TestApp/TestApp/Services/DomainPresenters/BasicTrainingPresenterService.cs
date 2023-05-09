using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;

namespace TestApp.Services.DomainPresenters
{
    public static class BasicTrainingPresenterService
    {

        public const string DefaultDropsetSeparator = "+";
        public const string DefaultAmrapString = "max";
        public const string DefaultFullRestString = "max";
        public const string DefaultRestMeasUnitString = "''";
        public const string DefaultRepetitionsUnsetString = "";
        public const string DefaultEmptyString = "";
        public const string DefaultRangeSeparatorString = " - ";
        public const string DefaultIntensityTechniquesSeparatorString = ", ";


        /// <summary>
        /// Get the repetitions from the string. This is intended to be used on a repetitions chunk, namely a single working set unit - so clustered repetitions should be handled by the caller.
        /// </summary>
        /// <param name="displayedRepetitions">The formatted string describing the repetions chunk</param>
        /// <returns>The value of the repetitions ready to bhe used inside a <see cref="WorkingSetTemplate"/></returns>
        public static int? ParseDisplayedRepetitions(string displayedRepetitions)
        {
            if (displayedRepetitions.Contains(DefaultAmrapString))
                return AppEnvironment.WsAmrapValue;

            if (string.IsNullOrWhiteSpace(displayedRepetitions) || displayedRepetitions == DefaultRepetitionsUnsetString)
                return null;

            return int.Parse(displayedRepetitions);
        }

        /// <summary>
        /// Get the descriptive string of the specified working set
        /// </summary>
        /// <param name="workingSet">The working set</param>
        /// <returns>The string to be displayed</returns>
        public static string GetDisplayedRepetitions(WorkingSetTemplate workingSet)
        {
            if (workingSet.IsAmrap())
                return DefaultAmrapString;

            if (workingSet.NotSpecified())
                return DefaultRepetitionsUnsetString;

            return workingSet.Repetitions.ToString();
        }


        /// <summary>
        /// Get the descriptive string of the specified working set
        /// </summary>
        /// <param name="clusteredSets">The working set</param>
        /// <returns>The string to be displayed</returns>
        public static string GetDisplayedClusterdRepetitions(IList<WorkingSetTemplate> clusteredSets)
        {
            string formattedText = GetDisplayedRepetitions(clusteredSets.First());

            for (int iws = 1; iws < clusteredSets.Count; iws++)     // Skip the first one
                formattedText += DefaultDropsetSeparator + GetDisplayedRepetitions(clusteredSets[iws]);

            return formattedText;
        }

        /// <summary>
        /// Get the ready-to-display lifting tempo of the specified WSs, provided they all have the same tempo, otherwise leave it blank.
        /// </summary>
        /// <param name="workingSets">The working sets</param>
        /// <returns>The string to be displayed</returns>
        public static string GetDisplayedTempo(IEnumerable<WorkingSetTemplate> workingSets)
        {
            IEnumerable<string> tempos = workingSets.Select(x => x.LiftingTempo);

            if (tempos.Count() == 0 || tempos.Any(x => string.IsNullOrEmpty(x)))
                return DefaultEmptyString;

            if (tempos.HasOnlyOneDistinctValue())
                return tempos.First();

            return DefaultEmptyString;
        }

        /// <summary>
        /// Get the ready-to-display rest of the specified Working Set
        /// </summary>
        /// <param name="workingSet">The working set</param>
        /// <returns>The string representation of the rest</returns>
        public static string GetDisplayedRest(WorkingSetTemplate workingSet)
        {
            if (!workingSet.Rest.HasValue)
                return DefaultEmptyString;

            if (workingSet.IsFullRest())
                return DefaultFullRestString;

            return workingSet.Rest.ToString() + DefaultRestMeasUnitString;
        }

        /// <summary>
        /// Get the ready-to-display list of intensity techniques
        /// </summary>
        /// <param name="intensityTechniques">The intensity techniques</param>
        /// <returns>The string representation of the intensity techniques</returns>
        public static string GetDisplayeIntensityTechniques(IEnumerable<IntensityTechnique> intensityTechniques)
        {
            IEnumerable<string> abbreviations = intensityTechniques?.Select(x => x.Abbreviation) ?? new List<string>() { string.Empty, };

            return string.Join(DefaultIntensityTechniquesSeparatorString, abbreviations.Distinct());
        }

        /// <summary>
        /// Format as 'value' or 'minValue - maxValue'
        /// </summary>
        /// <param name="sortedList">The list storing the values to be displayed</param>
        /// <returns>The formatted string</returns>
        public static string GetFormattedRange<T>(IEnumerable<T> sortedList) where T : IEquatable<T>
        {
            if (sortedList.Count() == 0)
                return DefaultEmptyString;

            //if (sortedList.First() == sortedList.Last())
            if (sortedList.First().Equals(sortedList.Last()))
                return sortedList.First().ToString();

            return sortedList.First() + DefaultRangeSeparatorString + sortedList.Last();
        }

        /// <summary>
        /// Format as 'value' or 'minValue - maxValue' when the edge values are already ready-to-display
        /// </summary>
        /// <param name="minValue">The minimum value to be displayed</param>
        /// <param name="maxValue">The maximum value to be displayed</param>
        /// <returns>The formatted string</returns>
        public static string GetFormattedRange(string minValue, string maxValue)
        {
            if (minValue == maxValue)
                return minValue;

            return minValue + DefaultRangeSeparatorString + maxValue;
        }


        /// <summary>
        /// Maps the input text to a <see cref="WorkingSetTemplate"/> instance. 
        /// Only repetitions describing string are supported so far, IE: "10", "10+8+6" etc.
        /// For this reason, only the <see cref="WorkingSetTemplate.Repetitions"/> property and <see cref="WorkingSetTemplate.ProgressiveNumber"/> will be set.
        /// </summary>
        /// <param name="formattedRepetitions">The formatted text representing the WS.</param>
        /// <param name="startingProgressiveNumber">The WS progressive number which to start counting from</param>
        /// <returns>The WorkingSet instance with only Repetitions set -if possible, otherwise all the properties will be left null</returns>
        public static IEnumerable<WorkingSetTemplate> ParseFormattedRepetitions(string formattedRepetitions, int startingProgressiveNumber = 0)
        {
            if (string.IsNullOrWhiteSpace(formattedRepetitions))
                return new List<WorkingSetTemplate> { new WorkingSetTemplate() };    // No repetitions planned

            uint progressiveNumber = (uint)startingProgressiveNumber;

            if (formattedRepetitions.Contains(DefaultDropsetSeparator.Trim()))
            {
                List<WorkingSetTemplate> clusteredSets = new List<WorkingSetTemplate>();

                // A chunk is a single set inside a clustered WS
                string[] chunks = formattedRepetitions.Split(DefaultDropsetSeparator.Trim().ToCharArray());

                clusteredSets.Add(new WorkingSetTemplate() { Repetitions = ParseDisplayedRepetitions(chunks[0]), ProgressiveNumber = progressiveNumber++, });
                
                foreach (string chunk in chunks.Skip(1))
                {
                    clusteredSets.Add(new WorkingSetTemplate()
                    {
                        Repetitions = ParseDisplayedRepetitions(chunk),
                        IntensityTechniques = new ObservableCollection<IntensityTechnique> { Utils.AppEnvironment.NativeIntensityTechniques.Find("DS") },
                        ProgressiveNumber = progressiveNumber,
                    });
                }
                return clusteredSets;
            }

            return new List<WorkingSetTemplate> { new WorkingSetTemplate
            {
                Repetitions = ParseDisplayedRepetitions(formattedRepetitions),
                ProgressiveNumber = progressiveNumber,
            }};
        }


        public static int ParseFormattedRest(string formattedRest)
        {
            if (formattedRest == DefaultEmptyString)
                return AppEnvironment.WsFullRestValue;

            string parsedInput = Regex.Replace(Regex.Replace(formattedRest, DefaultRestMeasUnitString, ""),
                DefaultFullRestString, AppEnvironment.WsFullRestValue.ToString());

            int val = int.Parse(parsedInput);

            if (val != AppEnvironment.WsFullRestValue)
            {
                if (val < 0)
                    throw new FormatException($"The rest value must be non negative - {formattedRest}");
            }
            return val;
        }

        internal static TrainingEffort ParseFormattedEffort(string formattedEffort)
        {
            if (formattedEffort == DefaultEmptyString)
                return null;

            string[] parsedInput = formattedEffort.Split(" ".ToCharArray());                    //RIGM: we must ensure the space is added... The special keyboard should ensure this
            TrainingEffortType effortType = AppEnvironment.Efforts.Find(parsedInput.Last());

            return new TrainingEffort(float.Parse(parsedInput.First()), effortType.Id);
        }

        internal static string ParseFormattedTempo(string formattedTempo)
        {
            if (formattedTempo == DefaultEmptyString)
                return string.Empty;

            if (formattedTempo.Length != AppEnvironment.LiftingTempoStringLength ||
                Regex.IsMatch(formattedTempo, "/^[0-9x]", RegexOptions.IgnoreCase))
                    throw new FormatException($"The input cadence is invalid: {formattedTempo}");

            return formattedTempo;
        }
        internal static IEnumerable<IntensityTechnique> ParseIntensityTechniques(string intensityTechniques)
        {
            List<IntensityTechnique> parsed = new List<IntensityTechnique>();

            if (intensityTechniques == DefaultEmptyString)
                return parsed;

            IEnumerable<string> chunks = intensityTechniques.Split(DefaultIntensityTechniquesSeparatorString.Trim().ToCharArray()).Select(str => str.Trim());

            foreach(string itString in chunks)
            {
                IntensityTechnique it = AppEnvironment.NativeIntensityTechniques.FindOrDefault(itString) ??
                    new IntensityTechnique
                    {
                        Id = null,
                        Abbreviation = itString,
                    };
                parsed.Add(it);
            }
            return parsed;
        }

    }
}
