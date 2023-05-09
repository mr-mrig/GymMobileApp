using System.Collections.Generic;
using System.Linq;
using TestApp.Models.TrainingDomain;

namespace TestApp.Services.DomainPresenters
{
    public class WorkingSetPresenter : WorkUnitPresenter
    {





        public WorkingSetPresenter(WorkUnitTemplate workUnit) : base(workUnit)
        {

        }


        public WorkingSetPresenter() : this(null) { }


        

        #region ITrainingPresenter Implementaiton

        /// <summary>
        /// <para>String representation of the single working set - or the set of clustered WSs</para>
        /// <para>IE: "10"; "10+8+6": etc.</para>
        /// </summary>
        /// <returns>The formatted WS representaion as a string</returns>
        public override string ToFormattedRepetitions()
        {
            List<WorkingSetTemplate> workingSets = WorkUnit.WorkingSets.ToList();
            int wsCount = workingSets.Count;

            if (wsCount == 0)
                return BasicTrainingPresenterService.DefaultEmptyString;

            // Single repetition
            if (wsCount == 1)
                return  BasicTrainingPresenterService.GetDisplayedRepetitions(workingSets.First());

            // Clustered repetitions
            return BasicTrainingPresenterService.GetDisplayedClusterdRepetitions(workingSets);
        }

        public override string ToFormattedRest() => BasicTrainingPresenterService.GetDisplayedRest(WorkUnit.WorkingSets.Last());

        /// <summary>
        /// Parses the input text into the <see cref="WorkUnit"/> class property. 
        /// Only repetitions describing string are supported so far, IE: "10", "10+8+6", "4x10+6" etc.
        /// </summary>
        /// <param name="formattedText">The formatted text representing the WS.</param>
        //public override void ToModel(string formattedText)
        //{
        //    if (formattedText.Contains(DefaultSerieSeparatorString))
        //        base.ToModel(formattedText);


        //}
        #endregion

    }
}
