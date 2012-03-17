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
using CommerceBuilder.Payments;
using System.Collections.Generic;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;
using CommerceBuilder.Users;

public partial class Admin_Payment_EditPaymentMethodDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;
    private const bool ShowIntlPaymentMethods = true;

    public int PaymentMethodId
    {
        get { return AlwaysConvert.ToInt(ViewState["PaymentMethodId"]); }
        set { ViewState["PaymentMethodId"] = value; }
    }

    public void LoadDialog(int paymentMethodId)
    {
        PaymentMethodId = paymentMethodId;
        PaymentMethod _PaymentMethod = PaymentMethodDataSource.Load(PaymentMethodId);
        BindPaymentInstrumentList(_PaymentMethod);
        Name.Text = _PaymentMethod.Name;
        BindGatewayList(_PaymentMethod);
        UseGroupRestriction.SelectedIndex = (_PaymentMethod.PaymentMethodGroups.Count == 0) ? 0 : 1;
        BindGroups();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        //TRIGER ANY EVENT ATTACHED TO THE CANCEL
        if (Cancelled != null) Cancelled(sender, e);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            PaymentMethod _PaymentMethod = PaymentMethodDataSource.Load(PaymentMethodId);
            _PaymentMethod.Name = Name.Text;
            _PaymentMethod.PaymentInstrument = (PaymentInstrument)AlwaysConvert.ToInt16(PaymentInstrumentList.SelectedValue);
            _PaymentMethod.PaymentGatewayId = AlwaysConvert.ToInt(GatewayList.SelectedValue);
            //GROUP RESTRICTION
            _PaymentMethod.PaymentMethodGroups.DeleteAll();
            if (UseGroupRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in GroupList.Items)
                {
                    if (item.Selected) _PaymentMethod.PaymentMethodGroups.Add(new PaymentMethodGroup(0, AlwaysConvert.ToInt(item.Value)));
                }
            }
            _PaymentMethod.Save();
            //TRIGER ANY EVENT ATTACHED TO THE UPDATE
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(PaymentMethodId, _PaymentMethod.Name));
        }
    }

    protected void BindPaymentInstrumentList(PaymentMethod _PaymentMethod)
    {
        PaymentInstrumentList.SelectedIndex = -1;
        PaymentInstrumentList.Items.Clear();
        // LOAD INSTRUMENT SELECT BOX
        List<ListItem> tempItems = new List<ListItem>();
        foreach (object enumItem in Enum.GetValues(typeof(PaymentInstrument)))
        {
            PaymentInstrument instrument = ((PaymentInstrument)enumItem);
            switch (instrument)
            {
                case PaymentInstrument.Check:
                case PaymentInstrument.Discover:
                case PaymentInstrument.JCB:
                case PaymentInstrument.Mail:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.PayPal:
                case PaymentInstrument.Visa:
                    tempItems.Add(new ListItem(instrument.ToString(), ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.AmericanExpress:
                    tempItems.Add(new ListItem("American Express", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.DinersClub:
                    if (ShowIntlPaymentMethods) tempItems.Add(new ListItem("Diner's Club", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.PhoneCall:
                    tempItems.Add(new ListItem("Phone Call", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.SwitchSolo:
                    if (ShowIntlPaymentMethods) tempItems.Add(new ListItem("Switch/Solo", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.VisaDebit:
                    if (ShowIntlPaymentMethods) tempItems.Add(new ListItem("Visa Debit (Delta/Electron)", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.Maestro:
                    if (ShowIntlPaymentMethods) tempItems.Add(new ListItem(instrument.ToString(), ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.PurchaseOrder:
                    tempItems.Add(new ListItem("Purchase Order", ((short)instrument).ToString()));
                    break;
                case PaymentInstrument.CreditCard:
                    tempItems.Add(new ListItem("Credit Card", ((short)instrument).ToString()));
                    break;
            }
            tempItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });
            PaymentInstrumentList.Items.Clear();
            PaymentInstrumentList.Items.Add(new ListItem(string.Empty));
            PaymentInstrumentList.Items.AddRange(tempItems.ToArray());
        }
        ListItem item = PaymentInstrumentList.Items.FindByValue(_PaymentMethod.PaymentInstrumentId.ToString());
        if (item != null) PaymentInstrumentList.SelectedIndex = PaymentInstrumentList.Items.IndexOf(item);
    }

    protected void BindGatewayList(PaymentMethod _PaymentMethod)
    {
        GatewayList.Items.Clear();
        GatewayList.SelectedIndex = -1;
        GatewayList.Items.Add(new ListItem("", ""));
        foreach (PaymentGateway gateway in Token.Instance.Store.PaymentGateways)
        {
            if (IsAssignableGateway(gateway))
            {
                GatewayList.Items.Add(new ListItem(gateway.Name, gateway.PaymentGatewayId.ToString()));
            }
        }
        ListItem item = GatewayList.Items.FindByValue(_PaymentMethod.PaymentGatewayId.ToString());
        if (item != null) GatewayList.SelectedIndex = GatewayList.Items.IndexOf(item);
    }

    private bool IsAssignableGateway(PaymentGateway gateway)
    {
        string gcerClassId = Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.GiftCertificatePaymentProvider));
        string gchkClassId = Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.GoogleCheckout.GoogleCheckout));
        //string ppalClassId = Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.PayPal.PayPalProvider));

        if (gcerClassId.Equals(gateway.ClassId)
            || gchkClassId.Equals(gateway.ClassId)
            /*|| ppalClassId.Equals(gateway.ClassId)*/)
        {
            return false;
        }
        return true;
    }

    protected void UseGroupRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGroups();
    }

    protected void BindGroups()
    {
        PaymentMethod _PaymentMethod = PaymentMethodDataSource.Load(PaymentMethodId);
        GroupListPanel.Visible = (UseGroupRestriction.SelectedIndex > 0);
        if (GroupListPanel.Visible)
        {
            GroupList.DataSource = GroupDataSource.LoadForStore("Name");
            GroupList.DataBind();
            foreach (PaymentMethodGroup r in _PaymentMethod.PaymentMethodGroups)
            {
                ListItem item = GroupList.Items.FindByValue(r.GroupId.ToString());
                if (item != null) item.Selected = true;
            }
        }
    }
}
