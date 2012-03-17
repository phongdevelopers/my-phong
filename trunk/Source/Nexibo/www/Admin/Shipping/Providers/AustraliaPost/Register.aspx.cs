using CommerceBuilder.Shipping.Providers.AustraliaPost;
using System;
using System.Web;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

partial class Admin_Shipping_Providers_AustraliaPost_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage {

	private string noticeText = "accepts responsibility for any loss of, damage to, late delivery or non-delivery of goods ordered from our web site. To the maximum extent permitted by law, you agree to release our carriers from any liability relating to loss of, damage to, late delivery or non-delivery of any goods you order from this web site and to assign all rights to claim compensation or insurance against our carriers to us and expressly and irrevocably do so by submitting your order";

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(AustraliaPost));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e) 
	{
		StoreName.Text = CommerceBuilder.Common.Token.Instance.Store.Name;

        int shipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        ShipGateway shipGateway = ShipGatewayDataSource.Load(shipGatewayId);
        if (shipGateway != null)
        {
            AustraliaPost provider = shipGateway.GetProviderInstance() as AustraliaPost;
            if (provider != null)
            {
                Response.Redirect("Configure.aspx?ShipGatewayId=" + shipGatewayId.ToString());                
            }
        }

        //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
        ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ClassId);
        if (configuredProviders.Count > 0)
        {
            trInstanceName.Visible = true;
            InstanceName.Text = new AustraliaPost().Name + " #" + ((int)(configuredProviders.Count + 1)).ToString();            
        }        
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToMain();
    }
    
    protected void RegisterButton_Click(object sender, System.EventArgs e) {
		
		string terms = CommerceBuilder.Common.Token.Instance.Store.Settings.CheckoutTermsAndConditions;
		if(!string.IsNullOrEmpty(terms) && terms.Contains(noticeText))
		{		 
			ShipGateway gateway = new ShipGateway();
			gateway.Name = InstanceName.Text;
			gateway.ClassId = this.ClassId;
			AustraliaPost provider = new AustraliaPost();
			provider.AccountActive = true;
			gateway.UpdateConfigData(provider.GetConfigData());
            gateway.Enabled = true;
			gateway.Save();
			Response.Redirect(("Configure.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString()));
		}
		else 
		{
			ErrorPanel.Visible = true;
			ErrorMessage.Text = string.Format(ErrorMessage.Text,Page.ResolveUrl("~/Admin/Store/StoreSettings.aspx"));
		}
    }

    protected void RedirectToMain()
    {
        Response.Redirect("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(ClassId));
    }
}
