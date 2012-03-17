using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Web;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Caching;
using CommerceBuilder.Common;


namespace CommerceBuilder.Seo
{
    public class RedirectService : IRedirectService
    {
        private int _CacheSize;
        private int _FixedRedirectCount;
        private KeyedCollection<string, Redirect> _FixedRedirects;
        private RedirectCollection _DynamicRedirects;
        private bool _ValidCache = false;

        /// <summary>
        /// Creates a new instance of RedirectService using default cache size.
        /// </summary>
        public RedirectService()
            : this(1000)
        {
        }

        /// <summary>
        /// Creates a new instance of RedirectService using specified cache size.
        /// </summary>
        /// <param name="cacheSize">The maximum number of redirects to retain in memory.</param>
        public RedirectService(int cacheSize)
        {
            this._CacheSize = cacheSize;
        }

        /// <summary>
        /// Gets the configured cache size.  This is readonly and must be set at initialization.
        /// </summary>
        public int CacheSize
        {
            get { return _CacheSize; }
        }

        /// <summary>
        /// Given the request url, return the Redirect that applies to this path.
        /// </summary>
        /// <param name="sourceUrl">The source url</param>
        public Redirect LocateRedirect(string sourceUrl)
        {
            // MAKE SURE WE HAVE A VALID CACHE
            if (!_ValidCache) this.InitializeCache();

            // CHECK FOR FIXED REDIRECT IN CACHE
            string loweredSourceUrl = sourceUrl.ToLowerInvariant();
            if (this._FixedRedirects.Contains(loweredSourceUrl))
            {
                // RETURN CACHED ITEM
                return this._FixedRedirects[loweredSourceUrl];
            }

            // SEE IF WE HAVE MORE FIXED REDIRECTS IN DATABASE
            if (this._CacheSize < this._FixedRedirectCount)
            {
                // TRY TO FIND A FIXED REDIRECT BY QUERY
                Redirect redirect = RedirectDataSource.LoadForSourceUrl(loweredSourceUrl);
                if (redirect != null)
                {
                    // REDIRECT FOUND, KICK OUT THE LAST ITEM IN CACHE
                    this._FixedRedirects.RemoveAt(this._FixedRedirects.Count - 1);

                    // PUT THE NEW ITEM TO THE TOP OF CACHE
                    this._FixedRedirects.Insert(0, redirect);

                    // RETURN THE NEW REDIRECT
                    return redirect;
                }
            }

            // LOOK THROUGH DYNAMIC REDIRECTS
            foreach (Redirect item in this._DynamicRedirects)
            {
                if (item.AppliesToUrl(sourceUrl))
                {
                    // A MATCHING REDIRECT IS FOUND, RETURN IT
                    return item;
                }
            }

            // NO REDIRECT FOUND FOR THE GIVEN URL
            return null;
        }

        protected void InitializeCache()
        {
            // WE ALWAYS LOAD ALL DYNAMIC REDIRECTS TO CACHE
            _DynamicRedirects = RedirectDataSource.LoadDynamicRedirects();

            // WE LOAD FIXED REDIRECTS UP TO CACHE SIZE
            _FixedRedirects = new KeyedRedirectCollection();
            RedirectCollection tempFixedRedirects = RedirectDataSource.LoadFixedRedirects(this._CacheSize, 0);
            foreach (Redirect item in tempFixedRedirects)
            {
                _FixedRedirects.Add(item);
            }
            _FixedRedirectCount = RedirectDataSource.CountFixedRedirects();
            _ValidCache = true;
        }

        /// <summary>
        /// Reloads 
        /// </summary>
        public void ReloadCache()
        {
            _ValidCache = false;
        }

        // KeyedCollection is an abstract class, so have to derive
        private class KeyedRedirectCollection : KeyedCollection<string, Redirect>
        {
            protected override string GetKeyForItem(Redirect item)
            {
                return item.LoweredSourceUrl;
            }
        }

    }
}