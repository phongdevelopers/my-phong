using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Seo
{
    public class RedirectServiceLocator
    {
        public static IRedirectService Instance
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    IRedirectService service = context.Cache["IRedirectService"] as IRedirectService;
                    if (service == null)
                    {
                        service = new RedirectService(Store.GetCachedSettings().SeoCacheSize);
                        context.Cache.Remove("IRedirectService");
                        context.Cache.Add("IRedirectService", service, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.High, null);
                    }
                    return service;
                }
                return null;
            }
        }
    }
}
