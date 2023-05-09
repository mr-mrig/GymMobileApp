using TestApp.Models.TrainingDomain;

namespace TestApp.Services.DomainPresenters
{
    public interface ITrainingPresenter
    {




        /// <summary>
        /// The Work Unit model to be displayed
        /// </summary>
        WorkUnitTemplate WorkUnit { get; set; }




        string ToFormattedEffort();
        string ToFormattedTempo();
        string ToFormattedIntensityTechniques();
        string ToFormattedRest();


        /// <summary>
        /// <para>String representation of the whole training work by following the usual training conventions</para>
        /// <para>IE: Dropsets separated by '+', sets separated by '6', etc. </para>
        /// </summary>
        /// <returns>The formatted WS representaion as a string</returns>
        string ToFormattedRepetitions();

        /// <summary>
        /// From the string representation to the Work Unit object <see cref="WorkUnit"/>
        /// Examples of input repetitions: "10", "10+8+6", etc.
        /// </summary>
        /// <param name="formattedRepetitions">The formatted repetitions</param>
        /// <param name="formattedRest">The formatted rest</param>
        /// <param name="formattedTempo">The formatted lifting tempo</param>
        /// <param name="formattedEffort">The formatted effort</param>
        /// <param name="intensityTechniques">The string with the comma-spearated intensity techniques list</param>
        void ToModel(string formattedRepetitions, string formattedRest, string formattedTempo, string formattedEffort, string intensityTechniques);

    }
}
