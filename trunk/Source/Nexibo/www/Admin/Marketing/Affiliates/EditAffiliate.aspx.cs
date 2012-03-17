using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Web.UI;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;

public partial class Admin_Marketing_Affiliates_EditAffiliate : AbleCommerceAdminPage
{
    int _AffiliateId = 0;
    Affiliate _Affiliate;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _AffiliateId = AlwaysConvert.ToInt(Request.QueryString["AffiliateId"]);
        _Affiliate = AffiliateDataSource.Load(_AffiliateId);
        if (_Affiliate == null) Response.Redirect("Default.aspx");
        InstructionText.Text = string.Format(InstructionText.Text, Store.GetCachedSettings().AffiliateParameterName, _AffiliateId, GetHomeUrl());
        if (!Page.IsPostBack)
        {
            InitEditForm();
        }
        Caption.Text = string.Format(Caption.Text, _Affiliate.Name);
    }

    private string GetHomeUrl()
    {
        return string.Format("http://{0}{1}", Request.Url.Authority, this.ResolveUrl(NavigationHelper.GetHomeUrl()));
    }

    protected string GetSubtotal(object dataItem)
    {
        Order order = (Order)dataItem;
        return order.Items.TotalPrice(OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon).ToString("lc");
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Affiliate.Name = Name.Text;
            _Affiliate.ReferralDays = AlwaysConvert.ToInt16(ReferralDays.Text);
            _Affiliate.CommissionRate = AlwaysConvert.ToDecimal(CommissionRate.Text);
            _Affiliate.CommissionIsPercent = (CommissionType.SelectedIndex > 0);
            _Affiliate.CommissionOnTotal = (CommissionType.SelectedIndex == 2);
            _Affiliate.WebsiteUrl = WebsiteUrl.Text;
            _Affiliate.Email = Email.Text;
            _Affiliate.GroupId = AlwaysConvert.ToInt(AffiliateGroup.SelectedValue);

            AffiliateReferralPeriod referralPeriod = (AffiliateReferralPeriod)AlwaysConvert.ToByte(ReferralPeriod.SelectedValue);
            _Affiliate.ReferralPeriodId = (byte)referralPeriod;
            _Affiliate.ReferralPeriod = referralPeriod;

            if (referralPeriod != AffiliateReferralPeriod.Persistent && referralPeriod != AffiliateReferralPeriod.FirstOrder)
                _Affiliate.ReferralDays = AlwaysConvert.ToInt16(ReferralDays.Text);
            else
                _Affiliate.ReferralDays = 0;
            
            //ADDRESS INFORMATION
            _Affiliate.FirstName = FirstName.Text;
            _Affiliate.LastName = LastName.Text;
            _Affiliate.Company = Company.Text;
            _Affiliate.Address1 = Address1.Text;
            _Affiliate.Address2 = Address2.Text;
            _Affiliate.City = City.Text;
            _Affiliate.Province = Province.Text;
            _Affiliate.PostalCode = PostalCode.Text;
            _Affiliate.CountryCode = CountryCode.SelectedValue;
            _Affiliate.PhoneNumber = PhoneNumber.Text;
            _Affiliate.FaxNumber = FaxNumber.Text;
            _Affiliate.MobileNumber = MobileNumber.Text;
            _Affiliate.Save();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

    protected void InitEditForm()
    {
        //INITIALIZE FORM
        Name.Text = _Affiliate.Name;
        Email.Text = _Affiliate.Email;
        CommissionRate.Text = _Affiliate.CommissionRate.ToString("#.##");
        if (!_Affiliate.CommissionIsPercent) CommissionType.SelectedIndex = 0;
        else CommissionType.SelectedIndex = (_Affiliate.CommissionOnTotal ? 2 : 1);
        WebsiteUrl.Text = _Affiliate.WebsiteUrl;

        ListItem selectedReferralPeriod = ReferralPeriod.Items.FindByValue(((byte)_Affiliate.ReferralPeriod).ToString());
        selectedReferralPeriod.Selected = true;

        if (_Affiliate.ReferralPeriod != AffiliateReferralPeriod.Persistent && _Affiliate.ReferralPeriod != AffiliateReferralPeriod.FirstOrder)
        {
            ReferralDays.Text = _Affiliate.ReferralDays.ToString();
            EnableReferralDaysUI(true);
        }
        else
            EnableReferralDaysUI(false);
        
        if (_Affiliate.Group != null)
        {
            AffiliateGroup.DataBind();
            ListItem selectedGroup = AffiliateGroup.Items.FindByValue(_Affiliate.GroupId.ToString());          
            selectedGroup.Selected = true;
        }
        //INITIALIZE ADDRESS DATA
        FirstName.Text = _Affiliate.FirstName;
        LastName.Text = _Affiliate.LastName;
        Company.Text = _Affiliate.Company;
        Address1.Text = _Affiliate.Address1;
        Address2.Text = _Affiliate.Address2;
        City.Text = _Affiliate.City;
        Province.Text = _Affiliate.Province;
        PostalCode.Text = _Affiliate.PostalCode;
        //INITIALIZE COUNTRY
        CountryCode.DataSource = CountryDataSource.LoadForStore("Name");
        CountryCode.DataBind();
        if (string.IsNullOrEmpty(_Affiliate.CountryCode)) _Affiliate.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
        ListItem selectedCountry = CountryCode.Items.FindByValue(_Affiliate.CountryCode);
        if (selectedCountry != null) CountryCode.SelectedIndex = CountryCode.Items.IndexOf(selectedCountry);
        PhoneNumber.Text = _Affiliate.PhoneNumber;
        FaxNumber.Text = _Affiliate.FaxNumber;
        MobileNumber.Text = _Affiliate.MobileNumber;
    }
    
    protected void EnableReferralDaysUI(bool enabled) 
    {
        ReferralDaysLabel.Visible = enabled;
        ReferralDays.Visible = enabled;
        ReferralDaysLabel2.Visible = enabled;
        ReferralPeriodRequiredValidator.Visible = enabled;
        ReferralPeriodValidator.Visible = enabled;
    }

    protected void ReferralPeriod_SelectedIndexChanged(object sender, EventArgs e)
    {
        AffiliateReferralPeriod referralPeriod = (AffiliateReferralPeriod)AlwaysConvert.ToByte(ReferralPeriod.SelectedValue);
        EnableReferralDaysUI((referralPeriod != AffiliateReferralPeriod.Persistent && referralPeriod != AffiliateReferralPeriod.FirstOrder));
        ReferralDays.Text = _Affiliate.ReferralDays.ToString();
    }
}
