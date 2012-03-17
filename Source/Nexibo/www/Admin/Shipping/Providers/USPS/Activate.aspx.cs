using CommerceBuilder.Shipping.Providers.USPS;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_USPS_Activate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _ShipGatewayId;
    ShipGateway _ShipGateway;
    USPS _Provider;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        _ShipGateway = ShipGatewayDataSource.Load(_ShipGatewayId);
        if (_ShipGateway == null) RedirectToRegister();
        _Provider = _ShipGateway.GetProviderInstance() as USPS;
        if (_Provider == null) RedirectToRegister();
        if (_Provider.UserIdActive) RedirectToConfigure();
        if (!Page.IsPostBack)
        {
            UserId.Text = _Provider.UserId;
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if (UserIdActive.Checked)
        {
            _Provider.UserIdActive = UserIdActive.Checked;
            _ShipGateway.UpdateConfigData(_Provider.GetConfigData());
            _ShipGateway.Save();
            RedirectToConfigure();
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        _ShipGateway.Delete();
        Response.Redirect("Register.aspx");
    }

    protected void RedirectToRegister()
    {
        Response.Redirect("Register.aspx");
    }

    protected void RedirectToConfigure()
    {
        Response.Redirect("Configure.aspx?ShipGatewayId=" + _ShipGatewayId.ToString());
    }

}
