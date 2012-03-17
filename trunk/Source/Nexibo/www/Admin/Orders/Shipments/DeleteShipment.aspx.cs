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

public partial class Admin_Orders_Shipments_DeleteShipment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
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
        /*
        //ADD ITEMS TO SHIPMENTS LIST
        foreach (OrderShipment shipment in _OrderShipment.Order.Shipments)
        {
            if (shipment.OrderShipmentId != _ShipmentId)
            {
                string address = string.Format("{0} {1} {2} {3}", shipment.ShipToFirstName, shipment.ShipToLastName, shipment.ShipToAddress1, shipment.ShipToCity);
                if (address.Length > 50) address = address.Substring(0,47) + "...";
                string name = "Shipment #" + shipment.ShipmentNumber + " to "+ address;
                ShipmentsList.Items.Add(new ListItem(name, shipment.OrderShipmentId.ToString()));
            }
        }
        ShipmentsList.Items.Add(new ListItem("New shipment..."));
        */
    }

    protected void DeleteShipmentButton_Click(object sender, EventArgs e)
    {
        /*
        if (DeleteItems.Checked)
        {
            _OrderShipment.OrderItems.DeleteAll();
        }
        else
        {
            //MOVE ITEMS TO ANOTHER SHIPMENT
            int destShipmentId = AlwaysConvert.ToInt(ShipmentsList.SelectedItem.Value);
            OrderShipment destShipment = OrderShipmentDataSource.Load(destShipmentId);
            if (destShipment == null)
            {
                //NEED TO CREATE A NEW SHIPMENT
                destShipment = OrderShipment.Copy(_ShipmentId, false);
                destShipment.Save();
            }
            //LOOP ITEMS TO PUT INTO DESTINATION SHIPMENT
            foreach (OrderItem item in _OrderShipment.OrderItems)
            {
                item.OrderShipmentId = destShipment.OrderShipmentId;
                destShipment.OrderItems.Add(item);
            }
            //SAVE DESTINATION SHIPMENT
            destShipment.Save();
            _OrderShipment.OrderItems.Clear();
        }
        */
        //DELETE SHIPMENT ITEMS
        _OrderShipment.OrderItems.DeleteAll();
        //DELETE THE ORIGIN SHIPMENT
        _OrderShipment.Delete();
        Response.Redirect(CancelLink.NavigateUrl);
    }

}
