using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using TestApp.Models.Common;
using Xamarin.Forms;
using TestApp.Services.Utils.Extensions;
using TestApp.Controls.Events;
using System.Collections.Generic;

namespace TestApp.Controls.Templated
{
    public class OptionsGroupUniformGrid : Grid
    {


        // The number of columns to use if nothing is specified
        public const int DefaultColumnsNumber = 3;

        public event EventHandler SelectedItemChanged;



        /// <summary>
        /// <para>The number of columns to be set - The default value is <see cref="DefaultColumnsNumber"/></para>
        /// <para>IMPORTANT: this must be set before the ItemsSource otherwise it will have no effect!</para>
        /// </summary>
        public static readonly BindableProperty ColumnsNumberProperty = BindableProperty.Create(
            propertyName: nameof(ColumnsNumber),
            returnType: typeof(int),
            declaringType: typeof(OptionsGroupUniformGrid),
            defaultValue: DefaultColumnsNumber,
            defaultBindingMode: BindingMode.OneTime);


        /// <summary>
        /// The Items to be displeyed as options - No duplicates allowed
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IEnumerable),
            declaringType: typeof(OptionsGroupUniformGrid),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            validateValue: ItemsSourceValidator,
            propertyChanged: ItemsSourceChanged);


        /// <summary>
        /// The Items selected
        /// </summary>
        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItems),
            returnType: typeof(ObservableCollection<ISimpleTagElement>),
            declaringType: typeof(OptionsGroupUniformGrid),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);


        /// <summary>
        /// The Items selected
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            propertyName: nameof(ItemTemplate),
            returnType: typeof(DataTemplate),
            declaringType: typeof(OptionsGroupUniformGrid),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ItemTemplateChanged);



        #region Static properties functions

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OptionsGroupUniformGrid grid = (OptionsGroupUniformGrid)bindable;

            // The Control needs the ItemTemplate to be initialized - This happens when the Item Template is set before the Items Source
            if (grid.ItemTemplate != null)
                InitControl(grid, newValue as IList);
            
        }

        private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OptionsGroupUniformGrid grid = (OptionsGroupUniformGrid)bindable;

            // This happens when the Item Template is set after the Items Source
            if (newValue != null)
                InitControl(grid, grid.ItemsSource as IList);
        }

        private static void InitControl(OptionsGroupUniformGrid grid, IList sourceItems)
        {
            grid.Children.Clear();

            if (sourceItems != null)
            {
                // Init grid
                int colCounter = (int)grid.GetValue(ColumnsNumberProperty);
                int rowCounter = (int)Math.Ceiling((float)sourceItems.Count / (float)colCounter);
                int index = 0;

                grid.ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = GridLength.Star, },
                    new ColumnDefinition() { Width = GridLength.Star, },
                    new ColumnDefinition() { Width = GridLength.Star, },
                };
                grid.RowDefinitions = new RowDefinitionCollection();

                grid.ColumnSpacing = 0;
                grid.RowSpacing = 0;

                for (int irow = 0; irow < rowCounter; irow++)
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto, });

                // Draw childs
                for (int irow = 0; irow < rowCounter; irow++)
                {
                    for (int icol = 0; icol < colCounter; icol++)
                    {
                        if (index >= sourceItems.Count)
                            break;

                        View child = grid.CreateChildView(sourceItems[index]);

                        grid.Children.Add(child, icol, irow);
                        index++;
                    }
                }
            }
        }

        private static bool ItemsSourceValidator(BindableObject bindable, object value)
        {
            // Options cannot contain duplicates
            if (value is IEnumerable options)
                return !options.ContainsDuplicates(x => (x as SimpleTagElement).Id);

            return value == null;
        }
        #endregion


        #region Backing Properties

        /// <summary>
        /// The number of columns to be set - The default value is <see cref="DefaultColumnsNumber"/>
        /// </summary>
        public int ColumnsNumber
        {
            get => (int)GetValue(ColumnsNumberProperty);
            set => SetValue(ColumnsNumberProperty, value);
        }

        /// <summary>
        /// The Items to be displeyed as options - No duplicates allowed
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// The Items selected
        /// </summary>
        public ObservableCollection<ISimpleTagElement> SelectedItems
        {
            get => (ObservableCollection<ISimpleTagElement>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// The Items selected
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        #endregion


        public OptionsGroupUniformGrid()
        {

        }




        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (propertyName == SelectedItemsProperty.PropertyName)
            {
                if (SelectedItems != null)
                    SelectedItems.CollectionChanged -= SelectedItemsCollectionChanged;
            }

            base.OnPropertyChanging(propertyName);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == SelectedItemsProperty.PropertyName)
            {
                if (SelectedItems != null)
                {
                    SelectedItems.CollectionChanged -= SelectedItemsCollectionChanged;
                    SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;

                    foreach (var item in SelectedItems)
                        ToggleSelection(item, true);
                }
            }

            base.OnPropertyChanged(propertyName);
        }


        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<ISimpleTagElement> oldSelection = SelectedItems;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ToggleSelection(SelectedItems[e.NewStartingIndex], true);
                    SelectedItemChanged?.Invoke(this, new OptionSelectedEventsArgs(SelectedItems[e.NewStartingIndex], true));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    ToggleSelection(e.OldItems[0] as ISimpleTagElement, false);
                    SelectedItemChanged?.Invoke(this, new OptionSelectedEventsArgs(e.OldItems[0], false));
                    break;

                default:
                    // Other operations not supported
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
                    break;
            }
        }


        /// <summary>
        /// Align the selected items to the wrappers which keep track of the user selection
        /// </summary>
        private void ToggleSelection(ISimpleTagElement selectedElement, bool isSelected)
        {
            View toggledView = Children.SingleOrDefault(x => x.BindingContext.Equals(selectedElement));
            VisualStateManager.GoToState(toggledView, isSelected ? "Selected" : "Normal");
        }


        /// <summary>
        /// Create the view for the specific source item
        /// </summary>
        /// <param name="item">The source item</param>
        /// <returns>The View</returns>
        private View CreateChildView(object item)
        {
            View child;
            if (ItemTemplate is DataTemplateSelector templateSelector)
            {
                DataTemplate selectedItemTemplate = templateSelector.SelectTemplate(item, null);
                selectedItemTemplate.SetValue(BindingContextProperty, item);
                child = (View)selectedItemTemplate.CreateContent();
            }
            else
            {
                ItemTemplate.SetValue(BindingContextProperty, item);        // Item Template cannot be null! Watch out!
                child = (View)ItemTemplate.CreateContent();
            }

            // Attach selection event
            if (child is Button myButton)
                myButton.Clicked += OnItemSelected;

            else if (child is Switch mySwitch)
                mySwitch.Toggled += OnItemSelected;

            else if (child is CheckBox myCheck)
                myCheck.CheckedChanged += OnItemSelected;
            else
            {
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnItemSelected;
                child.GestureRecognizers.Add(tapGestureRecognizer);
                child.Focused += (o, e) =>
                {
                    VisualStateManager.GoToState(o as View, "Focused");
                };
            }

            // Init the visual state
            VisualStateManager.GoToState(child, "Normal");
            return child;
        }


        /// <summary>
        /// Raised whenever the user selects an item via GUI
        /// </summary>
        /// <param name="sender">The option View which has been selected</param>
        /// <param name="e">The Event Args</param>
        private void OnItemSelected(object sender, EventArgs e)
        {
            if (sender is View view)
            {
                ISimpleTagElement selectedTag = view.BindingContext as ISimpleTagElement;

                // If tag in list then it has been deselected, otherwise it has been selected
                bool isSelected = !SelectedItems.Remove(selectedTag);
                if (isSelected)
                    SelectedItems.Add(selectedTag);
            }
        }



    }
}