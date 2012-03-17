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

public partial class Admin_Orders_CancelOrder : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    private Order _Order;

    protected Order Order
    {
        get
        {
            if (_Order == null)
            {
                _Order = OrderDataSource.Load(this.OrderId);
            }
            return _Order;
        }
    }

    protected int OrderId
    {
        get
        {
            if (_OrderId.Equals(0))
            {
                _OrderId = PageHelper.GetOrderId();
            }
            return _OrderId;
        }
    }
    
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Comment.Text))
        {
            Order.Notes.Add(new OrderNote(OrderId, Token.Instance.UserId, DateTime.UtcNow, Comment.Text, IsPrivate.Checked ? NoteType.Private : NoteType.Public));
            Order.Notes.Save();
        }
        Order.Cancel();
        Response.Redirect( "ViewOrder.aspx?OrderNumber=" + Order.OrderNumber.ToString() + "&OrderId=" + OrderId.ToString());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack) BackButton.NavigateUrl = "ViewOrder.aspx?OrderNumber=" + Order.OrderNumber.ToString() + "&OrderId=" + OrderId.ToString();
    }

}
