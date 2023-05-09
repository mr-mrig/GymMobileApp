using System.Collections.ObjectModel;
using System.Linq;
using TestApp.Models.Base;
using TestApp.Models.Common;
using TestApp.Services.Utils;

namespace TestApp.Models.TrainingDomain
{
    public class WorkingSetTemplate : BaseEntityModel, IWorkingSet
    {

        #region Backing fields
        private uint? _progressiveNumber;
        private int? _repetitions;
        private int? _rest;
        private string _liftingTempo;
        private TrainingEffort _effort;
        private ObservableCollection<IntensityTechnique> _intensityTechniques;
        #endregion


        public uint? ProgressiveNumber
        {
            get => _progressiveNumber;
            set => Set(ref _progressiveNumber, value);
        }

        public int? Repetitions
        {
            get => _repetitions;
            set => Set(ref _repetitions, value);
        }

        public int? Rest
        {
            get => _rest;
            set => Set(ref _rest, value);
        }

        public string LiftingTempo
        {
            get => _liftingTempo;
            set => Set(ref _liftingTempo, value);
        }

        public TrainingEffort Effort
        {
            get => _effort;
            set => Set(ref _effort, value);
        }

        public ObservableCollection<IntensityTechnique> IntensityTechniques
        {
            get => _intensityTechniques;
            set => Set(ref _intensityTechniques, value);
        }



        #region Ctors
        public WorkingSetTemplate() { }

        /// <summary>
        /// <para>Deep copies the specified object.</para>
        /// <para>Please notice that all the fields are deeped copied, including the IDs</para>
        /// </summary>
        /// <param name="copyFrom">The object which this one should be the copy of</param>
        public WorkingSetTemplate(WorkingSetTemplate copyFrom) 
        {
            Id = copyFrom.Id;
            ProgressiveNumber = copyFrom.ProgressiveNumber;
            Repetitions = copyFrom.Repetitions;
            Rest = copyFrom.Rest;
            LiftingTempo = copyFrom.LiftingTempo;
            Effort = copyFrom.Effort == null ? null : new TrainingEffort(copyFrom.Effort.Effort.Value, copyFrom.Effort.EffortType.Id);

            if(copyFrom.IntensityTechniques != null)
                IntensityTechniques = new ObservableCollection<IntensityTechnique>(
                    copyFrom.IntensityTechniques.Select(x => new IntensityTechnique(x)));
        }
        #endregion




        /// <summary>
        /// Perform the "value copy" from another instance IE: all the non-identifying fields are copied
        /// </summary>
        /// <param name="another">The other instance to copy from</param>
        public void CopyFrom(WorkingSetTemplate another)
        {
            _repetitions = another.Repetitions;
            _rest = another.Rest;
            _liftingTempo = another.LiftingTempo;
            _effort = another.Effort;
            _intensityTechniques = another.IntensityTechniques;
        }


        //RIGM
        public bool IsDropset() => IntensityTechniques != null && IntensityTechniques.Select(x => x.Id).Any(x => x == AppEnvironment.DropsetId);
        public bool IsAmrap() => Repetitions.HasValue && Repetitions == AppEnvironment.WsAmrapValue;
        public bool NotSpecified() => !Repetitions.HasValue;
        public bool IsFullRest() => Rest == AppEnvironment.WsFullRestValue;
    }
}