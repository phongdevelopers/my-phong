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
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;
using System.Text.RegularExpressions;

public partial class Admin_Orders_Payments_ucProcessPayment : System.Web.UI.UserControl
{
    private int _OrderId = 0;
    private Order _Order;

    PaymentMethodCollection _OnlinePaymentMethods;
    private void InitOnlinePaymentMethods()
    {
        string sqlCriteria = "PaymentInstrumentId<>" + (short)PaymentInstrument.GiftCertificate
            + " AND PaymentInstrumentId<>" + (short)PaymentInstrument.GoogleCheckout;

        PaymentMethodCollection allMethods = PaymentMethodDataSource.LoadForCriteria(sqlCriteria);
        _OnlinePaymentMethods = new PaymentMethodCollection();
        foreach (PaymentMethod m in allMethods)
        {
            if ((m.PaymentGateway != null) && (m.PaymentInstrument != PaymentInstrument.PayPal)) _OnlinePaymentMethods.Add(m);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = PageHelper.GetOrderId();
        _Order = OrderDataSource.Load(_OrderId);
        InitOnlinePaymentMethods();
        SelectedPaymentMethod.DataSource = _OnlinePaymentMethods;
        SelectedPaymentMethod.DataBind();
        if (!Page.IsPostBack)
        {
            BindPaymentMethod();
            CancelLink.NavigateUrl += "?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString();
        }
    }

    protected PaymentMethod GetSelectedMethod()
    {
        int paymentMethodId = AlwaysConvert.ToInt(SelectedPaymentMethod.SelectedValue);
        foreach (PaymentMethod method in _OnlinePaymentMethods)
        {
            if (method.PaymentMethodId == paymentMethodId) return method;
        }
        return null;
    }

    protected void PopulateExpiration()
    {
        ExpirationYear.Items.Clear();
        int thisYear = DateTime.UtcNow.Year;
        for (int i = 0; (i <= 10); i++)
        {
            ExpirationYear.Items.Add(new ListItem((thisYear + i).ToString()));
        }
        //POPULATE START DATE DROPDOWN
        StartDateYear.Items.Clear();
        for (int i = 2000; (i <= thisYear); i++)
        {
            StartDateYear.Items.Add(new ListItem(i.ToString()));
        }
    }

    protected void ShowCheckForm()
    {
        CheckPanel.Visible = true;
        CheckPaymentAmount.Text = string.Format("{0:F2}", _Order.GetBalance(true));
        SaveButton.Visible = true;
        SaveButton.ValidationGroup = "Check";
    }

    private void ShowCreditCardForm(string paymentMethodName, int securityCodeLength, bool isIntlCard)
    {
        CreditCardPanel.Visible = true;
        SecurityCode.MaxLength = securityCodeLength;
        PopulateExpiration();
        CreditCardPaymentAmount.Text = string.Format("{0:F2}", _Order.GetBalance(true));
        CardName.Text = _Order.BillToFirstName + " " + _Order.BillToLastName;
        IntlCVVMessage.Visible = isIntlCard;
        trIssueNumber.Visible = isIntlCard;
        trStartDate.Visible = isIntlCard;
        SecurityCodeValidator.Enabled = !isIntlCard;
        SaveButton.Visible = true;
        SaveButton.ValidationGroup = "CreditCard";
    }

    private void BindPaymentMethod()
    {
        CheckPanel.Visible = false;
        CreditCardPanel.Visible = false;
        SaveButton.Visible = false;
        PaymentMethod method = GetSelectedMethod();
        if (method != null)
        {
            switch (method.PaymentInstrument)
            {
                case PaymentInstrument.AmericanExpress:
                    ShowCreditCardForm(method.Name, 4, false);
                    break;
                case PaymentInstrument.Discover:
                case PaymentInstrument.JCB:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                case PaymentInstrument.Maestro:
                case PaymentInstrument.SwitchSolo:
                case PaymentInstrument.VisaDebit:
                    ShowCreditCardForm(method.Name, 3, method.IsIntlDebitCard());
                    break;
                case PaymentInstrument.Check:
                    ShowCheckForm();
                    break;
            }
        }
    }

    protected void SelectedPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindPaymentMethod();
    }

    private bool CustomValidation()
    {
        //if isssue number is visible, we must validate additional rules
        if (trIssueNumber.Visible)
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
        }
        return true;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && CustomValidation())
        {
            PaymentMethod method = GetSelectedMethod();
            if (method != null)
            {
                Payment payment = new Payment();
                payment.OrderId = _OrderId;
                payment.PaymentMethodId = method.PaymentMethodId;
                AccountDataDictionary paymentInstrumentBuilder = new AccountDataDictionary();
                switch (method.PaymentInstrument)
                {
                    case PaymentInstrument.AmericanExpress:
                    case PaymentInstrument.Discover:
                    case PaymentInstrument.JCB:
                    case PaymentInstrument.MasterCard:
                    case PaymentInstrument.Visa:
                    case PaymentInstrument.DinersClub:
                    case PaymentInstrument.Maestro:
                    case PaymentInstrument.SwitchSolo:
                    case PaymentInstrument.VisaDebit:
                        paymentInstrumentBuilder["AccountName"] = CardName.Text;
                        paymentInstrumentBuilder["AccountNumber"] = CardNumber.Text;
                        paymentInstrumentBuilder["ExpirationMonth"] = ExpirationMonth.SelectedItem.Value;
                        paymentInstrumentBuilder["ExpirationYear"] = ExpirationYear.SelectedItem.Value;
                        paymentInstrumentBuilder["SecurityCode"] = SecurityCode.Text;
                        if (IssueNumber.Text.Length > 0) paymentInstrumentBuilder["IssueNumber"] = IssueNumber.Text;
                        if ((StartDateMonth.SelectedIndex > 0) && (StartDateYear.SelectedIndex > 0))
                        {
                            paymentInstrumentBuilder["StartDateMonth"] = StartDateMonth.SelectedItem.Value;
                            paymentInstrumentBuilder["StartDateYear"] = StartDateYear.SelectedItem.Value;
                        }
                        payment.ReferenceNumber = StringHelper.MakeReferenceNumber(CardNumber.Text);
                        payment.Amount = AlwaysConvert.ToDecimal(CreditCardPaymentAmount.Text);
                        break;
                    case PaymentInstrument.Check:
                        paymentInstrumentBuilder["RoutingNumber"] = RoutingNumber.Text;
                        paymentInstrumentBuilder["BankName"] = BankName.Text;
                        paymentInstrumentBuilder["AccountHolder"] = AccountHolder.Text;
                        paymentInstrumentBuilder["AccountNumber"] = BankAccountNumber.Text;
                        payment.ReferenceNumber = StringHelper.MakeReferenceNumber(BankAccountNumber.Text);
                        payment.Amount = AlwaysConvert.ToDecimal(CheckPaymentAmount.Text);
                        break;
                }
                if (payment.Amount > 0)
                {
                    //PRESERVE PAYMENT INSTRUMENT DATA
                    _Order.Payments.Add(payment);
                    _Order.Payments.Save();
                    //ADD IN PAYMENT INSTRUMENT DATA FOR PROCESSING
                    payment.AccountData = paymentInstrumentBuilder.ToString();
                    payment.Authorize(false);
                    //REDIRECT TO PAYMENT PAGE
                    Response.Redirect("Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _Order.OrderId.ToString());
                }
            }
        }
    }

    private void DisableAutoComplete()
    {
        CardNumber.Attributes.Add("autocomplete", "off");
        SecurityCode.Attributes.Add("autocomplete", "off");
        IssueNumber.Attributes.Add("autocomplete", "off");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisableAutoComplete();
    }
}
