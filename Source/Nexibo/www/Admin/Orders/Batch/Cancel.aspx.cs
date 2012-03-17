using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Orders_Batch_Cancel : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private OrderCollection _Orders;
    private string _OrderNumbers;
    
    protected void LoadOrders(params int[] orderIds)
    {
        _Orders = new OrderCollection();
        foreach (int orderId in orderIds)
        {
            Order order = OrderDataSource.Load(orderId);
            if (order != null) _Orders.Add(order);
        }
        _Orders.Sort("OrderId");
        List<string> OrderNumbers = new List<string>();
        foreach (Order order in _Orders)
            OrderNumbers.Add(order.OrderNumber.ToString());
        _OrderNumbers = string.Join(", ", OrderNumbers.ToArray());
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = Token.Instance.Session.SelectedOrderIds;
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        LoadOrders(selectedOrders.ToArray());
        OrderGrid.DataSource = _Orders;
        OrderGrid.DataBind();
        OrderList.Text = _OrderNumbers;
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
        return string.Empty;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        foreach (Order order in _Orders)
        {
            if (!string.IsNullOrEmpty(Comment.Text))
            {
                order.Notes.Add(new OrderNote(order.OrderId, Token.Instance.UserId, DateTime.UtcNow, Comment.Text, IsPrivate.Checked ? NoteType.Private : NoteType.Public));
                order.Notes.Save();
            }
            order.Cancel();
        }
        CommentPanel.Visible = false;
        ConfirmPanel.Visible = true;
        OrderList2.Text = _OrderNumbers;
    }

}
