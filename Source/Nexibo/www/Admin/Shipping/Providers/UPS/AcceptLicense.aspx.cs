using CommerceBuilder.Shipping.Providers.UPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_UPS_AcceptLicense : CommerceBuilder.Web.UI.AbleCommerceAdminPage {
    
    private int _ShipGatewayId;    
    private ShipGateway _ShipGateway;

    
    public int ShipGatewayId {
        get {
            if (_ShipGatewayId.Equals(0)) {
                _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
                if (_ShipGatewayId.Equals(0)) {
                    _ShipGatewayId = ShipGatewayDataSource.GetShipGatewayIdByClassId(Misc.GetClassId(typeof(UPS)));
                }
            }
            return _ShipGatewayId;
        }        
    }
    
    public ShipGateway ShipGateway {
        get {
            if ((_ShipGateway == null)) {
                if (ShipGatewayId.Equals(0)) {
                    _ShipGateway = new ShipGateway();
                    if (!_ShipGateway.Load(ShipGatewayId)) {
                        _ShipGateway = null;
                    }
                }                
            }
            return _ShipGateway;
        }
    }
    
    protected void Page_Load(object sender, System.EventArgs e) {
        // FIND WHETHER THIS CLASS IS ALREADY CONFIGURED
        int shipGatewayId = ShipGatewayDataSource.GetShipGatewayIdByClassId(Misc.GetClassId(typeof(UPS)));
        if ((ShipGateway == null)) {
            Response.Redirect("Default.aspx");
        }
        UPS provider = (UPS)ShipGateway.GetProviderInstance();
        if ((provider == null)) {
            Response.Redirect("Default.aspx");
        }
        if (provider.IsActive) {
            // REDIRECT TO CONFIGURE SCREEN
            Response.Redirect(("Configure.aspx?ShipGatewayId=" + shipGatewayId.ToString()));
        }
        if (!provider.IsRegistered) {
            // REDIRECT TO STANDARD REGISTRATION
            Response.Redirect(("Register1.aspx?ShipGatewayId=" + shipGatewayId.ToString()));
        }
        InstanceNameLabel.Text = _ShipGateway.Name;
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        ShipGateway.Delete();
        Response.Redirect("Default.aspx");
    }
}
