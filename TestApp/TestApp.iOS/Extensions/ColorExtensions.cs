using XF=Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace TestApp.iOS.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Map the Xamarin Forms Color to the iOS UIColor
        /// </summary>
        /// <param name="color">The Xamarin Forms color</param>
        /// <param name="defaultColor">The default color</param>
        /// <returns>UIColor.</returns>
        public static UIColor ToUIColorOrDefault(this XF::Color color, UIColor defaultColor)
        {
            if (color == XF::Color.Default)
                return defaultColor;

            return color.ToUIColor();
        }
    }
}