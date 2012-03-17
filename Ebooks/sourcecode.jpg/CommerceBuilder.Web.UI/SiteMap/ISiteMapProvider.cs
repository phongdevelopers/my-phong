using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Web.SiteMap
{
    public interface ISiteMapProvider
    {
        bool CreateSiteMap(SiteMapOptions options, ref List<string> messages);
        bool CreateAndCompressSiteMap(SiteMapOptions options, ref List<string> messages);        
    }
}
