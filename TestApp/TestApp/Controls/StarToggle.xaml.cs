using System;
using System.Windows.Input;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StarToggle : ContentView
    {

        private ICommand _toggleCommand;


        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            propertyName: nameof(IsToggled),
            returnType: typeof(bool),
            declaringType: typeof(StarToggle),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsToggledChanged);

        public static readonly BindableProperty IsTitleVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsTitleVisible),
            returnType: typeof(bool),
            declaringType: typeof(StarToggle),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty OnToggleCommandProperty = BindableProperty.Create(
            propertyName: nameof(OnToggleCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(StarToggle),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);


        #region Bindable properties methods

        private static void IsToggledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as StarToggle).ToggleButton.IsToggled = (bool)newValue;
        }
        #endregion


        #region Backing Properties

        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }
        public bool IsTitleVisible
        {
            get => (bool)GetValue(IsTitleVisibleProperty);
            set => SetValue(IsTitleVisibleProperty, value);
        }
        public ICommand OnToggleCommand
        {
            get => (ICommand)GetValue(OnToggleCommandProperty);
            set => SetValue(OnToggleCommandProperty, value);
        }
        #endregion




        /// <summary>
        /// Called on Toggle/Untoggle action
        /// </summary>
        public ICommand ToggleCommand => _toggleCommand ?? (_toggleCommand =
            new Command(async () =>
            {
                if(OnToggleCommand is ICommandAsync<bool> asyncCommand)
                    await asyncCommand?.ExecuteAsync(IsToggled);
                else
                    OnToggleCommand?.Execute(IsToggled);
            }, 
            () => IsEnabled));

        public StarToggle()
        {
            InitializeComponent();
        }

    }
}