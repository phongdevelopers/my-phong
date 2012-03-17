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
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;

public partial class Admin_People_Users_OrderHistoryDialog : System.Web.UI.UserControl
{
    private int _UserId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UserId"]);
        OrderGrid.DataSource = OrderDataSource.LoadForUser(_UserId, "OrderDate DESC");
        OrderGrid.DataBind();
    }
    protected string GetOrderStatus(Object orderStatusId)
    {
        OrderStatus status = OrderStatusDataSource.Load((int)orderStatusId);
        if (status != null) return status.Name;
        return string.Empty;
    }
    protected string GetPaymentStatus(object dataItem)
    {
        Order order = (Order)dataItem;
        if (order.PaymentStatus == OrderPaymentStatus.Paid) return "Paid";
        if (order.Payments.Count > 0)
        {
            order.Payments.Sort("PaymentDate");
            Payment lastPayment = order.Payments[order.Payments.Count - 1];
            return StringHelper.SpaceName(lastPayment.PaymentStatus.ToString());
        }
        return "Unpaid";
    }
    protected void OrderGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Order order = (Order)e.Row.DataItem;

            PlaceHolder phPaymentStatus = PageHelper.RecursiveFindControl(e.Row, "phPaymentStatus") as PlaceHolder;
            if (phPaymentStatus != null)
            {
                Image paymentStatus = new Image();
                paymentStatus.SkinID = GetPaymentStatusSkinID(order);
                phPaymentStatus.Controls.Add(paymentStatus);
            }
            PlaceHolder phShipmentStatus = PageHelper.RecursiveFindControl(e.Row, "phShipmentStatus") as PlaceHolder;
            if (phShipmentStatus != null)
            {
                Image shipmentStatus = new Image();
                shipmentStatus.SkinID = GetShipmentStatusSkinID(order);
                phShipmentStatus.Controls.Add(shipmentStatus);
            }
        }
    }
    protected string GetPaymentStatusSkinID(Order order)
    {
        if (!order.OrderStatus.IsValid) return "CodeRed";
        switch (order.PaymentStatus)
        {
            case OrderPaymentStatus.Unspecified:
            case OrderPaymentStatus.Unpaid:
                return "CodeYellow";
            case OrderPaymentStatus.Problem:
                return "CodeRed";
            default:
                return "CodeGreen";
        }
    }
    protected string GetShipmentStatusSkinID(Order order)
    {
        if (!order.OrderStatus.IsValid) return "CodeRed";
        switch (order.ShipmentStatus)
        {
            case OrderShipmentStatus.Unspecified:
            case OrderShipmentStatus.Unshipped:
                return "CodeYellow";
            default:
                return "CodeGreen";
        }
    }

    protected void OrderGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        OrderGrid.PageIndex = e.NewPageIndex;
        OrderGrid.DataBind();
    }
}
