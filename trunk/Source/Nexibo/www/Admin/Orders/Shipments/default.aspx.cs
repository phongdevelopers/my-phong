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
using CommerceBuilder.Shipping.Providers;

public partial class Admin_Orders_Shipments__Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    private Order _Order;
    private List<KeyValuePair<String, String>> _ShipmentListValues;

    protected int OrderNumber
    {
        get { return _Order.OrderNumber; }
    }

    protected OrderItemCollection GetDisplayItems(object dataItem)
    {
        OrderItemType[] displayTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon, OrderItemType.GiftWrap, OrderItemType.Credit, OrderItemType.Charge};
        OrderItemCollection productList = new OrderItemCollection();
        OrderItemCollection orderItems = dataItem as OrderItemCollection;
        if (orderItems != null)
        {
            foreach (OrderItem item in orderItems)
            {
                if (Array.IndexOf(displayTypes, item.OrderItemType) > -1) productList.Add(item);
            }
        }
        productList.Sort(new OrderItemComparer());
        return productList;
    }

    protected LSDecimal GetItemSubtotal(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        LSDecimal itemSubtotal = 0;
        foreach (OrderItem item in shipment.OrderItems)
        {
            if (item.OrderItemType == OrderItemType.Product) itemSubtotal += item.ExtendedPrice;
        }
        return itemSubtotal;
    }

    protected LSDecimal GetShippingSubtotal(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        LSDecimal itemSubtotal = 0;
        foreach (OrderItem item in shipment.OrderItems)
        {
            if (item.OrderItemType == OrderItemType.Shipping || item.OrderItemType == OrderItemType.Handling) itemSubtotal += item.ExtendedPrice;
        }
        return itemSubtotal;
    }
    
    protected LSDecimal GetTaxSubtotal(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        LSDecimal itemSubtotal = 0;
        foreach (OrderItem item in shipment.OrderItems)
        {
            if (item.OrderItemType == OrderItemType.Tax ) itemSubtotal += item.ExtendedPrice;
        }
        return itemSubtotal;
    }

    protected LSDecimal GetTotal(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        LSDecimal itemSubtotal = 0;
        foreach (OrderItem item in shipment.OrderItems)
        {
            itemSubtotal += item.ExtendedPrice;
        }
        return itemSubtotal;
    }

    protected string GetTrackingUrl(object dataItem)
    {
        TrackingNumber trackingNumber = (TrackingNumber)dataItem;
        if (trackingNumber.ShipGateway != null)
        {
            IShippingProvider provider = trackingNumber.ShipGateway.GetProviderInstance();
            TrackingSummary summary = provider.GetTrackingSummary(trackingNumber);
            if (summary != null)
            {
                // TRACKING DETAILS FOUND
                if (summary.TrackingResultType == TrackingResultType.InlineDetails)
                {
                    //send to view tracking page
                    return string.Format("ViewTrackingNumber.aspx?TrackingNumberId={0}", trackingNumber.TrackingNumberId.ToString());
                }
                else if (summary.TrackingResultType == TrackingResultType.ExternalLink)
                {
                    return summary.TrackingLink;
                }
            }
        }        
        return string.Empty;
    }
    
    protected string GetShipToAddress(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return shipment.FormatToAddress(true);
    }

    protected string GetAddressType(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        return shipment.ShipToResidence ? "Residential" : "Commercial";
    }

    protected string GetSku(object dataItem)
    {
        OrderItem orderItem = (OrderItem)dataItem;
        if (orderItem.OrderItemType == OrderItemType.Product) return orderItem.Sku;
        return StringHelper.SpaceName(orderItem.OrderItemType.ToString());
    }

    protected void BindShipmentsGrid()
    {
        EditShipmentsGrid.DataSource = _Order.Shipments;
        EditShipmentsGrid.DataBind();
    }

    protected List<KeyValuePair<String, String>> GetShipmentList(object Id)
    {
        int orderItemId = AlwaysConvert.ToInt(Id);
        _ShipmentListValues = new List<KeyValuePair<string, string>>();
        _ShipmentListValues.Add(new KeyValuePair<string, string>("No Shipment", "No Shipment|" + orderItemId));
        // add the shipments of this order
        foreach (OrderShipment shipment in _Order.Shipments)
        {
            String key = "Shipment " + shipment.ShipmentNumber + " (ID " + shipment.OrderShipmentId + ")";
            _ShipmentListValues.Add(new KeyValuePair<string, string>(key, shipment.OrderShipmentId + "|" + orderItemId));
        }
        _ShipmentListValues.Add(new KeyValuePair<string, string>("New Shipment", "New Shipment|" + orderItemId));
        return _ShipmentListValues;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect(NavigationHelper.GetAdminUrl("Orders/Default.aspx"));
        BindShipmentsGrid();
        AddShipmentLink.NavigateUrl = "AddShipment.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
    }
    
    protected bool ShowSplitLink(Object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        //DO NOT DISPLAY IF THIS SHIPMENT IS ALREADY SHIPPED
        if (shipment.IsShipped) return false;
        //ONLY DISPLAY IF THERE IS MORE THAN ONE PRODUCT IN THE SHIPMENT
        return (shipment.OrderItems.ProductCount > 1);
    }
    
    protected bool ShowMergeLink(Object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        //DO NOT DISPLAY IF THIS SHIPMENT IS ALREADY SHIPPED
        if (shipment.IsShipped) return false;
        foreach (OrderShipment otherShipment in _Order.Shipments)
        {
            //IF THERE IS MORE THAN ONE UNSHIPPED SHIPMENT, SHOW THE MERGE BUTTON
            if ((shipment.OrderShipmentId != otherShipment.OrderShipmentId) && (!otherShipment.IsShipped)) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if a shipment is empty
    /// </summary>
    /// <param name="dataItem">OrderShipment object</param>
    /// <returns>True if the shipment contains no products</returns>
    /// <remarks>Used to determine whether to use simple or advanced delete</remarks>
    protected bool ShowDeleteLink(object dataItem)
    {
        OrderShipment shipment = (OrderShipment)dataItem;
        foreach (OrderItem item in shipment.OrderItems)
        {
            if (item.OrderItemType == OrderItemType.Product)
                return false;
        }
        return true;
    }
    
    protected bool ShowDeleteButton(object dataItem)
    {
        //DO NOT SHOW BUTTON IF LINK IS VISIBLE
        if (ShowDeleteLink(dataItem)) return false;
        //WE CAN ONLY DELETE IF THE SHIPMENT IS NOT SHIPPED
        //AND THERE IS AT LEAST ONE OTHER UNSHIPPED SHIPMENT
        OrderShipment shipment = (OrderShipment)dataItem;
        if (shipment.IsShipped) return false;
        foreach (OrderShipment otherShipment in _Order.Shipments)
        {
            if ((!otherShipment.IsShipped) && (otherShipment.OrderShipmentId != shipment.OrderShipmentId)) return true;
        }
        return false;
    }

    protected void EditShipmentsGrid_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "DelShp")
        {
            int shipmentId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Order.Shipments.IndexOf(shipmentId);
            if (index > -1)
            {
                OrderShipment shipment = _Order.Shipments[index];
                shipment.OrderItems.DeleteAll();
                _Order.Shipments.DeleteAt(index);
                BindShipmentsGrid();
            }
        }
        else if (e.CommandName == "ChangeShipMethod")
        {
            int shipmentId = AlwaysConvert.ToInt(e.CommandArgument);
            int index = _Order.Shipments.IndexOf(shipmentId);
            if (index > -1)
            {
                // SHOW THE CHANGE SHIPMENT POPUP
                ChangeShipMethodShipmentId.Value = shipmentId.ToString();
                ChangeShipMethodDialogCaption.Text = string.Format(ChangeShipMethodDialogCaption.Text, index + 1);
                OrderShipment shipment = _Order.Shipments[index];
                ExistingShipMethod.Text = shipment.ShipMethodName;

                // GENERATE RATE QUOTES FOR ALL SHIPPING METHODS
                List<ShipRateQuote> rateQuotes = new List<ShipRateQuote>();
                ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForStore();
                foreach (ShipMethod method in shipMethods)
                {
                    ShipRateQuote quote = method.GetShipRateQuote(shipment);
                    if (quote != null) rateQuotes.Add(quote);
                }

                // GET LIST OF SHIPPING METHODS THAT WOULD BE AVAILABLE TO THE CUSTOMER
                ShipMethodCollection customerShipMethods = ShipMethodDataSource.LoadForShipment(shipment);

                // ADD RATE QUOTES TO THE DROPDOWN
                foreach (ShipRateQuote quote in rateQuotes)
                {
                    string name = string.Format("{0} : {1:lc}", quote.Name, quote.Rate);
                    if (customerShipMethods.IndexOf(quote.ShipMethodId) < 0)
                    {
                        // SHOW NOTE IF HIDDEN SHIPPING METHODS ARE AVAIALBLE
                        name = "** " + name;
                        HiddenShipMethodWarning.Visible = true;
                    }
                    NewShipMethod.Items.Add(new ListItem(name, quote.ShipMethodId.ToString()));
                }
                ChangeShipMethodPopup.Show();
            }
        }
    }

    protected bool IsReturnButtonVisible(Object dataItem)
    {
        bool visible = true;
        OrderShipment shipment = (OrderShipment)dataItem;
        visible = shipment.IsShipped;
        if (visible)
        {
            // IF COUNT OF RETURN_ABLE ITEMS MORE THEN ZERO
            int count = 0;
            foreach (OrderItem oi in shipment.OrderItems)
            {
                if (oi.OrderItemType == OrderItemType.Product) count++;
            }
            if (count == 0) visible = false;
        }
        return visible;
    }

    protected void ChangeShipMethodOKButton_Click(object source, EventArgs e)
    {
        int shipmentId = AlwaysConvert.ToInt(Request.Form[ChangeShipMethodShipmentId.UniqueID]);
        int index = _Order.Shipments.IndexOf(shipmentId);
        if (index > -1)
        {
            // WE FOUND THE TARGET SHIPMENT. REMOVE OLD SHIPPING LINE ITEMS
            OrderShipment shipment = _Order.Shipments[index];
            for (int i = shipment.OrderItems.Count - 1; i > 0; i--)
            {
                OrderItemType itemType = shipment.OrderItems[i].OrderItemType;
                if (itemType == OrderItemType.Shipping || itemType == OrderItemType.Handling)
                {
                    shipment.OrderItems.DeleteAt(i);
                }
            }

            // SEE IF WE HAVE A NEW SELECTED SHIPMETHOD
            int shipMethodId = AlwaysConvert.ToInt(Request.Form[NewShipMethod.UniqueID]);
            ShipMethod shipMethod = ShipMethodDataSource.Load(shipMethodId);
            if (shipMethod != null)
            {
                ShipRateQuote rate = shipMethod.GetShipRateQuote(shipment);
                if (rate != null)
                {
                    // ADD NEW SHIPPING LINE ITEMS TO THE ORDER
                    OrderItem shipRateLineItem = new OrderItem();
                    shipRateLineItem.OrderId = _OrderId;
                    shipRateLineItem.OrderItemType = OrderItemType.Shipping;
                    shipRateLineItem.OrderShipmentId = shipmentId;
                    shipRateLineItem.Name = shipMethod.Name;
                    shipRateLineItem.Price = rate.Rate;
                    shipRateLineItem.Quantity = 1;
                    shipRateLineItem.TaxCodeId = shipMethod.TaxCodeId;
                    shipRateLineItem.Save();
                    shipment.OrderItems.Add(shipRateLineItem);
                    if (rate.Surcharge > 0)
                    {
                        shipRateLineItem = new OrderItem();
                        shipRateLineItem.OrderId = _OrderId;
                        shipRateLineItem.OrderItemType = OrderItemType.Handling;
                        shipRateLineItem.OrderShipmentId = shipmentId;
                        shipRateLineItem.Name = shipMethod.Name;
                        shipRateLineItem.Price = rate.Surcharge;
                        shipRateLineItem.Quantity = 1;
                        shipRateLineItem.TaxCodeId = shipMethod.TaxCodeId;
                        shipRateLineItem.Save();
                        shipment.OrderItems.Add(shipRateLineItem);
                    }
                }
            }

            // UPDATE THE SHIPMENT WITH NEW METHOD ASSOCIATION
            shipment.ShipMethodId = shipMethodId;
            shipment.ShipMethodName = (shipMethod != null ? shipMethod.Name : string.Empty);
            shipment.Save();

            // RELOAD ORDER AND REBIND THE PAGE FOR UPDATED INFO
            _Order = OrderDataSource.Load(_OrderId, false);
            BindShipmentsGrid();
        }
    }
}