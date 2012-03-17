using CommerceBuilder.Shipping.Providers.DHLInternational;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

partial class Admin_Shipping_Providers_DHL_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ShipGatewayId = 0;

    private ShipGateway _ShipGateway;

    int ShipGatewayId
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

    ShipGateway ShipGateway
    {
        get
        {
            if ((_ShipGateway == null))
            {
                _ShipGateway = new ShipGateway();
                if (!_ShipGateway.Load(ShipGatewayId))
                {
                    _ShipGateway = null;
                }
            }
            return _ShipGateway;
        }
    }

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(DHLInternational));
        }
    }


    protected void Page_Load(object sender, System.EventArgs e)
    {
        // MAKE SURE A VALID SHIPGATEWAY ID IS FOUND
        if ((ShipGateway == null))
        {
            RedirectBack();
        }
        // GET THE PROVIDER INSTANCE AND MAKE SURE IT IS ACTIVE
        DHLInternational provider = (DHLInternational)ShipGateway.GetProviderInstance();
        if ((provider == null))
        {
            // THIS GATEWAY DOES NOT HAVE A DHL PROVIDER INSTANCE (UNEXPECTED)
            // Response.Redirect("Default.aspx")
            RedirectBack();
        }
        if (!provider.IsActive)
        {
            // THE PROVIDER IS NOT YET ACTIVE, REDIRECT TO REGISTRATION SCREEN
            Response.Redirect(("Default.aspx?ShipGatewayId=" + ShipGatewayId.ToString()));
        }
        if (!Page.IsPostBack)
        {
            // INITIALIZE THE FORM FIELDS
            UseDebugMode.SelectedValue = (provider.UseDebugMode ? "1" : "0");
            UseTestMode.SelectedValue = (provider.UseTestMode ? "1" : "0");
            EnablePackageBreakup.SelectedValue = (provider.EnablePackageBreakup ? "1" : "0");
            UserIDDisplay.Text = provider.UserID;
            PasswordDisplay.Text = provider.Password;
            InstanceName.Text = _ShipGateway.Name;
            ShippingKey.Text = provider.ShippingKey;
            AccountNumber.Text = provider.AccountNumber;
            DaysToShip.Text = provider.DaysToShip.ToString();
            LiveServerURL.Text = provider.LiveModeUrl;
            TestServerURL.Text = provider.TestModeUrl;
            TrackingURL.Text = provider.TrackingUrl;
            MaxPackageWeight.Text = provider.MaxPackageWeight.ToString();
            MinPackageWeight.Text = provider.MinPackageWeight.ToString();

            DutiableFlag.SelectedValue = (provider.DutiableFlag ? "1" : "0");
            CustomsValueMultiplier.Text = provider.CustomsValueMultiplier.ToString();
            CommerceLicensed.SelectedValue = (provider.CommerceLicensed ? "1" : "0");
            BindFilingType(provider.FilingType);
            FTRExemptionCode.Text = provider.FTRExemptionCode;
            ITNNumber.Text = provider.ITNNumber;
            EINCode.Text = provider.EINCode;            

            //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ShipGateway.ClassId);
            if (configuredProviders.Count > 1)
            {
                trInstanceName.Visible = true;
            }
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("../Default.aspx");
    }

    protected void RedirectBack()
    {
        DHLInternational provider;
        provider = new DHLInternational();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        DHLInternational provider = (DHLInternational)ShipGateway.GetProviderInstance();

        provider.UseDebugMode = (UseDebugMode.SelectedValue == "1");
        provider.UseTestMode = (UseTestMode.SelectedValue == "1");
        provider.EnablePackageBreakup = (EnablePackageBreakup.SelectedValue == "1");
        provider.ShippingKey = ShippingKey.Text;
        provider.AccountNumber = AccountNumber.Text;
        provider.UserID = UserIDDisplay.Text;
        provider.Password = PasswordDisplay.Text;
        provider.DaysToShip = AlwaysConvert.ToInt(DaysToShip.Text, 1);
        if (provider.DaysToShip < 1) provider.DaysToShip = 1;
        DaysToShip.Text = provider.DaysToShip.ToString();
        provider.LiveModeUrl = LiveServerURL.Text;
        provider.TestModeUrl = TestServerURL.Text;
        provider.TrackingUrl = TrackingURL.Text;
        provider.MaxPackageWeight = AlwaysConvert.ToDecimal(MaxPackageWeight.Text, (decimal)provider.MaxPackageWeight);
        provider.MinPackageWeight = AlwaysConvert.ToDecimal(MinPackageWeight.Text, (decimal)provider.MinPackageWeight);
	
	
        provider.DutiableFlag = (DutiableFlag.SelectedValue == "1");
        provider.CustomsValueMultiplier = AlwaysConvert.ToDecimal(CustomsValueMultiplier.Text,1);
        provider.CommerceLicensed = (CommerceLicensed.SelectedValue == "1");
        provider.FilingType = (DHLInternational.FilingTypeFlags)AlwaysConvert.ToEnum(typeof(DHLInternational.FilingTypeFlags), FilingType.SelectedValue, DHLInternational.FilingTypeFlags.ITN);
        provider.FTRExemptionCode = FTRExemptionCode.Text;
        provider.ITNNumber = ITNNumber.Text;
        provider.EINCode = EINCode.Text;

        ShipGateway.UpdateConfigData(provider.GetConfigData());
        ShipGateway.Name = InstanceName.Text;
        ShipGateway.Save();
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        ShipGateway.Delete();
        Response.Redirect("DeleteConfirm.aspx");
    }

    protected void BindFilingType(DHLInternational.FilingTypeFlags pType)
    {
        FilingType.Items.Clear();
        foreach (DHLInternational.FilingTypeFlags pkgType in Enum.GetValues(typeof(DHLInternational.FilingTypeFlags)))
        {
            ListItem newItem = new ListItem(pkgType.ToString(), pkgType.ToString());
            FilingType.Items.Add(newItem);
            if (pkgType == pType)
            {
                newItem.Selected = true;
            }
        }
    }
}
