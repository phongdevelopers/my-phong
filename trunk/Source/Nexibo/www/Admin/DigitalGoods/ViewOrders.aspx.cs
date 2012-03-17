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
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using System.Collections.Generic;

public partial class Admin_DigitalGoods_ViewOrders : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _DigitalGoodId;
    private DigitalGood _DigitalGood;
    protected void Page_Load(object sender, EventArgs e)
    {
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        if (_DigitalGood == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);
        //GET ALL ORDER ITEMS ASSOCIATED WITH DIGITAL GOOD
        OrderItemDigitalGoodCollection oidgs = OrderItemDigitalGoodDataSource.LoadForDigitalGood(_DigitalGoodId);
        //BUILD DISTINCT LIST OF ORDERS
        List<Order> orders = new List<Order>();
        foreach (OrderItemDigitalGood oidg in oidgs)
        {
            Order order = oidg.OrderItem.Order;
            if (orders.IndexOf(order) < 0) orders.Add(order);
        }
        //BIND TO GRID
        OrderGrid.DataSource = orders;
        OrderGrid.DataBind();
    }
}
