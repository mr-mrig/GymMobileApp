using Xamarin.Forms;

namespace TestApp.Triggers
{

    /// <summary>
    /// Trigger that performs an animation which fades the Visual Element to the specified opacity value.
    /// </summary>
    public class FadeAnimationTrigger : TriggerAction<VisualElement>
    {


        public const float DefaultFadeToOpacity = 1f;
        public const float DefaultFadeFromOpacity = 0.05f;
        public const uint DefaultDuration = 100;
        public readonly Easing DefaultEasingFunction = Easing.Linear;

        /// <summary>
        /// Value which to fade the element to
        /// </summary>
        public float? FadeToOpacity { set; get; } = null;

        /// <summary>
        /// The initial value which to fade the element from
        /// </summary>
        public float? FadeFromOpacity { set; get; } = null;

        /// <summary>
        /// The animation duration in milliseconds
        /// </summary>
        public uint? DurationMilliseconds { set; get; } = null;


        /// <summary>
        /// Trigger that performs an animation which fades the Visual Element to the specified opacity value.
        /// </summary>
        public FadeAnimationTrigger() { }


        protected override async void Invoke(VisualElement sender)
        {
            sender.Opacity = (double)FadeFromOpacity;

            await sender.FadeTo(
                FadeToOpacity ?? DefaultFadeToOpacity,
                DurationMilliseconds ?? DefaultDuration, 
                DefaultEasingFunction);
        }
    }
}
