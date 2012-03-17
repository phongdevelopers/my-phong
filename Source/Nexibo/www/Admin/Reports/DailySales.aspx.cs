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

public partial class Admin_Reports_DailySales : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    //CONTAINS THE REPORT
    List<OrderSummary> _DailySales;
    //TRACKS TOTALS FOR FOOTER
    bool _TotalsCalculated;
    LSDecimal _ProductTotal;
    LSDecimal _ShippingTotal;
    LSDecimal _TaxTotal;
    LSDecimal _DiscountTotal;
    LSDecimal _CouponTotal;
    LSDecimal _OtherTotal;
    LSDecimal _GrandTotal;
    LSDecimal _ProfitTotal;
    
    protected void BindReport()
    {
        //GET THE REPORT DATE
        DateTime reportDate = (DateTime)ViewState["ReportDate"];
        //UPDATE REPORT CAPTION
        ReportCaption.Visible = true;
        ReportCaption.Text = string.Format(ReportCaption.Text, reportDate);
        //RESET THE TOTALS
        _TotalsCalculated = false;
        //GET NEW DATA
        _DailySales = ReportDataSource.GetDailySales(reportDate);
        //BIND GRID
        DailySalesGrid.DataSource = _DailySales;
        DailySalesGrid.DataBind();
    }

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

    protected void ProcessButton_Click(Object sender, EventArgs e)
    {
        DateTime newReportDate = ReportDate.SelectedDate;
        ViewState["ReportDate"] = newReportDate;
        BindReport();
    }

    protected string GetTotal(string itemType)
    {
        if (!_TotalsCalculated)
        {
            _ProductTotal = 0;
            _ShippingTotal = 0;
            _TaxTotal = 0;
            _DiscountTotal = 0;
            _CouponTotal = 0;
            _OtherTotal = 0;
            _ProfitTotal = 0;
            _GrandTotal = 0;
            foreach (OrderSummary os in _DailySales)
            {
                _ProductTotal += os.ProductTotal;
                _ShippingTotal += os.ShippingTotal;
                _TaxTotal += os.TaxTotal;
                _DiscountTotal += os.DiscountTotal;
                _CouponTotal += os.CouponTotal;
                _OtherTotal += os.OtherTotal;
                _ProfitTotal += os.ProfitTotal;
                _GrandTotal += os.GrandTotal;
            }
            _TotalsCalculated = true;
        }
        switch (itemType.ToLowerInvariant())
        {
            case "product": return string.Format("{0:lc}", _ProductTotal);
            case "shipping": return string.Format("{0:lc}", _ShippingTotal);
            case "tax": return string.Format("{0:lc}", _TaxTotal);
            case "discount": return string.Format("{0:lc}", _DiscountTotal);
            case "coupon": return string.Format("{0:lc}", _CouponTotal);
            case "other": return string.Format("{0:lc}", _OtherTotal);
            case "grand": return string.Format("{0:lc}", _GrandTotal);
            case "profit": return string.Format("{0:lc}", _ProfitTotal);                
        }
        return itemType;
    }
}
