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
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Utility;
using WebChart;

public partial class Admin_Dashboard_PopularProducts : System.Web.UI.UserControl
{
    private bool _ForceRefresh = false;
    private int _Size = 8;

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
            if (_Size < 1) _Size = 8;
            //Initialize the chart data            
            initSalesChart(_ForceRefresh);
            initViewsChart(_ForceRefresh);
        }
        else this.Controls.Clear();
    }

    private void initSalesChart(bool forceRefresh)
    {
        string cacheKey = "0AD3A3DA-15F1-4f43-82A3-C0AC3262399D";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> salesData;
        if (forceRefresh || (cacheWrapper == null))
        {
            //GET SALES
            DateTime localNow = LocaleHelper.LocalNow;
            DateTime last60Days = (new DateTime(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0)).AddDays(-60);
            List<ProductSummary> productSales = ReportDataSource.GetSalesByProduct(last60Days, DateTime.MaxValue, _Size, 0, "TotalPrice DESC");
            if (productSales.Count > 0)
            {
                //BUILD BAR CHART
                WebChart.ColumnChart chart = (ColumnChart)SalesChart.Charts[0];
                chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
                chart.Fill.EndPoint = new System.Drawing.Point((int)SalesChart.Height.Value, (int)SalesChart.Width.Value);
                for (int i = 0; i < productSales.Count; i++)
                {
                    int roundedTotal = (int)Math.Round((double)productSales[i].TotalPrice, 0);
                    chart.Data.Add(new ChartPoint(productSales[i].Name, roundedTotal));
                }
                int internalHeight = (int)SalesChart.Height.Value - (int)SalesChart.TopPadding - (int)SalesChart.TopChartPadding - (int)SalesChart.BottomChartPadding - (int)(SalesChart.ChartPadding * 2);
                int columnHeight = (internalHeight / productSales.Count);
                columnHeight -= 3;
                if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
                //BIND THE CHART
                SalesChart.RedrawChart();
                //BIND THE DATA GRID
                SalesGrid.DataSource = productSales;
                SalesGrid.DataBind();
                //CACHE THE DATA
                salesData = new Dictionary<string, object>();
                salesData["ImageID"] = SalesChart.ImageID;
                salesData["DataSource"] = productSales;
                cacheWrapper = new CacheWrapper(salesData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            else
            {
                //NO PRODUCTS HAVE BEEN SOLD YET
                Control container = SalesChart.Parent;
                container.Controls.Clear();
                Panel noViewsPanel = new Panel();
                noViewsPanel.CssClass = "emptyData";
                Label noViewsMessage = new Label();
                noViewsMessage.Text = "No products have been sold yet.";
                noViewsPanel.Controls.Add(noViewsMessage);
                container.Controls.Add(noViewsPanel);
                //REMOVE SALES DATA TAB
                TabStrip1.Tabs.RemoveAt(1);
                MultiPage1.PageViews.RemoveAt(1);
            }
        }
        else
        {
            //USE CACHED VALUES
            salesData = (Dictionary<string, object>)cacheWrapper.CacheValue;
            SalesChart.ImageID = (string)salesData["ImageID"];
            SalesGrid.DataSource = (List<ProductSummary>)salesData["DataSource"];
            SalesGrid.DataBind();
        }
        DateTime cacheDate = (cacheWrapper != null) ? cacheWrapper.CacheDate : LocaleHelper.LocalNow;
        CacheDate1.Text = string.Format(CacheDate1.Text, cacheDate);
        CacheDate2.Text = string.Format(CacheDate2.Text, cacheDate);
    }

    private void initViewsChart(bool forceRefresh)
    {
        string cacheKey = "242003F5-5A58-44e9-BFB0-C077C6BEDBF2";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        Dictionary<string, object> viewsData;
        if (forceRefresh || (cacheWrapper == null))
        {
            //GET VIEWS
            SortableCollection<KeyValuePair<ICatalogable, int>> productViews = PageViewDataSource.GetViewsByProduct(_Size, 0, "ViewCount DESC");
            if (productViews.Count > 0)
            {
                //BUILD BAR CHART
                WebChart.ColumnChart chart = (ColumnChart)ViewsChart.Charts[0];
                chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
                chart.Fill.EndPoint = new System.Drawing.Point((int)ViewsChart.Height.Value, (int)ViewsChart.Width.Value);
                for (int i = 0; i < productViews.Count; i++)
                {
                    chart.Data.Add(new ChartPoint(((ICatalogable)productViews[i].Key).Name, productViews[i].Value));
                }
                int internalHeight = (int)ViewsChart.Height.Value - (int)ViewsChart.TopPadding - (int)ViewsChart.TopChartPadding - (int)ViewsChart.BottomChartPadding - (int)(ViewsChart.ChartPadding * 2);
                int columnHeight = (internalHeight / productViews.Count);
                columnHeight -= 3;
                if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
                //BIND THE CHART
                ViewsChart.RedrawChart();
                //BIND THE DATA GRID
                ViewsGrid.DataSource = productViews;
                ViewsGrid.DataBind();
                //CACHE THE DATA
                viewsData = new Dictionary<string, object>();
                viewsData["ImageID"] = ViewsChart.ImageID;
                viewsData["DataSource"] = productViews;
                cacheWrapper = new CacheWrapper(viewsData);
                Cache.Remove(cacheKey);
                Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            }
            else
            {
                //NO PRODUCTS HAVE BEEN VIEWED YET OR PAGE TRACKING IS DISABLED
                Control container = ViewsChart.Parent;
                container.Controls.Clear();
                Panel noViewsPanel = new Panel();
                noViewsPanel.CssClass = "emptyData";
                Label noViewsMessage = new Label();
                noViewsMessage.Text = "No products have been viewed yet or page tracking is disabled.";
                noViewsPanel.Controls.Add(noViewsMessage);
                container.Controls.Add(noViewsPanel);
                //REMOVE VIEWS DATA TAB
                TabStrip1.Tabs.RemoveAt(TabStrip1.Tabs.Count - 1);
                MultiPage1.PageViews.RemoveAt(MultiPage1.PageViews.Count-1);
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
        CacheDate3.Text = string.Format(CacheDate3.Text, cacheDate);
        CacheDate4.Text = string.Format(CacheDate4.Text, cacheDate);
    }

    protected void RefreshLink_Click(object sender, EventArgs e)
    {
        _ForceRefresh = true;
    }
}
