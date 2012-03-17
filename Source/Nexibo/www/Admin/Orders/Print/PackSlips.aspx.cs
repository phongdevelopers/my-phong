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
using CommerceBuilder.Common;
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

public partial class Admin_Orders_Print_PackSlips : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private List<string> orderNumbers;
    protected int ShipmentCount = 0;

    private string GetOrderNumbers(List<int> orders)
    {
        if (orderNumbers == null)
        {
            orderNumbers = new List<string>();
            foreach (int orderId in orders)
            {
                int orderNumber = OrderDataSource.LookupOrderNumber(orderId);
                orderNumbers.Add(orderNumber.ToString());
            }
        }
        if (orderNumbers.Count == 0) return string.Empty;
        return string.Join(", ", orderNumbers.ToArray());
    }

    /// <summary>
    /// Gets a collection of unshipped shipments for the selected orders.
    /// </summary>
    /// <param name="orders">The orderIds to gather shipments for.</param>
    /// <returns>A collection of unshipped shipments for the selected orders.</returns>
    protected OrderShipmentCollection GetShipments(params int[] orders)
    {
        OrderShipmentCollection shipments = new OrderShipmentCollection();
        foreach (int orderId in orders)
        {
            Order order = OrderDataSource.Load(orderId);
            if (order != null)
            {
                foreach (OrderShipment shipment in order.Shipments)
                {
                    if (shipment.ShipDate == System.DateTime.MinValue)
                    {
                        shipments.Add(shipment);
                    }
                }
            }
        }
        ShipmentCount = shipments.Count;
        return shipments;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = GetSelectedOrders();
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        ShipmentRepeater.DataSource = GetShipments(selectedOrders.ToArray());
        ShipmentRepeater.DataBind();
        OrderList.Text = GetOrderNumbers(selectedOrders);
    }

    private List<int> GetSelectedOrders()
    {
        List<int> selectedOrders = null;
        // CHECK FOR QUERYSTRING PARAMETERS
        if (!String.IsNullOrEmpty(Request.QueryString["OrderId"]))
        {
            int orderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            selectedOrders = new List<int>();
            selectedOrders.Add(orderId);
        }
        // CHECK SESSION
        else selectedOrders = Token.Instance.Session.SelectedOrderIds;
        return selectedOrders;
    }

    protected void ShipmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            OrderShipment shipment = (OrderShipment)e.Item.DataItem;
            Order order = shipment.Order;
            //UPDATE CAPTION
            Label ShipmentLabel = e.Item.FindControl("ShipmentLabel") as Label;
            Label ShipmentCountLabel = e.Item.FindControl("ShipmentCountLabel") as Label;
            if (ShipmentLabel != null)
            {
                ShipmentLabel.Text = string.Format(ShipmentLabel.Text, order.OrderNumber);
                if (ShipmentCountLabel != null)
                {
                    //HIDE SHIPMENT COUNT IF THERE IS ONLY ONE SHIPMENT
                    if (order.Shipments.Count > 1)
                    {
                        int index = order.Shipments.IndexOf(shipment.OrderShipmentId) + 1;
                        ShipmentCountLabel.Text = string.Format(ShipmentCountLabel.Text, index, order.Shipments.Count);
                    }
                    else
                    {
                        ShipmentCountLabel.Visible = false;
                    }
                }
            }
            //HIDE COMMENT ROW IF NO MESSAGE PROVIDED
            HtmlTableRow trShipMessage = e.Item.FindControl("trShipMessage") as HtmlTableRow;
            if (trShipMessage != null)
            {
                if (string.IsNullOrEmpty(shipment.ShipMessage)) trShipMessage.Visible = false;
            }
        }
    }

    protected OrderItemCollection GetProducts(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        Order order = shipment.Order;
        OrderItemCollection products = new OrderItemCollection();
        foreach (OrderItem item in order.Items)
        {
            if ((item.OrderItemType == OrderItemType.Product) && (item.OrderShipmentId == shipment.OrderShipmentId))
            {
                if (item.IsChildItem)
                {
                    // WHETHER THE CHILD ITEM DISPLAYS DEPENDS ON THE ROOT
                    OrderItem rootItem = item.GetParentItem(true);
                    if (rootItem.Product != null && rootItem.ItemizeChildProducts)
                    {
                        // ITEMIZED DISPLAY ENABLED, SHOW THIS CHILD ITEM
                        products.Add(item);
                    }
                }
                else products.Add(item);
            }
        }
        products.Sort(new OrderItemComparer());
        return products;
    }

}
