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
using CommerceBuilder.Common;

public partial class Admin_Orders_Payments_ucRecordPayment : System.Web.UI.UserControl
{
    private int _OrderId = 0;
    private Order _Order;

    protected Order Order
    {
        get
        {
            if (_Order == null)
            {
                _Order = OrderDataSource.Load(this.OrderId);
            }
            return _Order;
        }
    }

    protected int OrderId
    {
        get
        {
            if (_OrderId.Equals(0))
            {
                _OrderId = PageHelper.GetOrderId();
            }
            return _OrderId;
        }
    }

    PaymentMethodCollection methods;

    protected void Page_Load(object sender, EventArgs e)
    {
        string sqlCriteria = "PaymentInstrumentId<>" + (short)PaymentInstrument.GiftCertificate
        + " AND PaymentInstrumentId<>" + (short)PaymentInstrument.GoogleCheckout;

        methods = PaymentMethodDataSource.LoadForCriteria(sqlCriteria);

        if (!Page.IsPostBack)
        {
            CancelLink.NavigateUrl += "?OrderNumber=" + Order.OrderNumber.ToString() + "&OrderId=" + OrderId.ToString();
            SelectedPaymentMethod.DataSource = methods;
            SelectedPaymentMethod.DataBind();
            Amount.Text = string.Format("{0:F2}", Order.GetBalance(true));
            //LOAD PAYMENT STATUSES
            foreach (PaymentStatus status in Enum.GetValues(typeof(PaymentStatus)))
            {
                selPaymentStatus.Items.Add(new ListItem(status.ToString(), ((int)status).ToString()));
            }

            //ListItem completed = selPaymentStatus.Items.FindByValue(((int)PaymentStatus.Captured).ToString());
            //if (completed != null) completed.Selected = true;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        Payment payment = new Payment();
        payment.OrderId = OrderId;
        payment.PaymentMethodId = AlwaysConvert.ToInt(SelectedPaymentMethod.SelectedValue);
        payment.ReferenceNumber = ReferenceNumber.Text;
        payment.Amount = AlwaysConvert.ToDecimal(Amount.Text);
        payment.CurrencyCode = "USD";
        payment.PaymentStatusId = AlwaysConvert.ToInt16(selPaymentStatus.SelectedValue);
        payment.PaymentStatusReason = PaymentStatusReason.Text;
        Order.Payments.Add(payment);
        Order.Payments.Save();
        string addNote = string.Format("A payment of type {0} and amount {1:c} is recorded as {2}.", payment.PaymentMethodName, payment.Amount, payment.PaymentStatus);
        Order.Notes.Add(new OrderNote(Order.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, addNote, NoteType.SystemPrivate));
        Order.Notes.Save();
        Order.Save();
        Response.Redirect(CancelLink.NavigateUrl);
    }

    protected PaymentMethod GetSelectedMethod()
    {
        int paymentMethodId = AlwaysConvert.ToInt(SelectedPaymentMethod.SelectedValue);
        if (methods != null)
        {
            foreach (PaymentMethod method in methods)
            {
                if (method.PaymentMethodId == paymentMethodId) return method;
            }
        }
        return null;
    }

    protected void SelectedPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        PaymentMethod method = GetSelectedMethod();
        if (method != null)
        {
            selPaymentStatus.Items.Clear();
            if (method.IsCreditOrDebitCard())
            {
                foreach (PaymentStatus s in Enum.GetValues(typeof(PaymentStatus)))
                {
                    ListItem item = new ListItem(StringHelper.SpaceName(s.ToString()), ((int)s).ToString());
                    selPaymentStatus.Items.Add(item);
                }
            }
            else
            {
                ListItem item = new ListItem(StringHelper.SpaceName(PaymentStatus.Unprocessed.ToString()), ((int)PaymentStatus.Unprocessed).ToString());
                selPaymentStatus.Items.Add(item);

                item = new ListItem(StringHelper.SpaceName(PaymentStatus.Completed.ToString()), ((int)PaymentStatus.Completed).ToString());
                selPaymentStatus.Items.Add(item);
            }
        }
    }
}
