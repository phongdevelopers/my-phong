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

public partial class Admin_Reports_SalesByAffiliateDetail : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    private int _AffiliateCount = -1;
    protected int AffiliateCount
    {
        get
        {
            if (_AffiliateCount < 0)
            {
                _AffiliateCount = AffiliateDataSource.CountForStore();
            }
            return _AffiliateCount;
        }
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
            string tempDate = Request.QueryString["ReportDate"];
            DateTime reportDate = localNow;
            if ((!string.IsNullOrEmpty(tempDate)) && (tempDate.Length == 8))
            {
                try {
                    int month = AlwaysConvert.ToInt(tempDate.Substring(0,2));
                    int day = AlwaysConvert.ToInt(tempDate.Substring(2, 2));
                    int year = AlwaysConvert.ToInt(tempDate.Substring(4, 4));
                    reportDate = new DateTime(year, month, day);
                } catch { }
            }
            ViewState["ReportDate"] = reportDate;
            //BIND THE AFFILAITE LIST
            AffiliateList.DataSource = AffiliateDataSource.LoadForStore("Name");
            AffiliateList.DataBind();
            //INITIALIZE AFFILIATE LIST
            int affiliateId = AlwaysConvert.ToInt(Request.QueryString["AffiliateId"]);
            ListItem listItem = AffiliateList.Items.FindByValue(affiliateId.ToString());
            if (listItem != null) AffiliateList.SelectedIndex = AffiliateList.Items.IndexOf(listItem);
            //UPDATE DATE FILTER AND GENERATE REPORT
            UpdateDateFilter();
        }
    }

    protected void UpdateDateFilter()
    {
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        YearList.SelectedIndex = -1;
        ListItem yearItem = YearList.Items.FindByValue(reportDate.Year.ToString());
        if (yearItem != null) yearItem.Selected = true;
        MonthList.SelectedIndex = -1;
        ListItem monthItem = MonthList.Items.FindByValue(reportDate.Month.ToString());
        if (monthItem != null) monthItem.Selected = true;
        GenerateReport();
    }
    
    protected void DateFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["ReportDate"] = new DateTime(AlwaysConvert.ToInt(YearList.SelectedValue), AlwaysConvert.ToInt(MonthList.SelectedValue), 1);
        GenerateReport();
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1)).AddMonths(1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateFilter();
    }

    protected void PreviousButton_Click(object sender, EventArgs e)
    {
        DateTime newReportDate = (new DateTime(Convert.ToInt32(YearList.SelectedValue), Convert.ToInt32(MonthList.SelectedValue), 1)).AddMonths(-1);
        ViewState["ReportDate"] = newReportDate;
        UpdateDateFilter();
    }

    private void GenerateReport()
    {
        //GET THE REPORT DATE
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        DateTime startOfMonth = new DateTime(reportDate.Year, reportDate.Month, 1);
        DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
        HiddenStartDate.Value = startOfMonth.ToString();
        HiddenEndDate.Value = endOfMonth.ToString();
        //UPDATE REPORT CAPTION
        ReportDateCaption.Visible = true;
        ReportDateCaption.Text = string.Format(ReportDateCaption.Text, startOfMonth);
        ReportTimestamp.Text = string.Format(ReportTimestamp.Text, LocaleHelper.LocalNow);
        SummaryLink.NavigateUrl = string.Format(SummaryLink.NavigateUrl, startOfMonth);
        //GET SUMMARIES
        AffiliateSalesRepeater.Visible = true;
        AffiliateSalesRepeater.DataBind();
    }

    protected string GetCommissionRate(object dataItem)
    {
        AffiliateSalesSummary summary = (AffiliateSalesSummary)dataItem;
        Affiliate affiliate = summary.Affiliate;
        if (affiliate.CommissionIsPercent)
        {
            string format = "{0:0.##}% of {1:lc}";
            if (affiliate.CommissionOnTotal) return string.Format(format, affiliate.CommissionRate, summary.OrderTotal);
            return string.Format(format, affiliate.CommissionRate, summary.ProductSubtotal);
        }
        return string.Format("{0} x {1:lc}", summary.OrderCount, affiliate.CommissionRate);
    }

    protected string GetConversionRate(object dataItem)
    {
        AffiliateSalesSummary summary = (AffiliateSalesSummary)dataItem;
        if (summary.ReferralCount == 0) return "-";
        return string.Format("{0:0.##}%", summary.ConversionRate);
    }

    protected string GetOrderTotal(object dataItem)
    {
        AffiliateSalesSummary summary = (AffiliateSalesSummary)dataItem;
        Affiliate affiliate = summary.Affiliate;
        if (affiliate.CommissionIsPercent)
        {
            if (affiliate.CommissionOnTotal) return summary.OrderTotal.ToString("lc");
            return summary.ProductSubtotal.ToString("lc");
        }
        return summary.OrderTotal.ToString("lc");
    }

    protected OrderCollection GetAffiliateOrders(object dataItem)
    {
        AffiliateSalesSummary summary = (AffiliateSalesSummary)dataItem;
        return OrderDataSource.LoadForAffiliate(summary.AffiliateId, summary.StartDate, summary.EndDate, "OrderId ASC");
    }

}
