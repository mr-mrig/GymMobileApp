using System;
using Android.Content;
using Android.Graphics;
using TestApp.Droid.Utils.TypefaceCache;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace TestApp.Droid.Extensions
{
    /// <summary>
    /// Andorid specific extensions for the Font class.
    /// </summary>
    public static class FontExtensions
    {

        private const string DefaultFileExtensions = ".tff";


        /// <summary>
        /// Map the xamarin Forms <see cref="Xamarin.Forms.Font"/> to Android <see cref="Typeface"/>
        /// </summary>
        /// <returns>The Andorid typeface</returns>
        /// <param name="font">The Xamarin Forms Font</param>
        /// <param name="context">The Android Context</param>
        public static Typeface ToExtendedTypeface(this Font font, Context context)
        {
            Typeface typeface = null;

            // Search the cache
            string hashKey = font.ToHasmapKey();
            typeface = TypefaceCache.SharedCache.GetTypeface(hashKey);

            // If not found, try the custom asset folder
            if (typeface == null && !string.IsNullOrEmpty(font.FontFamily))
            {
                string filename = font.FontFamily;
                //if no extension given then assume and add .ttf
                if (filename.LastIndexOf(".", StringComparison.Ordinal) != filename.Length - 4)
                    filename = string.Format("{0}" + DefaultFileExtensions, filename);
                try
                {
                    string path = "fonts/" + filename;
                    typeface = Typeface.CreateFromAsset(context.Assets, path);
                }
                catch (Exception ex)
                {
                    try
                    {
                        typeface = Typeface.CreateFromFile("fonts/" + filename);
                    }
                    catch (Exception innerExc)
                    {


                    }
                }
            }
            //If still not found, default Xamarin.Forms implementation
            if (typeface == null)
                typeface = font.ToTypeface() ?? Typeface.Default;

            TypefaceCache.SharedCache.SetTypeface(hashKey, typeface);
            return typeface;
        }

        /// <summary>
        /// Provide an unique string identifier for the specified font
        /// </summary>
        /// <returns>The unique identifier</returns>
        /// <param name="font">The Xamarin Forms Font</param>
        private static string ToHasmapKey(this Font font)
            => string.Format("{0}.{1}.{2}.{3}", font.FontFamily, font.FontSize, font.NamedSize, (int)font.FontAttributes);

    }
}