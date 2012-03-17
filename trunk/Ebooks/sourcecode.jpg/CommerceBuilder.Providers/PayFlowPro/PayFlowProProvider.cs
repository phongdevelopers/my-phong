using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net;
using System.Collections.Specialized;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Payments;

namespace CommerceBuilder.Payments.Providers.PayFlowPro
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        public const string DEFAULT_LIVEURL = "https://payflowpro.paypal.com/transaction";
        public const string DEFAULT_TESTURL = "https://pilot-payflowpro.paypal.com/transaction";
        public const int DEFAULT_TIMEOUT = 45;
        public const int DEFAULT_PROXY_PORT = 443;

        string _Vendor = string.Empty;
        string _Partner = "";
        string _UserName = "";
        string _Password = "";
        bool _UseAuthCapture = false;
        bool _UseTestMode = true;
        int _Timeout = DEFAULT_TIMEOUT;
        string _LiveURL = DEFAULT_LIVEURL;
        string _TestURL = DEFAULT_TESTURL;
        string _CertPath = "";
        /*
        string _ProxyHost = "";
        int _ProxyPort = 80;
        string _ProxyUserName = "";
        string _ProxyPassword = "";
        */

        public string Vendor
        {
            get { return _Vendor; }
            set { _Vendor = value; }
        }
        public string Partner
        {
            get { return _Partner; }
            set { _Partner = value; }
        }
        public string UserName
        {
            get 
            {
                if (string.IsNullOrEmpty(_UserName)) return Vendor;                
                return _UserName;
            }
            set { _UserName = value; }
        }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        public string LiveURL
        {
            get { return _LiveURL; }
            set { _LiveURL = value; }
        }
        public string TestURL
        {
            get { return _TestURL; }
            set { _TestURL = value; }
        }
        public string CertPath
        {
            get { return _CertPath; }
            set { _CertPath = value; }
        }
        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }
        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        /*
        public string ProxyHost
        {
            get { return _ProxyHost; }
            set { _ProxyHost = value; }
        }

        public int ProxyPort
        {
            get { return _ProxyPort; }
            set { _ProxyPort = value; }
        }

        public string ProxyUserName
        {
            get { return _ProxyUserName; }
            set { _ProxyUserName = value; }
        }

        public string ProxyPassword
        {
            get { return _ProxyPassword; }
            set { _ProxyPassword = value; }
        }
        */

        public override string Name
        {
            get { return "PayPal PayFlow Pro"; }
        }

        public override string Description
        {
            get { return "PayPal PayFlow Pro, a market-leading payment gateway, connects your online store to your existing internet merchant account. It enables your business to accept credit cards, debit cards, more.  Plus accept PayPal too (U.S.only)"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.PayFlowPro.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "PayPal Payflow Pro HTTPS Service"; }
        }

        public override string ConfigReference
        {
            get { return "Partner : " + this.Partner + ", Vendor : " + this.Vendor; }
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
            gatewayLink.NavigateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_payflow-pro-overview-outside";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
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

            //get partner
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Partner:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Partner is usually related to your account provider (e.g. PayPal, VeriSign).</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtPartner = new TextBox();
            txtPartner.ID = "Config_Partner";
            txtPartner.Columns = 20;
            txtPartner.MaxLength = 100;
            txtPartner.Text = this.Partner;
            currentCell.Controls.Add(txtPartner);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get vendor (Merchant Login)
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Login:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Also known as \"vendor\", this is your primary account name..</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtVendor = new TextBox();
            txtVendor.ID = "Config_UserName";
            txtVendor.Columns = 20;
            txtVendor.MaxLength = 100;
            txtVendor.Text = this.Vendor;
            currentCell.Controls.Add(txtVendor);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get user name
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("User Name:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">If you have multiple users associated your PayFlow account or your user name is different from Merchant Login (vendor), enter the correct value here.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtUserName = new TextBox();
            txtUserName.ID = "Config_AccountUserName";
            txtUserName.Columns = 20;
            txtUserName.MaxLength = 100;
            txtUserName.Text = this.UserName;
            currentCell.Controls.Add(txtUserName);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get password
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Password:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Password for the merchant login (vendor) or user provided above.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtPassword = new TextBox();
            txtPassword.TextMode = TextBoxMode.Password;        
            txtPassword.ID = "Config_Password";
            txtPassword.Columns = 20;
            txtPassword.MaxLength = 100;
            txtPassword.Text = this.Password;
            txtPassword.Attributes["Value"] = this.Password;
            currentCell.Controls.Add(txtPassword);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get Charge Type
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Authorization Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Use \"Authorize\" to request authorization without capturing funds at the time of purchase. You can capture authorized transactions through the order admin interface. Use \"Authorize & Capture\" to capture funds immediately at the time of purchase.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblChargeType = new RadioButtonList();
            rblChargeType.ID = "Config_UseAuthCapture";
            rblChargeType.Items.Add(new ListItem("Authorize (recommended)", "false"));
            rblChargeType.Items.Add(new ListItem("Authorize & Capture", "true"));
            rblChargeType.Items[UseAuthCapture ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblChargeType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get gateway mode
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Gateway Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure to use the gateway in Live mode or Test mode.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGatewayMode = new RadioButtonList();
            rblGatewayMode.ID = "Config_UseTestMode";
            rblGatewayMode.Items.Add(new ListItem("Live Mode", "false"));
            rblGatewayMode.Items.Add(new ListItem("Test Mode", "true"));
            rblGatewayMode.Items[UseTestMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblGatewayMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get test url
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Test URL:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">URL for testing.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtTestURL = new TextBox();
            txtTestURL.ID = "Config_TestURL";
            txtTestURL.Columns = 50;
            txtTestURL.MaxLength = 280;
            txtTestURL.Text = this.TestURL;
            currentCell.Controls.Add(txtTestURL);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get live url
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Live URL:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">URL for live transactions.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtLiveURL = new TextBox();
            txtLiveURL.ID = "Config_LiveURL";
            txtLiveURL.Columns = 50;
            txtLiveURL.MaxLength = 280;
            txtLiveURL.Text = this.LiveURL;
            currentCell.Controls.Add(txtLiveURL);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            // get timeout
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Timeout:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Connection Timeout in seconds. 45 recomended.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtTimeout = new TextBox();
            txtTimeout.ID = "Config_Timeout";
            txtTimeout.Columns = 20;
            txtTimeout.MaxLength = 20;
            txtTimeout.Text = this.Timeout.ToString();
            currentCell.Controls.Add(txtTimeout);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);
            currentCell.Controls.Add(new LiteralControl("<br />"));

            /*
            //get proxy host
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Proxy Host:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Proxy Address if any.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtProxyHost = new TextBox();
            txtProxyHost.ID = "Config_ProxyHost";
            txtProxyHost.Columns = 50;
            txtProxyHost.MaxLength = 280;
            txtProxyHost.Text = this.ProxyHost;
            currentCell.Controls.Add(txtProxyHost);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get proxy port
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Proxy Port:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Proxy Port.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtProxyPort = new TextBox();
            txtProxyPort.ID = "Config_ProxyPort";
            txtProxyPort.Columns = 10;
            txtProxyPort.MaxLength = 10;
            txtProxyPort.Text = this.ProxyPort.ToString();
            currentCell.Controls.Add(txtProxyPort);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get proxy user name
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Proxy User Name:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">User name for proxy</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtProxyUserName = new TextBox();
            txtProxyUserName.ID = "Config_ProxyUserName";
            txtProxyUserName.Columns = 20;
            txtProxyUserName.MaxLength = 100;
            txtProxyUserName.Text = this.ProxyUserName;
            currentCell.Controls.Add(txtProxyUserName);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get proxy password
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Proxy Password:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Proxy Password.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtProxyPassword = new TextBox();
            txtProxyPassword.ID = "Config_ProxyPassword";
            txtProxyPassword.Columns = 20;
            txtProxyPassword.MaxLength = 100;
            txtProxyPassword.Text = this.ProxyPassword;
            currentCell.Controls.Add(txtProxyPassword);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);
            currentCell.Controls.Add(new LiteralControl("<br />"));

            //get certificate path
            /*currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Certificate Path:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Full Path of the certificate file.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtCertPath = new TextBox();
            txtCertPath.ID = "Config_CertPath";
            txtCertPath.Columns = 50;
            txtCertPath.MaxLength = 280;
            txtCertPath.Text = this.CertPath;
            currentCell.Controls.Add(txtCertPath);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);*/

            currentCell.Controls.Add(new LiteralControl("<br />"));
            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and PayflowPro is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.Refund
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Void
                    | SupportedTransactions.RecurringBilling);
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("Partner")) Partner = ConfigurationData["Partner"];
            if (ConfigurationData.ContainsKey("AccountUserName")) UserName = ConfigurationData["AccountUserName"];
            if (ConfigurationData.ContainsKey("UserName")) Vendor = ConfigurationData["UserName"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("Timeout")) Timeout = AlwaysConvert.ToInt(ConfigurationData["Timeout"], DEFAULT_TIMEOUT);
            if (ConfigurationData.ContainsKey("TestURL")) TestURL = ConfigurationData["TestURL"];
            if (ConfigurationData.ContainsKey("LiveURL")) LiveURL = ConfigurationData["LiveURL"];
            /*
            if (ConfigurationData.ContainsKey("ProxyHost")) ProxyHost = ConfigurationData["ProxyHost"];
            if (ConfigurationData.ContainsKey("ProxyPort")) ProxyPort = AlwaysConvert.ToInt(ConfigurationData["ProxyPort"], DEFAULT_PROXY_PORT);
            if (ConfigurationData.ContainsKey("ProxyUserName")) ProxyUserName = ConfigurationData["ProxyUserName"];
            if (ConfigurationData.ContainsKey("ProxyPassword")) ProxyPassword = ConfigurationData["ProxyPassword"];
            //if (ConfigurationData.ContainsKey("CertPath")) CertPath = ConfigurationData["CertPath"];
            */
            if (string.IsNullOrEmpty(TestURL)) TestURL = DEFAULT_TESTURL;
            if (string.IsNullOrEmpty(LiveURL)) LiveURL = DEFAULT_LIVEURL;
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            VerifyStatus();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");

            bool capture = this.UseAuthCapture || authorizeRequest.Capture;
            string requestDebug = string.Empty;
            string requestData = InitializeAuthRequest(payment, order, user, capture, ref requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData);
            if (!string.IsNullOrEmpty(responseData))
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                }

                return ProcessResponse(payment, responseData, capture ? TransactionType.AuthorizeCapture : TransactionType.Authorize);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            VerifyStatus();
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = captureRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            string requestDebug = "";
            string requestData = InitializeCaptureRequest(payment, authorizeTransaction, captureRequest.Amount, ref requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData);
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                return ProcessResponse(payment, responseData, captureRequest.TransactionType, captureRequest.Amount);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            VerifyStatus();
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction captureTransaction = creditRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            string requestDebug = "";
            string requestData = InitializeRefundRequest(payment, captureTransaction, creditRequest.Amount, ref requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData);
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                return ProcessResponse(payment, responseData, creditRequest.TransactionType, creditRequest.Amount);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            VerifyStatus();
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            string requestDebug = "";
            string requestData = InitializeVoidRequest(payment, authorizeTransaction, ref requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData);
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);

                return ProcessResponse(payment, responseData, TransactionType.Void);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        private string InitializeAuthRequest(Payment payment, Order order, User user, bool capture, ref string requestDebug)
        {
            VerifyPaymentInstrument(payment);
            Address address = user.PrimaryAddress;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            string accountInfo = BuildAccountInfo();
            string userInfo = BuildUserInfo(order, user, accountData);
            string paymentInfoDebug = "";
            string paymentInfo = BuildPaymentInfo(payment, order, payment.Amount, accountData, ref paymentInfoDebug);

            string requestString = accountInfo + "&" + userInfo + "&" + paymentInfo;

            if (this.UseDebugMode)
            {
                requestDebug = StringHelper.Replace(accountInfo, "&PWD="+this.Password, "&PWD=xxxx")
                + "&" + userInfo
                + "&" + paymentInfoDebug;
            }

            if (capture)
            {
                requestString = requestString + "&TRXTYPE=S";
                if (UseDebugMode)
                {
                    requestDebug = requestDebug + "&TRXTYPE=S";
                }
            }
            else
            {
                requestString = requestString + "&TRXTYPE=A";
                if (UseDebugMode)
                {
                    requestDebug = requestDebug + "&TRXTYPE=A";
                }
            }

            return requestString;
        }

        private string InitializeCaptureRequest(Payment payment, Transaction transaction, LSDecimal dAmount, ref string requestDebug)
        {
            VerifyPaymentInstrument(payment);
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            string accountInfo = BuildAccountInfo();
            string paymentInfoDebug = "";
            string paymentInfo = BuildPaymentInfo(payment, payment.Order, dAmount, accountData, ref paymentInfoDebug);

            string captureRequest = accountInfo + "&" + paymentInfo + "&TRXTYPE=D"
                + "&ORIGID=" + transaction.ProviderTransactionId;

            if (this.UseDebugMode)
            {
                requestDebug = StringHelper.Replace(accountInfo, "&PWD="+this.Password, "&PWD=xxxx")
                    + "&" + paymentInfoDebug + "&TRXTYPE=D"
                    + "&ORIGID=" + transaction.ProviderTransactionId;
            }

            return captureRequest;
        }

        private string InitializeRefundRequest(Payment payment, Transaction transaction, LSDecimal dAmount, ref string requestDebug)
        {
            VerifyPaymentInstrument(payment);
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountInfo = BuildAccountInfo();
            string paymentInfoDebug = "";
            string paymentInfo = BuildPaymentInfo(payment, payment.Order, dAmount, accountData, ref paymentInfoDebug);

            string refundRequest = accountInfo + "&" + paymentInfo + "&TRXTYPE=C"
                + "&ORIGID=" + transaction.ProviderTransactionId;

            if (this.UseDebugMode)
            {
                requestDebug = StringHelper.Replace(accountInfo, "&PWD=" + this.Password, "&PWD=xxxx") 
                    + "&" + paymentInfoDebug + "&TRXTYPE=C"
                    + "&ORIGID=" + transaction.ProviderTransactionId;
            }

            return refundRequest;
        }

        private string InitializeVoidRequest(Payment payment, Transaction transaction, ref string requestDebug)
        {
            VerifyPaymentInstrument(payment);
            string accountInfo = BuildAccountInfo();
            string voidRequest = "&TRXTYPE=V" + "&ORIGID=" + transaction.ProviderTransactionId;

            if (this.UseDebugMode)
            {
                requestDebug = StringHelper.Replace(accountInfo, "&PWD="+this.Password, "&PWD=xxxx")
                + voidRequest;
            }

            return accountInfo + voidRequest;
        }

        private string BuildAccountInfo()
        {
            return string.Format("PARTNER={0}&VENDOR={1}&USER={2}&PWD={3}&TENDER=C", Partner, Vendor, UserName, Password);
        }

        private string BuildUserInfo(Order order, User user, AccountDataDictionary accountData)
        {
            //Address address = user.PrimaryAddress;
            StringBuilder userInfo = new StringBuilder();
            string name, company, street, city, state, zip, country, email, phone, uid;
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                name = accountData["AccountName"];
            }
            else
            {
                name = order.BillToFirstName + " " + order.BillToLastName;
            }
            company = order.BillToCompany;
            //street = address.Address1 + ", " + address.Address2;
            street = order.BillToAddress1;
            city = order.BillToCity;
            state = order.BillToProvince;
            zip = order.BillToPostalCode;
            country = order.BillToCountryCode;
            email = order.BillToEmail;
            phone = order.BillToPhone; 
            uid = user.UserId.ToString();

            userInfo.Append(string.Format("NAME[{0}]={1}&COMPANYNAME[{2}]={3}&STREET[{4}]={5}", name.Length, name, company.Length, company, street.Length, street));
            userInfo.Append(string.Format("&CITY={0}&STATE={1}&ZIP={2}&COUNTRY={3}", city, state, zip, country));
            userInfo.Append(string.Format("&EMAIL={0}&PHONE={1}&CUSTCODE={2}", email, phone, uid));
            return userInfo.ToString();
        }

        private string BuildPaymentInfo(Payment payment, Order order, LSDecimal dAmount, AccountDataDictionary accountData, ref string paymentInfoDebug)
        {
            StringBuilder paymentInfo = new StringBuilder(); 
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            string expireDate = expirationMonth + expirationYear;
            string amount = String.Format("{0:F2}", dAmount);
            paymentInfo.Append(string.Format("ACCT={0}&EXPDATE={1}&AMT={2}", accountNumber, expireDate, amount));

            if (UseDebugMode)
            {
                string tempAccNumber = MakeReferenceNumber(accountNumber);
                paymentInfoDebug = string.Format("ACCT={0}&EXPDATE={1}&AMT={2}", tempAccNumber, expireDate, amount);
            }

            string securityCode = accountData.GetValue("SecurityCode");
            if (!string.IsNullOrEmpty(securityCode))
            {
                paymentInfo.Append("&CVV2=" + securityCode);
                if (UseDebugMode)
                {
                    paymentInfoDebug = paymentInfoDebug + "&CVV2=" + new string('x', securityCode.Length);
                }
            }

            paymentInfo.Append("&INVNUM=" + order.OrderNumber.ToString());
            if (UseDebugMode)
            {
                paymentInfoDebug = paymentInfoDebug + "&INVNUM=" + order.OrderNumber.ToString();
            }

            //TODO add description (COMMENT1)

            return paymentInfo.ToString();
        }

        private void VerifyStatus()
        {
            if (string.IsNullOrEmpty(this.Partner) ||
                string.IsNullOrEmpty(this.Vendor) ||
                string.IsNullOrEmpty(this.Password))
            {
                throw new InvalidOperationException("One of the required parameters is not set : Partner, Merchant Login, Password");
            }
        }

        private void VerifyPaymentInstrument(Payment payment)
        {
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                case PaymentInstrument.JCB:
                case PaymentInstrument.Maestro:
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }
        }

        private Transaction ProcessResponse(Payment payment, String responseData, TransactionType transactionType)
        {
            return ProcessResponse(payment, responseData, transactionType, payment.Amount);
        }

        private Transaction ProcessResponse(Payment payment, String responseData, TransactionType transactionType, LSDecimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;
            //PARSE THE URL ENCODED DATA
            NameValueCollection response = HttpUtility.ParseQueryString(responseData);
            int responseCode = AlwaysConvert.ToInt(response.Get("RESULT"), -9999);
            if (responseCode != 0)
            {
                //failed
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = response.Get("RESULT");
                transaction.ResponseMessage = response.Get("RESPMSG");
            }
            else
            {
                //successful
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = response.Get("PNREF");
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = transactionAmount;
                transaction.AuthorizationCode = response.Get("AUTHCODE");
                transaction.AVSResultCode = GetAVSCode(response.Get("AVSADDR"), response.Get("AVSZIP"), response.Get("IAVS"));
                transaction.CVVResultCode = response.Get("CVV2MATCH");
                if (string.IsNullOrEmpty(transaction.CVVResultCode)) transaction.CVVResultCode = "X";
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }
            return transaction;
        }

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("authorizeRequest.Payment");
            if (payment.Order == null) throw new ArgumentNullException("authorizeRequest.Payment.Order");
            User user = payment.Order.User;
            Order order = payment.Order; 
            if (user == null) throw new ArgumentNullException("authorizeRequest.Payment.Order.User");

            Transaction errTrans;

            //VALIDATE THE PAYMENT PERIOD
            string payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == string.Empty)
            {
                errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The specified payment interval is not valid for this processor.");
                return new AuthorizeRecurringTransactionResponse(errTrans);
            }

            //CREATE THE RESPONSE OBJECT
            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();

            //WE RUN AN INITIAL AUTH MANUALLY IF THE AMOUNT IS DIFFERENT FROM RECURRING CHARGE
            //OR WE RUN THE "OPTIONAL" INITIAL AMOUNT
            //EITHER WAY THE NUMBER OF REMAINING PAYMENTS IS REDUCED BY ONE
            int remainingPayments = authorizeRequest.NumberOfPayments - 1;
            //DETERMINE THE START DATE FOR THE FIRST AUTOMATIC PAYMENT
            DateTime startDate = GetNextPaymentDate(payPeriod);
            //SEE IF WE MUST RUN AN INITIAL AUTH FOR A DIFFERENT AMOUNT THAN THE RECURRING FEE
            LSDecimal recurringCharge = authorizeRequest.Amount;
            if (authorizeRequest.RecurringChargeSpecified)
            {
                if (authorizeRequest.Amount > 0)
                {
                    AuthorizeTransactionRequest authRequest = new AuthorizeTransactionRequest(authorizeRequest.Payment, authorizeRequest.RemoteIP);
                    authRequest.Capture = true;
                    authRequest.Amount = authorizeRequest.Amount;
                    Transaction initialAuthTx = DoAuthorize(authRequest);
                    if (initialAuthTx.TransactionStatus != TransactionStatus.Successful)
                    {
                        response.AddTransaction(initialAuthTx);
                        response.Status = TransactionStatus.Failed;
                        return response;
                    }
                    response.AddTransaction(initialAuthTx);
                }
                recurringCharge = authorizeRequest.RecurringCharge;
            }

            //KEEP TRACK OF SENSITIVE DATA THAT SHOULD NOT BE RECORDED
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //GENERATE REQUEST
            StringBuilder requestBuilder = new StringBuilder();
            requestBuilder.Append(string.Format("PARTNER={0}&USER={1}&PWD={2}&VENDOR={3}", Partner, UserName, Password, Vendor));
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            requestBuilder.Append("&TRXTYPE=R&TENDER=C&ACTION=A");
            requestBuilder.Append(string.Format("&PROFILENAME[{0}]={1}", authorizeRequest.SubscriptionName.Length, authorizeRequest.SubscriptionName));
            //BUILD THE ACCOUNT DATA
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            requestBuilder.Append(string.Format("&ACCT={0}&EXPDATE={1}{2}", accountNumber, expirationMonth, expirationYear));
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            string securityCode = accountData.GetValue("SecurityCode");
            if (!string.IsNullOrEmpty(securityCode))
            {
                requestBuilder.Append("&CVV2=" + securityCode);
                if (this.UseDebugMode) sensitiveData["CVV2=" + securityCode] = "CVV2=" + (new string('x', securityCode.Length));
            }
            //SET THE RECURRING CHARGE
            requestBuilder.Append("&AMT=" + recurringCharge.ToString("F2"));
            requestBuilder.Append("&START=" + startDate.ToString("MMddyyy"));
            requestBuilder.Append("&TERM=" + remainingPayments.ToString("F2"));
            requestBuilder.Append("&PAYPERIOD=" + payPeriod);
            requestBuilder.Append("&EMAIL=" + user.Email);
            if (!authorizeRequest.RecurringChargeSpecified)
            {
                requestBuilder.Append("&OPTIONALTRX=S&OPTIONALTRXAMT=" + authorizeRequest.Amount.ToString("F2"));
            }
            else if (authorizeRequest.Amount == 0)
            {
                //VALIDATE THE CARD INFORMATION IN CASE THE INITIAL CHARGE WAS 0
                requestBuilder.Append("&OPTIONALTRX=A");
            }

            string fullName;
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                fullName = accountData["AccountName"];
            }
            else
            {
                fullName = order.BillToFirstName + " " + order.BillToLastName;
            }

            requestBuilder.Append(string.Format("&NAME[{0}]={1}", fullName.Length, fullName));
            requestBuilder.Append(string.Format("&STREET[{0}]={1}", order.BillToAddress1.Length, order.BillToAddress1));
            requestBuilder.Append(string.Format("&ZIP[{0}]={1}", order.BillToPostalCode.Length, order.BillToPostalCode));
            if (!string.IsNullOrEmpty(order.BillToPhone)) requestBuilder.Append("&PHONE=" + order.BillToPhone); 
            string requestData = requestBuilder.ToString();

            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);

            //SEND THE REQUEST TO THE GATEWAY
            string responseData = SendRequestToGateway(requestData);

            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
            //VALIDATE RESPONSE
            if (responseData == null)
            {
                errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The Paypal server returned a null response.");
                return new AuthorizeRecurringTransactionResponse(errTrans);
            }
            //PROCESS RESPONSE
            NameValueCollection gatewayResponse = HttpUtility.ParseQueryString(responseData);
            int responseCode = AlwaysConvert.ToInt(gatewayResponse.Get("RESULT"), -9999);
            if (responseCode == 0)
            {
                //TRANSACTION SUCCEEDED
                Transaction transaction = new Transaction();
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.TransactionType = TransactionType.AuthorizeRecurring;
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = gatewayResponse.Get("RPREF");
                transaction.TransactionDate = LocaleHelper.LocalNow;
                transaction.Amount = recurringCharge;
                transaction.AuthorizationCode = gatewayResponse.Get("PROFILEID");
                transaction.AVSResultCode = GetAVSCode(gatewayResponse.Get("AVSADDR"), gatewayResponse.Get("AVSZIP"), gatewayResponse.Get("IAVS"));
                transaction.CVVResultCode = gatewayResponse.Get("CVV2MATCH");
                if (string.IsNullOrEmpty(transaction.CVVResultCode)) transaction.CVVResultCode = "X";
                response.AddTransaction(transaction);
                response.Status = transaction.TransactionStatus;
            }
            else
            {
                //TRANSACTION FAILED
                errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, responseCode.ToString(), gatewayResponse.Get("RESPMSG"));
                response.AddTransaction(errTrans);
                response.Status = errTrans.TransactionStatus;
            }
            return response;
        }

        private string SendRequestToGateway(string requestData)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.UseTestMode ? this.TestURL : this.LiveURL);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "text/namevalue";
            httpWebRequest.KeepAlive = false;
            //httpWebRequest.Connection = "close";
            httpWebRequest.Headers.Add("X-VPS-REQUEST-ID", Guid.NewGuid().ToString("N"));
            httpWebRequest.Headers.Add("X-VPS-VIT-CLIENT-CERTIFICATION-ID", "1E40BA63-C32A-4658-B434-5CFF5DC33F43");
            httpWebRequest.Headers.Add("X-VPS-CLIENT-TIMEOUT", this.Timeout.ToString());
            httpWebRequest.Headers.Add("X-VPS-VIT-INTEGRATION-PRODUCT", "AbleCommerce");
            httpWebRequest.Headers.Add("X-VPS-VIT-INTEGRATION-VERSION", "7.0");
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
            {
                response = responseStream.ReadToEnd();
                responseStream.Close();
            }
            return response;
        }

        private string GetAVSCode(string avsaddr, string avszip, string iavs)
        {
            if ((avsaddr == "X") && (avszip == "X")) return "U";
            return GetAVSCode((avsaddr == "Y"), (avszip == "Y"), (iavs == "Y"));
        }

        private string GetAVSCode(bool addressMatch, bool zipMatch, bool international)
        {
            if (!international)
            {
                //US DOMESTIC
                if (addressMatch)
                {
                    if (zipMatch) return "Y";
                    return "A";
                }
                else if (zipMatch)
                {
                    return "Z";
                }
                else
                {
                    return "N";
                }
            }
            else
            {
                //INTERNATIONAL
                if (addressMatch)
                {
                    if (zipMatch) return "M";
                    return "B";
                }
                else if (zipMatch)
                {
                    return "P";
                }
                else
                {
                    return "I";
                }
            }
        }

        private string GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 7) return "WEEK";
                    if (authorizeRequest.PaymentFrequency == 14) return "BIWK";
                    if (authorizeRequest.PaymentFrequency == 28) return "FRWK";
                    break;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return "MONT";
                    if (authorizeRequest.PaymentFrequency == 3) return "QTER";
                    if (authorizeRequest.PaymentFrequency == 6) return "SMYR";
                    if (authorizeRequest.PaymentFrequency == 12) return "YEAR";
                    break;
            }
            return string.Empty;
        }

        private DateTime GetNextPaymentDate(string payPeriod)
        {
            switch (payPeriod)
            {
                case "WEEK":
                    return LocaleHelper.LocalNow.AddDays(7);
                case "BIWK":
                    return LocaleHelper.LocalNow.AddDays(14);
                case "FRWK":
                    return LocaleHelper.LocalNow.AddDays(28);
                case "MONT":
                    return LocaleHelper.LocalNow.AddMonths(1);
                case "QTER":
                    return LocaleHelper.LocalNow.AddMonths(3);
                case "SMYR":
                    return LocaleHelper.LocalNow.AddMonths(6);
                case "YEAR":
                    return LocaleHelper.LocalNow.AddYears(1);
            }
            throw new ArgumentException("The specified payPeriod (" + payPeriod + ")is not valid for this processor.", "payPeriod");
        }
    }
}
