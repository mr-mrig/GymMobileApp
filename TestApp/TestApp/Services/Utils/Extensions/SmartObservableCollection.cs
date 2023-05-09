using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TestApp.Services.Utils.Extensions
{

    /// <summary>
    /// Observable Collection which raise only one System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged 
    /// when multiple items are added/removed simoultaneously
    /// </summary>
    /// <typeparam name="T">The items type</typeparam>
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {

        /// <summary>
        /// Observable Collection which raise only one System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged 
        /// when multiple items are added/removed simoultaneously
        /// </summary>
        public SmartObservableCollection() : base() { }

        /// <summary>
        /// Observable Collection which raise only one System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged 
        /// when multiple items are added/removed simoultaneously
        /// </summary>
        /// <param name="collection">The items</param>
        public SmartObservableCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Observable Collection which raise only one System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged 
        /// when multiple items are added/removed simoultaneously
        /// </summary>
        /// <param name="collection">The items</param>
        public SmartObservableCollection(List<T> list) : base(list) { }


        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
                Items.Add(item);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> range)
        {
            Items.Clear();
            AddRange(range);
        }
    }
}
