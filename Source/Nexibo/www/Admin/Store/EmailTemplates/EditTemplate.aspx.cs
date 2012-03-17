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

public partial class Admin_Store_EmailTemplates_EditTemplate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _EmailTemplateId;
    private EmailTemplate _EmailTemplate;

    protected void Page_Init(object sender, EventArgs e)
    {
        // LOCATE THE TEMPLATE TO EDIT
        _EmailTemplateId = AlwaysConvert.ToInt(Request.QueryString["EmailTemplateId"]);
        _EmailTemplate = EmailTemplateDataSource.Load(_EmailTemplateId);
        if (_EmailTemplate == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _EmailTemplate.Name);

        // LOAD TRIGGERS SELECT BOX
        InitTriggerGrid();
        MailFormat.Attributes.Add("onchange", "$get('" + MessageHtml.ClientID + "').style.visibility=(this.selectedIndex==0?'visible':'hidden');");
        MessageHtml.OnClientClick = "if(/^\\s*#.*?\\n/gm.test($get('" + Message.ClientID + "').value)){if(!confirm('WARNING: HTML editor may corrupt NVelocity script if you make changes in WYSIWYG mode.  Continue?'))return false;}";
        PageHelper.SetHtmlEditor(Message, MessageHtml);

        //INITIALIZE FORM ON FIRST VISIT
        if (!Page.IsPostBack)
        {
            Name.Text = _EmailTemplate.Name;
            FromAddress.Text = _EmailTemplate.FromAddress;
            ToAddress.Text = _EmailTemplate.ToAddress;
            CCAddress.Text = _EmailTemplate.CCList;
            BCCAddress.Text = _EmailTemplate.BCCList;
            FromAddress.Text = _EmailTemplate.FromAddress;
            Subject.Text = _EmailTemplate.Subject;
            Message.Text = _EmailTemplate.Body;
            MailFormat.SelectedIndex = (_EmailTemplate.IsHTML ? 0 : 1);
            foreach (GridViewRow row in TriggerGrid.Rows)
            {
                int eventId = AlwaysConvert.ToInt(TriggerGrid.DataKeys[row.RowIndex].Value);
                if (_EmailTemplate.Triggers.IndexOf(_EmailTemplateId, eventId) > -1)
                {
                    CheckBox selected = row.FindControl("Selected") as CheckBox;
                    if (selected != null) selected.Checked = true;
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
        trigger = new EventTriggerDataItem((int)StoreEvent.LowInventoryItemPurchased, StoreDataHelper.GetFriendlyStoreEventName(StoreEvent.LowInventoryItemPurchased), string.Empty, "$products, $variants");
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
        String oldName = _EmailTemplate.Name;
        _EmailTemplate.Name = Name.Text;
        _EmailTemplate.ToAddress = ToAddress.Text;
        _EmailTemplate.FromAddress = FromAddress.Text;
        _EmailTemplate.CCList = CCAddress.Text;
        _EmailTemplate.BCCList = BCCAddress.Text;
        _EmailTemplate.Subject = Subject.Text;
        _EmailTemplate.Body = Message.Text;
        _EmailTemplate.IsHTML = (MailFormat.SelectedIndex == 0);
        _EmailTemplate.Save();

        // UPDATE TRIGGERS
        _EmailTemplate.Triggers.DeleteAll();
        foreach (GridViewRow row in TriggerGrid.Rows)
        {
            CheckBox selected = row.FindControl("Selected") as CheckBox;
            if (selected.Checked)
            {
                int eventId = AlwaysConvert.ToInt(TriggerGrid.DataKeys[row.RowIndex].Value);
                _EmailTemplate.Triggers.Add(new EmailTemplateTrigger(0, eventId));
            }
        }
        _EmailTemplate.Save();


        // SEE IF NAME WAS UPDATED
        if (_EmailTemplate.Name != oldName)
        {
            // SEE IF FILE NAME NEEEDS TO BE CHANGED
            string generatedFileName = Regex.Replace(Name.Text, "[^a-zA-Z0-9 _-]", "") + " ID" + _EmailTemplate.EmailTemplateId + ".html";
            if (_EmailTemplate.ContentFileName != generatedFileName)
            {
                // INITIATE FILE NAME CHANGE
                string templatesPath = Path.Combine(FileHelper.ApplicationBasePath, "App_Data\\EmailTemplates\\" + Token.Instance.StoreId);
                string sourceFile = Path.Combine(templatesPath, _EmailTemplate.ContentFileName);
                string targetFile = Path.Combine(templatesPath, generatedFileName);
                try
                {
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                    File.Move(sourceFile, targetFile);
                    _EmailTemplate.ContentFileName = generatedFileName;
                }
                catch (Exception ex)
                {
                    string error = "Could not update the content file name for email template '{0}'.  Failed to rename file from {1} to {2}.";
                    Logger.Warn(string.Format(error, _EmailTemplate.Name, _EmailTemplate.ContentFileName, generatedFileName), ex);
                }

                // SAVE IN CASE THE RENAME WAS SUCCESSFUL
                // (IF RENAME FAILED, ISDIRTY FLAG WILL BE FALSE AND SAVE CALL WILL BE IGNORED)
                _EmailTemplate.Save();
            }
        }

        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        SavedMessage.Visible = true;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        MessageHtmlHelpText.Text = string.Format(MessageHtmlHelpText.Text, "App_Data/EmailTemplates/" + Token.Instance.StoreId.ToString() + "/" + _EmailTemplate.ContentFileName);
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