using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestApp.Behaviors
{

    /// <summary>
    /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
    /// This is a two-step animation which scales to a specific value and then scales again to the original size
    /// </summary>
    public class ToggleAnimationBehavior : Behavior<VisualElement>
    {

        private bool _isAnimating = false;


        /// <summary>
        /// Trigger that performs an animation which scales the Visual Element which might be suitable for a Toggle Button
        /// This is a two-step animation which scales to a specific value and then scales again to the original size
        /// </summary>
        public ToggleAnimationBehavior() { }


        protected override async void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            await Animate(bindable);
        }

        private async Task Animate(VisualElement sender)
        {
            if (_isAnimating)
                return;

            _isAnimating = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await sender.ScaleTo(1.2, 170, easing: Easing.Linear);
                    await Task.Delay(100);
                    await sender.ScaleTo(1, 170, easing: Easing.Linear);
                }
                finally
                {
                    _isAnimating = false;
                }
            });
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);
        }
    }
}
