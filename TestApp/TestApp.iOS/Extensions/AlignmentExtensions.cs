using System;
using UIKit;
using Xamarin.Forms;

namespace TestApp.iOS.Extensions
{
    public static class AlignmentExtensions
    {
        /// <summary>
        /// Map Xamarin Forms TextAlignment to the iOS Content Alignment
        /// </summary>
        /// <param name="alignment">The Xamarin Forms TextAlignment</param>
        /// <returns>UIControlContentVerticalAlignment</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static UIControlContentVerticalAlignment ToContentVerticalAlignment(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return UIControlContentVerticalAlignment.Center;
                case TextAlignment.End:
                    return UIControlContentVerticalAlignment.Bottom;
                case TextAlignment.Start:
                    return UIControlContentVerticalAlignment.Top;
            }

            throw new InvalidOperationException(alignment.ToString());
        }

        /// <summary>
        /// Map Xamarin Forms TextAlignment to the iOS Content Alignment
        /// </summary>
        /// <param name="alignment">The Xamarin Forms TextAlignment</param>
        /// <returns>UIControlContentHorizontalAlignment</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static UIControlContentHorizontalAlignment ToContentHorizontalAlignment(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return UIControlContentHorizontalAlignment.Center;
                case TextAlignment.End:
                    return UIControlContentHorizontalAlignment.Right;
                case TextAlignment.Start:
                    return UIControlContentHorizontalAlignment.Left;
            }

            throw new InvalidOperationException(alignment.ToString());
        }
    }
}