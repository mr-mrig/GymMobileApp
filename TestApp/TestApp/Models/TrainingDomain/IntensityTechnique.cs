using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class IntensityTechnique : BaseEntityModel
    {

        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLinking { get; set; }


        public IntensityTechnique() { }

        /// <summary>
        /// <para>Deep copies the specified object.</para>
        /// <para>Please notice that all the fields are deeped copied, including the IDs</para>
        /// </summary>
        /// <param name="copyFrom">The object which this one should be the copy of</param>
        public IntensityTechnique(IntensityTechnique copyFrom)
        {
            Id = copyFrom.Id;
            Abbreviation = copyFrom.Abbreviation;
            Name = copyFrom.Name;
            Description = copyFrom.Description;
            IsLinking = copyFrom.IsLinking;
        }
    }
}