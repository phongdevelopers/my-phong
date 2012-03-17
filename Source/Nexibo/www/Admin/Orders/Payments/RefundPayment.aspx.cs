using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Orders_Payments_RefundPayment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId = 0;
    private Order _Order;
    private int _PaymentId = 0;
    private Payment _Payment;
    private IPaymentProvider _PaymentProvider;
    private AccountDataDictionary _AccountData;

    protected void Page_Init(object sender, EventArgs e)
    {
        _PaymentId = AlwaysConvert.ToInt(Request.QueryString["PaymentId"]);
        _Payment = PaymentDataSource.Load(_PaymentId);
        if (_Payment == null) Response.Redirect("../Default.aspx");
        _OrderId = _Payment.OrderId;
        _Order = _Payment.Order;
        Caption.Text = string.Format(Caption.Text, (_Order.Payments.IndexOf(_PaymentId) + 1), _Payment.ReferenceNumber);
        PaymentReference.Text = _Payment.PaymentMethodName + " - " + _Payment.ReferenceNumber;
        PaymentAmount.Text = string.Format("{0:lc}", _Payment.Amount);
        if (!Page.IsPostBack) RefundAmount.Text = string.Format("{0:F2}", _Payment.Amount);

        _PaymentProvider = GetPaymentProvider(_Payment);
        if (_PaymentProvider != null && _PaymentProvider.RefundRequiresAccountData)
        {
            // INITIALIZE FIELDS FOR COLLECTING REFUND ACCOUNT DATA
            _AccountData = new AccountDataDictionary(_Payment.AccountData);
            bool isCheckPayment = (_Payment.PaymentMethod != null && _Payment.PaymentMethod.PaymentInstrument == PaymentInstrument.Check);
            if (isCheckPayment)
            {
                CheckFields.Visible = true;
                CreditCardFields.Visible = false;
                DebitCardFields.Visible = false;
                InitializeCheckFields();
            }
            else
            {
                CheckFields.Visible = false;
                CreditCardFields.Visible = true;
                bool isAuthNet = (_PaymentProvider.Name == "Authorize.Net");
                if (isAuthNet)
                {
                    trCreditCardExpiration.Visible = false;
                    CreditCardNumberLiteral.Text = " (last 4 digits or whole number)";
                    CreditCardNumberValidator.ValidationExpression = "^(\\d{4}|\\d{13,19})$";
                    CreditCardNumberValidator.ErrorMessage = "Enter the last 4 digits or the whole number between 13 and 19 digits long.";
                }
                DebitCardFields.Visible = _Payment.PaymentMethod != null && _Payment.PaymentMethod.IsIntlDebitCard();
                InitializeCreditCardFields();
            }
        }
        else
        {
            CheckFields.Visible = false;
            CreditCardFields.Visible = false;
            DebitCardFields.Visible = false;
        }
    }

    /// <summary>
    /// Gets the payment provider instance for the given payment, if available.
    /// </summary>
    /// <param name="payment">The payment to identify the provider for.</param>
    /// <returns>The IPaymentProvider instance for the payment, or null if none is available.</returns>
    private static IPaymentProvider GetPaymentProvider(Payment payment)
    {
        if (payment == null) return null;
        PaymentMethod paymentMethod = payment.PaymentMethod;
        if (paymentMethod == null) return null;
        PaymentGateway gateway = paymentMethod.PaymentGateway;
        if (gateway == null) return null;
        return gateway.GetInstance();
    }

    private void InitializeCreditCardFields()
    {
        if (HasValue(_AccountData, "AccountNumber"))
        {
            CreditCardNumber.Visible = false;
            CreditCardNumberLiteral.Visible = true;
            CreditCardNumberLiteral.Text = MakeReferenceNumber(_AccountData["AccountNumber"]);
        }
        if (HasValue(_AccountData, "ExpirationMonth") && HasValue(_AccountData, "ExpirationYear"))
        {
            CreditCardExpirationMonth.Visible = false;
            CreditCardExpirationYear.Visible = false;
            CreditCardExpirationLiteral.Text = _AccountData["ExpirationMonth"] + "/" + _AccountData["ExpirationYear"];
        }
        else
        {
            int thisYear = LocaleHelper.LocalNow.Year;
            for (int i = 0; i < 10; i++)
            {
                CreditCardExpirationYear.Items.Add(new ListItem(thisYear.ToString(), thisYear.ToString()));
                thisYear += 1;
            }
        }

        // IF THIS IS AN INTERNATIONAL DEBIT CARD WE HAVE TO INITIALIZE ADDITIONAL FIELDS
        if (DebitCardFields.Visible)
        {
            bool hasIssueNumber = HasValue(_AccountData, "IssueNumber");
            if (hasIssueNumber || (HasValue(_AccountData, "StartDateMonth") && HasValue(_AccountData, "StartDateYear")))
            {
                if (hasIssueNumber)
                {
                    trDebitCardStartDate.Visible = false;
                    DebitCardIssueNumber.Visible = false;
                    DebitCardIssueNumberLiteral.Text = _AccountData["IssueNumber"];
                }
                else
                {
                    trDebitCardIssueNumber.Visible = false;
                    DebitCardStartMonth.Visible = false;
                    DebitCardStartYear.Visible = false;
                    DebitCardStartDateLiteral.Text = _AccountData["StartDateMonth"] + "/" + _AccountData["StartDateYear"];
                }
            }
        }
    }

    private void InitializeCheckFields()
    {
        if (HasValue(_AccountData, "AccountHolder"))
        {
            CheckAccountHolderName.Visible = false;
            CheckAccountHolderNameLiteral.Visible = true;
            CheckAccountHolderNameLiteral.Text = _AccountData["AccountHolder"];
        }
        if (HasValue(_AccountData, "BankName"))
        {
            CheckBankName.Visible = false;
            CheckBankNameLiteral.Visible = true;
            CheckBankNameLiteral.Text = _AccountData["BankName"];
        }
        if (HasValue(_AccountData, "RoutingNumber"))
        {
            CheckRoutingNumber.Visible = false;
            CheckRoutingNumberLiteral.Visible = true;
            CheckRoutingNumberLiteral.Text = _AccountData["RoutingNumber"];
        }
        if (HasValue(_AccountData, "AccountNumber"))
        {
            CheckAccountNumber.Visible = false;
            CheckAccountNumberLiteral.Visible = true;
            CheckAccountNumberLiteral.Text = MakeReferenceNumber(_AccountData["AccountNumber"]);
        }
    }

    private static bool HasValue(AccountDataDictionary data, string varName)
    {
        return (data.ContainsKey(varName) && !string.IsNullOrEmpty(data[varName]));
    }

    private static string MakeReferenceNumber(string secretValue)
    {
        if (secretValue == null || secretValue.Length < 4)
        {
            throw new ArgumentException("The input secret value must be longer than 4 characters.", "secretValue");
        }
        return new string('*', secretValue.Length - 4) + secretValue.Substring(secretValue.Length - 4);
    }

    protected void SubmitRefundButton_Click(object sender, EventArgs e)
    {
        //GET THE CAPTURE AMOUNT
        LSDecimal refundAmount = AlwaysConvert.ToDecimal(RefundAmount.Text);
        if (refundAmount > 0 && _Payment.PaymentStatus == PaymentStatus.Captured)
        {
            RefundTransactionRequest refundRequest = new RefundTransactionRequest(_Payment, Request.UserHostAddress);
            if (CreditCardFields.Visible)
            {
                SetRequestForCreditCard(refundRequest);
            }
            else if (CheckFields.Visible)
            {
                SetRequestForCheck(refundRequest);
            }
            refundRequest.Amount = refundAmount;
            PaymentEngine.DoRefund(refundRequest);
            //_Payment.Refund(refundRequest);
            if (!string.IsNullOrEmpty(CustomerNote.Text))
            {
                OrderNote note = new OrderNote(_Order.OrderId, Token.Instance.UserId, DateTime.UtcNow, CustomerNote.Text, NoteType.Public);
                note.Save();
            }
        }
        Response.Redirect("Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    private void SetRequestForCreditCard(RefundTransactionRequest refundRequest)
    {
        if (HasValue(_AccountData, "AccountNumber"))
        {
            refundRequest.CardNumber = _AccountData["AccountNumber"];
        }
        else
        {
            refundRequest.CardNumber = CreditCardNumber.Text;
        }
        if (HasValue(_AccountData, "ExpirationMonth") && HasValue(_AccountData, "ExpirationYear"))
        {
            refundRequest.ExpirationMonth = AlwaysConvert.ToInt(_AccountData["ExpirationMonth"]);
            refundRequest.ExpirationYear = AlwaysConvert.ToInt(_AccountData["ExpirationYear"]);
        }
        else
        {
            refundRequest.ExpirationMonth = AlwaysConvert.ToInt(CreditCardExpirationMonth.SelectedValue);
            refundRequest.ExpirationYear = AlwaysConvert.ToInt(CreditCardExpirationYear.SelectedValue);
        }

        // SET ADDITIONAL FIELDS FOR INTERNATIONAL DEBIT
        if (DebitCardFields.Visible)
        {
            if (HasValue(_AccountData, "IssueNumber"))
            {
                refundRequest.ExtendedProperties["IssueNumber"] = _AccountData.GetValue("IssueNumber");
            }
            else if (DebitCardIssueNumber.Text.Length > 0)
            {
                refundRequest.ExtendedProperties["IssueNumber"] = DebitCardIssueNumber.Text;
            }
            if (HasValue(_AccountData, "StartDateMonth") || HasValue(_AccountData, "StartDateYear"))
            {
                refundRequest.ExtendedProperties["StartDateMonth"] = _AccountData.GetValue("StartDateMonth");
                refundRequest.ExtendedProperties["StartDateYear"] = _AccountData.GetValue("StartDateYear");
            }
            else if (DebitCardStartMonth.SelectedIndex > 0 && DebitCardStartYear.SelectedIndex > 0)
            {
                refundRequest.ExtendedProperties["StartDateMonth"] = DebitCardStartMonth.SelectedValue;
                refundRequest.ExtendedProperties["StartDateYear"] = DebitCardStartYear.SelectedValue;
            }
        }
    }

    private void SetRequestForCheck(RefundTransactionRequest refundRequest)
    {
        if (HasValue(_AccountData, "BankName"))
        {
            refundRequest.ExtendedProperties["BankName"] = _AccountData["BankName"];
        }
        else
        {
            refundRequest.ExtendedProperties["BankName"] = CheckBankName.Text;
        }
        if (HasValue(_AccountData, "RoutingNumber"))
        {
            refundRequest.ExtendedProperties["RoutingNumber"] = _AccountData["RoutingNumber"];
        }
        else
        {
            refundRequest.ExtendedProperties["RoutingNumber"] = CheckRoutingNumber.Text;
        }
        if (HasValue(_AccountData, "AccountHolder"))
        {
            refundRequest.ExtendedProperties["AccountHolder"] = _AccountData["AccountHolder"];
        }
        else
        {
            refundRequest.ExtendedProperties["AccountHolder"] = CheckAccountHolderName.Text;
        }
        if (HasValue(_AccountData, "AccountNumber"))
        {
            refundRequest.ExtendedProperties["AccountNumber"] = _AccountData["AccountNumber"];
        }
        else
        {
            refundRequest.ExtendedProperties["AccountNumber"] = CheckAccountNumber.Text;
        }
        refundRequest.ExtendedProperties["AccountType"] = CheckAccountType.SelectedValue;
    }

    protected void CancelRefundButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CreditCardNumber.Attributes.Add("autocomplete", "off");
        CheckBankName.Attributes.Add("autocomplete", "off");
        CheckRoutingNumber.Attributes.Add("autocomplete", "off");
        CheckAccountHolderName.Attributes.Add("autocomplete", "off");
        CheckAccountNumber.Attributes.Add("autocomplete", "off");
    }
}