using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;
using TestApp.ViewModels;
using TestApp.ViewModels.Base;

namespace TestApp.ViewModels
{
    public class WorkUnitViewModel : BaseViewModel
    {

        public const string DefaultSeparator = ", ";
        public const string DefaultDropsetSeparator = "+";
        public const string DefaultAmrapString = "max";
        public const string DefaultUnsetString = "...";
        public const string DefaultEmptyString = "";
        public const string DefaultRangeSeparatorString = " - ";
        public const string DefaultSerieSeparatorString = " x ";


        /// <summary>
        /// Chars used as separators between elements - <seealso cref="DefaultSeparator"/> if not specified
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// The Work Unit model to be displayed
        /// </summary>
        public WorkUnitTemplate WorkUnit { get; set; }

        /// <summary>
        /// The formatted representation of the Working Sets
        /// </summary>
        public ObservableCollection<WorkingSetViewModel> WorkingSets { get; set; }




        public WorkUnitViewModel(WorkUnitTemplate workUnit)
        {
            WorkUnit = workUnit;
            Separator = DefaultSeparator;
        }

        public WorkUnitViewModel() : this(null) { }


        #region Private Helpers

        /// <summary>
        /// Get the descriptive string of the specified repetitions
        /// </summary>
        /// <param name="repetitions">The repetitions value</param>
        /// <returns>The string to be displayed</returns>
        private string GetDisplayedRepetitions(int? repetitions)
        {
            if (repetitions == Services.Utils.AppEnvironment.WsAmrapValue)
                return DefaultAmrapString;

            return repetitions.ToString();
        }

        /// <summary>
        /// Format as 'value' or 'minValue - maxValue'
        /// </summary>
        /// <param name="sortedList">The list storing the values to be displayed</param>
        /// <returns>The formatted string</returns>
        private string GetFormattedRange<T>(IEnumerable<T> sortedList) where T : IEquatable<T>
        {
            if (sortedList.Count() == 0)
                return DefaultEmptyString;

            //if (sortedList.First() == sortedList.Last())
            if (sortedList.First().Equals(sortedList.Last()))
                return sortedList.First().ToString();

            return sortedList.First() + DefaultRangeSeparatorString + sortedList.Last();
        }
        #endregion


        #region Private Formatters
        public string ToFormattedEffort()
        {
            IEnumerable<TrainingEffort> efforts = WorkUnit.WorkingSets.Select(x => x.Effort);

            if (efforts.Count() == 0)
                return DefaultEmptyString;

            IEnumerable<TrainingEffortType> effortTypes = efforts.Where(x => x != null).Select(x => x.EffortType);

            if (effortTypes.Count() != efforts.Count() || !effortTypes.HasOnlyOneDistinctValue())
                return DefaultUnsetString;

            List<float> sortedEfforts = efforts
                .Select(x => x.Effort.Value)
                .OrderBy(x => x).ToList();

            return GetFormattedRange(sortedEfforts) + effortTypes.First().ToFormattedString();
        }

        public string ToFormattedIntensityTechniques()
        {
            IEnumerable<WorkingSetTemplate> workingSets = WorkUnit.WorkingSets;

            //RIGM: should we add Work Unit techniques?
            IEnumerable<string> itAbbreviations = workingSets.Where(x => x.IntensityTechniques?.DefaultIfEmpty() != null)
                .SelectMany(x => x.IntensityTechniques)
                ?.Select(x => x.Abbreviation) ?? new List<string>() { string.Empty, };

            return string.Join(Separator, itAbbreviations.Distinct());
        }

        public string ToFormattedRepetitions()
        {
            List<WorkingSetTemplate> workingSets = WorkUnit.WorkingSets.ToList();
            bool allEqual = true;
            int wsCount = workingSets.Count;
            WorkingSetTemplate current = null;
            string formattedText = string.Empty;

            List<int> dropsets = new List<int>();
            List<int> nonDropsets = new List<int>();
            int lastNonDropsetIndex = 0;        // A Work Unit cannot start with a DS

            if (wsCount == 0)
                return DefaultEmptyString;

            for (int iws = 0; iws < wsCount; iws++)
            {
                current = workingSets[iws];

                // If any WS has not been specified, then mark all of them as unset
                if (!current.Repetitions.HasValue)
                    return DefaultUnsetString;

                if (current.IsDropset())
                {
                    dropsets.Add(current.Repetitions.Value);
                    formattedText += DefaultDropsetSeparator;
                }
                else if (iws > 0)
                {
                    allEqual = allEqual && current.Repetitions == workingSets[lastNonDropsetIndex].Repetitions;
                    lastNonDropsetIndex = iws;
                    nonDropsets.Add(current.Repetitions.Value);

                    formattedText += Separator;
                }
                else
                    nonDropsets.Add(workingSets.First().Repetitions.Value);

                formattedText += current.IsAmrap() ? DefaultAmrapString : GetDisplayedRepetitions(current.Repetitions);
            }

            if (allEqual)
            {
                float dropsetsPerWs = (float)dropsets.Count / (float)nonDropsets.Count();   // No chance of division-by-zero here

                // If any dropset, we must check them as well before asserting that all the sets are equal
                if (dropsetsPerWs > 0)
                {
                    if (Utilities.IsDecimal(dropsetsPerWs))
                        return formattedText;
                    else
                    {
                        string dsString = string.Empty;

                        for (int iDs = 0; iDs < dropsets.Count - dropsetsPerWs; iDs++)
                        {
                            int index = iDs + (int)dropsetsPerWs;

                            if (index < dropsets.Count && dropsets[iDs] != dropsets[index])
                                return formattedText;

                            if (iDs < dropsetsPerWs)
                                dsString += dropsets[iDs].ToString() + DefaultDropsetSeparator;
                        }
                        return nonDropsets.Count().ToString() + DefaultSerieSeparatorString
                            + nonDropsets[0].ToString() + DefaultDropsetSeparator + dsString.TrimEnd(DefaultDropsetSeparator.ToCharArray());   // 3x12+10+8
                    }
                }
                else
                    // No dropsets -> all WS are equal
                    return wsCount.ToString() + DefaultSerieSeparatorString + GetDisplayedRepetitions(current.Repetitions);     // 3x10; 4x8 etc.
            }
            else
                return formattedText;       // 5,3,1; 10+8+6,10+8+10 etc.
        }

        public string ToFormattedRest()
        {
            IEnumerable<WorkingSetTemplate> workingSets = WorkUnit.WorkingSets;

            if (workingSets.Count() == 0)
                return DefaultEmptyString;

            List<int> sortedRest = workingSets.Select(x => x.Rest ?? -1)
                .OrderBy(x => x)
                .Where(x => x > -1).ToList();

            return GetFormattedRange(sortedRest);
        }

        public string ToFormattedTempo()
        {
            IEnumerable<string> tempos = WorkUnit.WorkingSets.Select(x => x.LiftingTempo);

            if (tempos.Count() == 0)
                return DefaultEmptyString;

            if (tempos.HasOnlyOneDistinctValue())
                return tempos.First();

            return DefaultUnsetString;
        }
        #endregion


        #region IWorkUnitPresenter Implementaiton

        public void ToModel()
        {
            throw new NotImplementedException();
        }


        public void ToFormatted()
        {
            WorkingSets = new ObservableCollection<WorkingSetViewModel>
                {
                    new WorkingSetViewModel
                    {
                        ProgressiveNumber = 0,
                        FormattedRepetitions = ToFormattedRepetitions(),
                        Rest = ToFormattedRest(),
                        LiftingTempo = ToFormattedTempo(),
                        Effort = ToFormattedEffort(),
                        IntensityTechniques = ToFormattedIntensityTechniques(),
                    }
                };
        }

        #endregion

    }
}
