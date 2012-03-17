using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.AuthorizeNet
{
    public class AuthorizeNet : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        string _MerchantLogin;
        string _TransactionKey;
        bool _UseAuthCapture = false;
        GatewayModeOption _GatewayMode = GatewayModeOption.ProductionServerTestMode;
        bool _IsSecureSource;

        public string MerchantLogin
        {
            get { return _MerchantLogin; }
            set { _MerchantLogin = value; }
        }

        public string TransactionKey
        {
            get { return _TransactionKey; }
            set { _TransactionKey = value; }
        }

        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }

        public GatewayModeOption GatewayMode
        {
            get { return _GatewayMode; }
            set { _GatewayMode = value; }
        }

        public bool IsSecureSource
        {
            get { return _IsSecureSource; }
            set { _IsSecureSource = value; }
        }

        public override string Name
        {
            get { return "Authorize.Net"; }
        }

        public override string Description
        {
            get { return "Authorize.Net is the preferred payment gateway among resellers and merchants for managing payment transactions using the power and speed of the Internet. Authorize.Net also provides other business enhancing products that help merchants manage their transactions. For more information about Authorize.Net, see the <a href=\"http://www.authorizenet.com/company/aboutus\" target=\"_blank\">About Us</a> page."; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.AuthorizeNet.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "AIM 3.1"; }
        }

        private Boolean UseTestGateway
        {
            get
            {
                return (this.GatewayMode == GatewayModeOption.TestServerLiveMode || this.GatewayMode == GatewayModeOption.TestServerTestMode);
            }
        }

        private Boolean UseTestRequest
        {
            get
            {
                return (this.GatewayMode == GatewayModeOption.ProductionServerTestMode || this.GatewayMode == GatewayModeOption.TestServerTestMode);
            }
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize | SupportedTransactions.AuthorizeCapture | SupportedTransactions.Capture | SupportedTransactions.PartialRefund | SupportedTransactions.Refund | SupportedTransactions.Void | SupportedTransactions.RecurringBilling);
            }
        }

        public override bool RefundRequiresAccountData
        {
            get
            {
                return true;
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("MerchantLogin")) MerchantLogin = ConfigurationData["MerchantLogin"];
            if (ConfigurationData.ContainsKey("TransactionKey")) TransactionKey = ConfigurationData["TransactionKey"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("GatewayMode")) GatewayMode = (GatewayModeOption)AlwaysConvert.ToInt(ConfigurationData["GatewayMode"]);
            if (ConfigurationData.ContainsKey("IsSecureSource")) IsSecureSource = (ConfigurationData["IsSecureSource"] == "on");
        }

        public override void BuildConfigForm(Control parentControl)
        {
            //CREATE CONFIG TABLE
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
            gatewayLink.NavigateUrl = "http://www.authorizenet.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable Authorize.Net, you must provide your merchant login and transaction key.  If you do not already have a transaction key, it can be generated from the Authorize.Net merchant interface.</p>"));
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

            /*
            configHtml.Append("<tr>\r\n");
            configHtml.Append("<td align=\"right\" width=\"60%\">\r\n");
            configHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            configHtml.Append("<b>Gateway Version:</b>\r\n");
            configHtml.Append("</font>\r\n");
            configHtml.Append("</td>\r\n");
            configHtml.Append("<td width=\"50%\">\r\n");
            configHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            configHtml.Append(this.Version + "\r\n");
            configHtml.Append("</font>\r\n");
            configHtml.Append("</td>\r\n");
            configHtml.Append("</tr>\r\n");
            configHtml.Append("</tr>\r\n");
            */

            //GET THE LOGIN (#0) AND SECURESOURCE (#6)
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Login:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your Authorize.Net merchant login is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantLogin = new TextBox();
            txtMerchantLogin.ID = "Config_MerchantLogin";
            txtMerchantLogin.Columns = 20;
            txtMerchantLogin.MaxLength = 50;
            txtMerchantLogin.Text = this.MerchantLogin;
            currentCell.Controls.Add(txtMerchantLogin);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            CheckBox chkIsSecureSource = new CheckBox();
            chkIsSecureSource.ID = "Config_IsSecureSource";
            chkIsSecureSource.Checked = this.IsSecureSource;
            currentCell.Controls.Add(chkIsSecureSource);
            currentCell.Controls.Add(new LiteralControl("&nbsp;Check here if you are a Wells Fargo SecureSource merchant."));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE TRANSACTION KEY
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Transaction Key:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">The transaction key for your merchant account is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtTransactionKey = new TextBox();
            txtTransactionKey.ID = "Config_TransactionKey";
            txtTransactionKey.Columns = 20;
            txtTransactionKey.MaxLength = 50;
            txtTransactionKey.Text = this.TransactionKey;
            currentCell.Controls.Add(txtTransactionKey);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE AUTHORIZATION MODE
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
            RadioButtonList rblTransactionType = new RadioButtonList();
            rblTransactionType.ID = "Config_UseAuthCapture";
            rblTransactionType.Items.Add(new ListItem("Authorize (recommended)", "false"));
            rblTransactionType.Items.Add(new ListItem("Authorize & Capture", "true"));
            rblTransactionType.Items[UseAuthCapture ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblTransactionType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE GATEWAY MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Gateway Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure AbleCommerce to use either the production or test gateway with live or test transactions. Enabling test mode in the Authorize.Net merchant interface will override the live mode option set here.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGatewayMode = new RadioButtonList();
            rblGatewayMode.ID = "Config_GatewayMode";
            rblGatewayMode.Items.Add(new ListItem("Production Gateway, Live Mode", ((int)GatewayModeOption.ProductionServerLiveMode).ToString()));
            rblGatewayMode.Items.Add(new ListItem("Production Gateway, Test Mode", ((int)GatewayModeOption.ProductionServerTestMode).ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Gateway, Live Mode", ((int)GatewayModeOption.TestServerLiveMode).ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Gateway, Test Mode", ((int)GatewayModeOption.TestServerTestMode).ToString()));
            rblGatewayMode.Items[(int)this.GatewayMode].Selected = true;
            currentCell.Controls.Add(rblGatewayMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and Authorize.Net is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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

        public override string ConfigReference
        {
            get { return "Merchant Login: " + this.MerchantLogin; }
        }

        public void BuildDisplayForm(Control parentControl)
        {
            //CREATE CONFIG TABLE
            HtmlTable displayTable = new HtmlTable();
            displayTable.CellPadding = 4;
            displayTable.CellSpacing = 0;
            HtmlTableRow currentRow;
            HtmlTableCell currentCell;
            displayTable.Attributes.Add("Class", "InputForm");

            //ADD CAPTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("TH");
            currentCell.Attributes.Add("Class", "SectionHeader");
            currentCell.ColSpan = 2;
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = this.Name;
            gatewayLink.NavigateUrl = "http://www.authorizenet.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"Instruction\">To enable Authorize.Net, you must provide your merchant login and transaction key.  If you do not already have a transaction key, it can be generated from the Authorize.Net merchant interface.</p>"));
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //display detected version
            /*
            displayHtml.Append("<tr>\r\n");
            displayHtml.Append("<td align=\"right\" width=\"60%\">\r\n");
            displayHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            displayHtml.Append("<b>Assembly:</b>\r\n");
            displayHtml.Append("</font>\r\n");
            displayHtml.Append("</td>\r\n");
            displayHtml.Append("<td width=\"50%\">\r\n");
            displayHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            displayHtml.Append(this.GetType().Assembly.GetName().Name.ToString() + "&nbsp;(v" + this.GetType().Assembly.GetName().Version.ToString() + ")\r\n");
            displayHtml.Append("</font>\r\n");
            displayHtml.Append("</td>\r\n");
            displayHtml.Append("</tr>\r\n");
            displayHtml.Append("<tr>\r\n");
            displayHtml.Append("<td align=\"right\" width=\"60%\">\r\n");
            displayHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            displayHtml.Append("<b>Gateway Version:</b>\r\n");
            displayHtml.Append("</font>\r\n");
            displayHtml.Append("</td>\r\n");
            displayHtml.Append("<td width=\"50%\">\r\n");
            displayHtml.Append("<font size=\"2\" face=\"Arial, Helvetica, sans-serif\">\r\n");
            displayHtml.Append(this.Version + "\r\n");
            displayHtml.Append("</font>\r\n");
            displayHtml.Append("</td>\r\n");
            displayHtml.Append("</tr>\r\n");
            displayHtml.Append("</tr>\r\n");
            */

            //DISPLAY THE LOGIN (#0) AND SECURESOURCE (#6)
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Login:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl(this.MerchantLogin));
            if (this.IsSecureSource)
            {
                currentCell.Controls.Add(new LiteralControl("&nbsp;(Wells Fargo SecureSource Merchant)"));
            }
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //DISPLAY THE TRANSACTION KEY
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Transaction Key:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl(this.TransactionKey));
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //DISPLAY THE AUTHORIZATION MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Authorization Mode:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl(UseAuthCapture ? "Authorize & Capture" : "Authorize"));
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //DISPLAY THE GATEWAY MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Gateway Mode:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            switch (this.GatewayMode)
            {
                case GatewayModeOption.ProductionServerLiveMode:
                    currentCell.Controls.Add(new LiteralControl("Production Gateway, Live Mode"));
                    break;
                case GatewayModeOption.ProductionServerTestMode:
                    currentCell.Controls.Add(new LiteralControl("Production Gateway, Test Mode"));
                    break;
                case GatewayModeOption.TestServerLiveMode:
                    currentCell.Controls.Add(new LiteralControl("Test Gateway, Live Mode"));
                    break;
                case GatewayModeOption.TestServerTestMode:
                    currentCell.Controls.Add(new LiteralControl("Test Gateway, Test Mode"));
                    break;
            }
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //DISPLAY THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl(UseDebugMode ? "On" : "Off"));
            currentRow.Cells.Add(currentCell);
            displayTable.Rows.Add(currentRow);

            //CREATE LITERAL CONTROL WITH HTML CONTENT
            parentControl.Controls.Add(displayTable);
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest transactionRequest)
        {
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Authorize(transactionRequest, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("x_login={0}&x_tran_key={1}", this.MerchantLogin, this.TransactionKey);
                string debugCredentials = "x_login=xxxxxxxx&x_tran_key=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Authorize(transactionRequest, response);
        }

        private bool IsCheckPayment(Payment payment)
        {
            if (payment != null)
            {
                PaymentMethod m = payment.PaymentMethod;
                if (m != null) return (m.PaymentInstrument == PaymentInstrument.Check);
            }
            return false;
        }

        private Transaction ProcessGatewayResponse_Authorize(AuthorizeTransactionRequest request, string authorizeResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = (this.UseAuthCapture || IsCheckPayment(request.Payment)) ? TransactionType.AuthorizeCapture : request.TransactionType;
            //PARSE THE RESPONSE FROM ANET
            string[] responseValues = authorizeResponse.Split("|".ToCharArray());
            int responseLength = responseValues.Length;
            if (responseLength < 6)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = authorizeResponse;
            }
            else
            {
                transaction.ProviderTransactionId = responseValues[6];
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = request.Amount;
                bool successful = (AlwaysConvert.ToInt(responseValues[0]) == 1);
                transaction.TransactionStatus = (successful ? TransactionStatus.Successful : TransactionStatus.Failed);
                if (!successful)
                {
                    transaction.ResponseCode = responseValues[2];
                    transaction.ResponseMessage = responseValues[3];
                }
                transaction.AuthorizationCode = responseValues[4];
                transaction.AVSResultCode = responseValues[5];
                if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = responseValues[38];
                if (string.IsNullOrEmpty(transaction.CVVResultCode)) transaction.CVVResultCode = "X";
                transaction.RemoteIP = request.RemoteIP;
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Capture(CaptureTransactionRequest request, string captureResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Capture;
            //PARSE THE RESPONSE FROM ANET
            string[] responseValues = captureResponse.Split("|".ToCharArray());
            int responseLength = responseValues.Length;
            if (responseLength < 6)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = captureResponse;
            }
            else
            {
                transaction.ProviderTransactionId = responseValues[6];
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = request.Amount;
                bool successful = (AlwaysConvert.ToInt(responseValues[0]) == 1);
                if (successful) {
                    transaction.TransactionStatus = TransactionStatus.Successful;
                } else {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                }
                transaction.ResponseCode = responseValues[2];
                transaction.ResponseMessage = responseValues[3];
                transaction.AuthorizationCode = responseValues[4];
                transaction.AVSResultCode = responseValues[5];
                if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = responseValues[38];
                transaction.RemoteIP = request.RemoteIP;
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Void(Payment payment, string voidResponse, VoidTransactionRequest request)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            //PARSE THE RESPONSE FROM ANET
            string[] responseValues = voidResponse.Split("|".ToCharArray());
            int responseLength = responseValues.Length;
            if (responseLength < 6)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = voidResponse;
            }
            else
            {
                transaction.ProviderTransactionId = responseValues[6];
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = request.Amount;
                bool successful = (AlwaysConvert.ToInt(responseValues[0]) == 1);
                if (successful)
                {
                    transaction.TransactionStatus = TransactionStatus.Successful;
                }
                else
                {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                }
                transaction.ResponseCode = responseValues[2];
                transaction.ResponseMessage = responseValues[3];
                transaction.AuthorizationCode = responseValues[4];
                transaction.AVSResultCode = responseValues[5];
                if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = responseValues[38];
                transaction.RemoteIP = request.RemoteIP;
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Refund(Payment payment, string refundResponse, RefundTransactionRequest request)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Refund;
            //PARSE THE RESPONSE FROM ANET
            string[] responseValues = refundResponse.Split("|".ToCharArray());
            int responseLength = responseValues.Length;
            if (responseLength < 6)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = refundResponse;
            }
            else
            {
                transaction.ProviderTransactionId = responseValues[6];
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = request.Amount;
                bool successful = (AlwaysConvert.ToInt(responseValues[0]) == 1);
                if (successful)
                {
                    transaction.TransactionStatus = TransactionStatus.Successful;
                }
                else
                {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                }
                transaction.ResponseCode = responseValues[2];
                transaction.ResponseMessage = responseValues[3];
                transaction.AuthorizationCode = responseValues[4];
                transaction.AVSResultCode = responseValues[5];
                if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = responseValues[38];
                transaction.RemoteIP = request.RemoteIP;
            }
            return transaction;
        }

        private string BuildGatewayRequestPart_MerchantAccountInformation()
        {
            if (this.GatewayMode == GatewayModeOption.ProductionServerTestMode || this.GatewayMode == GatewayModeOption.TestServerTestMode)
            {
                return String.Format("x_login={0}&x_tran_key={1}&x_version=3.1&x_test_request=TRUE", this.MerchantLogin, this.TransactionKey);
            }
            return String.Format("x_login={0}&x_tran_key={1}&x_version=3.1", this.MerchantLogin, this.TransactionKey);
        }

        private string BuildGatewayRequestPart_GatewayResponseConfiguration()
        {
            return "&x_delim_data=TRUE&x_delim_char=|&x_encap_char=&x_relay_response=FALSE";
        }

        private string BuildGatewayRequestPart_CustomerDetails(Order order, AccountDataDictionary accountData)
        {
            StringBuilder customerDetails = new StringBuilder();
            //ADD CUSTOMER INFORMATION
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                string[] names = accountData["AccountName"].Split(" ".ToCharArray());
                customerDetails.Append("&x_first_name=" + HttpUtility.UrlEncode(names[0]));
                if (names.Length > 1)
                {
                    customerDetails.Append("&x_last_name=" + HttpUtility.UrlEncode(string.Join(" ", names, 1, names.Length - 1)));
                }
                else
                {
                    //no last name. what to do? send empty string or no field at all? TODO : check the API
                    customerDetails.Append("&x_last_name=" + string.Empty);
                }
            }
            else
            {
                customerDetails.Append("&x_first_name=" + HttpUtility.UrlEncode(order.BillToFirstName));
                customerDetails.Append("&x_last_name=" + HttpUtility.UrlEncode(order.BillToLastName));
            }
            customerDetails.Append("&x_company=" + HttpUtility.UrlEncode(order.BillToCompany));
            customerDetails.Append("&x_address=" + HttpUtility.UrlEncode(order.BillToAddress1));
            customerDetails.Append("&x_city=" + HttpUtility.UrlEncode(order.BillToCity));
            customerDetails.Append("&x_state=" + HttpUtility.UrlEncode(order.BillToProvince));
            customerDetails.Append("&x_zip=" + HttpUtility.UrlEncode(order.BillToPostalCode));
            customerDetails.Append("&x_country=" + HttpUtility.UrlEncode(order.BillToCountryCode));
            customerDetails.Append("&x_phone=" + HttpUtility.UrlEncode(order.BillToPhone));
            customerDetails.Append("&x_fax=" + HttpUtility.UrlEncode(order.BillToFax));
            //APPEND ADDITIONAL CUSTOMER DATA
            if (order.UserId != 0) customerDetails.Append("&x_cust_id=" + order.UserId.ToString());
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                customerDetails.Append("&x_customer_ip=" + context.Request.ServerVariables["REMOTE_HOST"]);
            }
            //TODO: x_customer_tax_id
            customerDetails.Append("&x_email=" + HttpUtility.UrlEncode(order.BillToEmail));
            //TODO: EMAIL_CUSTOMER AND MERCHANT_EMAIL
            return customerDetails.ToString();
        }

        private string BuildGatewayRequestPart_InvoiceInformation(Order order)
        {
            //APPEND INVOICE INFORMATION
            StringBuilder invoiceDetails = new StringBuilder();
            invoiceDetails.Append("&x_invoice_num=" + order.OrderNumber);
            invoiceDetails.Append("&x_description=" + HttpUtility.UrlEncode(StringHelper.RemoveSpecialChars(order.Store.Name) + " Order #" + order.OrderNumber));
            //TODO: ITEMIZED ORDER INFORMATION?
            return invoiceDetails.ToString();
        }

        private string BuildGatewayRequestPart_CustomerShippingAddress(OrderShipment shipment)
        {
            StringBuilder shippingAddress = new StringBuilder();
            shippingAddress.Append("&x_ship_to_first_name=" + HttpUtility.UrlEncode(shipment.ShipToFirstName));
            shippingAddress.Append("&x_ship_to_last_name=" + HttpUtility.UrlEncode(shipment.ShipToLastName));
            shippingAddress.Append("&x_ship_to_company=" + HttpUtility.UrlEncode(shipment.ShipToCompany));
            shippingAddress.Append("&x_ship_to_address=" + HttpUtility.UrlEncode(shipment.ShipToAddress1));
            shippingAddress.Append("&x_ship_to_city=" + HttpUtility.UrlEncode(shipment.ShipToCity));
            shippingAddress.Append("&x_ship_to_state=" + HttpUtility.UrlEncode(shipment.ShipToProvince));
            shippingAddress.Append("&x_ship_to_zip=" + HttpUtility.UrlEncode(shipment.ShipToPostalCode));
            shippingAddress.Append("&x_ship_to_country=" + HttpUtility.UrlEncode(shipment.ShipToCountryCode));
            return shippingAddress.ToString();
        }

        private string BuildGatewayRequestPart_AuthorizeTransactionData(AuthorizeTransactionRequest authorizeRequest, AccountDataDictionary accountData, Dictionary<string, string> sensitiveData)
        {
            StringBuilder transactionData = new StringBuilder();
            //APPEND AMOUNT
            transactionData.Append(String.Format("&x_Amount={0:F2}", authorizeRequest.Amount));
            //IF CURRENCY CODE IS PASSED, SET IT
            //OTHERWISE LEAVE BLANK TO USE THE DEFAULT IN ANET MERCHANT SETTINGS
            if (!string.IsNullOrEmpty(authorizeRequest.CurrencyCode))
            {
                transactionData.Append("&x_currency_code=" + authorizeRequest.CurrencyCode);
            }

            //DETERMINE METHOD AND TYPE
            Payment payment = authorizeRequest.Payment;
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    transactionData.Append("&x_method=CC");
                    bool capture = (this.UseAuthCapture || authorizeRequest.Capture);
                    transactionData.Append(capture ? "&x_type=AUTH_CAPTURE" : "&x_type=AUTH_ONLY");
                    break;
                case PaymentInstrument.Check:
                    transactionData.Append("&x_method=ECHECK&x_echeck_type=WEB&x_recurring_billing=NO");
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }

            //APPEND PAYMENT INSTRUMENT DETAILS
            //AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            if (instrument != PaymentInstrument.Check)
            {
                string accountNumber = accountData.GetValue("AccountNumber");
                transactionData.Append("&x_card_num=" + accountNumber);
                if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
                string expirationMonth = accountData.GetValue("ExpirationMonth");
                if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
                string expirationYear = accountData.GetValue("ExpirationYear");
                transactionData.Append("&x_exp_date=" + System.Web.HttpUtility.UrlEncode(expirationMonth + "/" + expirationYear));
                //PROCESS CREDIT CARD ACCOUNT DATA
                string securityCode = accountData.GetValue("SecurityCode");
                if (!string.IsNullOrEmpty(securityCode))
                {
                    transactionData.Append("&x_card_code=" + securityCode);
                    if (this.UseDebugMode) sensitiveData["x_card_code=" + securityCode] = "x_card_code=" + (new string('x', securityCode.Length));
                }
            }
            else
            {
                //PROCESS CHECK ACCOUNT DATA
                string routingNumber = HttpUtility.UrlEncode(accountData.GetValue("RoutingNumber"));
                transactionData.Append("&x_bank_aba_code=" + routingNumber);
                string accountNumber = HttpUtility.UrlEncode(accountData.GetValue("AccountNumber"));
                transactionData.Append("&x_bank_acct_num=" + accountNumber);
                if (this.UseDebugMode)
                {
                    //need to replace routing number with truncated version
                    sensitiveData[routingNumber] = MakeReferenceNumber(routingNumber);
                    sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
                }
                string accountType = accountData.GetValue("AccountType");
                if (string.IsNullOrEmpty(accountType)) accountType = "CHECKING";
                transactionData.Append("&x_bank_acct_type=" + HttpUtility.UrlEncode(accountType));
                transactionData.Append("&x_bank_name=" + HttpUtility.UrlEncode(accountData.GetValue("BankName")));
                transactionData.Append("&x_bank_acct_name=" + HttpUtility.UrlEncode(accountData.GetValue("AccountHolder")));
                transactionData.Append("&x_echeck_type=WEB");

                //APPEND WELLS FARGO SECURE SOURCE DETAILS
                //append org type for securesource transactions
                string customerType = accountData.GetValue("CustomerType");
                if (string.IsNullOrEmpty(customerType) || (!customerType.Equals("I") && !customerType.Equals("B"))) customerType = "I";
                transactionData.Append("&x_customer_organization_type=" + customerType);
                //look for drivers license data
                if (accountData.ContainsKey("LicenseNumber") && accountData.ContainsKey("LicenseState") && accountData.ContainsKey("BirthDate"))
                {
                    string licenseNumber = HttpUtility.UrlEncode(accountData.GetValue("LicenseNumber"));
                    string licenseState = HttpUtility.UrlEncode(accountData.GetValue("LicenseState"));
                    DateTime birthDate = DateTime.Parse(accountData.GetValue("BirthDate"));
                    transactionData.Append("&x_drivers_license_num=" + licenseNumber);
                    transactionData.Append("&x_drivers_license_state=" + licenseState);
                    //dob must be in the standard AbleCommerce date format of yyyy-MM-dd
                    transactionData.Append("&x_drivers_license_dob=" + HttpUtility.UrlEncode(birthDate.ToString("yyyy/MM/dd")));
                    if (this.UseDebugMode)
                    {
                        //need to replace license number with truncated version
                        sensitiveData[licenseNumber] = MakeReferenceNumber(licenseNumber);
                        sensitiveData[HttpUtility.UrlEncode(birthDate.ToString("yyyy/MM/dd"))] = "yyyy/mm/dd";
                    }
                }
            }
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_CaptureTransactionData(CaptureTransactionRequest captureRequest)
        {
            StringBuilder transactionData = new StringBuilder();
            //APPEND AMOUNT
            transactionData.Append(String.Format("&x_Amount={0:F2}", captureRequest.Amount));
            //IF CURRENCY CODE IS PASSED, SET IT
            //OTHERWISE LEAVE BLANK TO USE THE DEFAULT IN ANET MERCHANT SETTINGS
            if (!string.IsNullOrEmpty(captureRequest.CurrencyCode))
            {
                transactionData.Append("&x_currency_code=" + captureRequest.CurrencyCode);
            }

            //DETERMINE METHOD AND TYPE
            Payment payment = captureRequest.Payment;
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    Transaction authorizeTransaction = captureRequest.AuthorizeTransaction;
                    transactionData.Append("&x_method=CC&x_type=PRIOR_AUTH_CAPTURE&x_trans_id=" + HttpUtility.UrlEncode(authorizeTransaction.ProviderTransactionId));
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_VoidTransactionData(VoidTransactionRequest voidRequest)
        {
            StringBuilder transactionData = new StringBuilder();
            //APPEND AMOUNT
            transactionData.Append(String.Format("&x_Amount={0:F2}", voidRequest.Amount));
            //IF CURRENCY CODE IS PASSED, SET IT
            //OTHERWISE LEAVE BLANK TO USE THE DEFAULT IN ANET MERCHANT SETTINGS
            if (!string.IsNullOrEmpty(voidRequest.CurrencyCode))
            {
                transactionData.Append("&x_currency_code=" + voidRequest.CurrencyCode);
            }

            //DETERMINE METHOD AND TYPE
            Payment payment = voidRequest.Payment;
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    transactionData.Append("&x_method=CC&x_type=VOID&x_trans_id=" + HttpUtility.UrlEncode(voidRequest.AuthorizeTransaction.ProviderTransactionId));
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_RefundTransactionData(RefundTransactionRequest refundRequest, Dictionary<string,string> sensitiveData)
        {
            StringBuilder transactionData = new StringBuilder();
            //APPEND AMOUNT
            transactionData.Append(String.Format("&x_Amount={0:F2}", refundRequest.Amount));
            //IF CURRENCY CODE IS PASSED, SET IT
            //OTHERWISE LEAVE BLANK TO USE THE DEFAULT IN ANET MERCHANT SETTINGS
            if (!string.IsNullOrEmpty(refundRequest.CurrencyCode))
            {
                transactionData.Append("&x_currency_code=" + refundRequest.CurrencyCode);
            }

            //DETERMINE METHOD AND TYPE
            Payment payment = refundRequest.Payment;
            PaymentMethod method = payment.PaymentMethod;
            PaymentInstrument instrument = (method != null) ? method.PaymentInstrument : PaymentInstrument.Unknown;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    transactionData.Append("&x_method=CC&x_type=CREDIT&x_trans_id=" + HttpUtility.UrlEncode(refundRequest.CaptureTransaction.ProviderTransactionId));
                    if (!string.IsNullOrEmpty(refundRequest.CardNumber))
                    {
                        transactionData.Append("&x_card_num=" + refundRequest.CardNumber);
                        if (this.UseDebugMode) sensitiveData[refundRequest.CardNumber] = MakeReferenceNumber(refundRequest.CardNumber);
                    }
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_Level2Data(Order order)
        {
            if (order != null)
            {
                decimal taxAmount = (decimal)order.Items.TotalPrice(OrderItemType.Tax);
                decimal shippingAmount = (decimal)order.Items.TotalPrice(OrderItemType.Shipping, OrderItemType.Handling);
                return "&x_tax=" + taxAmount.ToString("F2") + "&x_freight=" + shippingAmount.ToString("F2");
            }
            return string.Empty;
        }

        private string BuildGatewayRequest_Authorize(AuthorizeTransactionRequest request, Dictionary<string, string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = request.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            //GENERATE REQUEST
            StringBuilder sb = new StringBuilder();
            sb.Append(BuildGatewayRequestPart_MerchantAccountInformation());
            sb.Append(BuildGatewayRequestPart_GatewayResponseConfiguration());
            sb.Append(BuildGatewayRequestPart_CustomerDetails(order,accountData));
            sb.Append(BuildGatewayRequestPart_InvoiceInformation(order));
            //IF ONLY ONE SHIPMENT IN ORDER, APPEND CUSTOMER SHIPPING ADDRESS
            if (order.Shipments.Count == 1)
            {
                sb.Append(BuildGatewayRequestPart_CustomerShippingAddress(order.Shipments[0]));
            }
            sb.Append(BuildGatewayRequestPart_AuthorizeTransactionData(request, accountData, sensitiveData));
            sb.Append(BuildGatewayRequestPart_Level2Data(order));
            return sb.ToString();
        }

        private string BuildGatewayRequest_Capture(CaptureTransactionRequest request)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = request.Payment;
            if (payment == null) throw new ArgumentNullException("transactionRequest.Payment");
            Transaction authorizeTransaction = request.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");
            //GENERATE REQUEST
            StringBuilder sb = new StringBuilder();
            sb.Append(BuildGatewayRequestPart_MerchantAccountInformation());
            sb.Append(BuildGatewayRequestPart_GatewayResponseConfiguration());
            sb.Append(BuildGatewayRequestPart_CaptureTransactionData(request));
            sb.Append(BuildGatewayRequestPart_Level2Data(payment.Order));
            return sb.ToString();
        }

        private string BuildGatewayRequest_Void(VoidTransactionRequest voidRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("voidRequest.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("voidRequest.AuthorizeTransaction");
            //GENERATE REQUEST
            StringBuilder request = new StringBuilder();
            request.Append(BuildGatewayRequestPart_MerchantAccountInformation());
            request.Append(BuildGatewayRequestPart_GatewayResponseConfiguration());
            request.Append(BuildGatewayRequestPart_VoidTransactionData(voidRequest));
            return request.ToString();
        }

        private string BuildGatewayRequest_Refund(RefundTransactionRequest refundRequest, Dictionary<string,string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = refundRequest.Payment;
            if (payment == null) throw new ArgumentNullException("refundRequest.Payment");
            Transaction captureTransaction = refundRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("refundRequest.AuthorizeTransaction");
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            //GENERATE REQUEST
            StringBuilder request = new StringBuilder();
            request.Append(BuildGatewayRequestPart_MerchantAccountInformation());
            request.Append(BuildGatewayRequestPart_GatewayResponseConfiguration());
            Order order = payment.Order;
            request.Append(BuildGatewayRequestPart_InvoiceInformation(order));
            request.Append(BuildGatewayRequestPart_CustomerDetails(order,accountData));
            request.Append(BuildGatewayRequestPart_RefundTransactionData(refundRequest, sensitiveData));
            return request.ToString();
        }

        private string SendRequestToGateway(string requestData)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.UseTestGateway ? "https://test.authorize.net/gateway/transact.dll" : "https://secure.authorize.net/gateway/transact.dll");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }
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

        private XmlDocument SendXmlRequestToGateway(XmlDocument requestData)
        {
            string responseData;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.UseTestGateway ? "https://apitest.authorize.net/xml/v1/request.api" : "https://api.authorize.net/xml/v1/request.api");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "text/xml";
            httpWebRequest.KeepAlive = true;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData.OuterXml);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            using (StreamReader responseDataStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
            {
                responseData = responseDataStream.ReadToEnd();
                responseDataStream.Close();
            }
            XmlDocument response = new XmlDocument();
            response.LoadXml(responseData);
            return response;
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Capture(captureRequest);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("x_login={0}&x_tran_key={1}", this.MerchantLogin, this.TransactionKey);
                string debugCredentials = "x_login=xxxxxxxx&x_tran_key=xxxxxxxx";
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest.Replace(credentials, debugCredentials), null);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Capture(captureRequest, response);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Void(voidRequest);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("x_login={0}&x_tran_key={1}", this.MerchantLogin, this.TransactionKey);
                string debugCredentials = "x_login=xxxxxxxx&x_tran_key=xxxxxxxx";
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest.Replace(credentials, debugCredentials), null);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Void(voidRequest.Payment, response, voidRequest);
        }

        public override Transaction DoRefund(RefundTransactionRequest refundRequest)
        {
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Refund(refundRequest, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("x_login={0}&x_tran_key={1}", this.MerchantLogin, this.TransactionKey);
                string debugCredentials = "x_login=xxxxxxxx&x_tran_key=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Refund(refundRequest.Payment, response, refundRequest);
        }

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            Transaction errTrans;
            //KEEP TRACK OF SENSITIVE DATA THAT SHOULD NOT BE RECORDED
            Dictionary<string, string> debugReplacements = new Dictionary<string, string>();
            //GENERATE REQUEST
            XmlDocument arbRequest = new XmlDocument();
            arbRequest.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><ARBCreateSubscriptionRequest />");
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "merchantAuthentication/name", this.MerchantLogin);
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "merchantAuthentication/transactionKey", this.TransactionKey);
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/name", authorizeRequest.SubscriptionName);
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/interval/length", authorizeRequest.PaymentFrequency.ToString());
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case PaymentFrequencyUnit.Day:
                    XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/interval/unit", "days");
                    break;
                case PaymentFrequencyUnit.Month:
                    XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/interval/unit", "months");
                    break;
                default:
                    errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "U", "Unsupported payment frequency unit: " + authorizeRequest.PaymentFrequencyUnit.ToString());
                    return new AuthorizeRecurringTransactionResponse(errTrans);                    
                    
            }
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/startDate", LocaleHelper.LocalNow.ToString("yyyy-MM-dd"));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/totalOccurrences", authorizeRequest.NumberOfPayments.ToString());
            //DETERMINE IF THERE IS A DIFFERENT INITIAL AND RECURRING AMOUNT
            if (authorizeRequest.RecurringChargeSpecified)
            {
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/paymentSchedule/trialOccurrences", "1");
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/amount", authorizeRequest.RecurringCharge.ToString("F2"));
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/trialAmount", authorizeRequest.Amount.ToString("F2"));
            }
            else
            {
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/amount", authorizeRequest.Amount.ToString("F2"));
            }
            //DETERMINE METHOD AND TYPE
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    string accountNumber = accountData.GetValue("AccountNumber");
                    XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/payment/creditCard/cardNumber", accountNumber);
                    if (this.UseDebugMode) debugReplacements.Add(accountNumber, MakeReferenceNumber(accountNumber));
                    string expirationMonth = accountData.GetValue("ExpirationMonth");
                    if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
                    string expirationYear = accountData.GetValue("ExpirationYear");
                    if (expirationYear.Length == 2) expirationYear = "20" + expirationYear;
                    XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/payment/creditCard/expirationDate", expirationYear + "-" + expirationMonth);
                    break;
                case PaymentInstrument.Check:
                    errTrans =  Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "Check is not yet implemented!");
                    return new AuthorizeRecurringTransactionResponse(errTrans);
                /*
                //PROCESS CHECK ACCOUNT DATA
                string routingNumber = HttpUtility.UrlEncode(accountData.GetValue("RoutingNumber"));
                transactionData.Append("&x_bank_aba_code=" + routingNumber);
                string accountNumber = HttpUtility.UrlEncode(accountData.GetValue("AccountNumber"));
                transactionData.Append("&x_bank_acct_num=" + accountNumber);
                if (this.UseDebugMode)
                {
                //need to replace routing number with truncated version
                debugReplacements.Add(routingNumber + "|" + MakeReferenceNumber(routingNumber));
                debugReplacements.Add(accountNumber + "|" + MakeReferenceNumber(accountNumber));
                }
                string accountType = accountData.GetValue("AccountType");
                if (string.IsNullOrEmpty(accountType)) accountType = "CHECKING";
                transactionData.Append("&x_bank_acct_type=" + HttpUtility.UrlEncode(accountType));
                transactionData.Append("&x_bank_name=" + HttpUtility.UrlEncode(accountData.GetValue("BankName")));
                transactionData.Append("&x_bank_acct_name=" + HttpUtility.UrlEncode(accountData.GetValue("AccountName")));
                transactionData.Append("&x_echeck_type=WEB");
                */
                default:
                    errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The requested payment instrument is not supported: " + instrument.ToString());
                    return new AuthorizeRecurringTransactionResponse(errTrans);
            }

            //COMBINE ORDER AND PAYMENT ID TO PREVENT DUPLICATE ERRORS
            //WHEN MORE THAN ONE SUBSCRIPTION EXISTS FOR A SINGLE ORDER
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/order/invoiceNumber", string.Format("{0}:{1}", order.OrderNumber, payment.SubscriptionId));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/order/description", string.Format("Order #{0}, Sub #{1}", order.OrderNumber, payment.SubscriptionId));

            //CUSTOMER TYPE SHOULD BE 'I' FOR INDIVIDUAL OR 'B' FOR BUSINESS
            string customerType = accountData.GetValue("CustomerType");
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/type", ((customerType == "B") ? "business" : "individual"));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/id", order.UserId.ToString());
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/email", order.BillToEmail);
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/phoneNumber", order.BillToPhone);
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/faxNumber", order.BillToFax, true, false);
            //look for drivers license data
            if (accountData.ContainsKey("LicenseNumber") && accountData.ContainsKey("LicenseState") && accountData.ContainsKey("BirthDate"))
            {
                string licenseNumber = HttpUtility.UrlEncode(accountData.GetValue("LicenseNumber"));
                string licenseState = HttpUtility.UrlEncode(accountData.GetValue("LicenseState"));
                DateTime birthDate = DateTime.Parse(accountData.GetValue("BirthDate"));
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/driversLicense/number", licenseNumber);
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/driversLicense/state", licenseState);
                XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/customer/driversLicense/dateOfBirth", birthDate.ToString("yyyy-MM-dd"));
                if (this.UseDebugMode)
                {
                    //need to replace license number with truncated version
                    debugReplacements.Add(licenseNumber, MakeReferenceNumber(licenseNumber));
                }
            }
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/firstName", StringHelper.Truncate(order.BillToFirstName, 50));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/lastName", StringHelper.Truncate(order.BillToLastName, 50));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/company", StringHelper.Truncate(order.BillToCompany, 50));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/address", StringHelper.Truncate(order.BillToAddress1, 60));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/city", StringHelper.Truncate(order.BillToCity, 40));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/state", StringHelper.Truncate(order.BillToProvince, 40));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/zip", StringHelper.Truncate(order.BillToPostalCode, 20));
            XmlUtility.SetElementValue(arbRequest.DocumentElement, "subscription/billTo/country", order.BillToCountryCode);
            string requestDocument = arbRequest.OuterXml;
            //INSERT THE NAMESPACE FOR THE DOCUMENT ELEMENT
            requestDocument = requestDocument.Replace("<ARBCreateSubscriptionRequest", "<ARBCreateSubscriptionRequest xmlns=\"AnetApi/xml/v1/schema/AnetApiSchema.xsd\"");
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("x_login={0}&x_tran_key={1}", this.MerchantLogin, this.TransactionKey);
                string debugCredentials = "x_login=xxxxxxxx&x_tran_key=xxxxxxxx";
                debugReplacements[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDocument, debugReplacements);
            }
            //SEND REQUEST
            arbRequest = new XmlDocument();
            arbRequest.LoadXml(requestDocument);
            XmlDocument arbResponse = this.SendXmlRequestToGateway(arbRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, arbResponse.OuterXml, null);
            //PROCESS RESPONSE AND RETURN RESULT
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(arbResponse.NameTable);
            nsmgr.AddNamespace("ns1", "AnetApi/xml/v1/schema/AnetApiSchema.xsd");
            string responseCode = XmlUtility.GetElementValue(arbResponse.DocumentElement, "ns1:messages/ns1:message/ns1:code", nsmgr);
            string responseMessage = XmlUtility.GetElementValue(arbResponse.DocumentElement, "ns1:messages/ns1:message/ns1:text", nsmgr);
            if (XmlUtility.GetElementValue(arbResponse.DocumentElement, "ns1:messages/ns1:resultCode", nsmgr).ToLowerInvariant() == "ok")
            {
                //SUCCESS RESPONSE
                Transaction transaction = new Transaction();
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.TransactionType = TransactionType.AuthorizeRecurring;
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.Amount = authorizeRequest.Amount;
                transaction.RemoteIP = authorizeRequest.RemoteIP;
                transaction.ResponseCode = responseCode;
                transaction.ResponseMessage = responseMessage;
                transaction.TransactionDate = LocaleHelper.LocalNow;
                transaction.AuthorizationCode = XmlUtility.GetElementValue(arbResponse.DocumentElement, "ns1:subscriptionId", nsmgr);
                return new AuthorizeRecurringTransactionResponse(transaction);
            }
            //ERROR RESPONSE
            errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, TransactionType.AuthorizeRecurring, authorizeRequest.Amount, responseCode, responseMessage, authorizeRequest.RemoteIP);
            return new AuthorizeRecurringTransactionResponse(errTrans);
        }

        public enum GatewayModeOption : int
        {
            ProductionServerLiveMode = 0, ProductionServerTestMode, TestServerLiveMode, TestServerTestMode
        }
    }
}
