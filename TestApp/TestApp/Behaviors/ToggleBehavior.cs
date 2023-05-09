using System;
using System.Linq;
using System.Windows.Input;
using TestApp.Behaviors.Base;
using Xamarin.Forms;

namespace TestApp.Behaviors
{
    public class ToggleBehavior : BindableBehavior<View>
    {





        public static readonly BindableProperty IsToggledProperty = BindableProperty.CreateAttached(
            propertyName: nameof(IsToggled),
            returnType: typeof(bool),
            declaringType: typeof(ToggleBehavior),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }



        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(ToggleBehavior),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }



        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            propertyName: nameof(CommandParameter),
            returnType: typeof(object),
            declaringType: typeof(ToggleBehavior),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }




        protected override void OnAttachedTo(View owner)
        {
            base.OnAttachedTo(owner);

            if (owner is Button myButton)
                myButton.Clicked += Toggled;
            
            else if (owner is Switch mySwitch)
                mySwitch.Toggled += Toggled;
            
            else
            {
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += Toggled;
                owner.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private void Toggled(object sender, EventArgs e)
        {
            IsToggled = !IsToggled;
            Command?.Execute(CommandParameter);
        }

        protected override void OnDetachingFrom(View owner)
        {
            base.OnDetachingFrom(owner);

            if (owner is Button myButton)
                myButton.Clicked -= Toggled;

            else if (owner is Switch mySwitch)
                mySwitch.Toggled -= Toggled;

            else
            {
                TapGestureRecognizer gestureRec = owner.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;
                if (gestureRec != null)
                    gestureRec.Tapped -= Toggled;
            }
        }


    }
}
