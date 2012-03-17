using System;
using System.Web;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;
using CommerceBuilder.Common;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Default implementation of IUrlRewriter
    /// </summary>
    public class DefaultUrlRewriter : IUrlRewriter
    {
        private const string REWRITE_PATTERN = "(.*)/.*-(C|c|P|p|W|w|L|l)(\\d+)(?:[Cc](\\d+))?\\.aspx(?:\\?(.*))?";
        private int _CacheSize;
        private KeyedCollection<string, CustomUrl> _CustomUrls;
        private int _CustomUrlCount;
        bool _ValidCache = false;

        /// <summary>
        /// Gets the configured cache size
        /// </summary>
        public int CacheSize
        {
            get
            {
                return this._CacheSize;
            }
        }

        /// <summary>
        /// Creates a new instance of DefaultUrlRewriter
        /// </summary>
        public DefaultUrlRewriter() : this(1000) { }

        /// <summary>
        /// Creates a new instance of DefaultUrlRewriter
        /// </summary>
        /// <param name="cacheSize">The maximum number of custom urls that can be stored in memory 
        /// at one time.  This value never needs to be higher than the number of custom URLs configured.</param>
        public DefaultUrlRewriter(int cacheSize)
        {
            this._CacheSize = cacheSize;
        }

        /// <summary>
        /// Returns the rewritten URL for the given source URL
        /// </summary>
        /// <param name="sourceUrl">The given source URL</param>
        /// <returns>The rewritten URL</returns>
        public string RewriteUrl(string sourceUrl)
        {
            // VALIDATE INPUT
            if (string.IsNullOrEmpty(sourceUrl)) return string.Empty;

            // GET APPLICATION PATH
            string appPath = "/";
            HttpRequest httpRequest = HttpContextHelper.SafeGetRequest();
            if (httpRequest != null)
            {
                appPath = httpRequest.ApplicationPath;
                if (!appPath.EndsWith("/")) appPath += "/";
            }

            // CHECK FOR CUSTOM URL
            string appRelativeUrl = UrlHelper.GetAppRelativeUrl(sourceUrl, appPath);
            CustomUrl customUrl = LoadCustomUrl(appRelativeUrl);
            if (customUrl != null)
            {
                string query = UrlHelper.GetQueryString(sourceUrl);
                return DoCustomRewrite(appPath, customUrl, query);
            }
            else
            {
                return DoDefaultRewrite(sourceUrl);
            }
        }

        private CustomUrl LoadCustomUrl(string resourceUrl)
        {
            // MAKE SURE WE HAVE A VALID CACHE
            if (!_ValidCache) this.InitializeCache();

            // CHECK FOR CUSTOM URL IN CACHE
            string loweredSourceUrl = resourceUrl.ToLowerInvariant();
            if (this._CustomUrls.Contains(loweredSourceUrl))
            {
                // RETURN CACHED ITEM
                return this._CustomUrls[loweredSourceUrl];
            }
            // SEE IF WE HAVE MORE CUSTOM URLS IN DATABASE
            if (this._CacheSize < this._CustomUrlCount)
            {
                // TRY TO FIND CUSTOM URL BY QUERY
                CustomUrl customUrl = CustomUrlDataSource.LoadCustomUrl(loweredSourceUrl);
                if (customUrl != null)
                {
                    // CUSTOM URL FOUND, KICK OUT THE LAST ITEM IN CACHE
                    this._CustomUrls.RemoveAt(this._CustomUrls.Count - 1);

                    // PUT THE NEW ITEM TO THE TOP OF CACHE
                    this._CustomUrls.Insert(0, customUrl);

                    // RETURN THE NEW CUSTOM URL
                    return customUrl;
                }
            }

            // NO REWRITE FOUND FOR THE GIVEN URL
            return null;
        }

        public string DoCustomRewrite(string serverPath, CustomUrl customUrl, string query)
        {
            StringBuilder dynamicUrl = new StringBuilder();
            string displayPage = string.Empty;
            switch (customUrl.CatalogNodeType)
            {
                case CatalogNodeType.Category:
                    displayPage = CatalogDataSource.GetDisplayPage(customUrl.CatalogNodeId, CatalogNodeType.Category);
                    dynamicUrl.Append(serverPath);
                    dynamicUrl.Append(displayPage);
                    dynamicUrl.Append("?CategoryId=" + customUrl.CatalogNodeId);
                    break;

                case CatalogNodeType.Product:
                    displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(customUrl.CatalogNodeId), CatalogNodeType.Product);
                    dynamicUrl.Append(serverPath);
                    dynamicUrl.Append(displayPage);
                    dynamicUrl.Append("?ProductId=" + customUrl.CatalogNodeId);
                    break;

                case CatalogNodeType.Webpage:
                    displayPage = CatalogDataSource.GetDisplayPage(customUrl.CatalogNodeId, CatalogNodeType.Webpage);
                    dynamicUrl.Append(serverPath);
                    dynamicUrl.Append(displayPage);
                    dynamicUrl.Append("?WebpageId=" + customUrl.CatalogNodeId);
                    break;

                case CatalogNodeType.Link:
                    displayPage = CatalogDataSource.GetDisplayPage(customUrl.CatalogNodeId, CatalogNodeType.Link);
                    dynamicUrl.Append(serverPath);
                    dynamicUrl.Append(displayPage);
                    dynamicUrl.Append("?LinkId=" + customUrl.CatalogNodeId);
                    break;
            }
            //PRESERVE QUERY STRING
            if (!string.IsNullOrEmpty(query))
                dynamicUrl.Append("&" + query);
            return dynamicUrl.ToString();
        }

        public string DoDefaultRewrite(string sourceUrl)
        {
            // CHECK FOR URL FITTING AC PATTERN
            Match urlMatch;
            urlMatch = Regex.Match(sourceUrl, REWRITE_PATTERN, (RegexOptions.IgnoreCase & RegexOptions.Compiled));
            if (urlMatch.Success)
            {
                // URL FOUND, PARSE AND REWRITE
                string serverPath = (urlMatch.Groups[1].ToString() + "/");
                string objectType = urlMatch.Groups[2].ToString();
                string objectId = urlMatch.Groups[3].ToString();
                string categoryId = urlMatch.Groups[4].ToString();
                string urlParams = urlMatch.Groups[5].ToString();
                string displayPage = string.Empty;
                // DISPLAY PAGE NOT PRESENT, LOOK UP FROM TABLE
                // TODO:LOOKUP DISPLAY PAGE FROM RULES
                // Dim displayPage As String = RewriteUrlLookupEngine.GetDisplayPage(app, objectType, objectId)
                StringBuilder dynamicUrl = new StringBuilder();
                switch (objectType)
                {
                    case "C":
                    case "c":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Category);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?CategoryId=" + objectId))));
                        break;
                    case "P":
                    case "p":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Product);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?ProductId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                    case "W":
                    case "w":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Webpage);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?WebpageId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                    case "L":
                    case "l":
                        displayPage = CatalogDataSource.GetDisplayPage(AlwaysConvert.ToInt(objectId), CatalogNodeType.Link);
                        dynamicUrl.Append((serverPath
                                        + (displayPage + ("?LinkId=" + objectId))));
                        if ((categoryId.Length > 0))
                        {
                            dynamicUrl.Append(("&CategoryId=" + categoryId));
                        }
                        break;
                }
                if ((urlParams.Length > 0))
                {
                    dynamicUrl.Append(("&" + urlParams));
                }
                return dynamicUrl.ToString();
            }
            return sourceUrl;
        }

        /// <summary>
        /// Invalidate the cache
        /// </summary>
        public void ReloadCache()
        {
            _ValidCache = false;
        }

        /// <summary>
        /// Initialize the cache
        /// </summary>
        protected void InitializeCache()
        {
            // WE LOAD FIXED REDIRECTS UP TO CACHE SIZE
            _CustomUrls = new KeyedCustomUrlCollection();
            CustomUrlCollection tempCustomUrls = CustomUrlDataSource.LoadForStore(this._CacheSize, 0);
            foreach (CustomUrl item in tempCustomUrls)
            {
                _CustomUrls.Add(item);
            }
            _CustomUrlCount = CustomUrlDataSource.CountForStore();
            _ValidCache = true;
        }

        // KeyedCollection is an abstract class, so have to derive
        private class KeyedCustomUrlCollection : KeyedCollection<string, CustomUrl>
        {
            protected override string GetKeyForItem(CustomUrl item)
            {
                return item.LoweredUrl;
            }
        }
    }
}