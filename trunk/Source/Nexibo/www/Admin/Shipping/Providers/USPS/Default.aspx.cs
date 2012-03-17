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
using CommerceBuilder.Shipping.Providers.USPS;

public partial class Admin_Shipping_USPS_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        //DEFAULT PAGE, REDIRECT TO APPROPRIATE LOCATION
        int _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        ShipGateway _ShipGateway = ShipGatewayDataSource.Load(_ShipGatewayId);
        if (_ShipGateway != null) 
        {
            USPS provider = _ShipGateway.GetProviderInstance() as USPS;
            if (provider != null)
            {
                if (provider.UserIdActive) Response.Redirect("Configure.aspx?ShipGatewayId=" + _ShipGatewayId.ToString());
                Response.Redirect("Activate.aspx?ShipGatewayId=" + _ShipGatewayId.ToString());
            }
        }
        Response.Redirect("Register.aspx");
    }

}
