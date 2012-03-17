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
using WebChart;

public partial class Admin_Dashboard_ActivityByHour : System.Web.UI.UserControl
{
    private DateTime _CacheDate;
    private bool _ForceRefresh = false;

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Token.Instance.User.IsInRole(CommerceBuilder.Users.Role.ReportAdminRoles))
        {
            initLast24HourChart(_ForceRefresh);
            initHourChart(_ForceRefresh);
            initDayChart(_ForceRefresh);
            initMonthChart(_ForceRefresh);
            CacheDate1.Text = string.Format(CacheDate1.Text, _CacheDate);
            CacheDate2.Text = string.Format(CacheDate2.Text, _CacheDate);
            CacheDate3.Text = string.Format(CacheDate3.Text, _CacheDate);
            CacheDate4.Text = string.Format(CacheDate4.Text, _CacheDate);
        }
        else this.Controls.Clear();
    }

    private void initLast24HourChart(bool forceRefresh)
    {
        string cacheKey = "15B642D6-FB16-4027-989C-6F40FA821A73";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<int, int>> viewsByHour = PageViewDataSource.GetViewsByHour(true, DateTime.UtcNow.AddHours(-24));
            //RESULTS ARE SORTED FROM 0 (MIDNIGHT) TO 23 (11PM)
            int thisHour = LocaleHelper.LocalNow.Hour;
            //SHIFT SO IT GOES FOR PAST 24 HOURS
            for (int i = 0; i <= thisHour; i++)
            {
                KeyValuePair<int, int> tempCount = viewsByHour[0];
                viewsByHour.RemoveAt(0);
                viewsByHour.Add(tempCount);
            }
            //CREATE CHART
            WebChart.ColumnChart chart = (ColumnChart)Last24HoursChart.Charts[0];
            chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
            chart.Fill.EndPoint = new System.Drawing.Point(100, 400);
            //BUILD THE CHART DATA
            for (int i = 0; i < viewsByHour.Count; i++)
            {
                string dayName;
                int hour = viewsByHour[i].Key;
                if (hour == 0)
                {
                    dayName = "12a";
                }
                else if (hour == 12)
                {
                    dayName = "12p";
                }
                else if (hour > 12)
                {
                    hour -= 12;
                    dayName = hour.ToString() + "p";
                }
                else dayName = hour.ToString() + "a";
                chart.Data.Add(new ChartPoint(dayName, viewsByHour[i].Value));
            }
            //ADJUST COLUMN PADDING
            int internalHeight = (int)Last24HoursChart.Height.Value - (int)Last24HoursChart.TopPadding - (int)Last24HoursChart.TopChartPadding - (int)Last24HoursChart.BottomChartPadding - (int)(Last24HoursChart.ChartPadding * 2);
            int columnHeight = (internalHeight / viewsByHour.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
            //BIND THE CHART
            Last24HoursChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(Last24HoursChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            Last24HoursChart.ImageID = (string)cacheWrapper.CacheValue;
        }

        _CacheDate = cacheWrapper.CacheDate;
    }

    private void initHourChart(bool forceRefresh)
    {
        string cacheKey = "59A0ABAC-9204-49ab-A333-85340024E802";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<int, int>> viewsByHour = PageViewDataSource.GetViewsByHour(true);
            //RESULTS ARE SORTED FROM 0 (MIDNIGHT) TO 23 (11PM)
            //SHIFT SO IT GOES FROM 6AM TO 5AM INSTEAD
            for (int i = 0; i < 6; i++)
            {
                KeyValuePair<int, int> tempCount = viewsByHour[0];
                viewsByHour.RemoveAt(0);
                viewsByHour.Add(tempCount);
            }
            //CREATE CHART
            WebChart.ColumnChart chart = (ColumnChart)ViewsByHourChart.Charts[0];
            chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
            chart.Fill.EndPoint = new System.Drawing.Point(100, 400);
            //BUILD THE CHART DATA
            for (int i = 0; i < viewsByHour.Count; i++)
            {
                string dayName;
                int hour = viewsByHour[i].Key;
                if (hour == 0)
                {
                    dayName = "12a";
                }
                else if (hour == 12)
                {
                    dayName = "12p";
                }
                else if (hour > 12)
                {
                    hour -= 12;
                    dayName = hour.ToString() + "p";
                }
                else dayName = hour.ToString() + "a";
                chart.Data.Add(new ChartPoint(dayName, viewsByHour[i].Value));
            }
            //ADJUST COLUMN PADDING
            int internalHeight = (int)ViewsByHourChart.Height.Value - (int)ViewsByHourChart.TopPadding - (int)ViewsByHourChart.TopChartPadding - (int)ViewsByHourChart.BottomChartPadding - (int)(ViewsByHourChart.ChartPadding * 2);
            int columnHeight = (internalHeight / viewsByHour.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
            //BIND THE CHART
            ViewsByHourChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(ViewsByHourChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            ViewsByHourChart.ImageID = (string)cacheWrapper.CacheValue;
        }

        _CacheDate = cacheWrapper.CacheDate;
    }

    private void initDayChart(bool forceRefresh)
    {
        string cacheKey = "D7A9D943-D24B-47ad-9E43-559390C14156";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<int, int>> viewsByDay = PageViewDataSource.GetViewsByDay(true);
            //CREATE CHART
            WebChart.ColumnChart chart = (ColumnChart)ViewsByDayChart.Charts[0];
            chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
            chart.Fill.EndPoint = new System.Drawing.Point(100, 400);
            //BUILD THE CHART DATA
            for (int i = 0; i < viewsByDay.Count; i++)
            {
                chart.Data.Add(new ChartPoint(viewsByDay[i].Key.ToString(), viewsByDay[i].Value));
            }
            //ADJUST COLUMN PADDING
            int internalHeight = (int)ViewsByDayChart.Height.Value - (int)ViewsByDayChart.TopPadding - (int)ViewsByDayChart.TopChartPadding - (int)ViewsByDayChart.BottomChartPadding - (int)(ViewsByDayChart.ChartPadding * 2);
            int columnHeight = (internalHeight / viewsByDay.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
            //BIND THE CHART
            ViewsByDayChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(ViewsByDayChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            ViewsByDayChart.ImageID = (string)cacheWrapper.CacheValue;
        }
        _CacheDate = cacheWrapper.CacheDate;
    }

    private void initMonthChart(bool forceRefresh)
    {
        string cacheKey = "69F41EE8-327B-401c-BE1A-A2F9208BC257";
        CacheWrapper cacheWrapper = Cache[cacheKey] as CacheWrapper;
        if (forceRefresh || (cacheWrapper == null))
        {
            //LOAD VIEWS
            SortableCollection<KeyValuePair<int, int>> viewsByMonth = PageViewDataSource.GetViewsByMonth(true);
            //CREATE CHART
            WebChart.ColumnChart chart = (ColumnChart)ViewsByMonthChart.Charts[0];
            chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
            chart.Fill.EndPoint = new System.Drawing.Point(100, 400);
            //define values of series to be charted
            for (int i = 0; i < viewsByMonth.Count; i++)
            {
                chart.Data.Add(new ChartPoint(viewsByMonth[i].Key.ToString(), viewsByMonth[i].Value));
            }
            //ADJUST COLUMN PADDING
            int internalHeight = (int)ViewsByMonthChart.Height.Value - (int)ViewsByMonthChart.TopPadding - (int)ViewsByMonthChart.TopChartPadding - (int)ViewsByMonthChart.BottomChartPadding - (int)(ViewsByMonthChart.ChartPadding * 2);
            int columnHeight = (internalHeight / viewsByMonth.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
            //BIND THE CHART
            ViewsByMonthChart.RedrawChart();
            //CACHE THE DATA
            cacheWrapper = new CacheWrapper(ViewsByMonthChart.ImageID);
            Cache.Remove(cacheKey);
            Cache.Add(cacheKey, cacheWrapper, null, DateTime.UtcNow.AddMinutes(5).AddSeconds(-1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        else
        {
            //USE CACHED VALUES
            ViewsByMonthChart.ImageID = (string)cacheWrapper.CacheValue;
        }
        _CacheDate = cacheWrapper.CacheDate;
    }

    protected void RefreshLink_Click(object sender, EventArgs e)
    {
        _ForceRefresh = true;
    }
}
