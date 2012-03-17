using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Messaging;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

public partial class Admin_Store_EmailTemplates_AddTemplate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        InitTriggerGrid();
        MailFormat.Attributes.Add("onchange", "$get('" + MessageHtml.ClientID + "').style.visibility=(this.selectedIndex==0?'visible':'hidden');");
        MessageHtml.OnClientClick = "if(/^\\s*#.*?\\n/gm.test($get('" + Message.ClientID + "').value)){if(!confirm('WARNING: HTML editor may corrupt NVelocity script if you make changes in WYSIWYG mode.  Continue?'))return false;}";
        PageHelper.SetHtmlEditor(Message, MessageHtml);

        if (!Page.IsPostBack)
        {
            EmailTemplate template = EmailTemplateDataSource.Load(AlwaysConvert.ToInt(Request.QueryString["EmailTemplateId"]));
            if (template == null)
            {
                // EXPECTED CASE, NEW TEMPLATE
                FromAddress.Text = Store.GetCachedSettings().DefaultEmailAddress;
            }
            else
            {
                // LOAD (FOR NEW COPY)
                Name.Text = StringHelper.Truncate("Copy of " + template.Name, 100);
                FromAddress.Text = template.FromAddress;
                ToAddress.Text = template.ToAddress;
                CCAddress.Text = template.CCList;
                BCCAddress.Text = template.BCCList;
                FromAddress.Text = template.FromAddress;
                Subject.Text = template.Subject;
                Message.Text = template.Body;
                MailFormat.SelectedIndex = (template.IsHTML ? 0 : 1);
                foreach (GridViewRow row in TriggerGrid.Rows)
                {
                    int eventId = AlwaysConvert.ToInt(TriggerGrid.DataKeys[row.RowIndex].Value);
                    if (template.Triggers.IndexOf(template.EmailTemplateId, eventId) > -1)
                    {
                        CheckBox selected = row.FindControl("Selected") as CheckBox;
                        if (selected != null) selected.Checked = true;
                    }
                }
            }
        }
    }

    private void InitTriggerGrid()
    {
        // BUILD A LIST SUPPORTED TRIGGER DATA
        List<EventTriggerDataItem> triggers = new List<EventTriggerDataItem>();
        EventTriggerDataItem trigger;

        // ORDER EVENTS
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderPlaced, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderPlaced), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderPaid, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderPaid), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderPaidNoShipments, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderPaidNoShipments), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderPaidPartial, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderPaidPartial), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderPaidCreditBalance, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderPaidCreditBalance), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderShipped, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderShipped), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderShippedPartial, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderShippedPartial), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.ShipmentShipped, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.ShipmentShipped), "Customer, Vendor", "$customer, $order, $payments, $shipment");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderNoteAddedByMerchant, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderNoteAddedByMerchant), "Customer, Vendor", "$customer, $order, $note");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderNoteAddedByCustomer, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderNoteAddedByCustomer), "Customer, Vendor", "$customer, $order, $note");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderStatusUpdated, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderStatusUpdated), "Customer, Vendor", "$customer, $order, $oldstatusname");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.OrderCancelled, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.OrderCancelled), "Customer, Vendor", "$customer, $order, $payments");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.GiftCertificateValidated, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.GiftCertificateValidated), "Customer, Vendor", "$customer, $order, $giftcertificate");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.PaymentAuthorized, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.PaymentAuthorized), "Customer, Vendor", "$customer, $order, $payment");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.PaymentAuthorizationFailed, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.PaymentAuthorizationFailed), "Customer, Vendor", "$customer, $order, $payment");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.PaymentCaptured, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.PaymentCaptured), "Customer, Vendor", "$customer, $order, $payment");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.PaymentCapturedPartial, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.PaymentCapturedPartial), "Customer, Vendor", "$customer, $order, $payment");
        triggers.Add(trigger);
        trigger = new EventTriggerDataItem((int)StoreEvent.PaymentCaptureFailed, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.PaymentCaptureFailed), "Customer, Vendor", "$customer, $order, $payment");
        triggers.Add(trigger);

        // CUSTOMER EVENTS
        trigger = new EventTriggerDataItem((int)StoreEvent.CustomerPasswordRequest, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.CustomerPasswordRequest), "Customer", "$customer, $resetPasswordLink");
        triggers.Add(trigger);

        // INVENTORY EVENTS
        trigger = new EventTriggerDataItem((int)StoreEvent.LowInventoryItemPurchased, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.LowInventoryItemPurchased), string.Empty, "$products");
        triggers.Add(trigger);

        // BIND THE EVENT DATA
        TriggerGrid.DataSource = triggers;
        TriggerGrid.DataBind();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        EmailTemplate template = new EmailTemplate();
        template.Name = Name.Text;
        template.ToAddress = ToAddress.Text;
        template.FromAddress = FromAddress.Text;
        template.CCList = CCAddress.Text;
        template.BCCList = BCCAddress.Text;
        template.Subject = Subject.Text;        
        string temporaryFileName = Guid.NewGuid().ToString();
        template.ContentFileName = temporaryFileName;
        template.IsHTML = (MailFormat.SelectedIndex == 0);
        foreach (GridViewRow row in TriggerGrid.Rows)
        {
            CheckBox selected = row.FindControl("Selected") as CheckBox;
            if (selected.Checked)
            {
                int eventId = AlwaysConvert.ToInt(TriggerGrid.DataKeys[row.RowIndex].Value);
                template.Triggers.Add(new EmailTemplateTrigger(0, eventId));
            }
        }
        template.Save();

        // RENAME THE FILE AND SAVE        
        template.Body = Message.Text;
        template.ContentFileName = Regex.Replace(Name.Text, "[^a-zA-Z0-9 _-]", "") + " ID" + template.EmailTemplateId + ".html";
        template.Save();

        // DELETE TEMPORARY CONTENT FILE
        string templatesPath = "App_Data\\EmailTemplates\\" + Token.Instance.StoreId;
        templatesPath = Path.Combine(FileHelper.ApplicationBasePath, templatesPath);
        string tempFilePath = Path.Combine(templatesPath, temporaryFileName);
        try
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        catch(Exception ex)
        {
            Logger.Warn("Error deleting temporary email template content file '" + tempFilePath + "'.", ex);
        }

        Response.Redirect("Default.aspx");
    }

    protected class EventTriggerDataItem
    {
        private int _EventId;
        private string _Name;
        private string _EmailAliases;
        private string _NVelocityVariables;
        public int EventId { get { return _EventId; } }
        public string Name { get { return _Name; } }
        public string EmailAliases { get { return _EmailAliases; } }
        public string NVelocityVariables { get { return _NVelocityVariables; } }
        private EventTriggerDataItem() { }
        public EventTriggerDataItem(int eventId, string name, string emailAliases, string nVelocityVariables)
        {
            this._EventId = eventId;
            this._Name = name;
            this._EmailAliases = emailAliases;
            this._NVelocityVariables = nVelocityVariables;
        }
    }
}