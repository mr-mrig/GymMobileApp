using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestApp.Triggers
{

    /// <summary>
    /// Trigger that performs an animation which smoothly changes the color to the specified HSV saturation component.
    /// </summary>
    public class ColorSaturationAnimationTrigger : TriggerAction<VisualElement>
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
        /// Trigger that performs an animation which smoothly changes the color to the specified HSV saturation component.
        /// </summary>
        public ColorSaturationAnimationTrigger() { }


        protected override async void Invoke(VisualElement sender)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            Color fromColor = Color.Red;
            Color toColor = Color.Green;

            sender.Animate(GetType().Name,
                callback: (t) => Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R),
                    fromColor.G + t * (toColor.G - fromColor.G),
                    fromColor.B + t * (toColor.B - fromColor.B),
                    fromColor.A + t * (toColor.A - fromColor.A)), 
                length: DurationMilliseconds ?? DefaultDuration, 
                easing: DefaultEasingFunction,
                finished: (v, c) => taskCompletionSource.SetResult(c));

            await taskCompletionSource.Task;
        }
    }
}
