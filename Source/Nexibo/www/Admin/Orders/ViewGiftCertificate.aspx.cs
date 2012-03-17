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

public partial class Admin_Orders_ViewGiftCertificate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _GiftCertificateId;
    private GiftCertificate _GiftCertificate;

       
    protected void Page_Init(object sender, EventArgs e)
    {
        _GiftCertificateId = AlwaysConvert.ToInt(Request.QueryString["GiftCertificateId"]);
        _GiftCertificate = GiftCertificateDataSource.Load(_GiftCertificateId);
        if (_GiftCertificate == null) Response.Redirect("Default.aspx");        
        if (!Page.IsPostBack)
        {
            //UPDATE CAPTION
            Caption.Text = String.Format(Caption.Text, _GiftCertificate.Name,_GiftCertificate.OrderItem.Order.OrderId);
            BindGiftCertificate();
        }
    }

    protected void BindGiftCertificate()
    {
        Name.Text = _GiftCertificate.Name;

        if (string.IsNullOrEmpty(_GiftCertificate.SerialNumber))
        {
            Serial.Text = "Not Assigned Yet.";
        }
        else
        {
            Serial.Text = _GiftCertificate.SerialNumber;
        }        
        
        Balance.Text = string.Format("{0:lc}",_GiftCertificate.Balance);
        
        // SHOW LAST DESCRIPTION
        if (_GiftCertificate.Transactions != null && _GiftCertificate.Transactions.Count > 0)
        {
            GiftCertificateTransaction gct = _GiftCertificate.Transactions[_GiftCertificate.Transactions.Count - 1];
            Description.Text =  gct.Description + " (Modified at " + gct.TransactionDate.ToString() + ")";
        }
        
        if (_GiftCertificate.IsExpired())
        {
            Expires.Text = "Already expired at " + _GiftCertificate.ExpirationDate.ToString();
        }
        else if (_GiftCertificate.ExpirationDate == null || _GiftCertificate.ExpirationDate.Equals(DateTime.MinValue))
        {
            Expires.Text = "N/A";
        }
        else
        {
            Expires.Text = _GiftCertificate.ExpirationDate.ToString();
        }
    }

    

}
