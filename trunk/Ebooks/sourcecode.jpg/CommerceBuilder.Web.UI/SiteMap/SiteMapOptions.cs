using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

namespace CommerceBuilder.Web.SiteMap
{
    public class SiteMapOptions
    {
        private string _SiteMapFileName = "SiteMap.xml";
        private bool _OverwriteSiteMapFile = true;
        private bool _IncludeProducts = true;
        private bool _IncludeCategories = true;
        private bool _IncludeWebpages = true;
        private string _CompressedSiteMapFileName = "SitemMap.xml.gz";
        private bool _OverwriteCompressedFile = true;
        private string _SiteMapDataPath = "";
        private decimal _DefaultUrlPriority = 0.5M;
        private changefreq _DefaultChangeFrequency = changefreq.weekly;
        
        private bool IsDirty = false;

        public string SiteMapFileName
        {
            get { return _SiteMapFileName; }
            set
            {
                if (!_SiteMapFileName.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _SiteMapFileName = value;
                    IsDirty = true;
                }
            }
        }

        public bool OverwriteSiteMapFile
        {
            get { return _OverwriteSiteMapFile; }
            set
            {
                if (!_OverwriteSiteMapFile.Equals(value))
                {
                    _OverwriteSiteMapFile = value;
                    IsDirty = true;
                }
            }
        }

        public bool IncludeProducts
        {
            get { return _IncludeProducts; }
            set
            {
                if (!_IncludeProducts.Equals(value))
                {
                    _IncludeProducts = value;
                    IsDirty = true;
                }
            }
        }

        public bool IncludeWebpages
        {
            get { return _IncludeWebpages; }
            set
            {
                if (!_IncludeWebpages.Equals(value))
                {
                    _IncludeWebpages = value;
                    IsDirty = true;
                }
            }
        }

        public bool IncludeCategories
        {
            get { return _IncludeCategories; }
            set
            {
                if (!_IncludeCategories.Equals(value))
                {
                    _IncludeCategories = value;
                    IsDirty = true;
                }
            }
        }

        public string CompressedSiteMapFileName
        {
            get { return _CompressedSiteMapFileName; }
            set
            {
                if (!_CompressedSiteMapFileName.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _CompressedSiteMapFileName = value;
                    IsDirty = true;
                }
            }
        }

        public bool OverwriteCompressedFile
        {
            get { return _OverwriteCompressedFile; }
            set
            {
                if (!_OverwriteCompressedFile.Equals(value))
                {
                    _OverwriteCompressedFile = value;
                    IsDirty = true;
                }
            }
        }


        public string SiteMapDataPath
        {
            get { return _SiteMapDataPath; }
            set
            {
                if (!_SiteMapDataPath.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _SiteMapDataPath = value;
                    IsDirty = true;
                }
            }
        }

        public changefreq DefaultChangeFrequency
        {
            get { return _DefaultChangeFrequency; }
            set
            {
                if (_DefaultChangeFrequency != value)
                {
                    _DefaultChangeFrequency = value;
                    IsDirty = true;
                }
            }
        }

        public decimal DefaultUrlPriority
        {
            get { return _DefaultUrlPriority; }
            set
            {
                if (value >= 0 && value <= 1 && value != _DefaultUrlPriority)
                {
                    _DefaultUrlPriority = value;
                    IsDirty = true;
                }
            }
        }

        public void Load(ISiteMapOptionKeys settingKeys)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            _CompressedSiteMapFileName = settings.GetValueByKey(settingKeys.CompressedSiteMapFileName);
            _SiteMapDataPath = settings.GetValueByKey(settingKeys.SiteMapDataPath);
            _SiteMapFileName = settings.GetValueByKey(settingKeys.SiteMapFileName);
            _IncludeProducts = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.IncludeProducts), true);
            _IncludeCategories = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.IncludeCategories), true);
            _IncludeWebpages = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.IncludeWebpages), true);
            _OverwriteCompressedFile = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.OverwriteCompressedFile), true);
            _OverwriteSiteMapFile = AlwaysConvert.ToBool(settings.GetValueByKey(settingKeys.OverwriteSiteMapFile), true);
            _DefaultChangeFrequency = (changefreq)AlwaysConvert.ToEnum(typeof(changefreq), settings.GetValueByKey(settingKeys.DefaultChangeFrequency), changefreq.weekly);
            _DefaultUrlPriority = AlwaysConvert.ToDecimal(settings.GetValueByKey(settingKeys.DefaultUrlPriority), 0.5M);
            IsDirty = false;
        }

        public void Save(ISiteMapOptionKeys settingKeys)
        {
            if (IsDirty)
            {
                StoreSettingCollection settings = Token.Instance.Store.Settings;
                settings.SetValueByKey(settingKeys.CompressedSiteMapFileName, CompressedSiteMapFileName);
                settings.SetValueByKey(settingKeys.SiteMapDataPath, SiteMapDataPath);
                settings.SetValueByKey(settingKeys.SiteMapFileName, SiteMapFileName);
                settings.SetValueByKey(settingKeys.IncludeProducts, IncludeProducts.ToString());
                settings.SetValueByKey(settingKeys.IncludeCategories, IncludeCategories.ToString());
                settings.SetValueByKey(settingKeys.IncludeWebpages, IncludeWebpages.ToString());
                settings.SetValueByKey(settingKeys.OverwriteCompressedFile, OverwriteCompressedFile.ToString());
                settings.SetValueByKey(settingKeys.OverwriteSiteMapFile, OverwriteSiteMapFile.ToString());
                settings.SetValueByKey(settingKeys.DefaultChangeFrequency, DefaultChangeFrequency.ToString());
                settings.SetValueByKey(settingKeys.DefaultUrlPriority, DefaultUrlPriority.ToString());
                settings.Save();
            }
        }
    }
}
