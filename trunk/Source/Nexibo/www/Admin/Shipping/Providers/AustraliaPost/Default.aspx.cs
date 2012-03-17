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
using CommerceBuilder.Shipping.Providers.AustraliaPost;

public partial class Admin_Shipping_Providers_AustraliaPost_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        //DEFAULT PAGE, REDIRECT TO APPROPRIATE LOCATION
        int _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        ShipGateway _ShipGateway = ShipGatewayDataSource.Load(_ShipGatewayId);
        if (_ShipGateway != null) 
        {
            AustraliaPost provider = _ShipGateway.GetProviderInstance() as AustraliaPost;
            if (provider != null)
            {
                Response.Redirect("Configure.aspx?ShipGatewayId=" + _ShipGatewayId.ToString());                
            }
        }
        Response.Redirect("Register.aspx");
    }

}
