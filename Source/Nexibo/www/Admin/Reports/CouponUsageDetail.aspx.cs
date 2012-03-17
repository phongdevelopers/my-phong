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
using CommerceBuilder.Marketing;

public partial class Admin_Reports_CouponUsageDetail : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    private int _CouponCount = -1;
    protected int CouponCount
    {
        get
        {
            if (_CouponCount < 0)
            {
                _CouponCount = CouponDataSource.CountForStore();
            }
            return _CouponCount;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string tempDateFilter = Request.QueryString["DateFilter"];
            if (string.IsNullOrEmpty(tempDateFilter)) tempDateFilter = "THISMONTH";
            UpdateDateFilter(tempDateFilter);

            DateTime startDate = AlwaysConvert.ToDateTime(Request.QueryString["StartDate"], DateTime.MinValue);
            DateTime endDate = AlwaysConvert.ToDateTime(Request.QueryString["EndDate"], DateTime.MaxValue);
            if (startDate != DateTime.MinValue)
                StartDate.SelectedDate = startDate;
            if (endDate != DateTime.MaxValue)
                EndDate.SelectedDate = endDate;

            //BIND THE COUPON LIST
            CouponList.DataSource = OrderDataSource.GetCouponCodes();
            CouponList.DataBind();
            //INITIALIZE COUPON LIST
            string couponCode = Request.QueryString["CouponCode"];
            ListItem listItem = CouponList.Items.FindByValue(couponCode);
            if (listItem != null) CouponList.SelectedIndex = CouponList.Items.IndexOf(listItem);
            //GENERATE REPORT ON FIRST VISIT
            ReportButton_Click(sender, e);
        }
    }

    private void UpdateDateFilter(string filter)
    {
        DateTime startDate, endDate;
        StoreDataHelper.SetDateFilter(filter, out startDate, out endDate);
        StartDate.SelectedDate = startDate;
        EndDate.SelectedDate = endDate;
    }

    protected void DateFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDateFilter(DateFilter.SelectedValue);
        DateFilter.SelectedIndex = 0;
    }
   
    protected void ReportButton_Click(object sender, EventArgs e)
    {
        //UPDATE REPORT CAPTION
        if (StartDate.SelectedDate > DateTime.MinValue)
        {
            ReportFromDate.Text = string.Format(ReportFromDate.Text, StartDate.SelectedDate);
            ReportFromDate.Visible = true;
        }
        if (EndDate.SelectedEndDate > DateTime.MinValue)
        {
            ReportToDate.Text = string.Format(ReportToDate.Text, EndDate.SelectedEndDate);
            ReportToDate.Visible = true;
        }
        ReportTimestamp.Text = string.Format(ReportTimestamp.Text, LocaleHelper.LocalNow);
        SummaryLink.NavigateUrl = string.Format(SummaryLink.NavigateUrl, 0);
        //GET SUMMARIES
        CouponSalesRepeater.Visible = true;
        CouponSalesRepeater.DataBind();
        ReportAjax.Update();
    }

    protected OrderCollection GetCouponOrders(object dataItem)
    {
        CouponSummary summary = (CouponSummary)dataItem;
        return OrderDataSource.LoadForCouponCode(summary.CouponCode, summary.StartDate, summary.EndDate, "O.OrderId ASC");
    }

}
