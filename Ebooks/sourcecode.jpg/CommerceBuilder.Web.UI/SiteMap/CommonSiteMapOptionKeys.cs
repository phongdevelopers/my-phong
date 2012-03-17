using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Web.SiteMap
{
    public class CommonSiteMapOptionKeys : ISiteMapOptionKeys
    {
        public string SiteMapFileName { get { return "CommonSiteMap_SiteMapFileName"; } }
        public string OverwriteSiteMapFile { get { return "CommonSiteMap_OverwriteSiteMapFile"; } }
        public string IncludeProducts { get { return "CommonSiteMap_IncludeProducts"; } }
        public string IncludeCategories { get { return "CommonSiteMap_IncludeCategories"; } }
        public string IncludeWebpages { get { return "CommonSiteMap_IncludeWebpages"; } }
        public string CompressedSiteMapFileName { get { return "CommonSiteMap_CompressedSiteMapFileName"; } }
        public string OverwriteCompressedFile { get { return "CommonSiteMap_OverwriteCompressedFile"; } }
        public string SiteMapDataPath { get { return "CommonSiteMap_SiteMapDataPath"; } }
        public string DefaultChangeFrequency { get { return "CommonSiteMap_DefaultChangeFrequency"; } }
        public string DefaultUrlPriority { get { return "CommonSiteMap_DefaultUrlPriority"; } }
    }
}
