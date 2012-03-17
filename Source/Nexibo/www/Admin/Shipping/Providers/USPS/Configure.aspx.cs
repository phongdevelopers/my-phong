using CommerceBuilder.Shipping.Providers.USPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_USPS_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    int _ShipGatewayId;
    ShipGateway _ShipGateway;

    public int ShipGatewayId
    {
        get
        {
            if (_ShipGatewayId.Equals(0))
            {
                _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
            }
            return _ShipGatewayId;
        }
    }

    public ShipGateway ShipGateway
    {
        get
        {
            if ((_ShipGateway == null))
            {
                if (!ShipGatewayId.Equals(0))
                {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(ShipGatewayId))
                    {
                        _ShipGateway = null;
                    }
                }
            }
            return _ShipGateway;
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        // RETURN TO MAIN MENU
        RedirectToManageProvider();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            USPS provider = (USPS)ShipGateway.GetProviderInstance();
            provider.UseDebugMode = (UseDebugMode.SelectedValue.Equals("1"));
            provider.UseTestMode = (UseTestMode.SelectedValue.Equals("1"));
            provider.UserId = UserId.Text;
            provider.LiveModeUrl = LiveServerURL.Text;
            provider.TestModeUrl = TestServerURL.Text;
            provider.TrackingUrl = TrackingURL.Text;
            provider.MaxPackageWeight = AlwaysConvert.ToDecimal(MaxPackageWeight.Text, (decimal)provider.MaxPackageWeight);
            provider.MinPackageWeight = AlwaysConvert.ToDecimal(MinPackageWeight.Text, (decimal)provider.MinPackageWeight);
            ShipGateway.UpdateConfigData(provider.GetConfigData());
            ShipGateway.Name = InstanceName.Text;
            ShipGateway.Save();
        }
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        this.ShipGateway.Delete();
        // RETURN TO MAIN MENU
        RedirectToManageProvider();
    }

    protected void RedirectToManageProvider()
    {
        USPS provider;
        provider = new USPS();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if ((ShipGateway == null))
        {
            RedirectToManageProvider();
        }
        USPS provider = (USPS)ShipGateway.GetProviderInstance();

        // If Not provider.IsActive then
        //      Response.Redirect("Default.aspx?ShipGatewayId=" & ShipGatewayId.ToString())
        // Else
        // End if
        if (!Page.IsPostBack)
        {
            UseDebugMode.SelectedValue = (provider.UseDebugMode ? "1" : "0");
            UseTestMode.SelectedValue = (provider.UseTestMode ? "1" : "0");
            UserId.Text = provider.UserId;
            InstanceName.Text = _ShipGateway.Name;
            LiveServerURL.Text = provider.LiveModeUrl;
            TestServerURL.Text = provider.TestModeUrl;
            TrackingURL.Text = provider.TrackingUrl;
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
}