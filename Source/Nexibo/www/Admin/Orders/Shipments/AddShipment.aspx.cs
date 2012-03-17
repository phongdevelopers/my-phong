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

public partial class Admin_Orders_Shipments_AddShipment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;    
    private Order _Order;

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        ShipToCountryCode.DataSource = Token.Instance.Store.Countries;
        ShipToCountryCode.DataTextField = "Name";
        ShipToCountryCode.DataValueField = "CountryCode";
        ShipToCountryCode.DataBind();
        BindShippingAddresses(this.AddressList);
    }

    protected void BindShippingAddresses(DropDownList shippingAddress)
    {
        //BIND (OR RE-BIND) SHIPPING ADDRESSES
        ListItem listItem;
        
        //ADD ORDER BILL TO ADDRESS
        string tempAddress = _Order.BillToFirstName + " " + _Order.BillToLastName + " " + _Order.BillToAddress1;
        if (tempAddress.Length > 32) tempAddress = tempAddress.Substring(0, 29) + "...";
        string itemText = string.Format("{0} {1}", tempAddress, _Order.BillToCity);
        listItem = new ListItem(itemText, "B_" + _OrderId);
        shippingAddress.Items.Add(listItem);

        //ADD SHIP TO ADDRESSES
        foreach (OrderShipment shipment in _Order.Shipments)
        {
            tempAddress = shipment.ShipToFirstName + " " + shipment.ShipToLastName + " " + shipment.ShipToAddress1;
            if (tempAddress.Length > 32) tempAddress = tempAddress.Substring(0, 29) + "...";
            itemText = string.Format("{0} {1}", tempAddress, shipment.ShipToCity);
            listItem = new ListItem(itemText, "S_" + shipment.OrderShipmentId);
            if (shippingAddress.Items.FindByText(itemText) == null) shippingAddress.Items.Add(listItem);
        }
        if (shippingAddress.SelectedIndex < 0) shippingAddress.SelectedIndex = 0;
        
        //ADD ADDRESSES FROM USER ADDRESS BOOK
        Order order = OrderDataSource.Load(_OrderId);
        AddressCollection userAddresses = AddressDataSource.LoadForUser(order.UserId);
        foreach (Address address in userAddresses)
        {
            tempAddress = address.FullName + " " + address.Address1;
            if (tempAddress.Length > 32) tempAddress = tempAddress.Substring(0, 29) + "...";
            itemText = string.Format("{0} {1}", tempAddress, address.City);            
            listItem = new ListItem(itemText, address.AddressId.ToString());
            if (shippingAddress.Items.FindByText(itemText) == null) shippingAddress.Items.Add(listItem);
        }
        
        //ADD NEW ITEM
        shippingAddress.Items.Add(new ListItem("Add new..."));
    }

    private void SetAddress(OrderShipment newShipment)
    {
        if (AddressList.SelectedIndex < (AddressList.Items.Count - 1))
        {
            string selectedValue = AddressList.SelectedItem.Value;
            //USE EXISTING ADDRESS
            if (selectedValue.StartsWith("B_"))
            {
                //USE ORDER BILLING ADDRESS
                newShipment.ShipToFirstName = _Order.BillToFirstName;
                newShipment.ShipToLastName = _Order.BillToLastName;
                newShipment.ShipToAddress1 = _Order.BillToAddress1;
                newShipment.ShipToAddress2 = _Order.BillToAddress2;
                newShipment.ShipToCity = _Order.BillToCity;
                newShipment.ShipToProvince = _Order.BillToProvince;
                newShipment.ShipToPostalCode = _Order.BillToPostalCode;
                newShipment.ShipToCountryCode = _Order.BillToCountryCode;
                newShipment.ShipToPhone = _Order.BillToPhone;
                newShipment.ShipToCompany = _Order.BillToCompany;
                newShipment.ShipToFax = _Order.BillToFax;
            }
            else if (selectedValue.StartsWith("S_"))
            {
                //USE SHIPPING ADDRESS
                int shipmentId = AlwaysConvert.ToInt(AddressList.SelectedItem.Value.Split('_')[1]);
                int index = _Order.Shipments.IndexOf(shipmentId);
                if (index > -1)
                {
                    OrderShipment shipment = _Order.Shipments[index];
                    newShipment.ShipToFirstName = shipment.ShipToFirstName;
                    newShipment.ShipToLastName = shipment.ShipToLastName;
                    newShipment.ShipToAddress1 = shipment.ShipToAddress1;
                    newShipment.ShipToAddress2 = shipment.ShipToAddress2;
                    newShipment.ShipToCity = shipment.ShipToCity;
                    newShipment.ShipToProvince = shipment.ShipToProvince;
                    newShipment.ShipToPostalCode = shipment.ShipToPostalCode;
                    newShipment.ShipToCountryCode = shipment.ShipToCountryCode;
                    newShipment.ShipToPhone = shipment.ShipToPhone;
                    newShipment.ShipToCompany = shipment.ShipToCompany;
                    newShipment.ShipToFax = shipment.ShipToFax;
                }
            }
            else
            {
                //USE ADDRESS FROM ADDRESS BOOK
                int addressId = AlwaysConvert.ToInt(selectedValue);
                Address address = AddressDataSource.Load(addressId);
                newShipment.ShipToFirstName = address.FirstName;
                newShipment.ShipToLastName = address.LastName;
                newShipment.ShipToAddress1 = address.Address1;
                newShipment.ShipToAddress2 = address.Address2;
                newShipment.ShipToCity = address.City;
                newShipment.ShipToProvince = address.Province;
                newShipment.ShipToPostalCode = address.PostalCode;
                newShipment.ShipToCountryCode = address.CountryCode;
                newShipment.ShipToPhone = address.Phone;
                newShipment.ShipToCompany = address.Company;
                newShipment.ShipToFax = address.Fax;
            }
        }
        else
        {
            //ADD A NEW ADDRESS
            newShipment.ShipToFirstName = ShipToFirstName.Text;
            newShipment.ShipToLastName = ShipToLastName.Text;
            newShipment.ShipToAddress1 = ShipToAddress1.Text;
            newShipment.ShipToAddress2 = ShipToAddress2.Text;
            newShipment.ShipToCity = ShipToCity.Text;
            newShipment.ShipToProvince = ShipToProvince.Text;
            newShipment.ShipToPostalCode = ShipToPostalCode.Text;
            newShipment.ShipToCountryCode = ShipToCountryCode.SelectedValue;
            newShipment.ShipToPhone = ShipToPhone.Text;
            newShipment.ShipToCompany = ShipToCompany.Text;
        }
    }
    
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        OrderShipment newShipment = new OrderShipment();
        newShipment.OrderId = _OrderId;
        SetAddress(newShipment);
        if (ShipMethodList.SelectedIndex > -1)
        {
            newShipment.ShipMethodId = AlwaysConvert.ToInt(ShipMethodList.SelectedItem.Value);
            newShipment.ShipMethodName = ShipMethodList.SelectedItem.Text;
            // Add Shipping & Handling item
            OrderItem shipingCharges = new OrderItem();
            shipingCharges.Name = OrderItemType.Shipping.ToString();
            shipingCharges.OrderItemType = OrderItemType.Shipping;
            shipingCharges.Price = AlwaysConvert.ToDecimal(ShipCharges.Text);
            shipingCharges.OrderId = newShipment.OrderId;
            shipingCharges.Quantity = 1;
            shipingCharges.OrderShipmentId = newShipment.OrderShipmentId;
            newShipment.OrderItems.Add(shipingCharges);
        }
        newShipment.ShipMessage = ShipMessage.Text;         
        newShipment.Save();
        Response.Redirect( "Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        trNewAddress.Visible = (AddressList.SelectedIndex == (AddressList.Items.Count - 1));
        trShipCharge.Visible = (ShipMethodList.SelectedIndex != 0);
    }

}
