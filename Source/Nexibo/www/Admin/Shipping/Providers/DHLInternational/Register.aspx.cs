using CommerceBuilder.Shipping.Providers.DHLInternational;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_DHL_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage {

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(DHLInternational));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e) {
        int shipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        ShipGateway shipGateway = ShipGatewayDataSource.Load(shipGatewayId);
        if (shipGateway != null)
        {
            DHLInternational provider = shipGateway.GetProviderInstance() as DHLInternational;
            if (provider != null)
            {
                if (provider.IsActive) Response.Redirect("Configure.aspx?ShipGatewayId=" + shipGatewayId.ToString());
                Response.Redirect("Activate.aspx?ShipGatewayId=" + shipGatewayId.ToString());
            }
        }

        //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
        ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ClassId);
        if (configuredProviders.Count > 0)
        {
            trInstanceName.Visible = true;
            InstanceName.Text = new DHLInternational().Name + " #" + ((int)(configuredProviders.Count + 1)).ToString();
        }
    }


    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToMain();
    }
    
    protected void RegisterButton_Click(object sender, System.EventArgs e) {
        if (Page.IsValid)
        {
            ShipGateway gateway = new ShipGateway();
            gateway.Name = InstanceName.Text;
            gateway.ClassId = this.ClassId;
            DHLInternational provider = new DHLInternational();
            provider.UserID = DHLUserID.Text;
            provider.Password = DHLPassword.Text;
            gateway.UpdateConfigData(provider.GetConfigData());
            gateway.Enabled = true;
            gateway.Save();
            Response.Redirect("Configure.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString());
        }
    }

    protected void RedirectToMain()
    {
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(ClassId));
    }

}
