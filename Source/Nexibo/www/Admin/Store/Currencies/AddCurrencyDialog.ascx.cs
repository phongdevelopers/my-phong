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
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Store_Currencies_AddCurrencyDialog : System.Web.UI.UserControl
{
    public event PersistentItemEventHandler ItemAdded;


    protected void Page_Init(object sender, EventArgs e)
    {
        // JAVASCRIPT TO CONVERT ISO CODE VALUE TO UPPER CASE
        ISOCode.Attributes.Add("onBlur", "this.value = this.value.toUpperCase();");
    }

    protected void CreateCurrency()
    {
        if (Page.IsValid)
        {
            Currency currency = new Currency();
            currency.Name = Name.Text;
            currency.ExchangeRate = AlwaysConvert.ToDecimal(ExchangeRate.Text, 1M);
            currency.AutoUpdate = AutoUpdate.Checked;
            currency.ISOCode = ISOCode.Text;
            currency.ISOCodePattern = (byte)ISOCodePattern.SelectedIndex;
            currency.CurrencySymbol = CurrencySymbol.Text;
            currency.PositivePattern = (byte)PositivePattern.SelectedIndex;
            currency.NegativePattern = (byte)NegativePattern.SelectedIndex;
            currency.NegativeSign = NegativeSign.Text;
            currency.DecimalSeparator = DecimalSeparator.Text;
            currency.DecimalDigits = AlwaysConvert.ToInt(DecimalDigits.Text);
            currency.GroupSeparator = GroupSeparator.Text;
            currency.GroupSizes = GroupSizes.Text;
            currency.Save();
            if (currency.AutoUpdate) currency.UpdateExchangeRate(false);
            
            
            //RESET FORM
            Name.Text = string.Empty;
            Name.Focus();
            ExchangeRate.Text = "1";
            AutoUpdate.Checked = true;
            ManualUpdate.Checked = false;
            ISOCode.Text = string.Empty;
            ISOCodePattern.SelectedIndex = 0;
            CurrencySymbol.Text = "$";
            PositivePattern.SelectedIndex = 0;
            NegativePattern.SelectedIndex = 1;
            NegativeSign.Text = "-";
            DecimalSeparator.Text = ".";
            DecimalDigits.Text = "2";
            GroupSeparator.Text = ",";
            GroupSizes.Text = "3";
            if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(currency.CurrencyId, currency.Name));
            HideDisplayOptions_Click(null, null);
            
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        CreateCurrency();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ExchangeRateHelpText.Text = string.Format(ExchangeRateHelpText.Text, Token.Instance.Store.BaseCurrency.Name);
    }

    protected void ShowDisplayOptions_Click(object sender, ImageClickEventArgs e)
    {
        ShowDisplayOptions.Visible = false;
        HideDisplayOptions.Visible = true;
        trDisplayOptions1a.Visible = true;
        trDisplayOptions1b.Visible = true;
        trDisplayOptions2.Visible = true;
        trDisplayOptions3a.Visible = true;
        trDisplayOptions3b.Visible = true;
        trDisplayOptions4a.Visible = true;
        trDisplayOptions4b.Visible = true;
        trDisplayOptions5.Visible = true;
        AddPopup.Show();
    }

    protected void HideDisplayOptions_Click(object sender, ImageClickEventArgs e)
    {
        ShowDisplayOptions.Visible = true;
        HideDisplayOptions.Visible = false;
        trDisplayOptions1a.Visible = false;
        trDisplayOptions1b.Visible = false;
        trDisplayOptions2.Visible = false;
        trDisplayOptions3a.Visible = false;
        trDisplayOptions3b.Visible = false;
        trDisplayOptions4a.Visible = false;
        trDisplayOptions4b.Visible = false;
        trDisplayOptions5.Visible = false;
        if (sender != null) AddPopup.Show(); 
    }
}
