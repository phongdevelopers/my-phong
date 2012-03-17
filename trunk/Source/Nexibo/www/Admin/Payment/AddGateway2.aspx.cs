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
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Collections.Generic;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;

public partial class Admin_Payment_AddGateway2 : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private List<PaymentMethod> _PaymentMethods;
    
    private string _ProviderClassId = String.Empty;
    protected string ProviderClassId
    {
        get
        {
            if (string.IsNullOrEmpty(_ProviderClassId))
            {
                _ProviderClassId = ViewState["ProviderClassId"] as string;
                if (string.IsNullOrEmpty(_ProviderClassId))
                {
                    Guid sessionKey = AlwaysConvert.ToGuid(Request.QueryString["ProviderClassId"]);
                    if ((sessionKey != Guid.Empty))
                    {
                        _ProviderClassId = (string)Session[sessionKey.ToString()];
                        ViewState["ProviderClassId"] = _ProviderClassId;
                    }
                }
            }
            return _ProviderClassId;
        }
    }

    private IPaymentProvider _ProviderInstance = null;
    protected IPaymentProvider ProviderInstance
    {
        get
        {
            if ((_ProviderInstance == null))
            {
                if (!string.IsNullOrEmpty(ProviderClassId))
                {
                    _ProviderInstance = Activator.CreateInstance(Type.GetType(ProviderClassId)) as IPaymentProvider;
                    if (_ProviderInstance != null)
                    {
                        _ProviderInstance.Initialize(0, GetConfigData());
                    }
                }
            }
            return _ProviderInstance;
        }
    }

    protected Dictionary<string, string> GetConfigData()
    {
        Dictionary<string, string> configData = new Dictionary<String, String>();
        string configPrefix = phInputForm.Parent.UniqueID + "$Config_";
        foreach (string key in Request.Form)
        {
            if (key.StartsWith(configPrefix))
            {
                configData.Add(key.Remove(0, configPrefix.Length), Request.Form[key]);
            }
        }
        return configData;
    }

    protected bool IsMethodAssigned(object dataItem)
    {
        //DEFAULT CHECKED FOR ANY UNASSIGNED CREDIT CARD
        //ALSO DEFAULT CHECKED PAYPAL FOR PAYPAL GATEWAY
        PaymentMethod method = (PaymentMethod)dataItem;
        //DO NOT DEFAULT CHECKED ANY PAYMENT ALREADY ASSIGNED TO A GATEWAY
        if (method.PaymentGatewayId != 0) return false;
        //DEFAULT CHECKED FOR UNASSIGNED CREDIT CARD METHODS
        if (method.IsCreditOrDebitCard()) return true;
        //DEFAULT CHECKED FOR PAYPAL IF THIS IS PAYPAL GATEWAY
        if (method.PaymentInstrument == PaymentInstrument.PayPal)
        {
            bool isPayPalGateway = (_ProviderInstance is CommerceBuilder.Payments.Providers.PayPal.PayPalProvider);
            return isPayPalGateway;
        }
        return false;
    }

    protected void Page_Init(object sender, System.EventArgs e)
    {
        //REDIRECT IF NO PROVIDER SPECIFIED
        if (ProviderInstance == null) Response.Redirect("AddGateway.aspx");
        //DYNAMICALLY BUILD FORM
        GetInputForm();
        //LOAD PAYMENT METHODS
        LoadPaymentMethods();
        //TOGGLE THE PAYMENT METHODS SECTION
        if (!ShowPaymentMethods(ProviderInstance))
            PaymentMethodPanel.Visible = false;
    }

    protected void GetInputForm()
    {
        phInputForm.Controls.Clear();
        ProviderInstance.BuildConfigForm(phInputForm);
    }

    private bool IsMethodVisible(PaymentMethod method)
    {
        PaymentInstrument[] hiddenMethods = { PaymentInstrument.GiftCertificate, PaymentInstrument.GoogleCheckout, PaymentInstrument.Mail, PaymentInstrument.PhoneCall, PaymentInstrument.PurchaseOrder };
        //DO NOT SHOW HIDDEN PAYMENT INTRUMENTS
        if (Array.IndexOf(hiddenMethods, method.PaymentInstrument) > -1) return false;
        //ONLY SHOW PAYPAL FOR THAT GATEWAY
        bool isPayPalGateway = (_ProviderInstance is CommerceBuilder.Payments.Providers.PayPal.PayPalProvider);
        if (method.PaymentInstrument == PaymentInstrument.PayPal) return isPayPalGateway;
        //MUST BE CREDIT CARD, CHECK, OR UNKNOWN
        return true;
    }

    protected void LoadPaymentMethods()
    {
        _PaymentMethods = new List<PaymentMethod>();
        PaymentMethodCollection allPaymentMethods = PaymentMethodDataSource.LoadForStore("Name");
        foreach (PaymentMethod method in allPaymentMethods)
        {
            if (IsMethodVisible(method))
            {
                _PaymentMethods.Add(method);
            }
        }
        PaymentMethodList.DataSource = _PaymentMethods;
        PaymentMethodList.DataBind();
        PaymentMethodPanel.Visible = (_PaymentMethods.Count > 0);
    }

    private void ClearSessionKey()
    {
        Guid sessionKey = AlwaysConvert.ToGuid(Request.QueryString["ProviderClassId"]);
        if ((sessionKey != Guid.Empty))
        {
            Session.Remove(sessionKey.ToString());
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        ClearSessionKey();
        Response.Redirect("Gateways.aspx");
    }

    protected void BackButton_Click(object sender, System.EventArgs e)
    {
        ClearSessionKey();
        Response.Redirect("AddGateway.aspx");
    }

    protected PaymentMethod GetPaymentMethod(int paymentMethodId)
    {
        foreach (PaymentMethod method in _PaymentMethods)
        {
            if (method.PaymentMethodId.Equals(paymentMethodId)) return method;
        }
        return null;
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        PaymentGateway gateway = new PaymentGateway();
        gateway.ClassId = ProviderClassId;
        gateway.Name = ProviderInstance.Name;
        gateway.UpdateConfigData(this.GetConfigData());
        gateway.Save();
        ClearSessionKey();
        if (ShowPaymentMethods(ProviderInstance))
        {
            //UPDATE PAYMENT METHODS
            int index = 0;
            foreach (DataListItem item in PaymentMethodList.Items)
            {
                int paymentMethodId = AlwaysConvert.ToInt(PaymentMethodList.DataKeys[index]);
                PaymentMethod method = GetPaymentMethod(paymentMethodId);
                if (method != null)
                {
                    CheckBox cbMethod = (CheckBox)PageHelper.RecursiveFindControl(item, "Method");
                    if (cbMethod.Checked) method.PaymentGatewayId = gateway.PaymentGatewayId;
                    method.Save();
                }
                index++;
            }
        }
        Response.Redirect("Gateways.aspx");
    }

    private bool ShowPaymentMethods(IPaymentProvider provider)
    {
        string providerClassId = Misc.GetClassId(provider.GetType());
        string gcerClassId = Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.GiftCertificatePaymentProvider));
        string gchkClassId = Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.GoogleCheckout.GoogleCheckout));
        bool unassignable = gcerClassId.Equals(providerClassId) || gchkClassId.Equals(providerClassId);
        return !unassignable;
    }

}
