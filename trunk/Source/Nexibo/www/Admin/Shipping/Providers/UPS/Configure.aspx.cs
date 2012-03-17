using CommerceBuilder.Shipping.Providers.UPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;


partial class Admin_Shipping_Providers_UPS_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage {
    
    private int _ShipGatewayId;    
    private ShipGateway _ShipGateway;


    
    public int ShipGatewayId {
        get {
            if (_ShipGatewayId.Equals(0)) {
                _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
            }
            return _ShipGatewayId;
        }
    }
    
    public ShipGateway ShipGateway {
        get {
            if ((_ShipGateway == null)) {
                if (!ShipGatewayId.Equals(0)) {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(ShipGatewayId)) {
                        _ShipGateway = null;
                    }
                }                
            }
            return _ShipGateway;
        }
    }
    
    protected string ClassId {
        get {
            return  Misc.GetClassId(typeof(UPS));
        }
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToManageProvider();
    }
    
    protected void SaveButton_Click(object sender, System.EventArgs e) {
        UPS provider = (UPS)ShipGateway.GetProviderInstance();
        int customerTypeIndex;
        if (CustomerType_DailyPickup.Checked) {
            customerTypeIndex = 0;
        }
        else if (CustomerType_Occasional.Checked) {
            customerTypeIndex = 1;
        }
        else {
            customerTypeIndex = 2;
        }
        provider.CustomerType = ((UPS.UpsCustomerType)(customerTypeIndex));
        provider.UseInsurance = (UseInsurance.SelectedValue.Equals("1"));
        provider.UseDebugMode = (UseDebugMode.SelectedValue.Equals("1"));
        provider.UseTestMode = (UseTestMode.SelectedValue.Equals("1"));
        provider.LiveModeUrl = LiveServerURL.Text;
        provider.TestModeUrl = TestServerURL.Text;
        provider.TrackingUrl = TrackingURL.Text;
        provider.ShipperNumber = ShipperNumber.Text;
        provider.MaxPackageWeight = AlwaysConvert.ToDecimal(MaxPackageWeight.Text, (decimal)provider.MaxPackageWeight);
        provider.MinPackageWeight = AlwaysConvert.ToDecimal(MinPackageWeight.Text, (decimal)provider.MinPackageWeight);


        ShipGateway.UpdateConfigData(provider.GetConfigData());
        //  update the name of the gateway if needed
        ShipGateway.Name = InstanceName.Text;
        ShipGateway.Save();
    }
    
    protected void Page_Load(object sender, System.EventArgs e) {
        if ((ShipGateway == null)) {
            RedirectToManageProvider();
        }
        // GET THE PROVIDER INSTANCE AND MAKE SURE IT IS ACTIVE
        UPS provider = (UPS)ShipGateway.GetProviderInstance();
        if ((provider == null)) {
            // THIS GATEWAY DOES NOT HAVE A UPS PROVIDER INSTANCE (UNEXPECTED)
            RedirectToManageProvider();
        }
        if (!provider.IsActive) {
            // THE PROVIDER IS NOT YET ACTIVE, REDIRECT TO REGISTRATION SCREEN
            Response.Redirect(("Default.aspx?ShipGatewayId=" + ShipGatewayId.ToString()));
        }
        AccessKey.Text = provider.AccessKey;
        if (!Page.IsPostBack) {
            // INITIALIZE THE FORM FIELDS
            if ((provider.CustomerType == UPS.UpsCustomerType.DailyPickup)) {
                CustomerType_DailyPickup.Checked = true;
            }
            else if ((provider.CustomerType == UPS.UpsCustomerType.Occasional)) {
                CustomerType_Occasional.Checked = true;
            }
            else {
                CustomerType_Retail.Checked = true;
            }
            UseInsurance.SelectedValue = (provider.UseInsurance ? "1" : "0");
            UseDebugMode.SelectedValue = (provider.UseDebugMode ? "1" : "0");
            UseTestMode.SelectedValue = (provider.UseTestMode ? "1" : "0");
            InstanceName.Text = _ShipGateway.Name;
            LiveServerURL.Text = provider.LiveModeUrl;
            TestServerURL.Text = provider.TestModeUrl;
            TrackingURL.Text = provider.TrackingUrl;
            ShipperNumber.Text = provider.ShipperNumber;
            MaxPackageWeight.Text = provider.MaxPackageWeight.ToString();
            MinPackageWeight.Text = provider.MinPackageWeight.ToString();

            //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ShipGateway.ClassId);
            if (configuredProviders.Count > 0)
            {
                trInstanceName.Visible = true;
            }

        }
    }
    
    protected void DeleteButton_Click(object sender, System.EventArgs e) {
        ShipGateway.Delete();
        Response.Redirect("DeleteConfirm.aspx");
    }
    
    protected void RedirectToManageProvider() {
        UPS provider;
        provider = new UPS();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }
}
