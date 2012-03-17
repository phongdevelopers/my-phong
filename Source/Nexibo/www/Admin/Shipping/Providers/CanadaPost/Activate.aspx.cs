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
using CommerceBuilder.Shipping.Providers.CanadaPost;
using CommerceBuilder.Shipping;

public partial class Admin_Shipping_Providers_CanadaPost_Activate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    private int _ShipGatewayId;
    private ShipGateway _ShipGateway;
    CanadaPost _Provider;

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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (GetShipGateway == null)
        {
            RedirectToManageProvider();
        }

        if (!Page.IsPostBack)
        {
            CanadaPost provider = (CanadaPost)GetShipGateway.GetProviderInstance();
            CanadaPostMerchantCPCID.Text = provider.MerchantCPCID;            
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if (UserIdActive.Checked)
        {
            _Provider = _ShipGateway.GetProviderInstance() as CanadaPost;
            _Provider.AccountActive = UserIdActive.Checked;
            _ShipGateway.UpdateConfigData(_Provider.GetConfigData());
            _ShipGateway.Save();
            RedirectToConfigure();
        }
    }

    protected void RedirectToConfigure()
    {
        Response.Redirect("Configure.aspx?ShipGatewayId=" + _ShipGatewayId.ToString());
    }



    protected void RedirectToManageProvider()
    {
        CanadaPost provider = new CanadaPost();
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType())));
    }
    

}
