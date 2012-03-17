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

public partial class Admin_Store_Currencies_EditCurrencyDialog : System.Web.UI.UserControl
{
    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;

    protected void Page_Init(object sender, EventArgs e)
    {
        // JAVASCRIPT TO CONVERT ISO CODE VALUE TO UPPER CASE
        ISOCode.Attributes.Add("onBlur", "this.value = this.value.toUpperCase();");
    }

    public int CurrencyId
    {
        get { return AlwaysConvert.ToInt(ViewState["CurrencyId"]); }
        set { ViewState["CurrencyId"] = value; }
    }

    protected void UpdateCurrency()
    {
        if (Page.IsValid)
        {
            Currency currency = CurrencyDataSource.Load(this.CurrencyId);
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
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(this.CurrencyId, currency.Name));

            // RESET CURRENCY ID
            this.CurrencyId = 0;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        UpdateCurrency();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        // RESET THE CURRENCY ID
        this.CurrencyId = 0;
        //TRIGER ANY EVENT ATTACHED TO THE CANCEL
        if (Cancelled != null) Cancelled(sender, e);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Currency currency = CurrencyDataSource.Load(this.CurrencyId);
        if (currency != null)
        {
            //LOAD FORM
            Name.Text = currency.Name;
            AutoUpdate.Checked = currency.AutoUpdate;
            ManualUpdate.Checked = !AutoUpdate.Checked;
            ExchangeRate.Text = currency.ExchangeRate.ToString("#.####");
            ISOCode.Text = currency.ISOCode;
            ISOCodePattern.SelectedIndex = (int)currency.ISOCodePattern;
            CurrencySymbol.Text = currency.CurrencySymbol;
            PositivePattern.SelectedIndex = (int)currency.PositivePattern;
            NegativePattern.SelectedIndex = (int)currency.NegativePattern;
            NegativeSign.Text = currency.NegativeSign;
            DecimalSeparator.Text = currency.DecimalSeparator;
            DecimalDigits.Text = currency.DecimalDigits.ToString();
            GroupSeparator.Text = currency.GroupSeparator;
            GroupSizes.Text = currency.GroupSizes;
            trExchangeRate1.Visible = !currency.IsBaseCurrency;
            trExchangeRate2.Visible = trExchangeRate1.Visible;
            if (trExchangeRate1.Visible)
                ExchangeRateHelpText.Text = string.Format(ExchangeRateHelpText.Text, Token.Instance.Store.BaseCurrency.Name);


            EditCaption.Text = string.Format(EditCaption.Text, currency.Name);
            EditPopup.OnCancelScript = String.Format("__doPostBack('{0}','{1}')", CancelButton.UniqueID, "");
            EditPopup.Show();
        }
        else
        {
            this.Controls.Clear();            
        }
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
    }
}
