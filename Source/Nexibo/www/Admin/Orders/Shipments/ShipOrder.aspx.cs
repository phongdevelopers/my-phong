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

public partial class Admin_Orders_Shipments_ShipOrder : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    private Order _Order;
    private int _OrderShipmentId = 0;
    private OrderShipment _OrderShipment;

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_OrderShipmentId);
        if (_OrderShipment == null) Response.Redirect("../Default.aspx");
        _OrderId = _OrderShipment.OrderId;
        _Order = _OrderShipment.Order;
        //SHOW SHIPMENT NUMBER
        if (_Order.Shipments.Count > 1)
        {
            trShipmentNumber.Visible = true;
            ShipmentNumber.Text = string.Format(ShipmentNumber.Text, _Order.Shipments.IndexOf(_OrderShipmentId) + 1, _Order.Shipments.Count);
        }
        OrderId.Text = _Order.OrderNumber.ToString();
        OrderDate.Text = string.Format("{0:g}", _Order.OrderDate);
        ShippingMethod.Text = _OrderShipment.ShipMethodName;
        if (!string.IsNullOrEmpty(_OrderShipment.ShipMessage))
        {
            ShipMessagePanel.Visible = true;
            ShipMessage.Text = _OrderShipment.ShipMessage;
        }
        ShipFrom.Text = _OrderShipment.FormatFromAddress();
        ShipTo.Text = _OrderShipment.FormatToAddress();
        ShipmentItems.DataSource = GetShipmentItems();
        ShipmentItems.DataBind();
        ShipGateway.DataSource = ShipGatewayDataSource.LoadForStore();
        ShipGateway.DataBind();
        if (ShipGateway.Items.Count > 1)
        {
            //TRY TO PRESET THE CORRECT GATEWAY
            if (_OrderShipment.ShipMethod != null)
            {
                ListItem item = ShipGateway.Items.FindByValue(_OrderShipment.ShipMethod.ShipGatewayId.ToString());
                if (item != null) item.Selected = true;
            }
        }
        else
        {
            ShipGateway.Visible = false;
        }
        CancelButton.NavigateUrl += "?OrderNumber=" + _OrderShipment.Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
    }

    protected List<OrderItem> GetShipmentItems()
    {
        List<OrderItem> items = new List<OrderItem>();
        foreach (OrderItem item in _Order.Items)
        {
            if ((item.OrderShipmentId == _OrderShipmentId) && (item.OrderItemType == OrderItemType.Product)) items.Add(item);
        }
        return items;
    }

    protected void ShipButton_Click(object sender, EventArgs e)
    {
        //WE HAVE TO LOOK FOR ANY ITEMS NOT BEING SHIPPED
        //BUILD A DICTIONARY OF QUANTITY TO SHIP
        bool itemFound = false;
        bool isPartial = false;
        bool quantityExceeded = false;
        Dictionary<int, short> quantities = new Dictionary<int, short>();
        foreach (GridViewRow row in ShipmentItems.Rows)
        {
            HiddenField hf = (HiddenField)row.FindControl("Id");
            int orderItemId = AlwaysConvert.ToInt(hf.Value);
            int index = _OrderShipment.OrderItems.IndexOf(orderItemId);
            if (index > -1)
            {
                TextBox tb = (TextBox)row.FindControl("Quantity");
                short qty = AlwaysConvert.ToInt16(tb.Text);
                itemFound = itemFound || (qty > 0);
                isPartial = isPartial || (qty < _OrderShipment.OrderItems[index].Quantity);
                quantityExceeded = quantityExceeded || (qty > _OrderShipment.OrderItems[index].Quantity);
                quantities.Add(orderItemId, qty);
            }
        }

        if ((itemFound) && (!quantityExceeded))
        {
            //CHECK IF WE ARE NOT SHIPPING ALL OF THE ITEMS
            if (isPartial)
            {
                //AT LEAST ONE ITEM MUST BE MOVED TO A NEW SHIPMENT
                //CREATE A COPY OF THIS SHIPMENT
                OrderShipment newShipment = OrderShipment.Copy(_OrderShipmentId, false);
                newShipment.Save();
                _Order.Shipments.Add(newShipment);
                //KEEP TRACK OF ITEMS TO REMOVE FROM THE CURRENT SHIPMENT
                List<int> removeItems = new List<int>();
                //LOOP THE ITEMS AND DECIDE WHICH TO PUT IN THE NEW SHIPMENT
                foreach (OrderItem item in _OrderShipment.OrderItems)
                {
                    int searchItemId = (item.ParentItemId == 0) ? item.OrderItemId : item.ParentItemId;
                    if (quantities.ContainsKey(searchItemId))
                    {
                        short shipQty = quantities[searchItemId];
                        if (shipQty != item.Quantity)
                        {
                            if (shipQty > 0)
                            {
                                //WE HAVE TO SPLIT THIS ITEM
                                OrderItem newItem = OrderItem.Copy(item.OrderItemId, true);
                                newItem.Quantity = (short)(item.Quantity - shipQty);
                                newItem.OrderShipmentId = newShipment.OrderShipmentId;
                                newItem.Save();
                                newShipment.OrderItems.Add(newItem);
                                //UPDATE THE CURRENT ITEM
                                item.Quantity = shipQty;
                                item.Save();
                            }
                            else
                            {
                                //THIS ITEM JUST NEEDS TO BE MOVED
                                item.OrderShipmentId = newShipment.OrderShipmentId;
                                item.Save();
                                newShipment.OrderItems.Add(item);
                                removeItems.Add(item.OrderItemId);
                            }
                        }
                    }
                }
                //REMOVE ANY ITEMS THAT WERE MOVED TO ANOTHER SHIPMENT
                foreach (int id in removeItems)
                {
                    int delIndex = _OrderShipment.OrderItems.IndexOf(id);
                    if (delIndex > -1) _OrderShipment.OrderItems.RemoveAt(delIndex);
                }
            }

			//Add the Tracking Number
            int shipgwId = AlwaysConvert.ToInt(ShipGateway.SelectedValue);
            string trackingData = AddTrackingNumber.Text;
            if (!string.IsNullOrEmpty(trackingData))
            {
                TrackingNumber tnum = new TrackingNumber();
                tnum.TrackingNumberData = trackingData;
                tnum.ShipGatewayId = shipgwId;
                tnum.OrderShipmentId = _OrderShipment.OrderShipmentId;                
                _OrderShipment.TrackingNumbers.Add(tnum);
            }
            //SHIP THE CURRENT SHIPMENT
            _OrderShipment.Ship();
            //RETURN TO SHIPMENTS PAGE
            Response.Redirect(CancelButton.NavigateUrl);
        }
        else
        {
            CustomValidator quantityError = new CustomValidator();
            if (quantityExceeded)
                quantityError.ErrorMessage = "You cannot move more than the existing quantity.";
            else
                quantityError.ErrorMessage = "You must pick at least one item to move.";
            quantityError.Text = "&nbsp;";
            quantityError.IsValid = false;
            phQuantityValidation.Controls.Add(quantityError);
        }
    }

}
