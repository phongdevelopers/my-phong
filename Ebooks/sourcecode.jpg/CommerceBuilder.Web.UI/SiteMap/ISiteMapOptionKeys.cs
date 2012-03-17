using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Web.SiteMap
{
    public interface ISiteMapOptionKeys
    {
        string SiteMapFileName { get; }
        string OverwriteSiteMapFile { get; }
        string IncludeProducts { get; }
        string IncludeCategories { get; }
        string IncludeWebpages { get; }
        string CompressedSiteMapFileName { get; }
        string OverwriteCompressedFile { get; }
        string SiteMapDataPath { get; }
        string DefaultChangeFrequency { get;}
        string DefaultUrlPriority { get;}
    }
}
