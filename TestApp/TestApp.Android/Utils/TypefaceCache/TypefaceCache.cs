namespace TestApp.Droid.Utils.TypefaceCache
{

    /// <summary>
    /// Caches for typefaces as a Singleton. 
    /// This is used for performance reasons. 
    /// </summary>
    public static class TypefaceCache
    {
        private static ITypefaceCache _typefaceCache;


        /// <summary>
        /// The shared typeface cache as a Singleton
        /// </summary>
        public static ITypefaceCache SharedCache
        {
            get => _typefaceCache ?? new DefaultTypefaceCache();
            set
            {
                if (_typefaceCache != null && _typefaceCache.GetType() == typeof(DefaultTypefaceCache))
                    ((DefaultTypefaceCache)_typefaceCache).Clear();

                _typefaceCache = value;
            }
        }


    }
}