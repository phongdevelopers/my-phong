using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Web.SiteMap
{
    public class SiteMapUrl
    {
        private string _loc;
        private DateTime _lastmod;
        private bool _changeFrequencySpecified = false;
        private changefreq _changefreq;
        private decimal _priority;
        
        public SiteMapUrl()
        {
            _loc = string.Empty;
            _lastmod = DateTime.MinValue;
            //_changefreq = changefreq.weekly;
            _changeFrequencySpecified = false;
            _priority = 0.5M;
        }

        public SiteMapUrl(string urlLocation) : this()
        {
            _loc = urlLocation;
        }

        public SiteMapUrl(string urlLocation, DateTime lastMod, changefreq changeFreq, decimal priority)
        {
            _loc = urlLocation;
            _lastmod = lastMod;
            _changefreq = changeFreq;
            _changeFrequencySpecified = true;
            _priority = priority;
        }

        public SiteMapUrl(string urlLocation, changefreq changeFreq, decimal priority)
        {
            _loc = urlLocation;
            _lastmod = DateTime.MinValue;
            _changefreq = changeFreq;
            _changeFrequencySpecified = true;
            _priority = priority;
        }

        public string Location
        {
            get { return _loc; }
            set { _loc = value; }
        }

        public DateTime LastModified
        {
            get { return _lastmod; }
            set { _lastmod = value; }
        }

        public changefreq ChangeFrequency
        {
            get { return _changefreq; }
            set { _changefreq = value; }
        }

        public bool ChangeFrequencySpecified
        {
            get { return _changeFrequencySpecified; }
            set { _changeFrequencySpecified = value; }
        }

        public decimal Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

    }

    /*public enum ChangeFrequency
    {
        always,
        hourly,
        daily,
        weekly,
        monthly,
        yearly,
        never,
        unknown
    }*/
}
