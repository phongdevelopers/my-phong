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

public partial class Admin_Orders_Shipments_EditShipment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ShipmentId;
    private OrderShipment _OrderShipment;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ShipmentId = AlwaysConvert.ToInt(Request.QueryString["OrderShipmentId"]);
        _OrderShipment = OrderShipmentDataSource.Load(_ShipmentId);
        if (_OrderShipment == null) Response.Redirect(CancelLink.NavigateUrl);
       
        //BIND THE ADDRESS
        initAddress();
        //BIND ADDITIONAL DETAILS
        ShipMessage.Text = _OrderShipment.ShipMessage;
        Caption.Text = string.Format(Caption.Text, _OrderShipment.ShipmentNumber);
        CancelLink.NavigateUrl += "?OrderNumber=" + _OrderShipment.Order.OrderNumber.ToString() + "&OrderId=" + _OrderShipment.OrderId.ToString();

        TrackingNumbersLabel.Visible = (_OrderShipment.TrackingNumbers != null && _OrderShipment.TrackingNumbers.Count > 0);
        ShipGateway.DataSource = ShipGatewayDataSource.LoadForStore();
        ShipGateway.DataBind();
        trAddTrackingNumber.Visible = (_OrderShipment.TrackingNumbers.Count == 0);        
    }

    protected void initAddress()
    {
        ShipToFirstName.Text = _OrderShipment.ShipToFirstName;
        ShipToLastName.Text = _OrderShipment.ShipToLastName;
        ShipToAddress1.Text = _OrderShipment.ShipToAddress1;
        ShipToAddress2.Text = _OrderShipment.ShipToAddress2;
        ShipToCity.Text = _OrderShipment.ShipToCity;
        ShipToProvince.Text = _OrderShipment.ShipToProvince;
        ShipToPostalCode.Text = _OrderShipment.ShipToPostalCode;
        ShipToCountryCode.DataSource = Token.Instance.Store.Countries;
        ShipToCountryCode.DataTextField = "Name";
        ShipToCountryCode.DataValueField = "CountryCode";
        ShipToCountryCode.DataBind();
        ShipToCountryCode.SelectedValue = _OrderShipment.ShipToCountryCode;
        ShipToPhone.Text = _OrderShipment.ShipToPhone;
        ShipToCompany.Text = _OrderShipment.ShipToCompany;
        ShipToFax.Text = _OrderShipment.ShipToFax;
        ShipToResidence.SelectedIndex = (_OrderShipment.ShipToResidence ? 0 : 1);
    }
   
    protected void SaveButton_Click(object sender, EventArgs e)
    {           
       
        //UPDATE ADDRESS
        _OrderShipment.ShipToFirstName = ShipToFirstName.Text;
        _OrderShipment.ShipToLastName = ShipToLastName.Text;
        _OrderShipment.ShipToAddress1 = ShipToAddress1.Text;
        _OrderShipment.ShipToAddress2 = ShipToAddress2.Text;
        _OrderShipment.ShipToCity = ShipToCity.Text;
        _OrderShipment.ShipToProvince = ShipToProvince.Text;
        _OrderShipment.ShipToPostalCode = ShipToPostalCode.Text;
        _OrderShipment.ShipToCountryCode = ShipToCountryCode.SelectedValue;
        _OrderShipment.ShipToPhone = ShipToPhone.Text;
        _OrderShipment.ShipToCompany = ShipToCompany.Text;
        _OrderShipment.ShipToFax = StringHelper.StripHtml(ShipToFax.Text);
        _OrderShipment.ShipToResidence = (ShipToResidence.SelectedIndex == 0);
        
        //UPDATE OTHER DETAILS
        _OrderShipment.ShipMessage = ShipMessage.Text;
        _OrderShipment.Save();
        //DELETE ANY ITEMS WITH A ZERO QUANTITY
        for (int i = _OrderShipment.OrderItems.Count - 1; i >= 0; i--)
        {
            if (_OrderShipment.OrderItems[i].Quantity < 1) _OrderShipment.OrderItems.DeleteAt(i);
        }
        
        foreach (GridViewRow gvr in TrackingGrid.Rows)
        {
            TextBox trackingNumberData = (TextBox)gvr.Cells[0].FindControl("TrackingNumberData");
            int trackingNumberId = AlwaysConvert.ToInt(TrackingGrid.DataKeys[gvr.RowIndex].Value);
            TrackingNumber trackingNumber = TrackingNumberDataSource.Load(trackingNumberId);
            if (trackingNumber != null && trackingNumberData != null)
            {
                trackingNumber.TrackingNumberData = trackingNumberData.Text;
                Response.Write(trackingNumber.Save().ToString());
            }
        }
        //REDIRECT TO SHIPMENT PAGE
        Response.Redirect(CancelLink.NavigateUrl);
    }
    
    protected void AddTrackingNumber_Click(Object sender, EventArgs e)
    {
        int shipgwId = AlwaysConvert.ToInt(ShipGateway.SelectedValue);
        string trackingData = TrackingNumber.Text;
        if (!string.IsNullOrEmpty(trackingData))
        {
            TrackingNumber tnum = new TrackingNumber();
            tnum.TrackingNumberData = trackingData;
            tnum.ShipGatewayId = shipgwId;
            tnum.OrderShipmentId = _OrderShipment.OrderShipmentId;
            _OrderShipment.TrackingNumbers.Add(tnum);
            _OrderShipment.Save();
            Response.Redirect(Request.Url.ToString()); 
        }
    }    


}
