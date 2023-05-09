using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /// <summary>
    /// View that allows to sequentially select an item from a list by pressing two buttons.
    /// It requires the items to provide a suitable Equals<object> implementation to work properly.
    /// Furthermore ToString() is used to show an evidence of the selected item hence it might be wise to override it
    /// </summary>
    public partial class HorizontalScrollableViewSelector : ContentView
    {


        private int _selectedIndex;


        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IList),
            declaringType: typeof(HorizontalScrollableViewSelector),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: ItemsSourceChanged,
            propertyChanging: ItemsSourceChanging);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(object),
            declaringType: typeof(HorizontalScrollableViewSelector),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: SelectedItemChanged);

        #region Bindable Properties Methods
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HorizontalScrollableViewSelector control && control.ItemsSource != null && newValue != null)
            {
                control._selectedIndex = control.GetSelectedIndex();
                // Notify Commands
                (control.GoToNextCommand as Command).ChangeCanExecute();
                (control.GoToPreviousCommand as Command).ChangeCanExecute();
            }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HorizontalScrollableViewSelector control && control.SelectedItem != null && newValue != null)
            {
                control._selectedIndex = control.GetSelectedIndex();
                // Notify Commands
                (control.GoToNextCommand as Command).ChangeCanExecute();
                (control.GoToPreviousCommand as Command).ChangeCanExecute();

                // Subscribe to changes
                if (newValue is INotifyCollectionChanged observableCollection)
                    (control.ItemsSource as INotifyCollectionChanged).CollectionChanged += control.ObservableCollection_CollectionChanged;
            }
        }

        private static void ItemsSourceChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HorizontalScrollableViewSelector control && control.SelectedItem != null && oldValue != null)
            {
                // Unsubscribe!
                if (oldValue is INotifyCollectionChanged observableCollection)
                    (control.ItemsSource as INotifyCollectionChanged).CollectionChanged -= control.ObservableCollection_CollectionChanged;
            }
        }
        #endregion





        private void ObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }


        #region Backing Fields
        private ICommand _goToNextCommand;
        private ICommand _goToPreviousCommand;

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        #endregion

        #region Commands
        public ICommand GoToNextCommand => _goToNextCommand ?? (_goToNextCommand = new Command(
            x => SelectNextItem(), 
            x => IsEnabled && ItemsSource != null && _selectedIndex < ItemsSource.Count - 1));

        public ICommand GoToPreviousCommand => _goToPreviousCommand ?? (_goToPreviousCommand = new Command(
            x => SelectPreviousItem(), 
            x => IsEnabled && ItemsSource != null && _selectedIndex > 0));
        #endregion


        #region Commands Actions
        private void SelectNextItem()
        {
            SelectedItem = ItemsSource[++_selectedIndex];
        }

        private void SelectPreviousItem()
        {
            SelectedItem = ItemsSource[--_selectedIndex];
        }
        #endregion


        #region Ctor
        /// <summary>
        /// View that allows to sequentially select an item from a list by pressing two buttons.
        /// It requires the items to provide a suitable Equals<object> implementation to work properly.
        /// Furthermore ToString() is used to show an evidence of the selected item hence it might be wise to override it
        /// </summary>
        public HorizontalScrollableViewSelector()
        {
            InitializeComponent();
        }
        #endregion

        private int GetSelectedIndex() => ItemsSource.IndexOf(SelectedItem);
    }
}