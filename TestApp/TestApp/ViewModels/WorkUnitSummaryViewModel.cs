//using System.Collections.ObjectModel;
//using TestApp.Models.TrainingDomain;
//using TestApp.ViewModels.Base;

//namespace TestApp.ViewModels
//{
//    public class WorkUnitSummaryViewModel : BaseViewModel
//    {


//        private bool _isSelected;
//        private string _workingSetsSummary;
//        private WorkUnitTemplate _workUnit;


//        /// <summary>
//        /// Whether the Work Unit has been selected by the user
//        /// </summary>
//        public bool IsSelected
//        {
//            get => _isSelected;
//            set
//            {
//                _isSelected = value;
//                RaisePropertyChanged();
//            }
//        }

//        /// <summary>
//        /// The concise representation of the Working Sets of the Work Unit
//        /// </summary>
//        public string WorkingSetsSummary
//        {
//            get => _workingSetsSummary;
//            set
//            {
//                _workingSetsSummary = value;
//                RaisePropertyChanged();
//            }
//        }

//       /// <summary>
//       /// The model
//       /// </summary>
//        public WorkUnitTemplate WorkUnit
//        {
//            get => _workUnit;
//            set
//            {
//                _workUnit = value;
//                RaisePropertyChanged();
//            }
//        }
//    }
//}
