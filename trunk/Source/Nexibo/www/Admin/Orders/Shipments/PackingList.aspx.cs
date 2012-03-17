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

public partial class Admin_Orders_Shipments_PackingList : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    int _OrderShipmentId = 0;
    OrderShipment _OrderShipment;
    
    protected void Page_Load(object sender, System.EventArgs e) {
        _OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
        if (_OrderShipment == null)
        {
            Response.Clear();
            Response.Write("Invalid order shipment specified.");
            Response.End();
        }
        if (!Page.IsPostBack)
        {
            NonShippingItemsGrid.DataSource = GetDisplayItems(_OrderShipment.OrderItems);
            NonShippingItemsGrid.DataBind();
            Order myOrder = _OrderShipment.Order;
            int myShipmentNumber = (myOrder.Shipments.IndexOf(_OrderShipmentId) + 1);
            Caption.Text = string.Format(Caption.Text, myOrder.OrderNumber);
            PrintCaption.Text = string.Format(Caption.Text, myOrder.OrderNumber);
            OrderDate.Text = string.Format("{0:d}", myOrder.OrderDate);
            ShipmentNumber.Text = string.Format(ShipmentNumber.Text, myShipmentNumber, myOrder.Shipments.Count);
			ShipmentWeight.Text = _OrderShipment.OrderItems.TotalWeight().ToString() + " " + Token.Instance.Store.WeightUnit.ToString();
            ShipTo.Text = _OrderShipment.FormatToAddress();
            ShipFrom.Text = _OrderShipment.FormatFromAddress();
            ShippingMethod.Text = _OrderShipment.ShipMethodName;
            AddressType.Text = (_OrderShipment.ShipToResidence) ? "Residential":"Commercial";
            ShipMessage.Text = _OrderShipment.ShipMessage;
        }
    }

    protected OrderItemCollection GetDisplayItems(object dataItem)
    {
        OrderItemCollection productList = new OrderItemCollection();
        OrderItemCollection orderItems = dataItem as OrderItemCollection;
        if (orderItems != null)
        {
            foreach (OrderItem item in orderItems)
            {
                if (item.OrderItemType == OrderItemType.Product)
                {
                    if (item.IsChildItem)
                    {
                        // WHETHER THE CHILD ITEM DISPLAYS DEPENDS ON THE ROOT
                        OrderItem rootItem = item.GetParentItem(true);
                        if (rootItem.Product != null && rootItem.ItemizeChildProducts)
                        {
                            // ITEMIZED DISPLAY ENABLED, SHOW THIS CHILD ITEM
                            productList.Add(item);
                        }
                    }
                    else productList.Add(item);
                }
            }
        }
        productList.Sort(new OrderItemComparer());
        return productList;
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect( "Default.aspx?OrderNumber=" + _OrderShipment.Order.OrderNumber.ToString() + "&OrderId=" + _OrderShipment.OrderId.ToString());
    }

    protected string GetSku(object dataItem)
    {
        OrderItem orderItem = (OrderItem)dataItem;
        if (orderItem.OrderItemType == OrderItemType.Product) return orderItem.Sku;
        return StringHelper.SpaceName(orderItem.OrderItemType.ToString());
    }

}
