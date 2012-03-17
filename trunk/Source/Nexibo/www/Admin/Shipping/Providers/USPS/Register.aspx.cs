using CommerceBuilder.Shipping.Providers.USPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_USPS_Register : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(USPS));
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ShipGateway gateway = new ShipGateway();
            gateway.Name = InstanceName.Text;
            gateway.ClassId = this.ClassId;
            USPS provider = new USPS();
            provider.UserId = UserId.Text;
            gateway.UpdateConfigData(provider.GetConfigData());
            gateway.Enabled = true;
            gateway.Save();
            
            if (UserIdActive.Checked)
            {
                provider.UserIdActive = UserIdActive.Checked;
                gateway.UpdateConfigData(provider.GetConfigData());
                gateway.Save();                
            }
            Response.Redirect("Configure.aspx?ShipGatewayId=" + gateway.ToString());
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Default.aspx");
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
        ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ClassId);
        if (configuredProviders.Count > 0)
        {
            trInstanceName.Visible = true;
            InstanceName.Text = "USPS #" + ((int)(configuredProviders.Count + 1)).ToString();
        }
    }

}
