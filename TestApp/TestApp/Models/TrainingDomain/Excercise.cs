using TestApp.Models.Base;

namespace TestApp.Models.TrainingDomain
{
    public class Excercise : BaseEntityModel
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public uint MuscleGroupId { get; set; }


        public Excercise() { }

        /// <summary>
        /// <para>Deep copies the specified object.</para>
        /// <para>Please notice that all the fields are deeped copied, including the IDs</para>
        /// </summary>
        /// <param name="copyFrom">The object which this one should be the copy of</param>
        public Excercise(Excercise copyFrom) 
        {
            Id = copyFrom.Id;
            Name = copyFrom.Name;
            Description = copyFrom.Description;
            ImageUrl = copyFrom.ImageUrl;
            MuscleGroupId = copyFrom.MuscleGroupId;
        }
    }
}
