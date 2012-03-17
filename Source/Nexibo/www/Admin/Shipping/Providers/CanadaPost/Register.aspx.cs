using CommerceBuilder.Shipping.Providers.CanadaPost;
using System;
using System.Web;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

partial class Admin_Shipping_Providers_CanadaPost_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage {

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(CanadaPost));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e) {
        int shipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        ShipGateway shipGateway = ShipGatewayDataSource.Load(shipGatewayId);
        if (shipGateway != null)
        {
            CanadaPost provider = shipGateway.GetProviderInstance() as CanadaPost;
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
            InstanceName.Text = new CanadaPost().Name + " #" + ((int)(configuredProviders.Count + 1)).ToString();            
        }        
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToMain();
    }
    
    protected void RegisterButton_Click(object sender, System.EventArgs e) {
        ShipGateway gateway = new ShipGateway();
        gateway.Name = InstanceName.Text;
        gateway.ClassId = this.ClassId;
        CanadaPost provider = new CanadaPost();
        provider.MerchantCPCID = CanadaPostMerchantCPCID.Text;
        gateway.UpdateConfigData(provider.GetConfigData());
        gateway.Enabled = true;
        gateway.Save();
        Response.Redirect(("Activate.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString()));
    }

    protected void RedirectToMain()
    {
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(ClassId));
    }
}
