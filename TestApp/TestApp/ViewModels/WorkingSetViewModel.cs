using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class WorkingSetViewModel : BaseViewModel
    {


        #region Backing fields
        private uint _progressiveNumber;
        private int _clusteredSetsCounter;
        private string _formattedRepetitions;
        private string _rest;
        private string _liftingTempo;
        private string _effort;
        private string _intensityTechniques;
        #endregion


        public uint ProgressiveNumber
        {
            get => _progressiveNumber;
            set
            {
                _progressiveNumber = value;
                RaisePropertyChanged();
            }
        }

        public int ClusteredSetsCounter
        {
            get => _clusteredSetsCounter;
            set
            {
                _clusteredSetsCounter = value;
                RaisePropertyChanged();
            }
        }

        public string FormattedRepetitions
        {
            get => _formattedRepetitions;
            set
            {
                _formattedRepetitions = value;
               RaisePropertyChanged();
            }
        }

        public string Rest
        {
            get => _rest;
            set
            {
                _rest = value;
                RaisePropertyChanged();
            }
        }

        public string LiftingTempo
        {
            get => _liftingTempo;
            set
            {
                _liftingTempo = value;
                RaisePropertyChanged();
            }
        }

        public string Effort
        {
            get => _effort;
            set
            {
                _effort = value;
                RaisePropertyChanged();
            }
        }

        public string IntensityTechniques
        {
            get => _intensityTechniques;
            set
            {
                _intensityTechniques = value;
                RaisePropertyChanged();
            }
        }

        #region Commands

        public Command RepetitionsChangedCommand { get; set; }
        public Command RestChangedCommand { get; set; }
        public Command LiftingTempoChangedCommand { get; set; }
        public Command EffortChangedCommand { get; set; }
        public Command OpenIntensityTechniquesManagerCommand { get; set; }
        #endregion

    }
}