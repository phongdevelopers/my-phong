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

public partial class Admin_Orders_Shipments_AddProduct : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    
    private int _OrderShipmentId;
    private OrderShipment _OrderShipment;    
    public void Page_Init(object sender, EventArgs e)
    {
        _OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
        if (_OrderShipment == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _OrderShipment.ShipmentNumber, _OrderShipment.OrderId);
    }    

}
