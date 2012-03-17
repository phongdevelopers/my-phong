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

public partial class Admin_Orders_Shipments_MergeShipment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ShipmentId;
    private OrderShipment _OrderShipment;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ShipmentId = AlwaysConvert.ToInt(Request.QueryString["ShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_ShipmentId);
        if (_OrderShipment == null)
        {
            int orderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            int orderNumber = OrderDataSource.LookupOrderNumber(orderId);
            Response.Redirect( "Default.aspx?OrderNumber=" + orderNumber.ToString() + "&OrderId=" + orderId.ToString());
        }
        Caption.Text = string.Format(Caption.Text, _OrderShipment.ShipmentNumber);
        CancelLink.NavigateUrl += "?OrderNumber=" + _OrderShipment.Order.OrderNumber.ToString() + "&OrderId=" + _OrderShipment.OrderId.ToString();
        //BIND ITEMS
        ShipmentItems.DataSource = _OrderShipment.OrderItems;
        ShipmentItems.DataBind();
        //ADD ITEMS TO SHIPMENTS LIST
        foreach (OrderShipment shipment in _OrderShipment.Order.Shipments)
        {
            if ((shipment.OrderShipmentId != _ShipmentId) && (!shipment.IsShipped))
            {
                string address = string.Format("{0} {1} {2} {3}", shipment.ShipToFirstName, shipment.ShipToLastName, shipment.ShipToAddress1, shipment.ShipToCity);
                if (address.Length > 50) address = address.Substring(0, 47) + "...";
                string name = "Shipment #" + shipment.ShipmentNumber + " to " + address;
                ShipmentsList.Items.Add(new ListItem(name, shipment.OrderShipmentId.ToString()));
            }
        }
    }
    
    protected void MergeButton_Click(object sender, EventArgs e)
    {
        int otherShipmentId = AlwaysConvert.ToInt(ShipmentsList.SelectedValue);
        OrderShipment otherShipment = OrderShipmentDataSource.Load(otherShipmentId);
        if (otherShipment != null)
        {
            foreach (OrderItem item in _OrderShipment.OrderItems)
            {
                item.OrderShipmentId = otherShipmentId;
                item.Save();
            }
            _OrderShipment.OrderItems.Clear();
            _OrderShipment.Delete();
        }
        Response.Redirect(CancelLink.NavigateUrl);
    }

}
