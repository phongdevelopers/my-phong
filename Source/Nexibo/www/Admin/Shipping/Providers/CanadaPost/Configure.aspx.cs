using CommerceBuilder.Shipping.Providers.CanadaPost;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_CanadaPost_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage {
    
    //private Guid _ShipGatewayId;
    private int _ShipGatewayId = 0;
    
    private ShipGateway _ShipGateway;
    
    public int ShipGatewayId {
        get {
            if (_ShipGatewayId == 0) {
                _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
            }
            return _ShipGatewayId;
        }
    }
    
    public ShipGateway ThisShipGateway {
        get {
            if ((_ShipGateway == null)) {
                if (ShipGatewayId!=0) {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(ShipGatewayId)) {
                        _ShipGateway = null;
                    }
                }
            }
            return _ShipGateway;
        }
    }

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(CanadaPost));
        }
    }
    
    protected void Page_Load(object sender, System.EventArgs e) {
        if ((ThisShipGateway == null)) {
            RedirectToManageProvider();
        }
        CanadaPost provider = (CanadaPost)ThisShipGateway.GetProviderInstance();
        if ((provider == null)) {
            RedirectToManageProvider();
        }
        if (!provider.IsActive) {
            // THE PROVIDER IS NOT YET ACTIVE, REDIRECT TO REGISTRATION SCREEN
            Response.Redirect(("Default.aspx?ShipGatewayId=" + ShipGatewayId.ToString()));
        }
        if (!Page.IsPostBack) {
            // INITIALIZE THE FORM FIELDS
            UseDebugMode.SelectedValue = ( provider.UseDebugMode ? "1" : "0" );
            UseTestMode.SelectedValue = ( provider.UseTestMode ? "1" : "0" );
            EnablePackageBreakup.SelectedValue = (provider.EnablePackageBreakup ? "1" : "0");
            MerchantCPCIDDisplay.Text = provider.MerchantCPCID;
            InstanceName.Text = ThisShipGateway.Name;
            LiveServerURL.Text = provider.LiveModeUrl;
            TestServerURL.Text = provider.TestModeUrl;
	    TrackingURL.Text = provider.TrackingUrl;
            MaxPackageWeight.Text = provider.MaxPackageWeight.ToString();
            MinPackageWeight.Text = provider.MinPackageWeight.ToString();

            //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ThisShipGateway.ClassId);
            if (configuredProviders.Count > 1)
            {
                trInstanceName.Visible = true;
            }
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        RedirectToManageProvider();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        CanadaPost provider = (CanadaPost)ThisShipGateway.GetProviderInstance();
        provider.UseDebugMode = (UseDebugMode.SelectedValue.Equals("1"));
        provider.UseTestMode = (UseTestMode.SelectedValue.Equals("1"));
        provider.EnablePackageBreakup = (EnablePackageBreakup.SelectedValue == "1");
        provider.MerchantCPCID = MerchantCPCIDDisplay.Text;
        provider.LiveModeUrl = LiveServerURL.Text;
        provider.TestModeUrl = TestServerURL.Text;
	provider.TrackingUrl = TrackingURL.Text;
        provider.MaxPackageWeight = AlwaysConvert.ToDecimal(MaxPackageWeight.Text, (decimal)provider.MaxPackageWeight);
        provider.MinPackageWeight = AlwaysConvert.ToDecimal(MinPackageWeight.Text, (decimal)provider.MinPackageWeight);

        //provider.Name = InstanceName.Text;
        ThisShipGateway.UpdateConfigData(provider.GetConfigData());
        ThisShipGateway.Name = InstanceName.Text;
        ThisShipGateway.Save();
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e) {
        ThisShipGateway.Delete();
        // Response.Redirect("DeleteConfirm.aspx")
        RedirectToManageProvider();
    }
    
    protected void RedirectToManageProvider() {
        CanadaPost provider;
        provider = new CanadaPost();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }

}
