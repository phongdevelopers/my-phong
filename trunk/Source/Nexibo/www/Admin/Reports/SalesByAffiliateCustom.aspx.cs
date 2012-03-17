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
using System.Text;

public partial class Admin_Reports_SalesByAffiliateCustom : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string tempDateFilter = Request.QueryString["DateFilter"];
            if (!string.IsNullOrEmpty(tempDateFilter))
            {
                int dateFilter = AlwaysConvert.ToInt(tempDateFilter);
                ListItem item = DateFilter.Items.FindByValue(dateFilter.ToString());
                if (item != null) DateFilter.SelectedIndex = DateFilter.Items.IndexOf(item);
                UpdateDateFilter();
            }
            else
            {
                //DEFAULT TO CURRENT DAY
                StartDate.SelectedDate = LocaleHelper.LocalNow;
            }
        }
    }

    protected void UpdateDateFilter()
    {
        int dateFilter = AlwaysConvert.ToInt(DateFilter.SelectedValue);
        DateTime fromDate;
        DateTime localNow = LocaleHelper.LocalNow;
        switch (dateFilter)
        {
            case 0:
                //today
                StartDate.SelectedDate = new DateTime(localNow.Year, localNow.Month, localNow.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 1:
                //this week
                DateTime firstDayOfWeek = localNow.AddDays(-1 * (int)localNow.DayOfWeek);
                StartDate.SelectedDate = new DateTime(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 2:
                //last week
                DateTime firstDayOfLastWeek = localNow.AddDays((-1 * (int)localNow.DayOfWeek) - 7);
                DateTime lastDayOfLastWeek = firstDayOfLastWeek.AddDays(6);
                StartDate.SelectedDate = new DateTime(firstDayOfLastWeek.Year, firstDayOfLastWeek.Month, firstDayOfLastWeek.Day);
                EndDate.SelectedDate = new DateTime(lastDayOfLastWeek.Year, lastDayOfLastWeek.Month, lastDayOfLastWeek.Day, 23, 59, 59);
                break;
            case 3:
                //this month
                StartDate.SelectedDate = new DateTime(localNow.Year, localNow.Month, 1);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 4:
                //last month
                DateTime lastMonth = localNow.AddMonths(-1);
                StartDate.SelectedDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                DateTime lastDayOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year,lastMonth.Month));
                EndDate.SelectedDate = lastDayOfLastMonth;
                break;
            case 5:
                //LAST 30 DAYS
                fromDate = localNow.AddDays(-30);
                StartDate.SelectedDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 6:
                //LAST 60 DAYS
                fromDate = localNow.AddDays(-60);
                StartDate.SelectedDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 7:
                //LAST 90 DAYS
                fromDate = localNow.AddDays(-90);
                StartDate.SelectedDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 8:
                //LAST 120 DAYS
                fromDate = localNow.AddDays(-120);
                StartDate.SelectedDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            case 9:
                //This Year
                StartDate.SelectedDate = new DateTime(localNow.Year, 1, 1);
                EndDate.SelectedDate = DateTime.MinValue;
                break;
            default:
                //DEFAULT TO ALL DATES
                StartDate.SelectedDate = DateTime.MinValue;
                EndDate.SelectedDate = DateTime.MinValue;
                break;
        }
        DateFilter.SelectedIndex = 0;
        GenerateReport();
    }
    
    protected void DateFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDateFilter();
    }

    protected void AffiliateSalesGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != AffiliateSalesGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected string GetDetailUrl(object dataItem)
    {
        AffiliateSalesSummary a = (AffiliateSalesSummary)dataItem;
        StringBuilder url = new StringBuilder();
        url.Append("SalesByAffiliateDetail.aspx?AffiliateId=" + a.AffiliateId.ToString());
        if (a.StartDate > DateTime.MinValue) url.Append(string.Format("&StartDate={0:MMddyyyy}", a.StartDate));
        if (a.EndDate > DateTime.MinValue) url.Append(string.Format("&EndDate={0:MMddyyyy}", a.EndDate));
        return url.ToString();
            
    }

    protected void ReportButton_Click(object sender, EventArgs e)
    {
        GenerateReport();
    }

    private void GenerateReport()
    {
        AffiliateSalesGrid.DataBind();
    }

    protected void AffiliateSalesGrid_DataBound(object sender, EventArgs e)
    {
        if (StartDate.SelectedDate > DateTime.MinValue)
        {
            ReportFrom.Text = string.Format(ReportFrom.Text, StartDate.SelectedDate);
            ReportFrom.Visible = true;
        }
        ReportTo.Visible = true;
        if (EndDate.SelectedDate > DateTime.MinValue) ReportTo.Text = string.Format(ReportTo.Text, EndDate.SelectedDate.ToString("d"));
        else ReportTo.Text = string.Format(ReportTo.Text, LocaleHelper.LocalNow.ToString("d"));
        ReportTimestamp.Text = string.Format(ReportTimestamp.Text, LocaleHelper.LocalNow);
    }

}
