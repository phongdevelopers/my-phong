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

public partial class Admin_Orders_Shipments_ReturnShipment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ShipmentId;
    private OrderShipment _OrderShipment;
    protected void Page_Init(object sender, EventArgs e)
    {
        _ShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_ShipmentId);
        if (_OrderShipment == null) Response.Redirect(CancelLink.NavigateUrl);
        if (!_OrderShipment.IsShipped) Response.Redirect(CancelLink.NavigateUrl);
        //BIND ITEMS
        BindShipmentItems();
        
        Caption.Text = string.Format(Caption.Text, _OrderShipment.ShipmentNumber);
        CancelLink.NavigateUrl += "?OrderNumber=" + _OrderShipment.Order.OrderNumber.ToString() + "&OrderId=" + _OrderShipment.OrderId.ToString();

    }
    protected void BindShipmentItems()
    {
        List<OrderItem> shipmentItems = new List<OrderItem>();
        foreach (OrderItem oi in _OrderShipment.OrderItems)
        {
            if(oi.OrderItemType == OrderItemType.Product){
                shipmentItems.Add(oi);
            }
        }
        
        ShipmentItems.DataSource = shipmentItems ;
        ShipmentItems.DataBind();
    }
    
    protected void ShipmentItems_OnPreRender(object sender, EventArgs e)
    {
        foreach (GridViewRow row in ShipmentItems.Rows)
        {
            Control control = row.FindControl("Qty");
            if (control != null)
            {
                TextBox Qty = null;
                Qty = control as TextBox;
                if (Qty != null)
                {
                    Qty.Attributes.Add("onchange", "QuantityCheck(this);");
                    Qty.Attributes.Add("onfocus", "val = this.value;");
                }
            }
        }

    }

    protected void ShipmentItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Return"))
        {
            int orderItemId = AlwaysConvert.ToInt(e.CommandArgument);
            OrderItem orderItem = OrderItemDataSource.Load(orderItemId);
            ReturnItemDialog1.OrderItem = orderItem;
            ReturnItemDialog1.Visible = true;
        }
    }

}
