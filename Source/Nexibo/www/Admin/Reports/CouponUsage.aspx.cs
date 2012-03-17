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

public partial class Admin_Reports_CouponUsage : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime localNow = LocaleHelper.LocalNow;
            StartDate.SelectedDate = new DateTime(localNow.Year, 1, 1);
            EndDate.SelectedDate = localNow;
            BindReport();
        }
    }

    protected void BindReport() 
    {
        HiddenStartDate.Value = StartDate.SelectedStartDate.ToString();
        HiddenEndDate.Value = EndDate.SelectedEndDate.ToString();
        CouponSalesGrid.DataBind();
        ReportFromDate.Text = string.Format(ReportFromDate.Text, StartDate.SelectedDate);
        ReportFromDate.Visible = true;
        ReportToDate.Text = string.Format(ReportToDate.Text, EndDate.SelectedDate);
        ReportToDate.Visible = true;
    }

    protected void CouponSalesGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != CouponSalesGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        BindReport();
    }

}
