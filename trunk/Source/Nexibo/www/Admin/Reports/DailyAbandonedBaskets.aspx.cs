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

public partial class Admin_Reports_DailyAbandonedBaskets : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime tempDate = AlwaysConvert.ToDateTime(Request.QueryString["Date"], System.DateTime.MinValue);
            if (tempDate == System.DateTime.MinValue) tempDate = LocaleHelper.LocalNow;
            ViewState["ReportDate"] = tempDate;
            ReportDate.SelectedDate = tempDate;
            BindReport();
        }
    }

    protected void BindReport() 
    {
        DateTime reportDate = AlwaysConvert.ToDateTime(ViewState["ReportDate"],DateTime.MinValue);
        List<CommerceBuilder.Reporting.BasketSummary> abandonedBaskets = ReportDataSource.GetDailyAbandonedBaskets(reportDate.Year, reportDate.Month, reportDate.Day);
        DailyAbandonedBasketsGrid.DataSource = abandonedBaskets;
        DailyAbandonedBasketsGrid.DataBind();
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, ReportDate.SelectedDate);
    }
    
    protected int GetUserId(int basketId)
    {
        Basket basket = BasketDataSource.Load(basketId);
        if (basket != null)
        {
            return basket.UserId;
        }
        return basketId;
    }

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        DateTime newReportDate = ReportDate.SelectedDate;
        ViewState["ReportDate"] = newReportDate;
        BindReport();
    }
}
