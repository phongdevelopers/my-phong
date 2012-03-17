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
using CommerceBuilder.Orders;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Marketing_Affiliates_Default : AbleCommerceAdminPage
{
    protected string ExpandIconUrl
    {
        get { return NavigationHelper.GetThemeImageUrl("icons/plus.png"); }
    }

    protected string CollapseIconUrl
    {
        get { return NavigationHelper.GetThemeImageUrl("icons/minus.png"); }
    }

    protected void AddAffiliateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Affiliate affiliate = new Affiliate();
            affiliate.Name = AddAffiliateName.Text;
            affiliate.ReferralPeriod = AffiliateReferralPeriod.Persistent;
            affiliate.Save();
            Response.Redirect("EditAffiliate.aspx?AffiliateId=" + affiliate.AffiliateId.ToString());
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddAffiliateName, AddAffiliateButton.ClientID);
        AddAffiliateName.Focus();

        if (!Page.IsPostBack)
        {
            Store store = Token.Instance.Store;
            StoreSettingCollection settings = store.Settings;

            AffiliateParameter.Text = settings.AffiliateParameterName;
            TrackerUrl.Text = settings.AffiliateTrackerUrl;

            // ENSURE THE CORRECT SETTING FORM ELEMENTS ARE DISPLAYED
            SelfSignup.Checked = settings.AffiliateAllowSelfSignup;
            ListItem itemAffiliatePersistence = AffiliatePersistence.Items.FindByValue(((byte)settings.AffiliatePersistence).ToString());
            if (itemAffiliatePersistence != null)
            {
                AffiliatePersistence.SelectedIndex = AffiliatePersistence.Items.IndexOf(itemAffiliatePersistence);
            }
            ReferralPeriod.Text = settings.AffiliateReferralPeriod.ToString();
            CommissionRate.Text = settings.AffiliateCommissionRate.ToString();
            if (!settings.AffiliateCommissionIsPercent) CommissionType.SelectedIndex = 0;
            else CommissionType.SelectedIndex = (settings.AffiliateCommissionOnTotal ? 2 : 1);
            SelfSignup_CheckedChanged(sender, e);
        }

        int affiliateCount = AffiliateDataSource.CountForStore();
        if (affiliateCount > 0)
        {
            // AFFILIATES CREATED, HIDE SETTINGS BY DEFAULT
            ToggleSettingsButton.ImageUrl = this.ExpandIconUrl;
            ToggleSettingsPanel.Style["display"] = "none";
        }
        else
        {
            // NO AFFILIATES CREATED, SHOW SETTINGS BY DEFAULT
            ToggleSettingsButton.ImageUrl = this.CollapseIconUrl;
            ToggleSettingsPanel.Style["display"] = "block";
        }
    }

    protected int GetOrderCount(object dataItem)
    {
        Affiliate a = (Affiliate)dataItem;
        return OrderDataSource.CountForAffiliate(a.AffiliateId);
    }

    protected int GetUserCount(object dataItem)
    {
        Affiliate a = (Affiliate)dataItem;
        return UserDataSource.CountForAffiliate(a.AffiliateId);
    }

    protected string GetHomeUrl(object dataItem)
    {
        Affiliate a = (Affiliate)dataItem;
        return string.Format("http://{0}{1}?{2}={3}", Request.Url.Authority, this.ResolveUrl(NavigationHelper.GetHomeUrl()), Store.GetCachedSettings().AffiliateParameterName, a.AffiliateId);
    }

    protected string GetCommissionRate(object dataItem)
    {
        Affiliate affiliate = (Affiliate)dataItem;
        if (affiliate.CommissionIsPercent)
        {
            string format = "{0:0.##}% of {1}";
            if (affiliate.CommissionOnTotal) return string.Format(format, affiliate.CommissionRate, "order total");
            return string.Format(format, affiliate.CommissionRate, "product subtotal");
        }
        return string.Format("{0:lc} per order", affiliate.CommissionRate);
    }

    protected string GetPersistenceLabel(object dataItem)
    {
        Affiliate affiliate = (Affiliate)dataItem;
        switch (affiliate.ReferralPeriod)
        {
            case AffiliateReferralPeriod.FirstOrder:
                return "First Order";
            case AffiliateReferralPeriod.NumberOfDays:
                return "First " + affiliate.ReferralDays + " Days";
            case AffiliateReferralPeriod.FirstOrderWithinNumberOfDays:
                return "First Order Within " + affiliate.ReferralDays + " Days";
            default:
                return "Persistent";
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // UPDATE THE SETTINGS
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            settings.AffiliateParameterName = StringHelper.Truncate(AffiliateParameter.Text.Trim(), 200);
            settings.AffiliateTrackerUrl = StringHelper.Truncate(TrackerUrl.Text.Trim(), 200);
            settings.AffiliateAllowSelfSignup = SelfSignup.Checked;
            settings.AffiliateReferralRule = ReferralRule.NewSignupsOnly;
            settings.AffiliatePersistence = ((AffiliateReferralPeriod)AlwaysConvert.ToByte(AffiliatePersistence.SelectedValue));
            AffiliateReferralPeriod referralPeriod = ((AffiliateReferralPeriod)AlwaysConvert.ToByte(AffiliatePersistence.SelectedValue));
            if ((referralPeriod != AffiliateReferralPeriod.Persistent && referralPeriod != AffiliateReferralPeriod.FirstOrder))
            {
                settings.AffiliateReferralPeriod = AlwaysConvert.ToInt16(ReferralPeriod.Text);
            }
            else
            {
                settings.AffiliateReferralPeriod = 0;
            }
            settings.AffiliateCommissionRate = AlwaysConvert.ToDecimal(CommissionRate.Text);
            settings.AffiliateCommissionIsPercent = (CommissionType.SelectedIndex > 0);
            settings.AffiliateCommissionOnTotal = (CommissionType.SelectedIndex == 2);
            settings.Save();
            AffiliateSettingsMessage.Text = string.Format("Settings saved at {0}", DateTime.Now.ToString());
        }
    }

    protected void SelfSignup_CheckedChanged(object sender, EventArgs e)
    {
        trPersistence.Visible = SelfSignup.Checked;
        trCommissionRate.Visible = SelfSignup.Checked;
        if (SelfSignup.Checked) AffiliatePersistence_SelectedIndexChanged(sender, e);
        else trReferralPeriod.Visible = false;
    }

    protected void AffiliatePersistence_SelectedIndexChanged(Object sender, EventArgs e)
    {
        AffiliateReferralPeriod referralPeriod = (AffiliateReferralPeriod)AlwaysConvert.ToByte(AffiliatePersistence.SelectedValue);
        trReferralPeriod.Visible = (referralPeriod != AffiliateReferralPeriod.Persistent && referralPeriod != AffiliateReferralPeriod.FirstOrder);
        ReferralPeriod.Text = Token.Instance.Store.Settings.AffiliateReferralPeriod.ToString(); ;
    }
}
