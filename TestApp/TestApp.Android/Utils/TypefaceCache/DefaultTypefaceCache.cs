using System.Collections.Generic;
using Android.Graphics;

namespace TestApp.Droid.Utils.TypefaceCache
{



    internal class DefaultTypefaceCache : ITypefaceCache
    {
        private Dictionary<string, Typeface> _cacheDict;


        /// <summary>
        /// Default implementation of the typeface cache.
        /// </summary>
        public DefaultTypefaceCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }


        public Typeface GetTypeface(string key)
        {
            if (_cacheDict.ContainsKey(key))
            {
                return _cacheDict[key];
            }
            else
            {
                return null;
            }
        }

        public void SetTypeface(string key, Typeface typeface)
        {
            _cacheDict[key] = typeface;
        }

        public void RemoveTypeface(string key)
        {
            _cacheDict.Remove(key);
        }

        public void Clear()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }

    }

}