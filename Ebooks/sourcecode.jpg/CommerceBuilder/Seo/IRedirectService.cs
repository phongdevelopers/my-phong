using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Seo
{
    public interface IRedirectService
    {
        Redirect LocateRedirect(string sourceUrl);
        void ReloadCache();
    }
}
