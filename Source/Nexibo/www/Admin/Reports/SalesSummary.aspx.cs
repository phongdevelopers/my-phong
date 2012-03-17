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
using CommerceBuilder.Reporting;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

public partial class Admin_Reports_SalesSummary : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        DateTime fromDate = PickerAndCalendar1.SelectedStartDate;
        DateTime toDate = PickerAndCalendar2.SelectedEndDate;

        // if(fromDate == DateTime.MinValue) fromDate 
        SalesSummary salesSummary = ReportDataSource.GetSalesSummary(fromDate, toDate, true);

        if (salesSummary != null)
        {
            trResults.Visible = true;
            ProductSales.Text = String.Format("{0:lc}", salesSummary.ProductTotal);
            ProductDiscounts.Text = String.Format("{0:lc}", salesSummary.DiscountTotal);
            ProductSalesLessDiscounts.Text = String.Format("{0:lc}", salesSummary.ProductTotal + salesSummary.DiscountTotal);
            GiftWrapCharges.Text = String.Format("{0:lc}", salesSummary.GiftWrapTotal);
            CouponsRedeemed.Text = String.Format("{0:lc}", salesSummary.CouponTotal);
            TaxesCollected.Text = String.Format("{0:lc}", salesSummary.TaxTotal);
            ShippingCharges.Text = String.Format("{0:lc}", salesSummary.ShippingTotal);
            TotalCharges.Text = String.Format("{0:lc}", salesSummary.TotalReceivables);
            TotalOrders.Text = salesSummary.OrderCount.ToString();
            TotalItemsSold.Text = salesSummary.ProductCount.ToString();
            NumberOfCustomers.Text = salesSummary.UserCount.ToString();

            LSDecimal avgOrderAmount = 0;
            if (salesSummary.OrderCount > 0)
            {
                avgOrderAmount = salesSummary.GrandTotal / salesSummary.OrderCount;
            }
            AverageOrderAmount.Text = String.Format("{0:lc}", avgOrderAmount);
        }
    }
}
