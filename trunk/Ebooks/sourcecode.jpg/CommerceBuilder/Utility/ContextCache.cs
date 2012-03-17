using System.Web;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Configuration;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// The cache for the current HTTP request context
    /// </summary>
    public static class ContextCache
    {
        private static int _ContextCacheSize;
        
        static ContextCache()
        {
            AbleCommerceApplicationSection appConfig = AbleCommerceApplicationSection.GetSection();
            _ContextCacheSize = appConfig.ContextCacheSize;
        }

        /// <summary>
        /// Gets an object form the cache for the given key
        /// </summary>
        /// <param name="key">Key for which to get the object from cache</param>
        /// <returns>Cached object for the specified key or null if the object is not found</returns>
        public static object GetObject(string key)
        {
            HttpContext context = HttpContext.Current;
            if ((context != null) && Token.Instance.EnableRequestCaching)
            {
                string actualKey = "requestCache_" + key;
                if (context.Items.Contains(actualKey)) return context.Items[actualKey];
            }
            return null;
        }

        /// <summary>
        /// Sets an object in the cache for the specified key
        /// </summary>
        /// <param name="key">Key for which to set the object in the cache</param>
        /// <param name="value">The value to set</param>
        public static void SetObject(string key, object value)
        {
            HttpContext context = HttpContext.Current;
            if ((context != null) && (Token.Instance.EnableRequestCaching))
            {
                if (context.Items.Count <= _ContextCacheSize)
                {
                    string actualKey = "requestCache_" + key;
                    context.Items[actualKey] = value;
                }
            }
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public static void ClearCache()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                List<string> removeKeys = new List<string>();
                foreach (string key in context.Items.Keys)
                {
                    if (key.StartsWith("requestCache_")) removeKeys.Add(key);
                }
                foreach (string key in removeKeys)
                {
                    context.Items.Remove(key);
                }
            }
        }
    }
}
