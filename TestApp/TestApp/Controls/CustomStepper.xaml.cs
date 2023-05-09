using System;
using System.Windows.Input;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomStepper : ContentView
    {


        /// <summary>
        /// The selected current value
        /// </summary>
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(float),
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: null);


        /// <summary>
        /// The minimum allowed value - Included
        /// </summary>
        public static readonly BindableProperty MinimumValueProperty = BindableProperty.Create(
            propertyName: nameof(MinimumValue),
            returnType: typeof(float),
            declaringType: typeof(CustomStepper),
            defaultValue: 0f,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: MinimumValueChanged);
    

        /// <summary>
        /// The maximum allowed value - Included
        /// </summary>
        public static readonly BindableProperty MaximumValueProperty = BindableProperty.Create(
            propertyName: nameof(MaximumValue),
            returnType: typeof(float),
            declaringType: typeof(CustomStepper),
            defaultValue: float.MaxValue,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: MaximumValueChanged);


        /// <summary>
        /// The increment amount
        /// </summary>
        public static readonly BindableProperty IncrementProperty = BindableProperty.Create(
            propertyName: nameof(Increment),
            returnType: typeof(float),
            declaringType: typeof(CustomStepper),
            defaultValue: 1f,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: null);

        /// <summary>
        /// The action to be performed when the stepper value changes
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomStepper),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);



        #region Bindable properties methods

        private static void MaximumValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CustomStepper control = bindable as CustomStepper;

            // Coerce the initial value according to the Maximum - Can't do this in the CoerceValueFunction as the Maximum might not be set
            if (control.Value > control.MaximumValue)
                control.Value = control.MaximumValue;
        }

        private static void MinimumValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CustomStepper control = bindable as CustomStepper;

            // Coerce the initial value according to the Minimum - Can't do this in the CoerceValueFunction as the Minimum might not be set
            if (control.Value < control.MinimumValue)
                control.Value = control.MinimumValue;
        }
        #endregion


        #region Backing Properties

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);

                if(Command != null && Command.CanExecute(value))
                {
                    if (Command is ICommandAsync<object> asyncCommand)
                        asyncCommand.ExecuteAsync(value);
                    else
                        Command?.Execute(value);
                }
            }
        }
        public float MinimumValue
        {
            get => (float)GetValue(MinimumValueProperty);
            set => SetValue(MinimumValueProperty, value);
        }
        public float MaximumValue
        {
            get => (float)GetValue(MaximumValueProperty);
            set => SetValue(MaximumValueProperty, value);
        }
        
        public float Increment
        {
            get => (float)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }
        
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion



        public CustomStepper()
        {
            InitializeComponent();
        }


        #region Event handlers

        private void DecreaseButton_Clicked(object sender, EventArgs e)
        {
            float newValue = Value - Increment;
            Value = IsAboveOrEqualLowerLimit(newValue) ? newValue : MinimumValue;
        }

        private void IncreaseButton_Clicked(object sender, EventArgs e)
        {
            float newValue = Value + Increment;
            Value = IsBelowOrEqualUpperLimit(newValue) ? newValue : MaximumValue;
        }
        #endregion



        private bool CheckBoundaries(float value) => IsAboveOrEqualLowerLimit(value) && IsBelowOrEqualUpperLimit(value);

        private bool IsAboveOrEqualLowerLimit(float value) => value >= MinimumValue;
        private bool IsBelowOrEqualUpperLimit(float value) => value <= MaximumValue;
    }
}