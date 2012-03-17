using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Payments;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout
{
    public class GoogleCheckout : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        string _MerchantID = string.Empty;
        string _MerchantKey = string.Empty;
        bool _TrackingEnabled = false;
        bool _CouponsEnabled = true;
        bool _GiftCertificatesEnabled = true;
        bool _UseTestMode = true;
        int _ExpirationMinutes = 30;
        bool _UseBasicAuth = false;
        LSDecimal _DefaultShipRate;

        public override string Name
        {
            get { return "Google Checkout"; }
        }

        public override string Version
        {
            get { return "1.1"; }
        }

        public override string Description
        {
            get { return "Google Checkout"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.GoogleCheckout.Logo.gif");
            return string.Empty;
        }

        public string MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }

        public string MerchantKey
        {
            get { return _MerchantKey; }
            set { _MerchantKey = value; }
        }

        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        public bool UseBasicAuth
        {
            get { return _UseBasicAuth; }
            set { _UseBasicAuth = value; }
        }

        public bool CouponsEnabled
        {
            get { return _CouponsEnabled; }
            set { _CouponsEnabled = value; }
        }

        public bool GiftCertificatesEnabled
        {
            get { return _GiftCertificatesEnabled; }
            set { _GiftCertificatesEnabled = value; }
        }

        public bool TrackingEnabled
        {
            get { return _TrackingEnabled; }
            set { _TrackingEnabled = value; }
        }

        public int ExpirationMinutes
        {
            get { return _ExpirationMinutes;}
            set { _ExpirationMinutes = value;}
        }

        public LSDecimal DefaultShipRate
        {
            get { return _DefaultShipRate; }
            set { _DefaultShipRate = value; }
        }

        public EnvironmentType Environment
        {
            get 
            {
                if (UseTestMode)
                {
                    return EnvironmentType.Sandbox;
                }
                else
                {
                    return EnvironmentType.Production;
                }
            }
        }

        public override string ConfigReference
        {
            get { return "MerchantID : " + _MerchantID; }
        }

        public override void BuildConfigForm(Control parentControl)
        {

            HtmlTable configTable = new HtmlTable();
            configTable.CellPadding = 4;
            configTable.CellSpacing = 0;
            HtmlTableRow currentRow;
            HtmlTableCell currentCell;
            configTable.Attributes.Add("class", "inputForm");
            configTable.Attributes.Add("style", "border:none");

            //ADD CAPTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "sectionHeader");
            currentCell.ColSpan = 2;
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = this.Name;
            gatewayLink.NavigateUrl = "http://checkout.google.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable Google Checkout, you must provide Merchant ID and Merchant Key.  To locate these values, log into your Google Checkout merchant account, go to the Settings tab, and then click the 'Integration' menu item.  Your ID and key will be listed to the right.  "));
            currentCell.Controls.Add(new LiteralControl("While you are viewing your Google Checkout integration settings, also be sure that you check the option to only accept signed XML carts and provide the corect value for the API callback URL.</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //display assembly information
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Assembly:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            Label lblAssembly = new Label();
            lblAssembly.ID = "AssemblyInfo";
            lblAssembly.Text = this.GetType().Assembly.GetName().Name.ToString() + "&nbsp;(v" + this.GetType().Assembly.GetName().Version.ToString() + ")";
            currentCell.Controls.Add(lblAssembly);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //post security
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Post security:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">AbleCommerce sends digitally signed XML shopping carts.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("checked"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //show callback URL
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Callback URL:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Provide this value in your Google Checkout account. Please note that for live integration GoogleCheckout requires you to have SSL enabled.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            string callbackUrl = Token.Instance.Store.StoreUrl;
            if (!callbackUrl.EndsWith("/")) callbackUrl += "/";
            callbackUrl += "Checkout/Google/NotificationListener.ashx";
            callbackUrl = StringHelper.Replace(callbackUrl, "http", "https",StringComparison.InvariantCultureIgnoreCase);
            currentCell.Controls.Add(new LiteralControl(callbackUrl));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //post security
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Callback method:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">AbleCommerce requires XML callback messages from Google.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("XML"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get MerchantID
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant ID:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your Google Checkout Merchant ID is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantID = new TextBox();
            txtMerchantID.ID = "Config_MerchantID";
            txtMerchantID.Columns = 70;
            txtMerchantID.MaxLength = 280;
            txtMerchantID.Text = this.MerchantID;
            currentCell.Controls.Add(txtMerchantID);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get MerchantKey
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Key:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your Google Checkout Merchant Key is Required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantKey = new TextBox();
            txtMerchantKey.ID = "Config_MerchantKey";
            txtMerchantKey.Columns = 70;
            txtMerchantKey.MaxLength = 280;
            txtMerchantKey.Text = this.MerchantKey;
            currentCell.Controls.Add(txtMerchantKey);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Expiration Minutes
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Expiration Minutes:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Minutes after which the Google Cart expires.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtExpirationMinutes = new TextBox();
            txtExpirationMinutes.ID = "Config_ExpirationMinutes";
            txtExpirationMinutes.Columns = 20;
            txtExpirationMinutes.MaxLength = 20;
            txtExpirationMinutes.Text = this.ExpirationMinutes.ToString();
            currentCell.Controls.Add(txtExpirationMinutes);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Default Shipping Rate
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Default Ship Rate:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">A Default value of ship rate that is used if Merchant Calculated Shipping Fails.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtDefaultShipRate = new TextBox();
            txtDefaultShipRate.ID = "Config_DefaultShipRate";
            txtDefaultShipRate.Columns = 20;
            txtDefaultShipRate.MaxLength = 20;
            txtDefaultShipRate.Text = this.DefaultShipRate.ToString();
            currentCell.Controls.Add(txtDefaultShipRate);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get coupons enabled
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Coupons Enabled:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Indicate whether coupons are enabled or not.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblCouponsEnabled = new RadioButtonList();
            rblCouponsEnabled.ID = "Config_CouponsEnabled";
            rblCouponsEnabled.Items.Add(new ListItem("Enabled", "true"));
            rblCouponsEnabled.Items.Add(new ListItem("Disabled", "false"));
            rblCouponsEnabled.Items[CouponsEnabled ? 0 : 1].Selected = true;
            currentCell.Controls.Add(rblCouponsEnabled);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get giftcerts enabled
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Gift Certificates Enabled:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Indicate whether gift certificates are enabled or not.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGiftCertificatesEnabled = new RadioButtonList();
            rblGiftCertificatesEnabled.ID = "Config_GiftCertificatesEnabled";
            rblGiftCertificatesEnabled.Items.Add(new ListItem("Enabled", "true"));
            rblGiftCertificatesEnabled.Items.Add(new ListItem("Disabled", "false"));
            rblGiftCertificatesEnabled.Items[GiftCertificatesEnabled ? 0 : 1].Selected = true;
            currentCell.Controls.Add(rblGiftCertificatesEnabled);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get basic auth enabled
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Use Basic Auth:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Enhances security but requires additional server configuration.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblUseBasicAuth = new RadioButtonList();
            rblUseBasicAuth.ID = "Config_UseBasicAuth";
            rblUseBasicAuth.Items.Add(new ListItem("Enabled", "true"));
            rblUseBasicAuth.Items.Add(new ListItem("Disabled", "false"));
            rblUseBasicAuth.Items[UseBasicAuth ? 0 : 1].Selected = true;
            currentCell.Controls.Add(rblUseBasicAuth);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Tracking Enabled
            /*currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("User Tracking Enabled:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Indicate whether user tracking is enabled or not.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblTrackingEnabled = new RadioButtonList();
            rblTrackingEnabled.ID = "Config_TrackingEnabled";
            rblTrackingEnabled.Items.Add(new ListItem("Enabled", "true"));
            rblTrackingEnabled.Items.Add(new ListItem("Disabled", "false"));
            rblTrackingEnabled.Items[TrackingEnabled ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblTrackingEnabled);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);*/
            

            //get gateway mode
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Gateway Environment:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure to use Google Checkout in Sandbox or Production Environment.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGatewayMode = new RadioButtonList();
            rblGatewayMode.ID = "Config_UseTestMode";
            rblGatewayMode.Items.Add(new ListItem("Production Environment", "false"));
            rblGatewayMode.Items.Add(new ListItem("Sandbox Environment", "true"));
            rblGatewayMode.Items[UseTestMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblGatewayMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            currentCell.Controls.Add(new LiteralControl("<br />"));

            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication is recorded in store's \"logs\" folder.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblDebugMode = new RadioButtonList();
            rblDebugMode.ID = "Config_UseDebugMode";
            rblDebugMode.Items.Add(new ListItem("Off", "false"));
            rblDebugMode.Items.Add(new ListItem("On", "true"));
            rblDebugMode.Items[UseDebugMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblDebugMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //CREATE LITERAL CONTROL WITH HTML CONTENT
            parentControl.Controls.Add(configTable);
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Capture
                    | SupportedTransactions.PartialCapture
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Refund );
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("MerchantID")) MerchantID = ConfigurationData["MerchantID"];
            if (ConfigurationData.ContainsKey("MerchantKey")) MerchantKey = ConfigurationData["MerchantKey"];            
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("UseBasicAuth")) UseBasicAuth = AlwaysConvert.ToBool(ConfigurationData["UseBasicAuth"], false);
            if (ConfigurationData.ContainsKey("CouponsEnabled")) CouponsEnabled = AlwaysConvert.ToBool(ConfigurationData["CouponsEnabled"], true);
            if (ConfigurationData.ContainsKey("GiftCertificatesEnabled")) GiftCertificatesEnabled = AlwaysConvert.ToBool(ConfigurationData["GiftCertificatesEnabled"], true);
            if (ConfigurationData.ContainsKey("TrackingEnabled")) TrackingEnabled = AlwaysConvert.ToBool(ConfigurationData["TrackingEnabled"], true);
            if (ConfigurationData.ContainsKey("ExpirationMinutes")) ExpirationMinutes = AlwaysConvert.ToInt(ConfigurationData["ExpirationMinutes"], 0);
            if (ConfigurationData.ContainsKey("DefaultShipRate")) DefaultShipRate = AlwaysConvert.ToDecimal(ConfigurationData["DefaultShipRate"], 0);            
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            //New orders are already authorized in GoogleCheckout 
            //authorize should never be called from merchant admin. authorization updates take place 
            //on google checkout call-backs.
            throw new NotSupportedException("Direct Credit Card Authorization Requests are not supported.");
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            return AC.AcNotifier.ChargeOrder(this, captureRequest);            
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            return AC.AcNotifier.RefundOrder(this, creditRequest);            
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            //Cancel order is different from void. Void is not supported
            //return AC.AcNotifier.CancelOrder(this, voidRequest);
            throw new NotSupportedException("Voiding of authorizations is not supported.");
        }

        public static GoogleCheckout GetInstance()
        {
            int gwID = PaymentGatewayDataSource.GetPaymentGatewayIdByClassId(Utility.Misc.GetClassId(typeof(GoogleCheckout)));
            if (gwID == 0)
            {
                return null;
            }
            else
            {
                PaymentGateway gateway = PaymentGatewayDataSource.Load(gwID);
                GoogleCheckout _GatewayInstance = (GoogleCheckout)gateway.GetInstance();
                return _GatewayInstance;
            }
        }

        //--------------------------------------------
        // Additional GoogleCheckout sepcific methods
        //--------------------------------------------

        public void CancelOrder(string googleOrderNumber, string reason)
        {
            AC.AcNotifier.CancelOrder(this, googleOrderNumber, reason);
        }

        public void AddMerchantOrderNumber(string googleOrderNumber, string acOrderNumber)
        {
            AC.AcNotifier.AddMerchantOrderNumber(this, googleOrderNumber, acOrderNumber);
        }

        public void SendBuyerMessage(string googleOrderNumber, string message)
        {
            SendBuyerMessage(googleOrderNumber, message, false);
        }

        public void SendBuyerMessage(string googleOrderNumber, string message, bool sendEmail)
        {
            AC.AcNotifier.SendBuyerMessage(this, googleOrderNumber, message, sendEmail);
        }

        public void ArchiveOrder(string googleOrderNumber)
        {
            AC.AcNotifier.ArchiveOrder(this, googleOrderNumber);
        }

        public void UnarchiveOrder(string googleOrderNumber)
        {
            AC.AcNotifier.UnarchiveOrder(this, googleOrderNumber);
        }

        public void ProcessOrder(string googleOrderNumber)
        {
            AC.AcNotifier.ProcessOrder(this, googleOrderNumber);
        }

        public void DeliverOrder(string googleOrderNumber)
        {
            DeliverOrder(googleOrderNumber, null, null, true);
        }

        public void DeliverOrder(string googleOrderNumber, bool sendEmail)
        {
            DeliverOrder(googleOrderNumber, null, null, sendEmail);
        }

        public void DeliverOrder(string googleOrderNumber, string carrier, string trackingNo)
        {
            DeliverOrder(googleOrderNumber, carrier, trackingNo, true);
        }

        public void DeliverOrder(string googleOrderNumber, string carrier, string trackingNo, bool sendEmail)
        {
            AC.AcNotifier.DeliverOrder(this, googleOrderNumber, carrier, trackingNo, sendEmail);
        }

        public void DeliverOrder(string googleOrderNumber, TrackingNumber number)
        {
            DeliverOrder(googleOrderNumber, number, true);
        }

        public void DeliverOrder(string googleOrderNumber, TrackingNumber number, bool sendEmail)
        {
            string trNumber = number == null ? string.Empty : number.TrackingNumberData;
            AC.AcNotifier.DeliverOrder(this, googleOrderNumber, AC.AcHelper.GetGCCarrierName(number), trNumber, sendEmail);
        }

        public void AddTrackingData(string googleOrderNumber, string carrier, string trackingNo)
        {
            AC.AcNotifier.AddTrackingData(this, googleOrderNumber, carrier, trackingNo);
        }
        
    }
}
