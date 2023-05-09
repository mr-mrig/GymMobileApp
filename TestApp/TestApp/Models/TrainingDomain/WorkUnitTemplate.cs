using System.Collections.Generic;
using System.Linq;
using TestApp.Models.Base;
using TestApp.Services.Utils.Extensions;

namespace TestApp.Models.TrainingDomain
{
    public class WorkUnitTemplate : BaseEntityModel
    {

        #region Backing fields
        private uint? _progressiveNumber;
        private uint? _noteId;
        private string _note;
        private Excercise _excercise;
        private IList<WorkingSetTemplate> _workingSets;
        private IntensityTechnique _intensityTechnique;
        #endregion




        public uint? ProgressiveNumber 
        { 
            get =>_progressiveNumber; 
            set => Set(ref _progressiveNumber, value);
        }

        public IntensityTechnique IntensityTechnique
        {
            get => _intensityTechnique;
            set => Set(ref _intensityTechnique, value);
        }

        public uint? NoteId 
        {
            get => _noteId;
            set => Set(ref _noteId, value);
        }

        public string Note
        {
            get => _note;
            set => Set(ref _note, value);
        }

        public Excercise Excercise
        {
            get => _excercise;
            set => Set(ref _excercise, value);
        }

        public IList<WorkingSetTemplate> WorkingSets    // Using ExtendedObservableCollection causes a huge event workload... Better off manually notify the callers...
        { 
            get => _workingSets;
            set
            {
                if(_workingSets != null && _workingSets is ExtendedObservableCollection<WorkingSetTemplate> observableWorkingSets)
                    observableWorkingSets.CollectionChanged -= WorkingSets_CollectionChanged;

                Set(ref _workingSets, value);

                if (_workingSets != null && _workingSets is ExtendedObservableCollection<WorkingSetTemplate> observableWorkingSets2)
                    observableWorkingSets2.CollectionChanged += WorkingSets_CollectionChanged;
            }
        }



        #region Ctors
        public WorkUnitTemplate() 
        {
            WorkingSets = new List<WorkingSetTemplate>();
        }

        /// <summary>
        /// <para>Deep copies the specified WorkUnitTemplate.</para>
        /// <para>Please notice that all the fields are deeped copied, including the IDs</para>
        /// </summary>
        /// <param name="copyFrom">The WorkUnit which this one should be the copy of</param>
        public WorkUnitTemplate(WorkUnitTemplate copyFrom) : this()
        {
            Id = copyFrom.Id;
            ProgressiveNumber = copyFrom.ProgressiveNumber;
            IntensityTechnique = IntensityTechnique == null ? null : new IntensityTechnique(copyFrom.IntensityTechnique);
            NoteId = copyFrom.NoteId;
            Note = copyFrom.Note;
            Excercise = new Excercise(copyFrom.Excercise);

            //if (copyFrom.WorkingSets is ExtendedObservableCollection<WorkingSetTemplate>)
            //    WorkingSets = new ExtendedObservableCollection<WorkingSetTemplate>(copyFrom.WorkingSets.Select(x => new WorkingSetTemplate(x)));
            //else
            //    WorkingSets = new List<WorkingSetTemplate>(copyFrom.WorkingSets.Select(x => new WorkingSetTemplate(x)));
            WorkingSets = new List<WorkingSetTemplate>(copyFrom.WorkingSets.Select(x => new WorkingSetTemplate(x)));
        }
        #endregion



        private void WorkingSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(WorkingSets));
        }

        /// <summary>
        /// Makes the progressive number consecutives IE: from 0 to WsCount - 1
        /// </summary>
        private void EnsureConsecutiveProgressiveNumbers()
        {
            for (int i = 0; i < WorkingSets.Count; i++)
                WorkingSets[i].ProgressiveNumber = (uint)i;
        }

        /// <summary>
        /// Makes the progressive number consecutives IE: from 0 to WsCount - 1
        /// </summary>
        /// <param name="fromProgressiveNumber">The Progressive Number which to start from: the one of the deleted item</param>
        private void EnsureConsecutiveProgressiveNumbersAfterDelete(int fromProgressiveNumber = 0)
        {
            foreach (var ws in WorkingSets.Where(x => x.ProgressiveNumber > fromProgressiveNumber))
                ws.ProgressiveNumber -= 1;
        }


        /// <summary>
        /// Adds an empty WS to the collection and manages all the automatic stuff (IE: progressive number etc.)
        /// The Id is set to NULL.
        /// </summary>
        public void AddDraftWorkingSet()
        {
            WorkingSets.Add(new WorkingSetTemplate
            {
                Id = null,
                ProgressiveNumber = (uint)WorkingSets.Count,
            });
            RaisePropertyChanged(nameof(WorkingSets));
        }

        /// <summary>
        /// Duplicate the last working set - if any - by copying its parameters and setting NULL Id.
        /// If no working set yet then just draft it.
        /// </summary>
        public void DupicateLastWorkingSet()
        {
            if (WorkingSets.Count > 0)
            {
                WorkingSetTemplate toAdd = new WorkingSetTemplate(WorkingSets.Last());
                toAdd.Id = null;
                toAdd.ProgressiveNumber = (uint)WorkingSets.Count;

                WorkingSets.Add(toAdd);
                RaisePropertyChanged(nameof(WorkingSets));
            }
            else
                AddDraftWorkingSet();
        }

        /// <summary>
        /// Adds the WS to the collection inserting it in the right position according to its progressive number
        /// </summary>
        /// <param name="workingSet">The Working Set to be added. Please notiuce that the Progressive Number should be consistent.</param>
        public void AddWorkingSet(WorkingSetTemplate workingSet)
        {
            int progressiveNumber = (int)workingSet.ProgressiveNumber;

            WorkingSets.Insert(progressiveNumber, workingSet);
            EnsureConsecutiveProgressiveNumbers();
            RaisePropertyChanged(nameof(WorkingSets));
        }


        /// <summary>
        /// Perform the "value copy" from another instance IE: the WSs value copies are performed, according to their Progressive Numbers
        /// Please notice that, if the specified WU has more WS than this one, these will be created and their IDs will be set to NULL
        /// </summary>
        /// <param name="another">The other instance to copy from</param>
        public void CopyFrom(WorkUnitTemplate another)
        {
            IList<WorkingSetTemplate> toDelete = new List<WorkingSetTemplate>();

            // Copy the shared WSs
            //WorkingSets = new List<WorkingSetTemplate>(
            //    WorkingSets.Select(x => {
            //        IEnumerable<WorkingSetTemplate> workingSetsFound = another.WorkingSets.Where(y => y.ProgressiveNumber == x.ProgressiveNumber);

            //        if (workingSetsFound.Count() == 1)
            //            x.CopyFrom(workingSetsFound.First());
            //        else
            //            toDelete.Add(x);

            //        return x;
            //    }));

            WorkingSets = new List<WorkingSetTemplate>(another.WorkingSets.Select(x => new WorkingSetTemplate(x)));
            RaisePropertyChanged(nameof(WorkingSets));
        }


        public void RemoveWorkingSet(WorkingSetTemplate toRemove)
        {
            WorkingSets.Remove(toRemove);
            EnsureConsecutiveProgressiveNumbersAfterDelete((int)toRemove.ProgressiveNumber);

            RaisePropertyChanged(nameof(WorkingSets));
        }

        public void RemoveWorkingSet(int progressiveNumber)
        {
            WorkingSets.RemoveAt(progressiveNumber);
            EnsureConsecutiveProgressiveNumbersAfterDelete(progressiveNumber);

            RaisePropertyChanged(nameof(WorkingSets));
        }
    }

}