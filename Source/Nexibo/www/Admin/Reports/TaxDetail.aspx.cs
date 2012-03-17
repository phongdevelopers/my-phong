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

public partial class Admin_Reports_TaxDetail : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // VALIDATE THE SPECIFIED TAX NAME
        string taxName = Request.QueryString["T"];
        if (!TaxReportDataSource.IsTaxNameValid(taxName)) Response.Redirect("Taxes.aspx");
        HiddenTaxName.Value = taxName;
        TaxNameLink.Text = taxName;

        if (!Page.IsPostBack)
        {
            // SET THE DEFAULT DATE FILTER
            StartDate.SelectedDate = AlwaysConvert.ToDateTime(Request.QueryString["StartDate"],DateTime.MinValue);
            EndDate.SelectedDate = AlwaysConvert.ToDateTime(Request.QueryString["EndDate"],DateTime.MaxValue);
            BindReport();    
        }
    }

    protected void BindReport() 
    {
        TaxesGrid.DataBind();
        ReportCaption.Text = string.Format(ReportCaption.Text, Server.HtmlEncode(HiddenTaxName.Value));
        ReportCaptionDateRange.Visible = true;
        ReportCaptionDateRange.Text = string.Format(ReportCaptionDateRange.Text, StartDate.SelectedDate, EndDate.SelectedDate);
    }

    protected string GetOrderLink(object dataItem)
    {
        TaxReportDetailItem detail = (TaxReportDetailItem)dataItem;
        return string.Format("~/Admin/Orders/ViewOrder.aspx?OrderId={0}&OrderNumber={1}", detail.OrderId, detail.OrderNumber);
    }

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        BindReport();
    }
}