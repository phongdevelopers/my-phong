using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using System.Collections.Generic;
using WebChart;

public partial class Admin_Reports_MonthlyAbandonedBaskets : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    List<AbandonedBasketsSummary> _AbandonedBaskets;
        
    protected void UpdateDateControls()
    {
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        YearList.SelectedIndex = -1;
        ListItem yearItem = YearList.Items.FindByValue(reportDate.Year.ToString());
        if (yearItem != null) yearItem.Selected = true;
        MonthList.SelectedIndex = -1;
        ListItem monthItem = MonthList.Items.FindByValue(reportDate.Month.ToString());
        if (monthItem != null) monthItem.Selected = true;
        BindReport();
    }

    protected void BindReport()
    {
        //GET THE REPORT DATE
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        //UPDATE REPORT CAPTION
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, reportDate);
        //GET SUMMARIES
        _AbandonedBaskets = ReportDataSource.GetMonthlyAbandonedBaskets(reportDate.Year, reportDate.Month);

        //FILTER OUT RESULTS FOR GRID DISPLAY
        if (FilterResults.Checked)
        {
            for (int i = (_AbandonedBaskets.Count - 1); i >= 0; i--)
            {
                if (_AbandonedBaskets[i].BasketCount == 0) _AbandonedBaskets.RemoveAt(i);
            }
        }

        AbandonedBasketGrid.DataSource = _AbandonedBaskets;
        AbandonedBasketGrid.DataBind();
        //UPDATE CHART
        UpdateChart();
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime localNow = LocaleHelper.LocalNow;
            int currentYear = localNow.Year;
            for (int i = -10; i < 11; i++)
            {
                string thisYear = ((int)(currentYear + i)).ToString();
                YearList.Items.Add(new ListItem(thisYear, thisYear));
            }
            ViewState["ReportDate"] = localNow;
            UpdateDateControls();
        }
        else
        {
            DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1));
            ViewState["ReportDate"] = newReportDate;
            UpdateDateControls();
        }
    }

    protected void UpdateChart()
    {
        //BUILD BAR CHART
        WebChart.ColumnChart chart = (ColumnChart)BasketChart.Charts[0];
        chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
        chart.Fill.EndPoint = new System.Drawing.Point((int)BasketChart.Height.Value, (int)BasketChart.Width.Value);
        for (int i = 0; i < _AbandonedBaskets.Count; i++)
        {
            chart.Data.Add(new ChartPoint(((int)(i+1)).ToString(), _AbandonedBaskets[i].BasketCount));
        }
        int internalHeight = (int)BasketChart.Height.Value - (int)BasketChart.TopPadding - (int)BasketChart.TopChartPadding - (int)BasketChart.BottomChartPadding - (int)(BasketChart.ChartPadding * 2);
        if (_AbandonedBaskets.Count > 0)
        {
            int columnHeight = (internalHeight / _AbandonedBaskets.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
        }
        //BIND THE CHART
        BasketChart.RedrawChart();
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1)).AddMonths(1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void PreviousButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1)).AddMonths(-1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void FilterResults_CheckedChanged(object sender, EventArgs e)
    {
        UpdateDateControls();
    }
}
