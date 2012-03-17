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
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Common;
using CommerceBuilder.Users;

public partial class Admin_Orders_Payments_CapturePayment : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _OrderId = 0;
    private Order _Order;
    private int _PaymentId = 0;
    private Payment _Payment;

    protected void InitVars()
    {
        _PaymentId = AlwaysConvert.ToInt(Request.QueryString["PaymentId"]);
        _Payment = PaymentDataSource.Load(_PaymentId);
        if (_Payment == null) Response.Redirect("../Default.aspx");
        _OrderId = _Payment.OrderId;
        _Order = _Payment.Order;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitVars();
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, (_Order.Payments.IndexOf(_PaymentId) + 1), _Payment.ReferenceNumber);
            CurrentPaymentStatus.Text = StringHelper.SpaceName(_Payment.PaymentStatus.ToString()).ToUpperInvariant();
            CurrentPaymentStatus.CssClass = CssHelper.GetPaymentStatusCssClass(_Payment.PaymentStatus);
            PaymentDate.Text = string.Format("{0:g}", _Payment.PaymentDate);
            Amount.Text = string.Format("{0:lc}", _Payment.Amount);
            PaymentMethod.Text = _Payment.PaymentMethodName;
            LSDecimal orig = _Payment.Transactions.GetTotalAuthorized();
            LSDecimal rem = _Payment.Transactions.GetRemainingAuthorized();
            LSDecimal bal = _Order.GetBalance(false);
            OriginalAuthorization.Text = string.Format("{0:lc}", orig);
            RemainingAuthorization.Text = string.Format("{0:lc}", rem);
            trRemainingAuthorization.Visible = (orig != rem);
            CaptureAmount.Text = string.Format("{0:F2}", bal);
            OrderBalance.Text = string.Format("{0:lc}", bal);
            trAdditionalCapture.Visible = IsPartialCaptureSupported();
        }
        AccountDataViewport.PaymentId = _PaymentId;
    }

    protected bool IsPartialCaptureSupported()
    {
        Transaction lastAuth = _Payment.Transactions.LastAuthorization;
        if (lastAuth != null)
        {
            PaymentGateway gateway = lastAuth.PaymentGateway;
            if (gateway != null)
            {
                IPaymentProvider instance = gateway.GetInstance();
                return ((instance.SupportedTransactions & SupportedTransactions.PartialCapture) == SupportedTransactions.PartialCapture);
            }
        }
        return false;
    }

    protected void SubmitCaptureButton_Click(object sender, EventArgs e)
    {
        //GET THE CAPTURE AMOUNT
        LSDecimal captureAmount = AlwaysConvert.ToDecimal(CaptureAmount.Text);
        bool finalCapture = NoAdditionalCapture.Checked;
        if (captureAmount > 0)
        {
            _Payment.Capture(captureAmount, finalCapture, false);
            if (!string.IsNullOrEmpty(CustomerNote.Text))
            {
                OrderNote note = new OrderNote(_Order.OrderId, Token.Instance.UserId, DateTime.UtcNow, CustomerNote.Text, NoteType.Public);
                note.Save();
            }
        }
        Response.Redirect( "Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

    protected void CancelCaptureButton_Click(object sender, EventArgs e)
    {
        Response.Redirect( "Default.aspx?OrderNumber=" + _Order.OrderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
    }

}
