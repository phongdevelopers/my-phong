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
using CommerceBuilder.Shipping.Providers;

public partial class Admin_Shipping_Providers_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected List<string> GetShipMethodList(object dataItem)
    {        
        ShipGateway gateway = (ShipGateway)dataItem;
        List<string> ShipMethods = new List<string>();
        foreach (ShipMethod method in gateway.ShipMethods)
        {
            ShipMethods.Add(method.Name);
        }
        return ShipMethods;
    }
    
    protected string GetConfigReference(object dataItem)
    {
        IShippingProvider provider = ((ShipGateway)dataItem).GetProviderInstance();
        if (provider != null)
        {
            return provider.Description;
        }
        return string.Empty;
    }

    protected string GetConfigUrl(object dataItem)
    {
        ShipGateway gateway = (ShipGateway)dataItem;
        IShippingProvider provider = gateway.GetProviderInstance();
        if (provider != null)
        {
            return provider.GetConfigUrl(Page.ClientScript) + "?ShipGatewayId=" + gateway.ShipGatewayId.ToString();
        }
        return string.Empty;
    }

    protected void Enabled_CheckedChanged(object sender, EventArgs e)
    {        
        CheckBox cbox = (CheckBox)sender;
        int sgwid = AlwaysConvert.ToInt(cbox.Text);
        ShipGateway sgw = ShipGatewayDataSource.Load(sgwid);
        if (sgw == null) return;
        sgw.Enabled = cbox.Checked;
        sgw.Save();
        GatewayGrid.DataBind();        
    }


    protected void ShipMethodList_DataBinding(object sender, EventArgs e)
    {

    }
    

}
