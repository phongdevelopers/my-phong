using System.Text.RegularExpressions;
using System.Collections.Generic;
using CommerceBuilder.Shipping.Providers.FedEx;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using CommerceBuilder.Common;
using CommerceBuilder.Users;

partial class Admin_Shipping_Providers_FedEx_Register1 : CommerceBuilder.Web.UI.AbleCommerceAdminPage {
    
    private void AddCustomErrorMessage(string message, Control controlToValidate) {
        CustomValidator customValidator = new CustomValidator();
        customValidator.IsValid = false;
        customValidator.ErrorMessage = message;
        customValidator.Text = "*";
        controlToValidate.Parent.Controls.Add(customValidator);
    }
    
    protected void FinishButton_Click(object sender, System.EventArgs e) {
        if (Page.IsValid) {
            // CUSTOM ERROR VALIDATION
            if (Country.SelectedValue.Equals("US")) {
                // MAKE SURE POSTALCODE AND PROVINCECODE WERE PROVIDED
                if (string.IsNullOrEmpty(Province.Text)) {
                    AddCustomErrorMessage("You must enter a valid 2 letter state abbreviation.", Province);
                }else {
                    // MAKE SURE THE VALUE IS VALID
                    Province.Text = Province.Text.ToUpperInvariant();
                    string[] states = "AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|AE|AA|AE|AE|AP".Split("|".ToCharArray());
                    if ((Array.IndexOf(states, Province.Text) < 0)) {
                        AddCustomErrorMessage("You must enter a valid 2 letter state abbreviation.", Province);
                    }
                }
                if (string.IsNullOrEmpty(PostalCode.Text)) {
                    AddCustomErrorMessage("Your 5 digit ZIP code is required.", PostalCode);
                }else {
                    // MAKE SURE ZIP IS VALID FORMAT
                    if (!Regex.IsMatch(PostalCode.Text, "^\\d{5}$")) {
                        AddCustomErrorMessage("You must enter a valid 5 digit ZIP code.", PostalCode);
                    }
                }
            }else if (Country.SelectedValue.Equals("CA")) {
                // MAKE SURE POSTALCODE AND PROVINCECODE WERE PROVIDED
                if (string.IsNullOrEmpty(Province.Text)) {
                    AddCustomErrorMessage("You must enter a valid 2 letter province abbreviation.", Province);
                }else {
                    // MAKE SURE THE VALUE IS VALID
                    Province.Text = Province.Text.ToUpperInvariant();
                    string[] states = "AB|BC|MB|NB|NL|NT|NS|NU|ON|PE|QC|SK|YT".Split("|".ToCharArray());
                    if ((Array.IndexOf(states, Province.Text) < 0)) {
                        AddCustomErrorMessage("You must enter a valid 2 letter province abbreviation.", Province);
                    }
                }
                if (string.IsNullOrEmpty(PostalCode.Text)) {
                    AddCustomErrorMessage("Your 6 character postal code is required.", PostalCode);
                }
                else {
                    // MAKE SURE ZIP IS VALID FORMAT
                    PostalCode.Text = PostalCode.Text.ToUpperInvariant().Replace(" ", "");
                    if (!Regex.IsMatch(PostalCode.Text, "^([A-Za-z]\\d[A-Za-z]( |-)*\\d[A-Za-z]\\d)$"))
                    {
                        AddCustomErrorMessage("You must enter a valid postal code.", PostalCode);
                    }
                }
            }
            // VALIDATE PHONE NUMBER            
            Phone.Text = Regex.Replace(Phone.Text, "[^0-9]", "");
            Regex phoneRegex = new Regex(@"^\d{10,16}$");
            if (!phoneRegex.IsMatch(Phone.Text))
            {
                PhoneValidator2.IsValid = false;
            }


            if (Page.IsValid) {
                // CONTINUE WITH REGISTRATION
                FedEx.FedExRegistrationInformation registration = new FedEx.FedExRegistrationInformation();
                registration.ShipperAccount = FedExAccountName.Text;
                registration.ContactName = ContactName.Text;
                registration.Company = CompanyName.Text;
                registration.Address1 = Address1.Text;
                registration.Address2 = Address2.Text;
                registration.City = City.Text;
                registration.CountryCode = Country.SelectedValue;
                if ((registration.CountryCode.Equals("US") || registration.CountryCode.Equals("CA"))) {
                    registration.StateProvinceCode = Province.Text;
                    registration.PostalCode = PostalCode.Text;
                }
                registration.ContactPhone = Phone.Text;
                registration.ContactEmail = Email.Text;
                // SEND THE REGISTRATION
                FedEx provider = new FedEx();
                FedEx.FedExRegistrationResult regResult;
                regResult = provider.Register(registration);
                if (regResult.successful) {
                    ShipGateway gateway = new ShipGateway();                    
                    gateway.ClassId = Misc.GetClassId(provider.GetType());
                    gateway.UpdateConfigData(provider.GetConfigData());
                    gateway.Name = InstanceName.Text;
                    gateway.Enabled = true;
                    gateway.Save();
                    Response.Redirect(("Configure.aspx?ShipGatewayId=" + gateway.ShipGatewayId.ToString()));
                }
                else {
                    AddCustomErrorMessage((regResult.errorCode + (" : " + regResult.errorMessage)), CustomErrorMessage);
                }
            }
        }
    }
    
    protected void RedirectToMain() {
        FedEx provider;
        provider = new FedEx();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToMain();
    }

    protected string ClassId
    {
        get
        {
            return Misc.GetClassId(typeof(FedEx));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e) {       
        if (!Page.IsPostBack) {
            // If USER HAS ACCEPTED, OTHERWISE REDIRECT TO STEP ONE
            bool isAccepted = AlwaysConvert.ToBool(Request["Accept"],false);
            if (!isAccepted)
            {
                Response.Redirect("Register.aspx");
            }
            // PREFILL DEFAULT VALUES FOR FORM            
            Warehouse defaultWarehouse = Token.Instance.Store.DefaultWarehouse;

            ContactName.Text = ((Token.Instance.User.PrimaryAddress.FirstName + (" " + Token.Instance.User.PrimaryAddress.LastName))).Trim(); ;
            CompanyName.Text = Token.Instance.Store.Name;
            Address1.Text = defaultWarehouse.Address1;
            Address2.Text = defaultWarehouse.Address2;
            City.Text = defaultWarehouse.City;
            string countryCode = defaultWarehouse.CountryCode;
            if ((countryCode.Equals("US") || countryCode.Equals("CA"))) {
                Province userProvince = new Province();
                if ((!string.IsNullOrEmpty(defaultWarehouse.Province)
                            && (defaultWarehouse.Province.Length == 2)))
                {
                    Province.Text = defaultWarehouse.Province;
                }                
                PostalCode.Text = defaultWarehouse.PostalCode;
            }
            ListItem countryItem = Country.Items.FindByValue(countryCode);
            if (countryItem !=null) {
                countryItem.Selected = true;
            }
            Phone.Text = Regex.Replace(defaultWarehouse.Phone, "[^0-9]", "");
            Email.Text = defaultWarehouse.Email;
            Register2Link.NavigateUrl = ("Register2.aspx");
            //CHECK WHETHER ANY PROVIDERS HAVE BEEN CONFIGURED
            ShipGatewayCollection configuredProviders = ShipGatewayDataSource.LoadForClassId(ClassId);
            if (configuredProviders.Count > 0)
            {
                trInstanceName.Visible = true;
                InstanceName.Text = new FedEx().Name + " #" + ((int)(configuredProviders.Count + 1)).ToString();
            }
        }
    }
}
