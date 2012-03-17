using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using System.Collections.Generic;
using CommerceBuilder.Shipping;
using CommerceBuilder.Shipping.Providers.DHLInternational;

public partial class Admin_Shipping_DHLInternational_Activate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    private int _ShipGatewayId = 0;
    private ShipGateway _ShipGateway;

    protected int ShipGatewayId
    {
        get
        {
            if (_ShipGatewayId == 0)
            {
                _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
            }
            return _ShipGatewayId;
        }
    }

    protected ShipGateway GetShipGateway
    {
        get
        {
            if (_ShipGateway == null)
            {
                if (!ShipGatewayId.Equals(0))
                {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(ShipGatewayId))
                    {
                        _ShipGateway = null;
                    }
                }
            }
            return _ShipGateway;
        }
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ShipGateway gateway = GetShipGateway;
            DHLInternational provider = (DHLInternational)gateway.GetProviderInstance();
            gateway.ClassId = Misc.GetClassId(provider.GetType());
            provider.UserID = DHLUserID.Text;
            provider.Password = DHLPassword.Text;
            gateway.UpdateConfigData(provider.GetConfigData());
            gateway.Save();
            Response.Redirect("Configure.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString());
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        RedirectToManageProvider();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (GetShipGateway == null)
        {
            RedirectToManageProvider();
        }

        DHLInternational provider;
        if (!Page.IsPostBack)
        {
            provider = (DHLInternational)GetShipGateway.GetProviderInstance();
            DHLUserID.Text = provider.UserID;
            DHLPassword.Text = provider.Password;
            InstanceNameLabel.Text = GetShipGateway.Name;
        }
    }

    protected void RedirectToManageProvider()
    {
        DHLInternational provider;
        provider = new DHLInternational();
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType())));
    }
    

}
