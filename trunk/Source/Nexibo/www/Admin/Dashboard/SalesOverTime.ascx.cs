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
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;
using System.Drawing;
using WebChart;

public partial class Admin_Dashboard_SalesOverTime : System.Web.UI.UserControl
{
    private bool _ForceRefresh = false;
    private DateTime _CacheDate;

    protected DateTime CacheDate
    {
        get { return _CacheDate; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Token.Instance.User.IsInRole(CommerceBuilder.Users.Role.ReportAdminRoles))
        {
            //Initialize the chart data            
            initDayChart(_ForceRefresh);
            initMonthChart(_ForceRefresh);
        }
        else this.Controls.Clear();
    }

    private void initDayChart(bool forceRefresh)
    {
        string cacheKey = "E38012C3-C1A0-45a2-A0FF-F32D8DDE043F";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<DateTime, decimal>> salesByDay = ReportDataSource.GetSalesForPastDays(7, true);
            //BUILD BAR CHART
            WebChart.ColumnChart chart = (ColumnChart)SalesByDayChart.Charts[0];
            chart.Fill.StartPoint = new Point(0, 0);
            chart.Fill.EndPoint = new Point(100, 400);
            for (int i = 0; i < salesByDay.Count; i++)
            {
                int roundedTotal = (int)Math.Round(salesByDay[i].Value, 0);
                chart.Data.Add(new ChartPoint(salesByDay[i].Key.ToString("MMM d"), roundedTotal));
            }
            chart.MaxColumnWidth = 30;
            //BIND THE CHART
            SalesByDayChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(SalesByDayChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            SalesByDayChart.ImageID = (string)cacheWrapper.CacheValue;
        }
        _CacheDate = cacheWrapper.CacheDate;
    }

    private void initMonthChart(bool forceRefresh)
    {
        string cacheKey = "39C69D16-CD5A-4287-8DD4-E21019A78B8E";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<DateTime, decimal>> salesByMonth = ReportDataSource.GetSalesForPastMonths(6, true);
            //BUILD BAR CHART
            WebChart.ColumnChart chart = (ColumnChart)SalesByMonthChart.Charts[0];
            for (int i = 0; i < salesByMonth.Count; i++)
            {
                int roundedTotal = (int)Math.Round(salesByMonth[i].Value, 0);
                chart.Data.Add(new ChartPoint(salesByMonth[i].Key.ToString("MMM yy"), roundedTotal));
            }
            chart.MaxColumnWidth = 30;
            //BIND THE CHART
            SalesByMonthChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(SalesByMonthChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            SalesByMonthChart.ImageID = (string)cacheWrapper.CacheValue;
        }
        _CacheDate = cacheWrapper.CacheDate;
    }

    protected void RefreshLink_Click(object sender, EventArgs e)
    {
        initDayChart(_ForceRefresh);
        initMonthChart(_ForceRefresh);
    }
}

