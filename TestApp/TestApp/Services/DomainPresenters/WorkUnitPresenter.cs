using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;

namespace TestApp.Services.DomainPresenters
{
    public class WorkUnitPresenter : ITrainingPresenter
    {

        public const string DefaultSeparator = ", ";
        public const string DefaultSerieSeparatorString = " x ";


        /// <summary>
        /// Chars used as separators between elements - <seealso cref="DefaultSeparator"/> if not specified
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// The Work Unit to be displayed
        /// </summary>
        public WorkUnitTemplate WorkUnit { get; set; }




        public WorkUnitPresenter(WorkUnitTemplate workUnit)
        {
            WorkUnit = workUnit ?? new WorkUnitTemplate();
            Separator = DefaultSeparator;

            if (WorkUnit.WorkingSets == null)
                WorkUnit.WorkingSets = new List<WorkingSetTemplate>();
        }


        public WorkUnitPresenter() : this(null) { }

        

        #region IWorkUnitPresenter Implementaiton
        public virtual string ToFormattedEffort()
        {
            IEnumerable<TrainingEffort> efforts = WorkUnit.WorkingSets.Select(x => x.Effort);
            IEnumerable<TrainingEffortType> effortTypes = efforts.Where(x => x != null).Select(x => x.EffortType);

            if (effortTypes.Count() != efforts.Count() || !effortTypes.HasOnlyOneDistinctValue())
                return BasicTrainingPresenterService.DefaultEmptyString;

            List<float> sortedEfforts = efforts
                .Select(x => x.Effort.Value)
                .OrderBy(x => x).ToList();

            return BasicTrainingPresenterService.GetFormattedRange(sortedEfforts) + effortTypes.First().ToFormattedString();
        }

        public virtual string ToFormattedIntensityTechniques()
        {
            //RIGM: should we add Work Unit techniques?
            IEnumerable<IntensityTechnique> techniques = WorkUnit.WorkingSets
                .Where(x => x.IntensityTechniques?.DefaultIfEmpty() != null)
                .SelectMany(x => x.IntensityTechniques);

            return BasicTrainingPresenterService.GetDisplayeIntensityTechniques(techniques);
        }

        /// <summary>
        /// <para>String representation of the whole Work Unit as a set of Working Sets by following the usual training conventions</para>
        /// <para>IE: "3x10"; "10,8,6"; "3x10+8+6"; "10+10,8+8"; etc. </para>
        /// </summary>
        /// <returns>The formatted WS representaion as a string</returns>
        public virtual string ToFormattedRepetitions()
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
                return BasicTrainingPresenterService.DefaultEmptyString;

            for (int iws = 0; iws < wsCount; iws++)
            {
                current = workingSets[iws];

                // If any WS has not been specified, then mark all of them as unset
                if (!current.Repetitions.HasValue)
                    return BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

                if (current.IsDropset())
                {
                    dropsets.Add(current.Repetitions.Value);
                    formattedText += BasicTrainingPresenterService.DefaultDropsetSeparator;
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

                formattedText += BasicTrainingPresenterService.GetDisplayedRepetitions(current);
            }

            if (allEqual)
            {
                float dropsetsPerWs = (float)dropsets.Count / (float)nonDropsets.Count();   // No chance of division-by-zero here

                // If any dropset, we must check them as well before asserting that all the sets are equal
                if (dropsetsPerWs > 0)
                {
                    if (Utilities.IsDecimal(dropsetsPerWs))     // This implies they are not equal
                        return formattedText;                   // 10+8,20+6; 10+10, 10+10+10 etc.
                    else
                    {
                        int iCluster = 0;
                        do
                        {
                            if (!dropsets.GroupedSequenceEqual((int)dropsetsPerWs))
                                return formattedText;
                        }
                        while (++iCluster < dropsetsPerWs);

                        return nonDropsets.Count().ToString() + DefaultSerieSeparatorString +
                            BasicTrainingPresenterService.GetDisplayedClusterdRepetitions(workingSets.Where((_, i) => i <= dropsetsPerWs).ToList());    // 4x10+8 etc.
                    }
                }
                return nonDropsets.Count().ToString() + DefaultSerieSeparatorString +
                    BasicTrainingPresenterService.GetDisplayedRepetitions(current);        // 3x10; 5x6; etc.
            }
            else
                return formattedText;       // 5,3,1; 10,10,8; etc.
        }

        public virtual string ToFormattedRest()
        {
            List<WorkingSetTemplate> sortedByRest = WorkUnit.WorkingSets
                .Where(x => x.Rest.HasValue && x.Rest > 0)
                .OrderBy(x => x.Rest)
                .ToList();

            if (sortedByRest.Count == 0)
                return BasicTrainingPresenterService.DefaultEmptyString;

            return BasicTrainingPresenterService.GetFormattedRange(
                BasicTrainingPresenterService.GetDisplayedRest(sortedByRest.First()),
                BasicTrainingPresenterService.GetDisplayedRest(sortedByRest.Last()));
        }

        public string ToFormattedTempo() => BasicTrainingPresenterService.GetDisplayedTempo(WorkUnit.WorkingSets);

        /// <summary>
        /// Parses the input text into the <see cref="WorkUnit"/> class property. 
        /// Please notice that the rest, tempo and effort must have one value only, while the reptitions can be group more WS into the compact form (IE: '3x10+8')
        /// Thus, valid input examples are: "10" - "90''" - "3030", "10 RM" or "4x10+6" - "max" - "1010" - "60%"
        /// </summary>
        /// <param name="formattedRepetitions">The formatted text representing the working sets repetitions.</param>
        /// <param name="formattedRest">The formatted text representing the working sets rest.</param>
        /// <param name="formattedTempo">The formatted text representing the working sets lifting tempo.</param>
        /// <param name="formattedEffort">The formatted text representing the working sets tempo.</param>
        /// <param name="intensityTechniques">The string with the comma-spearated intensity techniques list</param>
        public virtual void ToModel(string formattedRepetitions, string formattedRest, string formattedTempo, string formattedEffort, string intensityTechniques)
        {
            int progressiveNumber = 0;
            int workingSetsCounter;
            string workingSetString;

            WorkUnit = new WorkUnitTemplate { WorkingSets = new List<WorkingSetTemplate>() };

            if (formattedRepetitions.Contains(DefaultSerieSeparatorString))
            {
                // The input is something like '3x10', '3x10+8' etc.
                int wsStartingIndex = formattedRepetitions.IndexOf(DefaultSerieSeparatorString);
                workingSetsCounter = int.Parse(formattedRepetitions.Substring(0, wsStartingIndex));
                workingSetString = formattedRepetitions.Substring(wsStartingIndex + DefaultSerieSeparatorString.Length);
            }
            else
            {
                // The input is something like '8', '8,6,4', '10+10,8+8' etc.
                workingSetString = formattedRepetitions;
                workingSetsCounter = 1;
            }

            for(int i = 0; i < workingSetsCounter; i++)
            {
                foreach(string chunkString in workingSetString.Split(DefaultSeparator.Trim().ToCharArray()))
                {
                    IEnumerable<WorkingSetTemplate> clustered = BasicTrainingPresenterService.ParseFormattedRepetitions(chunkString, progressiveNumber);

                    foreach (WorkingSetTemplate ws in clustered)
                    {
                        ws.Rest = 0;
                        ws.LiftingTempo = BasicTrainingPresenterService.ParseFormattedTempo(formattedTempo);
                        ws.Effort = BasicTrainingPresenterService.ParseFormattedEffort(formattedEffort);
                        ws.IntensityTechniques = new ObservableCollection<IntensityTechnique>(BasicTrainingPresenterService.ParseIntensityTechniques(intensityTechniques));
                        WorkUnit.WorkingSets.Add(ws);
                    }
                    WorkUnit.WorkingSets.Last().Rest = BasicTrainingPresenterService.ParseFormattedRest(formattedRest);     // Clustered sets rest affects only the last WS

                    progressiveNumber += clustered.Count();
                }
            }
        }
        #endregion

    }
}
