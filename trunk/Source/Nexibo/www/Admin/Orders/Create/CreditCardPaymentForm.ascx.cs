using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Create_CreditCardPaymentForm : System.Web.UI.UserControl
{
    private int _UserId;
    private User _User;
    Basket _Basket;

    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    private string _ValidationGroup = "CreditCard";
    public string ValidationGroup
    {
        get { return _ValidationGroup; }
        set { _ValidationGroup = value; }
    }

    private bool _ValidationSummaryVisible = true;
    public bool ValidationSummaryVisible
    {
        get { return _ValidationSummaryVisible; }
        set { _ValidationSummaryVisible = value; }
    }

    private bool _AllowAmountEntry = false;
    public bool AllowAmountEntry
    {
        get { return _AllowAmountEntry; }
        set { _AllowAmountEntry = value; }
    }

    private LSDecimal _PaymentAmount = 0;
    public LSDecimal PaymentAmount
    {
        get { return _PaymentAmount; }
        set { _PaymentAmount = value; }
    }

    private void UpdateValidationOptions()
    {
        Amount.ValidationGroup = _ValidationGroup;
        AmountRequired.ValidationGroup = _ValidationGroup;
        CardType.ValidationGroup = _ValidationGroup;
        CardTypeRequired.ValidationGroup = _ValidationGroup;
        CardName.ValidationGroup = _ValidationGroup;
        CardNameRequired.ValidationGroup = _ValidationGroup;
        CardNumber.ValidationGroup = _ValidationGroup;
        CardNumberValidator1.ValidationGroup = _ValidationGroup;
        ExpirationDropDownValidator1.ValidationGroup = _ValidationGroup;
        CardNumberValidator2.ValidationGroup = _ValidationGroup;
        ExpirationMonth.ValidationGroup = _ValidationGroup;
        MonthValidator.ValidationGroup = _ValidationGroup;
        ExpirationYear.ValidationGroup = _ValidationGroup;
        YearValidator.ValidationGroup = _ValidationGroup;
        SecurityCode.ValidationGroup = _ValidationGroup;
        SecurityCodeValidator.ValidationGroup = _ValidationGroup;
        SecurityCodeValidator2.ValidationGroup = _ValidationGroup;
        IssueNumber.ValidationGroup = _ValidationGroup;
        StartDateMonth.ValidationGroup = _ValidationGroup;
        StartDateMonth.ValidationGroup = _ValidationGroup;
        IntlDebitValidator1.ValidationGroup = _ValidationGroup;
        IntlDebitValidator2.ValidationGroup = _ValidationGroup;
        StartDateValidator1.ValidationGroup = _ValidationGroup;
        CreditCardButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    private string FormatCardNames(List<string> cardNames)
    {
        if (cardNames == null || cardNames.Count == 0) return string.Empty;
        if (cardNames.Count == 1) return cardNames[0];
        string formattedNames = string.Join(", ", cardNames.ToArray());
        int lastComma = formattedNames.LastIndexOf(", ");
        string leftSide = formattedNames.Substring(0, lastComma);
        string rightSide = formattedNames.Substring(lastComma + 1);
        return leftSide + ", and" + rightSide;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE USER THAT THE ORDER IS BEING PLACED FOR
        _UserId = AlwaysConvert.ToInt(Request.QueryString["UID"]);
        _User = UserDataSource.Load(_UserId);
        if (_User == null) return;
        _Basket = _User.Basket;

        UpdateValidationOptions();
        //SET THE DEFAULT NAME
        CardName.Text = _User.PrimaryAddress.FullName;
        //POPULATE EXPIRATON DATE DROPDOWN
        int thisYear = LocaleHelper.LocalNow.Year;
        for (int i = 0; (i <= 10); i++)
        {
            ExpirationYear.Items.Add(new ListItem((thisYear + i).ToString()));
        }
        //POPULATE START DATE DROPDOWN
        for (int i = 1997; (i <= thisYear); i++)
        {
            StartDateYear.Items.Add(new ListItem(i.ToString()));
        }
        //LOAD AVAILABLE PAYMENT METHODS
        PaymentMethodCollection methods = StoreDataHelper.GetPaymentMethods(_UserId);
        List<string> creditCards = new List<string>();
        List<string> intlDebitCards = new List<string>();
        foreach (PaymentMethod method in methods)
        {
            if (method.IsCreditOrDebitCard())
            {
                CardType.Items.Add(new ListItem(method.Name, method.PaymentMethodId.ToString()));
                if (method.IsIntlDebitCard()) intlDebitCards.Add(method.Name);
                else creditCards.Add(method.Name);
            }
        }
        //HIDE THIS CONTROL IF THERE ARE NO CREDIT CARD PAYMENT METHODS
        if (CardType.Items.Count == 1)
        {
            Trace.Write(this.GetType().ToString(), "Output suppressed, no credit card payment methods detected.");
            this.Controls.Clear();
        }
        else
        {
            //SHOW OR HIDE INTL DEBIT FIELDS
            if (intlDebitCards.Count > 0)
            {
                trIntlCVV.Visible = true;
                if (creditCards.Count > 0)
                {
                    IntlCVVCredit.Visible = true;
                    IntlCVVCredit.Text = string.Format(IntlCVVCredit.Text, FormatCardNames(creditCards));
                }
                else IntlCVVCredit.Visible = false;
                IntlCVVDebit.Text = string.Format(IntlCVVDebit.Text, FormatCardNames(intlDebitCards));
                SecurityCodeValidator.Enabled = false;
                trIntlInstructions.Visible = true;
                IntlInstructions.Text = string.Format(IntlInstructions.Text, FormatCardNames(intlDebitCards));
                trIssueNumber.Visible = true;
                trStartDate.Visible = true;
            }
            else
            {
                trIntlCVV.Visible = false;
                trIntlInstructions.Visible = false;
                trIssueNumber.Visible = false;
                trStartDate.Visible = false;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        trAmount.Visible = this.AllowAmountEntry;
        DisableAutoComplete();

        // PREVENT DOUBLE POSTING
        StringBuilder submitScript = new StringBuilder();
        submitScript.Append("if(document.getElementById('" + FormIsSubmitted.ClientID + "').value==1 || Page_ClientValidate('CreditCard') == false) { return false; } ");
        submitScript.Append("this.disabled = true; ");
        submitScript.Append("document.getElementById('" + FormIsSubmitted.ClientID + "').value=1;");
        //GetPostBackEventReference obtains a reference to a client-side script function that causes the server to post back to the page.
        submitScript.Append(Page.ClientScript.GetPostBackEventReference(CreditCardButton, string.Empty));
        submitScript.Append(";");
        submitScript.Append("return false;");
        this.CreditCardButton.Attributes.Add("onclick", submitScript.ToString());
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (trAmount.Visible && string.IsNullOrEmpty(Amount.Text))
        {
            Amount.Text = GetPaymentAmount().ToString("F2");
        }
    }

    private LSDecimal GetPaymentAmount()
    {
        if (Page.IsPostBack)
        {
            if (trAmount.Visible && !string.IsNullOrEmpty(Amount.Text)) return AlwaysConvert.ToDecimal(Amount.Text);
            else if (this.PaymentAmount > 0) return this.PaymentAmount;
            else return _Basket.Items.TotalPrice();
        }
        else
        {
            if (this.PaymentAmount > 0) return this.PaymentAmount;
            return _Basket.Items.TotalPrice();
        }
    }

    private Payment GetPayment()
    {
        Payment payment = new Payment();
        payment.PaymentMethodId = AlwaysConvert.ToInt(CardType.SelectedValue);
        payment.Amount = GetPaymentAmount();
        AccountDataDictionary instrumentBuilder = new AccountDataDictionary();
        instrumentBuilder["AccountName"] = CardName.Text;
        instrumentBuilder["AccountNumber"] = CardNumber.Text;
        instrumentBuilder["ExpirationMonth"] = ExpirationMonth.SelectedItem.Value;
        instrumentBuilder["ExpirationYear"] = ExpirationYear.SelectedItem.Value;
        instrumentBuilder["SecurityCode"] = SecurityCode.Text;
        if (payment.PaymentMethod.IsIntlDebitCard())
        {
            if (IssueNumber.Text.Length > 0) instrumentBuilder["IssueNumber"] = IssueNumber.Text;
            if ((StartDateMonth.SelectedIndex > 0) && (StartDateYear.SelectedIndex > 0))
            {
                instrumentBuilder["StartDateMonth"] = StartDateMonth.SelectedItem.Value;
                instrumentBuilder["StartDateYear"] = StartDateYear.SelectedItem.Value;
            }
        }
        payment.ReferenceNumber = Payment.GenerateReferenceNumber(CardNumber.Text);
        payment.AccountData = instrumentBuilder.ToString();
        return payment;
    }

    private bool CustomValidation()
    {
        //if intl instructions are visible, we must validate additional rules
        if (trIntlInstructions.Visible)
        {
            PaymentMethod m = PaymentMethodDataSource.Load(AlwaysConvert.ToInt(CardType.SelectedValue));
            if (m != null)
            {
                if (m.IsIntlDebitCard())
                {
                    //INTERNATIONAL DEBIT CARD, ISSUE NUMBER OR START DATE REQUIRED
                    bool invalidIssueNumber = (!Regex.IsMatch(IssueNumber.Text, "\\d{1,2}"));
                    bool invalidStartDate = ((StartDateMonth.SelectedIndex == 0) || (StartDateYear.SelectedIndex == 0));
                    if (invalidIssueNumber && invalidStartDate)
                    {
                        IntlDebitValidator1.IsValid = false;
                        IntlDebitValidator2.IsValid = false;
                        return false;
                    }
                    //CHECK START DATE IS IN PAST
                    int selYear = AlwaysConvert.ToInt(StartDateYear.SelectedValue);
                    int curYear = DateTime.Now.Year;
                    if (selYear > curYear)
                    {
                        StartDateValidator1.IsValid = false;
                        return false;
                    }
                    else if (selYear == curYear)
                    {
                        int selMonth = AlwaysConvert.ToInt(StartDateMonth.SelectedValue);
                        int curMonth = DateTime.Now.Month;
                        if (selMonth > curMonth)
                        {
                            StartDateValidator1.IsValid = false;
                            return false;
                        }
                    }
                }
                else
                {
                    //CREDIT CARD, CVV IS REQUIRED
                    if (!Regex.IsMatch(SecurityCode.Text, "\\d{3,4}"))
                    {
                        SecurityCodeValidator2.IsValid = false;
                        return false;
                    }
                }
            }
        }
        return true;
    }

    protected void CreditCardButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && CustomValidation())
        {
            //CREATE THE PAYMENT OBJECT
            Payment payment = GetPayment();
            //PROCESS CHECKING OUT EVENT
            bool checkOut = true;
            if (CheckingOut != null)
            {
                CheckingOutEventArgs c = new CheckingOutEventArgs(payment);
                CheckingOut(this, c);
                checkOut = !c.Cancel;
            }
            if (checkOut)
            {
                //PROCESS A CHECKOUT
                CheckoutRequest checkoutRequest = new CheckoutRequest(payment);
                CheckoutResponse checkoutResponse = _Basket.Checkout(checkoutRequest, false);
                if (checkoutResponse.Success)
                {
                    if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                    Response.Redirect(NavigationHelper.GetReceiptUrl(checkoutResponse.OrderId));
                }
                else
                {
                    List<string> warningMessages = checkoutResponse.WarningMessages;
                    if (warningMessages.Count == 0)
                        warningMessages.Add("The order could not be submitted at this time.  Please try again later or contact us for assistance.");
                    if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                }
            }
        }
        else CreditCardButton.Text = "Pay With Card";

        // IF NOT SUCCESSFULL / ENABLE THE CHECKOUT BUTTON
        CreditCardButton.Enabled = true;
        FormIsSubmitted.Value = "0";
    }

    private void DisableAutoComplete()
    {
        CardNumber.Attributes.Add("autocomplete", "off");
        SecurityCode.Attributes.Add("autocomplete", "off");
        IssueNumber.Attributes.Add("autocomplete", "off");
    }
}
