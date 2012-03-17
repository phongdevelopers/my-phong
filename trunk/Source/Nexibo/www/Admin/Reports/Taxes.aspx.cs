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

public partial class Admin_Reports_Taxes : CommerceBuilder.Web.UI.AbleCommerceAdminPage
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

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        BindReport();
    }

    protected void BindReport() 
    {
        HiddenStartDate.Value = StartDate.SelectedStartDate.ToString();
        HiddenEndDate.Value = EndDate.SelectedEndDate.ToString();
        TaxesGrid.DataBind();
        ReportCaption.Text = string.Format(ReportCaption.Text, StartDate.SelectedDate, EndDate.SelectedDate);
        ReportCaption.Visible = true;
    }


    protected string GetTaxLink(object dataItem)
    {
        TaxReportSummaryItem summary = (TaxReportSummaryItem)dataItem;
        return string.Format("TaxDetail.aspx?T={0}&StartDate={1}&EndDate={2}", summary.TaxName, StartDate.SelectedDate.ToShortDateString(), EndDate.SelectedDate.ToShortDateString());
    }
}