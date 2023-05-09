using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestApp.Triggers
{

    /// <summary>
    /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
    /// This is a two-step animation which scales to a specific value and then scales again to the original size
    /// </summary>
    public class ToggleAnimationTrigger : TriggerAction<VisualElement>
    {


        public const float DefaultScaleToValue = 0.9f;
        public const uint DefaultDuration = 50;
        public readonly Easing DefaultEasingFunction = Easing.Linear;

        /// <summary>
        /// Value which to scale the element to, before scaling back to the original size
        /// </summary>
        public float? ScaleToValue { set; get; } = null;

        /// <summary>
        /// The duration of each step in milliseconds -> The full duration will be 2* this value + 100ms (which is the internal delay)
        /// </summary>
        public uint? DurationMilliseconds { set; get; } = null;


        /// <summary>
        /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
        /// This is a two-step animation which scales to a specific value and then scales again to the original size
        /// </summary>
        public ToggleAnimationTrigger() { }


        protected override async void Invoke(VisualElement sender)
        {
            float scaleToStep1 = ScaleToValue ?? DefaultScaleToValue;
            uint duration = DurationMilliseconds ?? DefaultDuration;

            await sender.ScaleTo(scaleToStep1, duration, DefaultEasingFunction);
            await Task.Delay(100);
            await sender.ScaleTo(1, duration, DefaultEasingFunction);
        }
    }
}
