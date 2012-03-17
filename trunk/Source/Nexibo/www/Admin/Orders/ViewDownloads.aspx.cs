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

public partial class Admin_Orders_ViewDownloads : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private OrderItemDigitalGood _OrderItemDigitalGood;
    private int _OrderItemDigitalGoodId = 0;

    protected OrderItemDigitalGood OrderItemDigitalGood
    {
        get
        {
            if (_OrderItemDigitalGood == null)
            {
                _OrderItemDigitalGood = OrderItemDigitalGoodDataSource.Load(this.OrderItemDigitalGoodId);
            }
            return _OrderItemDigitalGood;
        }
    }

    protected int OrderItemDigitalGoodId
    {
        get
        {
            if (_OrderItemDigitalGoodId.Equals(0))
            {
                _OrderItemDigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["OrderItemDigitalGoodId"]);
            }
            return _OrderItemDigitalGoodId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (OrderItemDigitalGood == null) Response.Redirect("Default.aspx");
        string name = (OrderItemDigitalGood.DigitalGood != null) ? OrderItemDigitalGood.DigitalGood.Name : OrderItemDigitalGood.OrderItem.Name;
        Caption.Text = string.Format(Caption.Text, name);
        DownloadsGrid.DataSource = OrderItemDigitalGood.Downloads;
        DownloadsGrid.DataBind();
        Order order = OrderItemDigitalGood.OrderItem.Order;
        DigitalGoodsLink.NavigateUrl += "?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString();
    }

}
