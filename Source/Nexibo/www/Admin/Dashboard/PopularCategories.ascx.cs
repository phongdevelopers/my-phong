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
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Reporting;
using CommerceBuilder.Utility;
using WebChart;

public partial class Admin_Dashboard_PopularCategories : System.Web.UI.UserControl
{
    private bool _ForceRefresh = false;
    private int _Size = 5;

    [Personalizable(), WebBrowsable()]
    public int Size
    {
        get { return _Size; }
        set { _Size = value; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Token.Instance.User.IsInRole(CommerceBuilder.Users.Role.ReportAdminRoles))
        {
            if (_Size < 1) _Size = 5;
            initViewChart(_ForceRefresh);
        }
        else this.Controls.Clear();
    }

    private void initViewChart(bool forceRefresh)
    {
        string cacheKey = "3C26BAC7-1D53-40ef-920B-5BDB705F363B";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))
        {
            SortableCollection<KeyValuePair<ICatalogable, int>> categoryViews = PageViewDataSource.GetViewsByCategory(_Size, 0, "ViewCount DESC");
            if (categoryViews.Count > 0)
            {
                //BUILD BAR CHART
                WebChart.ColumnChart chart = (ColumnChart)ViewsChart.Charts[0];
                chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
                chart.Fill.EndPoint = new System.Drawing.Point((int)ViewsChart.Height.Value, (int)ViewsChart.Width.Value);
                for (int i = 0; i < categoryViews.Count; i++)
                {
                    chart.Data.Add(new ChartPoint(categoryViews[i].Key.Name, categoryViews[i].Value));
                }
                int internalHeight = (int)ViewsChart.Height.Value - (int)ViewsChart.TopPadding - (int)ViewsChart.TopChartPadding - (int)ViewsChart.BottomChartPadding - (int)(ViewsChart.ChartPadding * 2);
                int columnHeight = (internalHeight / categoryViews.Count);
                columnHeight -= 3;
                if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
                //BIND THE CHART
                ViewsChart.RedrawChart();
                //BIND THE DATA GRID
                ViewsGrid.DataSource = categoryViews;
                ViewsGrid.DataBind();
                //CACHE THE DATA
                viewsData = new Dictionary<string, object>();
                viewsData["ImageID"] = ViewsChart.ImageID;
                viewsData["DataSource"] = categoryViews;
                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            else
            {
                //NO CATEGORIES HAVE BEEN VIEWED YET OR PAGE TRACKING IS NOT AVAIALBEL
                this.Controls.Clear();
                Panel noViewsPanel = new Panel();
                noViewsPanel.CssClass = "emptyData";
                Label noViewsMessage = new Label();
                noViewsMessage.Text = "No categories have been viewed or page tracking is disabled.";
                noViewsPanel.Controls.Add(noViewsMessage);
                this.Controls.Add(noViewsPanel);
            }
        }
        else
        {
            //USE CACHED VALUES
            viewsData = (Dictionary<string, object>)cacheWrapper.CacheValue;
            ViewsChart.ImageID = (string)viewsData["ImageID"];
            ViewsGrid.DataSource = (SortableCollection<KeyValuePair<ICatalogable, int>>)viewsData["DataSource"];
            ViewsGrid.DataBind();
        }
        DateTime cacheDate = (cacheWrapper != null) ? cacheWrapper.CacheDate : LocaleHelper.LocalNow;
        CacheDate1.Text = string.Format(CacheDate1.Text, cacheDate);
        CacheDate2.Text = string.Format(CacheDate2.Text, cacheDate);
    }

    protected void RefreshLink_Click(object sender, EventArgs e)
    {
        _ForceRefresh = true;
    }

}
