using System;
using Android.Views;

namespace TestApp.Droid.Extensions
{


    /// <summary>
    /// Class AlignmentExtensions.
    /// </summary>
    public static class AlignmentExtensions
    {
        /// <summary>
        /// To Android text alignment
        /// </summary>
        /// <param name="alignment">The Xamarin Forms alignment</param>
        /// <returns>TextAlignment</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TextAlignment ToDroidTextAlignment(this Xamarin.Forms.TextAlignment alignment)
        {
            switch (alignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    return TextAlignment.Center;
                case Xamarin.Forms.TextAlignment.End:
                    return TextAlignment.ViewEnd;
                case Xamarin.Forms.TextAlignment.Start:
                    return TextAlignment.ViewStart;
            }

            throw new InvalidOperationException(alignment.ToString());
        }

        /// <summary>
        /// To Android horizontal gravity flags.
        /// </summary>
        /// <param name="alignment">The Xamarin Forms alignment</param>
        /// <returns>GravityFlags</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static GravityFlags ToDroidHorizontalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
        {
            switch (alignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    return GravityFlags.CenterHorizontal;
                case Xamarin.Forms.TextAlignment.End:
                    return GravityFlags.Right;
                case Xamarin.Forms.TextAlignment.Start:
                    return GravityFlags.Left;
            }

            throw new InvalidOperationException(alignment.ToString());
        }

        /// <summary>
        /// To Android vertical gravity flags.
        /// </summary>
        /// <param name="alignment">The Xamarin Forms alignment</param>
        /// <returns>GravityFlags</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static GravityFlags ToDroidVerticalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
        {
            switch (alignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    return GravityFlags.CenterVertical;
                case Xamarin.Forms.TextAlignment.End:
                    return GravityFlags.Bottom;
                case Xamarin.Forms.TextAlignment.Start:
                    return GravityFlags.Top;
            }

            throw new InvalidOperationException(alignment.ToString());
        }
    }
}