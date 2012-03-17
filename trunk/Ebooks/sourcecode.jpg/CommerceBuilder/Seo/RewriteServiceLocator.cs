using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Seo
{
    public static class RewriteServiceLocator
    {
        /// <summary>
        /// Gets the application instance of the rewrite service
        /// </summary>
        /// <param name="provider">The provider class ID</param>
        public static IUrlRewriter GetInstance(string provider)
        {
            IUrlRewriter service = null;

            // FIRST LOOK IN CACHE FOR AN EXISTING REWRITE SERVICE OBJECT
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                service = context.Cache["IRewriteService_" + provider] as IUrlRewriter;
                if (service != null)
                {
                    return service;
                }
            }

            // NO CACHED OBJECT FOUND, WHAT SERVICE ARE WE USING?
            if (provider == "CommerceBuilder.Catalog.DefaultUrlRewriter")
            {
                // THE DEFAULT SERVICE, MAKE SURE CACHE IS CONFIGURED
                service = new DefaultUrlRewriter(Store.GetCachedSettings().SeoCacheSize);
            }
            else
            {
                // SOME CUSTOM SERVICE, ATTEMPT TO CREATE AN INSTANCE
                try
                {
                    Type providerType = Type.GetType(provider);
                    service = (IUrlRewriter)Activator.CreateInstance(providerType);
                }
                catch (Exception ex)
                {
                    Logger.Error("Unable to create instance for URL rewriter: " + provider, ex);
                }
            }

            // IF WE WERE ABLE TO CREATE THE SERVICE AND WE HAVE HTTPCONTEXT, CACHE THE SERVICE
            if (service != null && context != null)
            {
                context.Cache.Remove("IRewriteService_" + provider);
                context.Cache.Add("IRewriteService_" + provider, service, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.High, null);
            }

            // RETURN SERVICE (NULL, IF WE COULD NOT CREATE IT)
            return service;
        }
    }
}