using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TestApp.Services.Utils.Extensions
{

    /// <summary>
    /// Observable collection which notifies any change, including the changes in the items it contains
    /// It raises a System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged
    /// </summary>
    /// <typeparam name="T">The items type, which must implement <seealso cref="INotifyPropertyChanged"/></typeparam>
    public sealed class ExtendedObservableCollection<T> : SmartObservableCollection<T> where T : INotifyPropertyChanged
    {


        #region Ctors
        /// <summary>
        /// <para>Observable collection which notifies any change, including the changes in the items it contains
        /// It raises a System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged</para>
        /// <para>IMPORTANT: this might not work with collections which are ItemsSource of Views Controls</para>
        /// </summary>
        public ExtendedObservableCollection()
        {
            CollectionChanged += ExtendedCollectionChanged;
        }

        /// <summary>
        /// Observable collection which notifies any change, including the changes in the items it contains
        /// It raises a System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged
        /// <para>IMPORTANT: this might not work with collections which are ItemsSource of Views Controls</para>
        /// </summary>
        /// <param name="items">The collection items</param>
        public ExtendedObservableCollection(IEnumerable<T> items) : this()
        {
            foreach (var item in items)
                Add(item);
        }
        #endregion


        private void UnsubscribePropertyChanged(INotifyPropertyChanged item) => item.PropertyChanged -= ItemPropertyChanged; 
        private void SubscribePropertyChanged(INotifyPropertyChanged item) => item.PropertyChanged += ItemPropertyChanged; 

        private void ExtendedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Subscribe/unsubscribe when the collection changes
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                    SubscribePropertyChanged(item as INotifyPropertyChanged);
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                    UnsubscribePropertyChanged(item as INotifyPropertyChanged);
            }
        }

        /// <summary>
        /// IMPORTANT: ArgumentOutOfRangeException might be thrown after the call.
        /// This happens when ItemsSource, no solution found yet...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Raise CollectionChanged when an item changes
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender)));
        }


        #region Overrides

        protected override void ClearItems()
        {
            foreach (INotifyPropertyChanged item in this)
            {
                if (item != null)
                    UnsubscribePropertyChanged(item);
            }
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            UnsubscribePropertyChanged(Items[index]);
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, T item)
        {
            UnsubscribePropertyChanged(item);
            SubscribePropertyChanged(item);
            base.InsertItem(index, item);
        }
        #endregion
    }
}
