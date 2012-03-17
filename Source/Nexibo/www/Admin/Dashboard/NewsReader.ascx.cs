using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using CommerceBuilder.Utility;

public partial class Admin_Dashboard_NewsReader : System.Web.UI.UserControl
{
    private DateTime _CacheDate;

    protected List<RssNewsItem> GetNewsItems(bool forceRefresh)
    {
        List<RssNewsItem> newsItems;
        CacheWrapper cacheWrapper = Cache["AbleRssNews"] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            XmlDocument AbleRssXml = new XmlDocument();
            try
            {
                AbleRssXml.Load("http://www.ablecommerce.com/ac6rss.xml");
            }
            catch
            {
                AbleRssXml.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss></rss>");
            }
            newsItems = new List<RssNewsItem>();
            XmlNodeList newsItemNodes = AbleRssXml.SelectNodes("rss/channel/item");
            foreach (XmlNode node in newsItemNodes)
            {
                XmlElement ele = (XmlElement)node;
                RssNewsItem newsItem = new RssNewsItem();
                newsItem.PubDate = AlwaysConvert.ToDateTime(XmlUtility.GetElementValue(ele, "pubDate"), DateTime.UtcNow);
                newsItem.Link = XmlUtility.GetElementValue(ele, "link");
                newsItem.Title = XmlUtility.GetElementValue(ele, "title");
                newsItem.Description = XmlUtility.GetElementValue(ele, "description");
                newsItems.Add(newsItem);
            }
            cacheWrapper = new CacheWrapper(newsItems);
            Cache.Remove("AbleRssNews");
            Cache.Add("AbleRssNews", cacheWrapper, null, DateTime.UtcNow.AddDays(1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
        }
        else
        {
            newsItems = cacheWrapper.CacheValue as List<RssNewsItem>;
        }
        _CacheDate = cacheWrapper.CacheDate;
        return newsItems;
    }

    protected void BindRssList(bool forceRefresh)
    {
        RssList.DataSource = GetNewsItems(forceRefresh);
        RssList.DataBind();
        CachedAt.Text = string.Format("{0:f}", _CacheDate);
    }

    public class RssNewsItem
    {
        private DateTime _PubDate;
        private string _Link;
        private string _Title;
        private string _Description;
        public DateTime PubDate
        {
            get { return _PubDate; }
            set { _PubDate = value; }
        }
        public string Link
        {
            get { return _Link; }
            set { _Link = value; }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        BindRssList(false);
    }

    protected void RefreshButton_Click(object sender, EventArgs e)
    {
        BindRssList(true);
    }
}
