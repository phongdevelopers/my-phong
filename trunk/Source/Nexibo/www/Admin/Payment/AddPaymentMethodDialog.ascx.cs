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
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;

public partial class Admin_Payment_AddPaymentMethodDialog : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event PersistentItemEventHandler ItemAdded;
    private const bool ShowIntlPaymentMethods = true;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
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
            // LOAD GATEWAY SELECT BOX
            GatewayList.Items.Add(new ListItem("", ""));
            foreach (PaymentGateway gateway in Token.Instance.Store.PaymentGateways)
            {
                if (IsAssignableGateway(gateway))
                {
                    GatewayList.Items.Add(new ListItem(gateway.Name, gateway.PaymentGatewayId.ToString()));
                }
            }
            //GROUP RESTRICTION
            UseGroupRestriction.SelectedIndex = 0;
            BindGroups();
        }
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

    protected void PaymentInstrumentList_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (string.IsNullOrEmpty(Name.Text))
        {
            Name.Text = PaymentInstrumentList.SelectedItem.Text;
        }
        if (PaymentInstrumentList.SelectedValue == ((short)PaymentInstrument.PayPal).ToString())
        {
            GatewayList.SelectedIndex = 0;
            GatewayList.Enabled = false;
        }
        else
        {
            GatewayList.Enabled = true;
        }
    }

    protected void AddButton_Click(object sender, System.EventArgs e)
    {
        PaymentMethod method = new PaymentMethod();
        method.Name = Name.Text;
        method.PaymentInstrument = (PaymentInstrument)AlwaysConvert.ToInt16(PaymentInstrumentList.SelectedValue);
        method.PaymentGatewayId = AlwaysConvert.ToInt(GatewayList.SelectedValue);
        //GROUP RESTRICTION
        if (UseGroupRestriction.SelectedIndex > 0)
        {
            foreach (ListItem item in GroupList.Items)
            {
                if (item.Selected) method.PaymentMethodGroups.Add(new PaymentMethodGroup(0, AlwaysConvert.ToInt(item.Value)));
            }
        }
        method.OrderBy = (short)PaymentMethodDataSource.GetNextOrderBy();
        method.Save();
        //UPDATE THE ADD MESSAGE
        AddedMessage.Text = string.Format(AddedMessage.Text, method.Name);
        AddedMessage.Visible = true;
        //RESET THE ADD FORM
        PaymentInstrumentList.SelectedIndex = -1;
        Name.Text = string.Empty;
        GatewayList.SelectedIndex = -1;
        UseGroupRestriction.SelectedIndex = 0;
        BindGroups();
        //TRIGER ANY EVENT ATTACHED TO THE UPDATE
        if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(method.PaymentMethodId, method.Name));
    }

    protected void UseGroupRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGroups();
    }

    protected void BindGroups()
    {
        GroupListPanel.Visible = (UseGroupRestriction.SelectedIndex > 0);
        if (GroupListPanel.Visible)
        {
            GroupList.DataSource = GroupDataSource.LoadForStore("Name");
            GroupList.DataBind();
        }
    }
}
