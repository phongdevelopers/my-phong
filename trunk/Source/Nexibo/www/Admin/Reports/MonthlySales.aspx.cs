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

public partial class Admin_Reports_MonthlySales : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    //CONTAINS THE REPORT
    List<SalesSummary> _MonthlySales;
    //TRACKS TOTALS FOR FOOTER
    bool _TotalsCalculated;
    int _OrderCount;
    LSDecimal _ProductTotal;
    LSDecimal _ShippingTotal;
    LSDecimal _TaxTotal;
    LSDecimal _DiscountTotal;
    LSDecimal _CouponTotal;
    LSDecimal _GiftWrapTotal;
    LSDecimal _OtherTotal;
    LSDecimal _GrandTotal;
    LSDecimal _ProfitTotal;

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
        //RESET THE TOTALS
        _TotalsCalculated = false;
        //GET SUMMARIES
        _MonthlySales = ReportDataSource.GetMonthlySales(reportDate.Year, reportDate.Month);

        //UPDATE CHART
        UpdateChart();

        // FILTER OUT RESULTS FOR GRID DISPLAY
        if (FilterResults.Checked)
        {
            for (int i = (_MonthlySales.Count - 1); i >= 0; i--)
            {
                if (_MonthlySales[i].OrderCount == 0) _MonthlySales.RemoveAt(i);
            }
        }
        MonthlySalesGrid.DataSource = _MonthlySales;
        MonthlySalesGrid.DataBind();        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("pragma", "no-store,no-cache");
        Response.AppendHeader("cache-control", "no-cache, no-store,must-revalidate, max-age=-1");
        Response.AppendHeader("expires", "-1");
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
    }

    protected void UpdateChart()
    {
        //BUILD BAR CHART
        WebChart.ColumnChart chart = (ColumnChart)SalesChart.Charts[0];
        chart.Data.Clear();
        chart.Fill.StartPoint = new System.Drawing.Point(0, 0);
        chart.Fill.EndPoint = new System.Drawing.Point((int)SalesChart.Height.Value, (int)SalesChart.Width.Value);
        for (int i = 0; i < _MonthlySales.Count; i++)
        {
            chart.Data.Add(new ChartPoint(((int)(i + 1)).ToString(), (float)((decimal)_MonthlySales[i].GrandTotal)));
        }
        int internalHeight = (int)SalesChart.Height.Value - (int)SalesChart.TopPadding - (int)SalesChart.TopChartPadding - (int)SalesChart.BottomChartPadding - (int)(SalesChart.ChartPadding * 2);
        if (_MonthlySales.Count > 0)
        {
            int columnHeight = (internalHeight / _MonthlySales.Count);
            columnHeight -= 3;
            if (columnHeight > 8) chart.MaxColumnWidth = columnHeight;
        }
        //BIND THE CHART
        SalesChart.RedrawChart();
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

    protected string GetTotal(string itemType)
    {
        if (!_TotalsCalculated)
        {
            _OrderCount = 0;
            _ProductTotal = 0;
            _ShippingTotal = 0;
            _TaxTotal = 0;
            _DiscountTotal = 0;
            _CouponTotal = 0;
            _OtherTotal = 0;
            _ProfitTotal = 0;
            _GiftWrapTotal = 0;
            _GrandTotal = 0;
            foreach (SalesSummary ss in _MonthlySales)
            {
                _OrderCount += ss.OrderCount;
                _ProductTotal += ss.ProductTotal;
                _ShippingTotal += ss.ShippingTotal;
                _TaxTotal += ss.TaxTotal;
                _DiscountTotal += ss.DiscountTotal;
                _CouponTotal += ss.CouponTotal;
                _OtherTotal += ss.OtherTotal;
                _ProfitTotal += ss.ProfitTotal;
                _GiftWrapTotal += ss.GiftWrapTotal;
                _GrandTotal += ss.GrandTotal;
            }
            _TotalsCalculated = true;
        }
        switch (itemType.ToLowerInvariant())
        {
            case "count": return _OrderCount.ToString();
            case "product": return string.Format("{0:lc}", _ProductTotal);
            case "shipping": return string.Format("{0:lc}", _ShippingTotal);
            case "tax": return string.Format("{0:lc}", _TaxTotal);
            case "discount": return string.Format("{0:lc}", _DiscountTotal);
            case "coupon": return string.Format("{0:lc}", _CouponTotal);
            case "giftwrap": return string.Format("{0:lc}", _GiftWrapTotal);
            case "other": return string.Format("{0:lc}", _OtherTotal);
            case "grand": return string.Format("{0:lc}", _GrandTotal);
            case "profit": return string.Format("{0:lc}", _ProfitTotal);
        }
        return itemType;
    }

    protected void MonthList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1));
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void YearList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1));
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

    protected void FilterResults_CheckedChanged(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1));
        ViewState["ReportDate"] = newReportDate;
        UpdateDateControls();
    }

}
