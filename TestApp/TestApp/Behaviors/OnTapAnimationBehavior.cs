using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestApp.Behaviors
{

    /// <summary>
    /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
    /// This is a two-step animation which scales to a specific value and then scales again to the original size
    /// </summary>
    public class OnTapAnimationBehavior : Behavior<View>
    {


        public const float FirstStepScaleToDefaultValue = 1.2f;
        public const float SecondStepScaleToDefaultValue = 1f;
        public const int TransitionDelayDefaultValue = 100;
        public const uint FirstStepDurationMillisecondsDefaultValue = 100;
        public const uint SecondStepDurationMillisecondsDefaultValue = 100;


        private bool _isAnimating = false;



        public static readonly BindableProperty FirstStepScaleToProperty = BindableProperty.CreateAttached(
            propertyName: nameof(FirstStepScaleTo),
            returnType: typeof(float),
            declaringType: typeof(OnTapAnimationBehavior),
            defaultValue: FirstStepScaleToDefaultValue,
            defaultBindingMode: BindingMode.OneWay);

        public float FirstStepScaleTo
        {
            get => (float)GetValue(FirstStepScaleToProperty);
            set => SetValue(FirstStepScaleToProperty, value);
        }



        public static readonly BindableProperty FirstStepDurationMillisecondsProperty = BindableProperty.CreateAttached(
            propertyName: nameof(FirstStepDurationMilliseconds),
            returnType: typeof(uint),
            declaringType: typeof(OnTapAnimationBehavior),
            defaultValue: FirstStepDurationMillisecondsDefaultValue,
            defaultBindingMode: BindingMode.OneWay);

        public uint FirstStepDurationMilliseconds
        {
            get => (uint)GetValue(FirstStepDurationMillisecondsProperty);
            set => SetValue(FirstStepDurationMillisecondsProperty, value);
        }


        public static readonly BindableProperty SecondStepScaleToProperty = BindableProperty.CreateAttached(
            propertyName: nameof(SecondStepScaleTo),
            returnType: typeof(float),
            declaringType: typeof(OnTapAnimationBehavior),
            defaultValue: SecondStepScaleToDefaultValue,
            defaultBindingMode: BindingMode.OneWay);

        public float SecondStepScaleTo
        {
            get => (float)GetValue(SecondStepScaleToProperty);
            set => SetValue(SecondStepScaleToProperty, value);
        }



        public static readonly BindableProperty SecondStepDurationMillisecondsProperty = BindableProperty.CreateAttached(
            propertyName: nameof(SecondStepDurationMilliseconds),
            returnType: typeof(uint),
            declaringType: typeof(OnTapAnimationBehavior),
            defaultValue: SecondStepDurationMillisecondsDefaultValue,
            defaultBindingMode: BindingMode.OneWay);

        public uint SecondStepDurationMilliseconds
        {
            get => (uint)GetValue(SecondStepDurationMillisecondsProperty);
            set => SetValue(SecondStepDurationMillisecondsProperty, value);
        }



        public static readonly BindableProperty TransitionDelayMillisecondsProperty = BindableProperty.CreateAttached(
            propertyName: nameof(TransitionDelayMilliseconds),
            returnType: typeof(int),
            declaringType: typeof(OnTapAnimationBehavior),
            defaultValue: TransitionDelayDefaultValue,
            defaultBindingMode: BindingMode.OneWay);

        public int TransitionDelayMilliseconds
        {
            get => (int)GetValue(TransitionDelayMillisecondsProperty);
            set => SetValue(TransitionDelayMillisecondsProperty, value);
        }


        /// <summary>
        /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
        /// This is a two-step animation which scales to a specific value and then scales again to the original size
        /// </summary>
        public OnTapAnimationBehavior() { }



        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            //bindable.BindingContextChanged += OnBindingContextChanged;

            if (bindable is Button myButton)
            {
                myButton.Clicked += ViewTapped;
            }
            else if (bindable is Switch mySwitch)
            {
                mySwitch.Toggled += ViewTapped;
            }
            else
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += ViewTapped;
                bindable.GestureRecognizers.Add(tapGestureRecognizer);
            }

            base.OnAttachedTo(bindable);
        }

        private async void ViewTapped(object sender, EventArgs e)
        {
            await Animate(sender as View);
        }


        private async Task Animate(View sender)
        {
            if (_isAnimating)
                return;

            _isAnimating = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await sender.ScaleTo(FirstStepScaleTo, FirstStepDurationMilliseconds, easing: Easing.Linear);
                    await Task.Delay(TransitionDelayMilliseconds);
                    await sender.ScaleTo(SecondStepScaleTo, SecondStepDurationMilliseconds, easing: Easing.Linear);
                }
                finally
                {
                    _isAnimating = false;
                }
            });
        }

        protected override void OnDetachingFrom(View bindable)
        {
            //bindable.BindingContextChanged -= OnBindingContextChanged;

            var exists = bindable.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;

            if (exists != null)
                exists.Tapped -= ViewTapped;

            base.OnDetachingFrom(bindable);
        }
    }
}
