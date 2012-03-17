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
using CommerceBuilder.Utility;
using System.Collections.Generic;
using CommerceBuilder.Common;

public partial class Admin_People_Users_PurchaseHistoryDialog : System.Web.UI.UserControl
{
    private int _UserId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        List<PurchaseSummary> paidItems = (List<PurchaseSummary>)PaidOrdersDs.Select();
        paidItems.Sort(new PurchaseSummaryComparer(SortDirection.Ascending));
        
        List<PurchaseSummary> unpaidItems = (List<PurchaseSummary>)UnpaidOrdersDs.Select();
        unpaidItems.Sort(new PurchaseSummaryComparer(SortDirection.Ascending));

        DateTime firstOrderDate = DateTime.MinValue;
        if (paidItems.Count > 0)
            firstOrderDate = paidItems[0].OrderDate;
        if (unpaidItems.Count > 0)
        {
            if (unpaidItems[0].OrderDate < firstOrderDate)
                firstOrderDate = unpaidItems[0].OrderDate;
        }

        if (firstOrderDate != DateTime.MinValue)
            FirstOrder.Text = string.Format("{0:d}", firstOrderDate);


        PurchaseTotalSummary paidSummary = ReportDataSource.CalculatePurchaseHistoryTotals(paidItems);
        
        GrossProduct.Text = String.Format("{0:lc}", paidSummary.GrossProductsTotal);
        Discount.Text = String.Format("{0:lc}", (paidSummary.DiscountsTotal * -1));
        Coupon.Text = String.Format("{0:lc}", (paidSummary.CouponsTotal * -1));
        NetProduct.Text = String.Format("{0:lc}", paidSummary.NetProductTotal);

        ProfitLabel.Visible = paidSummary.CostOfGoodsSoldTotal > 0;
        Profit.Visible = paidSummary.CostOfGoodsSoldTotal > 0;

        Profit.Text = String.Format("{0:lc}", paidSummary.ProfitTotal);
        Taxes.Text = String.Format("{0:lc}", paidSummary.TaxesTotal);
        Shipping.Text = String.Format("{0:lc}", paidSummary.ShippingTotal);
        Other.Text = String.Format("{0:lc}", paidSummary.OtherTotal);
        TotalPayments.Text = String.Format("{0:lc}", paidSummary.TotalCharges);
        PurchasesToDate.Text = String.Format("{0:lc}", paidSummary.TotalCharges);
        
        PaidOrders.Text = paidSummary.OrderIds.Count.ToString();
        if (paidSummary.OrderIds.Count == 0) PaidOrdersPanel.Visible = false;


        PurchaseTotalSummary unpaidSummary = ReportDataSource.CalculatePurchaseHistoryTotals(unpaidItems);

        UnpaidGrossProduct.Text = String.Format("{0:lc}", unpaidSummary.GrossProductsTotal);
        UnpaidDiscount.Text = String.Format("{0:lc}", (unpaidSummary.DiscountsTotal * -1));
        UnpaidCoupon.Text = String.Format("{0:lc}", (unpaidSummary.CouponsTotal * -1));
        UnpaidNetProduct.Text = String.Format("{0:lc}", unpaidSummary.NetProductTotal);

        UnpaidProfitLabel.Visible = unpaidSummary.CostOfGoodsSoldTotal > 0;
        UnpaidProfit.Visible = unpaidSummary.CostOfGoodsSoldTotal > 0;

        UnpaidProfit.Text = String.Format("{0:lc}", unpaidSummary.ProfitTotal);
        UnpaidTaxes.Text = String.Format("{0:lc}", unpaidSummary.TaxesTotal);
        UnpaidShipping.Text = String.Format("{0:lc}", unpaidSummary.ShippingTotal);
        UnpaidOther.Text = String.Format("{0:lc}", unpaidSummary.OtherTotal);
        UnpaidTotalPayments.Text = String.Format("{0:lc}", unpaidSummary.UnpaidTotal);
        UnpaidPurchasedToDate.Text = String.Format("{0:lc}", unpaidSummary.TotalCharges);

        PendingOrders.Text = unpaidSummary.OrderIds.Count.ToString();
        if (unpaidSummary.OrderIds.Count == 0) UnpaidOrdersPanel.Visible = false;

        
    }

    protected void PaidOrderGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != PaidOrderGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    protected void UnPaidOrderGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (e.SortExpression != UnPaidOrderGrid.SortExpression) e.SortDirection = SortDirection.Descending;
    }

    public class PurchaseSummaryComparer : IComparer<PurchaseSummary>
    {
       
        SortDirection _SortDirection;
        public PurchaseSummaryComparer(SortDirection sortDirection)
        {
            _SortDirection = sortDirection;
        }

        #region IComparer Members
        public int Compare(PurchaseSummary x, PurchaseSummary y)
        {
            if (_SortDirection == SortDirection.Ascending)
                return x.OrderDate.CompareTo(y.OrderDate);
            return y.OrderDate.CompareTo(x.OrderDate);
        }
        #endregion
    }
}
