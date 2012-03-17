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
using CommerceBuilder.Common;
using CommerceBuilder.Payments;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Stores;

public partial class Admin_Orders_Payments_AccountDataViewport : System.Web.UI.UserControl
{
    public int PaymentId
    {
        get {
            if (ViewState["PaymentId"] == null) return 0;
            return (int)ViewState["PaymentId"];
        }
        set { ViewState["PaymentId"] = value; }
    }

    private Payment _Payment;
    private Payment Payment
    {
        get
        {
            if (_Payment == null) _Payment = PaymentDataSource.Load(this.PaymentId);
            return _Payment;
        }
    }

    private string _UnavailableText = string.Empty;
    public string UnavailableText
    {
        get { return _UnavailableText; }
        set { _UnavailableText = value; }
    }

    private bool ShowData
    {
        get
        {
            if (ViewState["ShowData"] == null) return false;
            return (bool)ViewState["ShowData"];
        }
        set { ViewState["ShowData"] = value; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //INITIALIZE THE ACCOUNT DATA VIEWPORT
        if (IsAccountDataAvailable())
        {
            //ACCOUNT DATA IS PRESENT AND USER HAS PERMISSION
            if (Request.IsSecureConnection)
            {
                //SSL IS AVAILABLE, MAKE DATA VISIBLE
                SSLRequiredPanel.Visible = false;
                if (ShowData)
                {
                    ShowAccountData.Visible = false;
                    AccountData.Visible = true;
                    AccountData.DataSource = new AccountDataDictionary(this.Payment.AccountData);
                    AccountData.DataBind();
                }
                else
                {
                    ShowAccountData.Visible = true;
                    AccountData.Visible = false;
                }
            }
            else
            {
                //SECURE CONNECTION NOT AVAILABLE
                SSLRequiredPanel.Visible = true;
                ShowAccountData.Visible = false;
                AccountData.Visible = false;
            }
        }
        else
        {
            //ACCOUNT DATA NOT PRESENT OR USER IS NOT PERMITTED
            this.Controls.Clear();
            if (!string.IsNullOrEmpty(_UnavailableText))
                this.Controls.Add(new LiteralControl(_UnavailableText));
        }
    }

    protected void ShowAccountData_Click(object sender, EventArgs e)
    {
        Payment payment = this.Payment;
        if (payment != null)
        {
            Logger.Audit(AuditEventType.ViewCardData, true, string.Empty, Token.Instance.UserId, payment.OrderId);
            ShowAccountData.Visible = false;
            AccountData.Visible = true;
            AccountData.DataSource = new AccountDataDictionary(payment.AccountData);
            AccountData.DataBind();
            this.ShowData = true;
        }
    }

    private bool IsAccountDataAvailable()
    {
        Payment payment = this.Payment;
        return ((payment != null) && payment.HasAccountData && Token.Instance.User.IsInRole(Role.OrderAdminRoles));
    }
}
