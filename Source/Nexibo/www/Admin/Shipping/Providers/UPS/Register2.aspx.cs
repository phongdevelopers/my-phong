
using CommerceBuilder.Shipping.Providers.UPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_UPS_Register2 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            UPS provider = new UPS();
            provider.UserId = UserId.Text;
            provider.Password = Password.Text;
            provider.AccessKey = AccessKey.Text;
            ShipGateway shipGateway = new ShipGateway();
            shipGateway.Name = provider.Name;
            shipGateway.ClassId = Misc.GetClassId(typeof(UPS));
            shipGateway.UpdateConfigData(provider.GetConfigData());
            shipGateway.Enabled = true;
            shipGateway.Save();
            Response.Redirect("Configure.aspx?ShipGatewayId=" + shipGateway.ShipGatewayId.ToString());
        }
    }
}
