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

public partial class Admin_Reports_TopProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime localNow = LocaleHelper.LocalNow;
            StartDate.SelectedDate = new DateTime(localNow.Year, 1, 1);
            EndDate.SelectedDate = localNow;

            DateTime startDate = AlwaysConvert.ToDateTime(Request.QueryString["StartDate"],DateTime.MinValue);
            DateTime endDate =  AlwaysConvert.ToDateTime(Request.QueryString["EndDate"],DateTime.MaxValue);
            if (startDate != DateTime.MinValue)
                StartDate.SelectedDate = startDate;
            if (endDate != DateTime.MaxValue)
                EndDate.SelectedDate = endDate;
            BindReport();
        }
    }

    protected void BindReport() 
    {
        HiddenStartDate.Value = StartDate.SelectedStartDate.ToString();
        HiddenEndDate.Value = EndDate.SelectedEndDate.ToString();
        TopProductGrid.DataBind();
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, StartDate.SelectedDate,EndDate.SelectedDate);
    }

    protected void TopProductGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if ((e.SortExpression != TopProductGrid.SortExpression) && (e.SortExpression != "Name")) e.SortDirection = SortDirection.Descending;
    }

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        BindReport();    
    }

}
