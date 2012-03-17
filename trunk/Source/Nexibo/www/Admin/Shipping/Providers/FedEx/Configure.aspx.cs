using CommerceBuilder.Shipping.Providers.FedEx;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;

partial class Admin_Shipping_Providers_FedEx_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage
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
            return Misc.GetClassId(typeof(FedEx));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        // MAKE SURE A VALID SHIPGATEWAY ID IS FOUND
        if ((ShipGateway == null))
        {
            RedirectToManageProvider();
        }
        // GET THE PROVIDER INSTANCE AND MAKE SURE IT IS ACTIVE
        FedEx provider = (FedEx)ShipGateway.GetProviderInstance();
        if ((provider == null))
        {
            // THIS GATEWAY DOES NOT HAVE A FedEx PROVIDER INSTANCE (UNEXPECTED)
            // Response.Redirect("Default.aspx")
            RedirectToManageProvider();
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
            IncludeDeclaredValue.SelectedValue = (provider.IncludeDeclaredValue ? "1" : "0");
            AccountNameDisplay.Text = provider.AccountNumber;
            MeterNumberDisplay.Text = provider.MeterNumber;
            InstanceName.Text = _ShipGateway.Name;
            BindDropOffType(provider.DropOffType);
            BindPackagingType(provider.PackagingType);
            LiveServerURL.Text = provider.LiveModeUrl;
            TestServerURL.Text = provider.TestModeUrl;
            TrackingURL.Text = provider.TrackingUrl;
            MaxPackageWeight.Text = provider.MaxPackageWeight.ToString();
            MinPackageWeight.Text = provider.MinPackageWeight.ToString();

            // UPDATE THE WEIGHT UNIT
            // ACCEPT WEIGHT AS PER STORE WEIGHT UNIT SETTINGS (BUG # 8821)
            WeightUnitLabel.Text = (Token.Instance.Store.WeightUnit == WeightUnit.Grams || Token.Instance.Store.WeightUnit == WeightUnit.Kilograms)?"kgs":"lbs";
            WeightUnitLabel2.Text = WeightUnitLabel.Text;

            // CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ShipGateway.ClassId);
            if (configuredProviders.Count > 1)
            {
                trInstanceName.Visible = true;
            }
        }
    }

    protected void RedirectToManageProvider()
    {
        FedEx provider;
        provider = new FedEx();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        RedirectToManageProvider();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        FedEx provider = (FedEx)ShipGateway.GetProviderInstance();
        provider.UseDebugMode = (UseDebugMode.SelectedValue == "1");
        provider.UseTestMode = (UseTestMode.SelectedValue == "1");
        provider.EnablePackageBreakup = (EnablePackageBreakup.SelectedValue == "1");
        provider.IncludeDeclaredValue = (IncludeDeclaredValue.SelectedValue == "1");
        provider.AccountNumber = AccountNameDisplay.Text;
        provider.MeterNumber = MeterNumberDisplay.Text;
        provider.DropOffType = (FedEx.FDXDropOffType)AlwaysConvert.ToEnum(typeof(FedEx.FDXDropOffType), DropOffType.SelectedValue, FedEx.FDXDropOffType.REGULARPICKUP);
        provider.PackagingType = (FedEx.FDXPackagingType)AlwaysConvert.ToEnum(typeof(FedEx.FDXPackagingType), PackagingType.SelectedValue, FedEx.FDXPackagingType.YOURPACKAGING);
        provider.LiveModeUrl = LiveServerURL.Text;
        provider.TestModeUrl = TestServerURL.Text;
        provider.TrackingUrl = TrackingURL.Text;
        provider.MaxPackageWeight = AlwaysConvert.ToDecimal(MaxPackageWeight.Text, (decimal)provider.MaxPackageWeight);
        provider.MinPackageWeight = AlwaysConvert.ToDecimal(MinPackageWeight.Text, (decimal)provider.MinPackageWeight);
        ShipGateway.UpdateConfigData(provider.GetConfigData());
        ShipGateway.Name = InstanceName.Text;
        ShipGateway.Save();
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        ShipGateway.Delete();
        Response.Redirect("DeleteConfirm.aspx");
    }

    protected void BindDropOffType(FedEx.FDXDropOffType dType)
    {
        DropOffType.Items.Clear();
        foreach (FedEx.FDXDropOffType dOffType in Enum.GetValues(typeof(FedEx.FDXDropOffType)))
        {
            ListItem newItem = new ListItem(dOffType.ToString(), dOffType.ToString());
            DropOffType.Items.Add(newItem);
            if (dOffType == dType)
            {
                newItem.Selected = true;
            }
        }
    }

    protected void BindPackagingType(FedEx.FDXPackagingType pType)
    {
        PackagingType.Items.Clear();
        foreach (FedEx.FDXPackagingType pkgType in Enum.GetValues(typeof(FedEx.FDXPackagingType)))
        {
            ListItem newItem = new ListItem(pkgType.ToString(), pkgType.ToString());
            PackagingType.Items.Add(newItem);
            if (pkgType == pType)
            {
                newItem.Selected = true;
            }
        }
    }
}