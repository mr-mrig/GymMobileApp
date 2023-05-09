using Android.Graphics;

namespace TestApp.Droid.Utils.TypefaceCache
{

    public interface ITypefaceCache
    {
        /// <summary>
        /// Removes typeface from cache
        /// </summary>
        /// <param name="key">The key to be added</param>
        /// <param name="typeface">The Typeface to be stored</param>
        void SetTypeface(string key, Typeface typeface);

        /// <summary>
        /// Removes the typeface
        /// </summary>
        /// <param name="key">The deletion key</param>
        void RemoveTypeface(string key);

        /// <summary>
        /// Retrieves the typeface
        /// </summary>
        /// <param name="key">The key to be fetched</param>
        /// <returns>The found Typeface</returns>
        Typeface GetTypeface(string key);
    }
}