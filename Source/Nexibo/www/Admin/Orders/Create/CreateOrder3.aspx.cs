using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Users;
using CommerceBuilder.Marketing;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Payments;
using CommerceBuilder.Stores;
using CommerceBuilder.Shipping;
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_CreateOrder3 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _UserId;
    private User _User;
    Basket _Basket;
    CountryCollection _Countries;

    private CountryCollection Countries
    {
        get
        {
            if (_Countries == null) InitializeCountries();
            return _Countries;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) Response.Redirect("CreateOrder1.aspx");
        _Basket = _User.Basket;
        MiniBasket1.BasketId = _Basket.BasketId;

        // PACKAGE BASKET IF NECESSARY
        if (!_Basket.IsPackaged) _Basket.Package();

        // INITIALIZE THE CAPTION
        string userName = _User.IsAnonymous ? "Unregistered User" : _User.UserName;
        Caption.Text = string.Format(Caption.Text, userName);

        // HIDE EMAIL ON BILLING ADDRESS FOR REGISTERED USERS
        if (_User.IsAnonymous)
        {
            BillToEmail.Visible = true;
            BillToEmailValidator.Enabled = true;
            BillToEmail.Text = _User.Email;
            BillToEmailLiteral.Visible = false;
        }
        else
        {
            BillToEmail.Visible = false;
            BillToEmailValidator.Enabled = false;
            BillToEmailLiteral.Visible = true;
            BillToEmailLiteral.Text = _User.Email;
        }

        //INITIALIZE BILLING COUNTRY LIST ON EVERY VISIT
        InitializeBillingCountryAndProvince();

        // INITIALIZE SHIPPING CONTROLS
        trAddressBook.Visible = _Basket.Items.HasShippableProducts;
        if (trAddressBook.Visible)
        {
            // THESE TASKS MUST OCCUR EVER VISIT DUE TO DISABLED VIEWSTATE
            InitializeAddressBook();
            InitializeShippingCountryAndProvince();

            // SEE IF WE NEED TO SHOW THE SHIPPING OFORM
            int shipAddressId = GetShippingAddressId();
            if (shipAddressId == 0 || shipAddressId == _User.PrimaryAddress.AddressId)
            {
                // SHIP TO BILLING ADDRESS
                ShipAddressPanel.Visible = false;
                trContinueButton1.Visible = true;
                BillAddressPanel.DefaultButton = "ContinueButton";
            }
            else
            {
                // INITIALIZE SHIPPING ADDRESS
                ShipAddressPanel.Visible = true;
                trContinueButton1.Visible = false;
                BillAddressPanel.DefaultButton = "ContinueButton2";
            }
        }

        // POPULATE BILLING FORM ON FIRST PAGE VISIT
        if (!Page.IsPostBack) InitializeBillingAddress();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // DETERMINE IF THE SHIPPING FORM MUST BE INITIALIZED
        bool initShipAddress;
        if (Page.IsPostBack)
        {
            // ON POSTBACK, WE ONLY INITIALIZE THE SHIPPING FORM IF THE ADDRESS SELECTION WAS CHANGED
            initShipAddress = (Request.Form["__EVENTTARGET"] == AddressBook.UniqueID);
        }
        else
        {
            // ON FIRST VISIT, WE INITIALIZE SHIPPING FORM IF THE SHIPPING ADDRESS IS NOT THE BILLING
            initShipAddress = (GetShippingAddressId() != _User.PrimaryAddress.AddressId);
        }
        if (initShipAddress) InitializeShippingAddress();
    }

    private void InitializeBillingCountryAndProvince()
    {
        // SET THE COUNTRY LIST
        BillToCountry.DataSource = this.Countries;
        BillToCountry.DataBind();

        // INITIALIZE THE BILLING COUNTRY AND PROVINCE LIST
        string countryCode, province;
        if (!Page.IsPostBack)
        {
            countryCode = _User.PrimaryAddress.CountryCode;
            province = _User.PrimaryAddress.Province;
            SelectCountryAndProvince(BillToCountry, countryCode, BillToProvince, BillToProvinceList, BillToPostalCodeRequired, province);
        }
        else
        {
            countryCode = Request.Form[BillToCountry.UniqueID];
            SelectCountry(BillToCountry, countryCode, BillToProvince, BillToProvinceList, BillToPostalCodeRequired);
            if (BillToProvinceList.Visible) province = Request.Form[BillToProvinceList.UniqueID];
            else province = Request.Form[BillToProvince.UniqueID];
            SelectProvince(BillToProvince, BillToProvinceList, province);
        }
    }

    private void InitializeShippingCountryAndProvince()
    {
        // SET THE COUNTRY LIST
        ShipToCountry.DataSource = this.Countries;
        ShipToCountry.DataBind();

        // INITIALIZE THE SHIPPING COUNTRY AND PROVINCE LIST
        string countryCode, province;
        if (!Page.IsPostBack)
        {
            Address shipAddress = GetShippingAddress();
            countryCode = shipAddress.CountryCode;
            province = shipAddress.Province;
            SelectCountryAndProvince(ShipToCountry, countryCode, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired, province);
        }
        else
        {
            countryCode = Request.Form[ShipToCountry.UniqueID];
            SelectCountry(ShipToCountry, countryCode, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired);
            if (ShipToProvinceList.Visible) province = Request.Form[ShipToProvinceList.UniqueID];
            else province = Request.Form[ShipToProvince.UniqueID];
            SelectProvince(ShipToProvince, ShipToProvinceList, province);
        }
    }

    /// <summary>
    /// Gets the address ID currently indicated for shipping the basket
    /// </summary>
    /// <returns>The address ID currently indicated for shipping the basket</returns>
    private int GetShippingAddressId()
    {
        int shipAddressId = 0;
        if (Page.IsPostBack)
        {
            // ON POSTBACKS, READ SHIP ADDRESS ID FROM THE FORM DATA
            shipAddressId = AlwaysConvert.ToInt(Request.Form[AddressBook.UniqueID]);
            // AUTOMATICALLY RETURN THE "NEW ADDRESS" VALUE
            if (shipAddressId == -1) return shipAddressId;
        }
        else
        {
            // ON FIRST VISIT, READ SHIP ADDRESS ID FROM BASKET
            if (_Basket.Shipments.Count > 0)
            {
                shipAddressId = _Basket.Shipments[0].AddressId;
            }
        }

        // IF ADDRESSID IS VALID, RETURN IT.  OTHERWISE RETURN BILLING ADDRESS ID
        int index = _User.Addresses.IndexOf(shipAddressId);
        if (index > -1) return shipAddressId;
        return _User.PrimaryAddress.AddressId;
    }

    /// <summary>
    /// Gets the address currently indicated for shipping the basket
    /// </summary>
    /// <returns>The address currently indicated for shipping the basket</returns>
    private Address GetShippingAddress()
    {
        int shipAddressId = GetShippingAddressId();
        if (shipAddressId == -1)
        {
            Address newAddress = new Address();
            newAddress.Residence = true;
            return newAddress;
        }
        int index = _User.Addresses.IndexOf(shipAddressId);
        if (index > -1) return _User.Addresses[index];
        return _User.PrimaryAddress;
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // SAVE ADDRESSES
            int shipAddressId = SaveBillingAddress();
            if (ShipAddressPanel.Visible)
            {
                shipAddressId = SaveShippingAddress();
            }

            // UPDATE SHIPPING ADDRESS IN BASKET
            foreach (BasketShipment shipment in _Basket.Shipments)
            {
                shipment.AddressId = shipAddressId;
            }
            _Basket.Save();

            // REDIRECT TO SHIPPING AND PAYMENT
            Response.Redirect("CreateOrder4.aspx?UID=" + _UserId);
        }
    }

    private int SaveBillingAddress()
    {
        Address billingAddress = _User.PrimaryAddress;
        billingAddress.FirstName = StringHelper.StripHtml(BillToFirstName.Text);
        billingAddress.LastName = StringHelper.StripHtml(BillToLastName.Text);
        billingAddress.Company = StringHelper.StripHtml(BillToCompany.Text);
        billingAddress.Address1 = StringHelper.StripHtml(BillToAddress1.Text);
        billingAddress.Address2 = StringHelper.StripHtml(BillToAddress2.Text);
        billingAddress.City = StringHelper.StripHtml(BillToCity.Text);
        billingAddress.Province = BillToProvinceList.Visible ? BillToProvinceList.SelectedValue : StringHelper.StripHtml(BillToProvince.Text);
        billingAddress.PostalCode = StringHelper.StripHtml(BillToPostalCode.Text);
        billingAddress.CountryCode = BillToCountry.SelectedValue;
        billingAddress.Residence = (BillToAddressType.SelectedIndex == 0);
        billingAddress.Phone = StringHelper.StripHtml(BillToPhone.Text);
        if (BillToEmail.Enabled) billingAddress.Email = StringHelper.StripHtml(BillToEmail.Text);
        billingAddress.Save();
        return billingAddress.AddressId;
    }

    private int SaveShippingAddress()
    {
        //UPDATE THE FIELDS FROM FORM
        Address shippingAddress = GetShippingAddress();
        shippingAddress.UserId = _UserId;
        shippingAddress.FirstName = StringHelper.StripHtml(ShipToFirstName.Text);
        shippingAddress.LastName = StringHelper.StripHtml(ShipToLastName.Text);
        shippingAddress.Company = StringHelper.StripHtml(ShipToCompany.Text);
        shippingAddress.Address1 = StringHelper.StripHtml(ShipToAddress1.Text);
        shippingAddress.Address2 = StringHelper.StripHtml(ShipToAddress2.Text);
        shippingAddress.City = StringHelper.StripHtml(ShipToCity.Text);
        shippingAddress.Province = ShipToProvinceList.Visible ? ShipToProvinceList.SelectedValue : StringHelper.StripHtml(ShipToProvince.Text);
        shippingAddress.PostalCode = StringHelper.StripHtml(ShipToPostalCode.Text);
        shippingAddress.CountryCode = ShipToCountry.SelectedValue;
        shippingAddress.Residence = (AlwaysConvert.ToInt(ShipToAddressType.SelectedValue) == 1);
        shippingAddress.Phone = StringHelper.StripHtml(ShipToPhone.Text);
        shippingAddress.Save();
        _User.Addresses.Add(shippingAddress);
        return shippingAddress.AddressId;
    }

    #region Shipping Address

    private void InitializeShippingAddress()
    {
        Address shippingAddress = GetShippingAddress();
        // INITIALIZE THE SHIPPING ADDRESS FORM
        ShipToFirstName.Text = shippingAddress.FirstName;
        ShipToLastName.Text = shippingAddress.LastName;
        ShipToCompany.Text = shippingAddress.Company;
        ShipToAddress1.Text = shippingAddress.Address1;
        ShipToAddress2.Text = shippingAddress.Address2;
        ShipToCity.Text = shippingAddress.City;
        ShipToPostalCode.Text = shippingAddress.PostalCode;
        ShipToAddressType.SelectedIndex = (shippingAddress.Residence ? 0 : 1);
        ShipToPhone.Text = shippingAddress.Phone;
        SelectCountryAndProvince(ShipToCountry, shippingAddress.CountryCode, ShipToProvince, ShipToProvinceList, ShipToPostalCodeRequired, shippingAddress.Province);
    }

    #endregion

    #region Countries
    private void InitializeCountries()
    {
        _Countries = CountryDataSource.LoadForStore("Name");
        //FIND STORE COUNTRY AND COPY TO FIRST POSITION
        string storeCountry = Token.Instance.Store.DefaultWarehouse.CountryCode;
        if (storeCountry.Length == 0) storeCountry = "US";
        int index = _Countries.IndexOf(storeCountry);
        if (index > -1)
        {
            Country breakItem = new Country(storeCountry);
            breakItem.Name = "----------";
            _Countries.Insert(0, breakItem);
            _Countries.Insert(0, _Countries[index + 1]);
            if (storeCountry == "US")
            {
                index = _Countries.IndexOf("CA");
                if (index > -1) _Countries.Insert(1, _Countries[index]);
            }
            else if (storeCountry == "CA")
            {
                index = _Countries.IndexOf("US");
                if (index > -1) _Countries.Insert(1, _Countries[index]);
            }
        }
    }

    private void SelectCountryAndProvince(DropDownList CountryList, string countryCode, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator, string province)
    {
        SelectCountry(CountryList, countryCode, ProvinceText, ProvinceList, PostalCodeValidator);
        SelectProvince(ProvinceText, ProvinceList, province);
    }

    private void SelectCountry(DropDownList CountryList, string countryCode, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator)
    {
        // WE CANNOT SELECT A COUNTRY IF THERE ARE NONE TO CHOOSE FROM!
        if (this.Countries.Count == 0) return;

        // LOCATE THE BEST COUNTRY TO SELECT, START WITH THE REQUESTED COUNTRY
        int index = this.Countries.IndexOf(countryCode);

        // THE REQUESTED COUNTRY WAS NOT FOUND, TRY TO LOCATE THE STORE COUNTRY
        if (index < 0) index = this.Countries.IndexOf(Token.Instance.Store.DefaultWarehouse.CountryCode);
        
        // THE STORE COUNTRY WAS NOT FOUND, USE THE FIRST IN THE LIST
        if (index < 0) index = 0;

        // UPDATE THE SELECTED INDEX AND PROVINCES
        CountryList.SelectedIndex = index;
        UpdateProvinces(CountryList.SelectedValue, ProvinceText, ProvinceList, PostalCodeValidator);
    }

    private void UpdateProvinces(string countryCode, TextBox ProvinceText, DropDownList ProvinceList, RequiredFieldValidator PostalCodeValidator)
    {
        //SEE WHETHER POSTAL CODE IS REQUIRED
        string[] countries = Store.GetCachedSettings().PostalCodeCountries.Split(",".ToCharArray());
        PostalCodeValidator.Enabled = (Array.IndexOf(countries, countryCode) > -1);
        //SEE WHETHER PROVINCE LIST IS DEFINED
        bool billTo = (ProvinceText.ID == "BillToProvince");
        ProvinceCollection provinces = ProvinceDataSource.LoadForCountry(countryCode);
        if (provinces.Count > 0)
        {
            ProvinceText.Visible = false;
            ProvinceText.Text = string.Empty;
            ProvinceList.Visible = true;
            ProvinceList.Items.Clear();
            ProvinceList.Items.Add(string.Empty);
            foreach (Province province in provinces)
            {
                string provinceValue = (!string.IsNullOrEmpty(province.ProvinceCode) ? province.ProvinceCode : province.Name);
                ProvinceList.Items.Add(new ListItem(province.Name, provinceValue));
            }
            if (billTo) BillToProvinceRequired.Enabled = true;
            else ShipToProvinceRequired.Enabled = true;
        }
        else
        {
            ProvinceList.Items.Clear();
            ProvinceList.Visible = false;
            ProvinceText.Visible = true;
            if (billTo) BillToProvinceRequired.Enabled = false;
            else ShipToProvinceRequired.Enabled = false;
        }
    }

    private void SelectProvince(TextBox ProvinceText, DropDownList ProvinceList, string province)
    {
        if (ProvinceText.Visible)
        {
            ProvinceText.Text = province;
        }
        else
        {
            ListItem selectedProvince = ProvinceList.Items.FindByValue(province);
            if (selectedProvince != null)
            {
                ProvinceList.SelectedIndex = ProvinceList.Items.IndexOf(selectedProvince);
            }
        }
    }

    #endregion

    private void InitializeBillingAddress()
    {
        Address billingAddress = _User.PrimaryAddress;
        BillToFirstName.Text = billingAddress.FirstName;
        BillToLastName.Text = billingAddress.LastName;
        BillToCompany.Text = billingAddress.Company;
        BillToAddress1.Text = billingAddress.Address1;
        BillToAddress2.Text = billingAddress.Address2;
        BillToCity.Text = billingAddress.City;
        BillToPostalCode.Text = billingAddress.PostalCode;
        if (!billingAddress.IsValid) billingAddress.Residence = true;
        BillToAddressType.SelectedIndex = (billingAddress.Residence ? 0 : 1);
        BillToPhone.Text = billingAddress.Phone;
        BillToEmail.Text = billingAddress.Email;
    }

    private void InitializeAddressBook()
    {
        //ONLY SHOW THE ADDRESES THAT ARE NOT THE PRIMARY (BILLING)
        AddressCollection addresses = new AddressCollection();
        addresses.AddRange(_User.Addresses);
        addresses.Sort("LastName");
        int defaultIndex = addresses.IndexOf(_User.PrimaryAddressId);
        if (defaultIndex > -1) addresses.RemoveAt(defaultIndex);
        if (addresses.Count > 0)
        {
            List<DictionaryEntry> formattedAddresses = new List<DictionaryEntry>();
            foreach (Address address in addresses)
                formattedAddresses.Add(new DictionaryEntry(address.AddressId, address.FullName + " " + address.Address1 + " " + address.City));
            //BIND THE ADDRESSES TO THE DATALIST
            AddressBook.DataSource = formattedAddresses;
            AddressBook.DataBind();
        }

        // MAKE SURE THE CORRECT ADDRESS IS SELECTED
        int shipAddressId;
        if (!Page.IsPostBack) shipAddressId = _Basket.Shipments[0].AddressId;
        else shipAddressId = AlwaysConvert.ToInt(Request.Form[AddressBook.UniqueID]);
        ListItem selectedItem = AddressBook.Items.FindByValue(shipAddressId.ToString());
        if (selectedItem != null) AddressBook.SelectedIndex = AddressBook.Items.IndexOf(selectedItem);
    }
}