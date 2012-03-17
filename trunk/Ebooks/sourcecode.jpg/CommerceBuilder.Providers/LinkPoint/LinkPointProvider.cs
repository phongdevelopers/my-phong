using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using LinkPointTransaction;

namespace CommerceBuilder.Payments.Providers.LinkPoint
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        private const string LIVEHOST = "secure.linkpt.net";
        private const string TESTHOST = "staging.linkpt.net";

        string _MerchantId = string.Empty;
        bool _IsTestAccount = false;
        bool _UseAuthCapture = false;
        GatewayModeType _GatewayMode = GatewayModeType.LIVE;

        public override string Name
        {
            get { return "LinkPoint"; }
        }

        public override string Description
        {
            get { return "LinkPoint Description here"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.LinkPoint.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "LinkPoint API v3.5"; }
        }

        public string MerchantId
        {
            get { return _MerchantId; }
            set { _MerchantId = value; }
        }

        public bool IsTestAccount
        {
            get { return _IsTestAccount; }
            set { _IsTestAccount = value; }
        }

        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }

        public GatewayModeType GatewayMode
        {
            get { return _GatewayMode; }
            set { _GatewayMode = value; }
        }

        public string CertificateFile
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\" + this.MerchantId + ".PEM");
            }
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.Refund
                    | SupportedTransactions.RecurringBilling);
            }
        }

        public override void BuildConfigForm(System.Web.UI.Control parentControl)
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
            gatewayLink.NavigateUrl = "http://linkpoint.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable LinkPoint, your PEM certificate file must be saved to the \"App_Data\" folder of your website.  The name of the file must be your store number followed by .PEM, for example \"123456.PEM\".</p>"));
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

            //GET THE STORE NAME
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Id:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Provide your 6 to 10 digit store number.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantId = new TextBox();
            txtMerchantId.ID = "Config_MerchantId";
            txtMerchantId.Columns = 20;
            txtMerchantId.MaxLength = 50;
            txtMerchantId.Text = this.MerchantId;
            currentCell.Controls.Add(txtMerchantId);
            //ADD CHECKBOX TO INDICATE TEST ACCOUNT
            currentCell.Controls.Add(new LiteralControl("<br />"));
            CheckBox chkTestAccount = new CheckBox();
            chkTestAccount.ID = "Config_IsTestAccount";
            chkTestAccount.Checked = this.IsTestAccount;
            chkTestAccount.Text = "Check here if you are using a test account.";
            currentCell.Controls.Add(chkTestAccount);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ATTEMPT TO DETECT THE PRESENCE OF THE SECURITY KEY FILE
            if (!String.IsNullOrEmpty(this.MerchantId))
            {
                currentRow = new HtmlTableRow();
                currentCell = new HtmlTableCell("th");
                currentCell.Attributes.Add("class", "rowHeader");
                currentCell.Attributes.Add("style", "white-space:normal;");
                currentCell.VAlign = "Top";
                currentCell.Width = "50%";
                currentCell.Controls.Add(new LiteralControl("PEM Certificate:"));
                currentCell.Controls.Add(new LiteralControl(String.Format("<br /><span class=\"helpText\">Detecting the presence of the {0}.PEM certificate file in the \"App_Data\" folder.</span>", this.MerchantId)));
                currentRow.Cells.Add(currentCell);
                currentCell = new HtmlTableCell();
                currentCell.VAlign = "Top";
                currentCell.Width = "50%";
                string securityKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\" + this.MerchantId + ".PEM");
                if (File.Exists(securityKeyPath))
                {
                    currentCell.Controls.Add(new LiteralControl("<span class=\"goodConditionText\">FOUND</span>"));
                }
                else
                {
                    currentCell.Controls.Add(new LiteralControl("<span class=\"errorConditionText\">NOT FOUND</span>"));
                }
                currentRow.Cells.Add(currentCell);
                configTable.Rows.Add(currentRow);
            }

            //GET THE AUTHORIZATION MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Authorization Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Use \"Authorize\" to linkpointRequest authorization without capturing funds at the time of purchase. You can capture authorized transactions through the order admin interface. Use \"Authorize & Capture\" to capture funds immediately at the time of purchase.</span>"));
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure the gateway to run live or test transactions.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGatewayMode = new RadioButtonList();
            rblGatewayMode.ID = "Config_GatewayMode";
            rblGatewayMode.Items.Add(new ListItem("Live Mode", GatewayModeType.LIVE.ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Mode (Approval)", GatewayModeType.GOOD.ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Mode (Decline)", GatewayModeType.DECLINE.ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Mode (Duplicate)", GatewayModeType.DUPLICATE.ToString()));
            switch (this.GatewayMode)
            {
                case GatewayModeType.LIVE: rblGatewayMode.Items[0].Selected = true; break;
                case GatewayModeType.GOOD: rblGatewayMode.Items[1].Selected = true; break;
                case GatewayModeType.DECLINE: rblGatewayMode.Items[2].Selected = true; break;
                case GatewayModeType.DUPLICATE: rblGatewayMode.Items[3].Selected = true; break;
            }
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between your store and LinkPoint is recorded in the \"App_Data\\logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
            get { return this.MerchantId; }
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            //REQUEST AUTH/CAPTURE IF SO CONFIGURED
            if (this.UseAuthCapture) authorizeRequest.Capture = true;
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //BUILD THE REQUEST
            string providerRequest = BuildProviderRequest_Authorize(authorizeRequest, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, providerRequest, sensitiveData);
            //SEND REQUEST
            string providerResponse = this.SendRequestToProvider(providerRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, providerResponse, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Authorize(authorizeRequest, providerResponse);
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            //BUILD THE REQUEST
            string providerRequest = BuildProviderRequest_Capture(captureRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, providerRequest, null);
            //SEND REQUEST
            string providerResponse = this.SendRequestToProvider(providerRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, providerResponse, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Capture(captureRequest, providerResponse);
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            //BUILD THE REQUEST
            string providerRequest = BuildProviderRequest_Credit(creditRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, providerRequest, null);
            //SEND REQUEST
            string providerResponse = this.SendRequestToProvider(providerRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, providerResponse, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Credit(creditRequest, providerResponse);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            //BUILD THE REQUEST
            string providerRequest = BuildProviderRequest_Void(voidRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, providerRequest, null);
            //SEND REQUEST
            string providerResponse = this.SendRequestToProvider(providerRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, providerResponse, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Void(voidRequest, providerResponse);
        }


        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();
            AuthorizeTransactionRequest authRequest;
            Transaction tr1, tr2, errTrans;

            //VALIDATE THE PAYMENT PERIOD
            string payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == string.Empty)
            {
                errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The specified payment interval is not valid for this processor.");
                return new AuthorizeRecurringTransactionResponse(errTrans);
            }

            if (authorizeRequest.RecurringChargeSpecified)
            {
                //make a sale transaction first
                authRequest = new AuthorizeTransactionRequest(authorizeRequest.Payment, authorizeRequest.RemoteIP);
                authRequest.Capture = true;
                authRequest.Amount = authorizeRequest.Amount;
                tr1 = DoAuthorize(authRequest);
                if (tr1.TransactionStatus != TransactionStatus.Successful)
                {
                    errTrans = Transaction.CreateErrorTransaction(PaymentGatewayId, authorizeRequest, "E", "Authorization Failed.");
                    errTrans.TransactionType = TransactionType.AuthorizeRecurring;
                    response.AddTransaction(tr1);
                    response.AddTransaction(errTrans);
                    response.Status = TransactionStatus.Failed;
                    return response;
                }
                response.AddTransaction(tr1);
            }

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //Create new recurring transaction now
            string providerRequest = BuildProviderRequest_AuthorizeRecurring(authorizeRequest, payPeriod, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, providerRequest, sensitiveData);
            //SEND REQUEST
            string providerResponse = this.SendRequestToProvider(providerRequest);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, providerResponse, null);
            //PROCESS RESPONSE AND RETURN RESULT
            tr2 = this.ProcessProviderResponse_AuthorizeRecurring(authorizeRequest, providerResponse);

            response.AddTransaction(tr2);
            response.Status = tr2.TransactionStatus;

            return response;
        }

        private string GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 1) return "daily";
                    if (authorizeRequest.PaymentFrequency == 7) return "weekly";
                    if (authorizeRequest.PaymentFrequency == 14) return "biweekly";
                    return "d" + authorizeRequest.PaymentFrequency;
                //break;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return "monthly";
                    if (authorizeRequest.PaymentFrequency == 2) return "bimonthly";
                    if (authorizeRequest.PaymentFrequency == 12) return "yearly";
                    return "m" + authorizeRequest.PaymentFrequency;
                //break;
            }
            return string.Empty;
        }

        private DateTime GetNextPaymentDate(AuthorizeRecurringTransactionRequest authRequest)
        {
            switch (authRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    return LocaleHelper.LocalNow.AddDays(authRequest.PaymentFrequency);
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    return LocaleHelper.LocalNow.AddMonths(authRequest.PaymentFrequency);
            }
            return LocaleHelper.LocalNow;
        }


        private GatewayModeType parseGatewayMode(string gatewayMode)
        {
            try
            {
                return (GatewayModeType)Enum.Parse(typeof(GatewayModeType), gatewayMode, true);
            }
            catch
            {
                return GatewayModeType.LIVE;
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("MerchantId")) this.MerchantId = ConfigurationData["MerchantId"];
            if (ConfigurationData.ContainsKey("IsTestAccount")) this.IsTestAccount = (ConfigurationData["IsTestAccount"] == "on");
            if (ConfigurationData.ContainsKey("UseAuthCapture")) this.UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], false);
            if (ConfigurationData.ContainsKey("GatewayMode")) this.GatewayMode = parseGatewayMode(ConfigurationData["GatewayMode"]);
        }

        #region BuildRequest

        private static string GetIP()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                return context.Request.UserHostAddress;
            }
            return string.Empty;
        }

        private static void parseTotals(Order order, LSDecimal paymentAmount, out LSDecimal subtotal, out LSDecimal shipping, out LSDecimal taxes, out LSDecimal total)
        {
            //SET THE ORDER TOTAL AMOUNTS
            total = paymentAmount;
            if (order.Items.TotalPrice() == paymentAmount)
            {
                shipping = order.Items.TotalPrice(OrderItemType.Shipping, OrderItemType.Handling);
                taxes = order.Items.TotalPrice(OrderItemType.Tax);
                subtotal = total - (shipping + taxes);
            }
            else
            {
                shipping = 0;
                taxes = 0;
                subtotal = total;
            }
        }

        private string BuildProviderRequest_Authorize(AuthorizeTransactionRequest authorizeRequest, Dictionary<string, string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("linkpointRequest.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("linkpointRequest.Payment.Order");
            User user = order.User;

            // create order
            LPOrderPart linkpointOrder = LPOrderFactory.createOrderPart("order");
            // create a part we will use to build the order
            LPOrderPart op = LPOrderFactory.createOrderPart();

            // Build 'orderoptions'
            // For a test, set result to GOOD, DECLINE, or DUPLICATE
            op.put("result", this.GatewayMode.ToString());
            op.put("ordertype", (string)(authorizeRequest.Capture ? "SALE" : "PREAUTH"));
            // add 'orderoptions to order
            linkpointOrder.addPart("orderoptions", op);

            // Build 'merchantinfo'
            op.clear();
            //SET CONFIGFILE TO THE MERCHANT ID (STORE NUMBER)
            op.put("configfile", this.MerchantId);
            // add 'merchantinfo to order
            linkpointOrder.addPart("merchantinfo", op);

            //BUILD TRANSACTIONDETAILS PART
            op.clear();
            op.put("transactionorigin", "ECI");
            string ip = PaymentProvider.GetIP();
            if (!string.IsNullOrEmpty(ip)) op.put("ip", ip);
            linkpointOrder.addPart("transactiondetails", op);

            //ADD PAYMENT INSTRUMENT
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    //BUILD 'CREDITCARD' PART
                    op.clear();
                    string accountNumber = accountData.GetValue("AccountNumber");
                    op.put("cardnumber", accountNumber);
                    if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
                    string expirationMonth = accountData.GetValue("ExpirationMonth");
                    if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
                    op.put("cardexpmonth", expirationMonth);
                    string expirationYear = accountData.GetValue("ExpirationYear");
                    if (expirationYear.Length > 2) expirationYear = StringHelper.Right(expirationYear, 2);
                    if (expirationYear.Length < 2) expirationYear = "0" + expirationYear;
                    op.put("cardexpyear", expirationYear);
                    //PROCESS CREDIT CARD ACCOUNT DATA
                    string securityCode = accountData.GetValue("SecurityCode");
                    if (!string.IsNullOrEmpty(securityCode) && Regex.Match(securityCode, "^\\d{3}$").Success)
                    {
                        //SECURITY CODE CAN ONLY BE THREE DIGIT INTEGER
                        op.put("cvmvalue", securityCode);
                        if (this.UseDebugMode) sensitiveData["cvmvalue>" + securityCode] = "cvmvalue>" + (new string('x', securityCode.Length));
                        op.put("cvmindicator", "provided");
                    }
                    //ADD 'CREDITCARD' PART TO ORDER
                    linkpointOrder.addPart("creditcard", op);
                    break;
                case PaymentInstrument.Check:
                    //BUILD 'TELECHECK' PART
                    op.clear();
                    op.put("routing", accountData.GetValue("RoutingNumber"));
                    op.put("account", accountData.GetValue("AccountNumber"));
                    op.put("bankname", accountData.GetValue("BankName"));
                    op.put("bankstate", accountData.GetValue("BankState"));
                    op.put("dl", accountData.GetValue("LicenseNumber"));
                    op.put("dlstate", accountData.GetValue("LicenseState"));
                    op.put("accounttype", "pc");
                    linkpointOrder.addPart("telecheck", op);
                    if (this.UseDebugMode)
                    {
                        sensitiveData[accountData.GetValue("AccountNumber")] = MakeReferenceNumber(accountData.GetValue("AccountNumber"));
                        sensitiveData[accountData.GetValue("LicenseNumber")] = (new string('x', accountData.GetValue("LicenseNumber").Length));
                    }
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }

            // SET BILLING PART
            op.clear();
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                op.put("name", accountData["AccountName"]);
            }
            else
            {
                op.put("name", string.Format("{0} {1}", order.BillToFirstName, order.BillToLastName));
            }
            op.put("address1", order.BillToAddress1);
            op.put("city", order.BillToCity);
            op.put("state", order.BillToProvince);
            op.put("zip", order.BillToPostalCode);
            op.put("country", order.BillToCountryCode);
            op.put("phone", order.BillToPhone);
            op.put("email", order.BillToEmail);
            //STRIP OUT THE ADDRESS NUMBER
            Match match = Regex.Match(order.BillToAddress1, "^[\\D]*(?<addrnum>\\d+).*$", RegexOptions.ExplicitCapture);
            if (match.Success)
                op.put("addrnum", match.Groups["addrnum"].ToString());
            op.put("zip", order.BillToPostalCode);
            // ADD BILLING PART TO ORDER
            linkpointOrder.addPart("billing", op);

            // Build 'payment'
            LSDecimal subtotal, shipping, taxes, total;
            parseTotals(order, authorizeRequest.Amount, out subtotal, out shipping, out taxes, out total);
            op.clear();
            op.put("subtotal", string.Format("{0:F2}", subtotal));
            op.put("tax", string.Format("{0:F2}", taxes));
            op.put("shipping", string.Format("{0:F2}", shipping));
            op.put("chargetotal", string.Format("{0:F2}", total));
            // add 'payment to order
            linkpointOrder.addPart("payment", op);

            //RETURN THE LINKPOINT ORDER XML (AUTHORIZE REQUEST)
            return linkpointOrder.toXML();
        }

        private string BuildProviderRequest_Capture(CaptureTransactionRequest captureRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("captureRequest.Payment");
            Transaction authorizeTransaction = captureRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("captureRequest.AuthorizeTransaction");

            // create order
            LPOrderPart linkpointOrder = LPOrderFactory.createOrderPart("order");
            // create a part we will use to build the order
            LPOrderPart op = LPOrderFactory.createOrderPart();

            // Build 'orderoptions'
            op.put("ordertype", "POSTAUTH");
            op.put("result", this.GatewayMode.ToString());
            // add 'orderoptions to order
            linkpointOrder.addPart("orderoptions", op);

            // Build 'merchantinfo'
            op.clear();
            //SET CONFIGFILE TO THE MERCHANT ID (STORE NUMBER)
            op.put("configfile", this.MerchantId);
            // add 'merchantinfo to order
            linkpointOrder.addPart("merchantinfo", op);

            // Build 'payment'
            op.clear();
            op.put("chargetotal", string.Format("{0:F2}", captureRequest.Amount));
            // add 'payment to order
            linkpointOrder.addPart("payment", op);

            //BUILD TRANSACTIONDETAILS PART
            op.clear();
            string origTransactionId = authorizeTransaction.ProviderTransactionId;
            if (origTransactionId.IndexOf(":") > -1)
            {
                string[] idParts = authorizeTransaction.ProviderTransactionId.Split(":".ToCharArray());
                op.put("oid", idParts[0]);
                op.put("tdate", idParts[1]);
            }
            else
            {
                op.put("oid", origTransactionId);
            }
            string ip = PaymentProvider.GetIP();
            if (!string.IsNullOrEmpty(ip)) op.put("ip", ip);
            linkpointOrder.addPart("transactiondetails", op);

            //RETURN THE LINKPOINT ORDER XML (CAPTURE REQUEST)
            return linkpointOrder.toXML();
        }

        private string BuildProviderRequest_Void(VoidTransactionRequest voidRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("voidRequest.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("voidRequest.AuthorizeTransaction");

            // create order
            LPOrderPart linkpointOrder = LPOrderFactory.createOrderPart("order");
            // create a part we will use to build the order
            LPOrderPart op = LPOrderFactory.createOrderPart();

            // Build 'orderoptions'
            op.put("result", this.GatewayMode.ToString());
            op.put("ordertype", "VOID");
            // add 'orderoptions to order
            linkpointOrder.addPart("orderoptions", op);

            // Build 'merchantinfo'
            op.clear();
            //SET CONFIGFILE TO THE MERCHANT ID (STORE NUMBER)
            op.put("configfile", this.MerchantId);
            // add 'merchantinfo to order
            linkpointOrder.addPart("merchantinfo", op);

            // Build 'payment'
            op.clear();
            op.put("chargetotal", string.Format("{0:F2}", voidRequest.Amount));
            // add 'payment to order
            linkpointOrder.addPart("payment", op);

            //BUILD TRANSACTIONDETAILS PART
            op.clear();
            string origTransactionId = authorizeTransaction.ProviderTransactionId;
            if (origTransactionId.IndexOf(":") > -1)
            {
                string[] idParts = authorizeTransaction.ProviderTransactionId.Split(":".ToCharArray());
                op.put("oid", idParts[0]);
                op.put("tdate", idParts[1]);
            }
            else
            {
                op.put("oid", origTransactionId);
            }
            string ip = PaymentProvider.GetIP();
            if (!string.IsNullOrEmpty(ip)) op.put("ip", ip);
            linkpointOrder.addPart("transactiondetails", op);
            
            //RETURN THE LINKPOINT ORDER XML (CAPTURE REQUEST)
            return linkpointOrder.toXML();
        }

        private string BuildProviderRequest_Credit(RefundTransactionRequest creditRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("creditRequest.Payment");
            Transaction captureTransaction  = creditRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("creditRequest.CaptureTransaction");

            // create order
            LPOrderPart linkpointOrder = LPOrderFactory.createOrderPart("order");
            // create a part we will use to build the order
            LPOrderPart op = LPOrderFactory.createOrderPart();

            // Build 'orderoptions'
            op.put("result", this.GatewayMode.ToString());
            op.put("ordertype", "CREDIT");
            // add 'orderoptions to order
            linkpointOrder.addPart("orderoptions", op);

            // Build 'merchantinfo'
            op.clear();
            //SET CONFIGFILE TO THE MERCHANT ID (STORE NUMBER)
            op.put("configfile", this.MerchantId);
            // add 'merchantinfo to order
            linkpointOrder.addPart("merchantinfo", op);

            // Build 'payment'
            op.clear();
            op.put("chargetotal", string.Format("{0:F2}", creditRequest.Amount));
            // add 'payment to order
            linkpointOrder.addPart("payment", op);

            //BUILD TRANSACTIONDETAILS PART
            op.clear();
            string origTransactionId = captureTransaction.ProviderTransactionId;
            if (origTransactionId.IndexOf(":") > -1)
            {
                string[] idParts = captureTransaction.ProviderTransactionId.Split(":".ToCharArray());
                op.put("oid", idParts[0]);
                op.put("tdate", idParts[1]);
            }
            else
            {
                op.put("oid", origTransactionId);
            }
            string ip = PaymentProvider.GetIP();
            if (!string.IsNullOrEmpty(ip)) op.put("ip", ip);
            linkpointOrder.addPart("transactiondetails", op);

            //RETURN THE LINKPOINT ORDER XML (CAPTURE REQUEST)
            return linkpointOrder.toXML();
        }


        private string BuildProviderRequest_AuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest, string payPeriod, Dictionary<string, string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("linkpointRequest.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("linkpointRequest.Payment.Order");
            User user = order.User;

            // create order
            LPOrderPart linkpointOrder = LPOrderFactory.createOrderPart("order");
            // create a part we will use to build the order
            LPOrderPart op = LPOrderFactory.createOrderPart();

            // Build 'orderoptions'
            // For a test, set result to GOOD, DECLINE, or DUPLICATE
            op.put("result", this.GatewayMode.ToString());
            op.put("ordertype", "SALE");
            // add 'orderoptions to order
            linkpointOrder.addPart("orderoptions", op);

            // Build 'merchantinfo'
            op.clear();
            //SET CONFIGFILE TO THE MERCHANT ID (STORE NUMBER)
            op.put("configfile", this.MerchantId);
            // add 'merchantinfo to order
            linkpointOrder.addPart("merchantinfo", op);

            //BUILD TRANSACTIONDETAILS PART
            op.clear();
            op.put("transactionorigin", "ECI");
            string ip = PaymentProvider.GetIP();
            if (!string.IsNullOrEmpty(ip)) op.put("ip", ip);
            linkpointOrder.addPart("transactiondetails", op);

            //ADD PAYMENT INSTRUMENT
            PaymentInstrument instrument = payment.PaymentMethod.PaymentInstrument;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                case PaymentInstrument.Discover:
                case PaymentInstrument.MasterCard:
                case PaymentInstrument.Visa:
                    //BUILD 'CREDITCARD' PART
                    op.clear();
                    string accountNumber = accountData.GetValue("AccountNumber");
                    op.put("cardnumber", accountNumber);
                    if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
                    string expirationMonth = accountData.GetValue("ExpirationMonth");
                    if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
                    op.put("cardexpmonth", expirationMonth);
                    string expirationYear = accountData.GetValue("ExpirationYear");
                    if (expirationYear.Length > 2) expirationYear = StringHelper.Right(expirationYear, 2);
                    if (expirationYear.Length < 2) expirationYear = "0" + expirationYear;
                    op.put("cardexpyear", expirationYear);
                    //PROCESS CREDIT CARD ACCOUNT DATA
                    string securityCode = accountData.GetValue("SecurityCode");
                    if (!string.IsNullOrEmpty(securityCode) && Regex.Match(securityCode, "^\\d{3}$").Success)
                    {
                        //SECURITY CODE CAN ONLY BE THREE DIGIT INTEGER
                        op.put("cvmvalue", securityCode);
                        if (this.UseDebugMode) sensitiveData["cvmvalue>" + securityCode] = "cvmvalue>" + (new string('x', securityCode.Length));
                        op.put("cvmindicator", "provided");
                    }
                    //ADD 'CREDITCARD' PART TO ORDER
                    linkpointOrder.addPart("creditcard", op);
                    break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }

            // SET BILLING PART
            op.clear();
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                op.put("name", accountData["AccountName"]);
            }
            else
            {
                op.put("name", string.Format("{0} {1}", order.BillToFirstName, order.BillToLastName));
            }
            op.put("address1", order.BillToAddress1);
            op.put("city", order.BillToCity);
            op.put("state", order.BillToProvince);
            op.put("zip", order.BillToPostalCode);
            op.put("country", order.BillToCountryCode);
            op.put("phone", order.BillToPhone);
            op.put("email", order.BillToEmail);
            //STRIP OUT THE ADDRESS NUMBER
            Match match = Regex.Match(order.BillToAddress1, "^[\\D]*(?<addrnum>\\d+).*$", RegexOptions.ExplicitCapture);
            if (match.Success)
                op.put("addrnum", match.Groups["addrnum"].ToString());
            op.put("zip", order.BillToPostalCode);
            // ADD BILLING PART TO ORDER
            linkpointOrder.addPart("billing", op);

            // Build 'payment'
            LSDecimal subtotal, shipping, taxes, total;
            LSDecimal amount = authorizeRequest.RecurringChargeSpecified ? authorizeRequest.RecurringCharge : authorizeRequest.Amount;
            parseTotals(order, amount, out subtotal, out shipping, out taxes, out total);
            op.clear();
            op.put("subtotal", string.Format("{0:F2}", subtotal));
            op.put("tax", string.Format("{0:F2}", taxes));
            op.put("shipping", string.Format("{0:F2}", shipping));
            op.put("chargetotal", string.Format("{0:F2}", total));
            // add 'payment to order
            linkpointOrder.addPart("payment", op);

            // build 'periodic' part
            int remainingPayments = authorizeRequest.NumberOfPayments;
            DateTime startDt = LocaleHelper.LocalNow;
            if (authorizeRequest.RecurringChargeSpecified)
            {
                startDt = GetNextPaymentDate(authorizeRequest);
                remainingPayments -= 1;
            }

            op.clear();
            op.put("action", "SUBMIT");
            op.put("startdate", startDt.ToString("yyyyMMdd"));
            op.put("periodicity", payPeriod);
            op.put("installments", remainingPayments.ToString());
            op.put("threshold", "1");
            // add 'periodic' to order
            linkpointOrder.addPart("periodic", op);

            //RETURN THE LINKPOINT ORDER XML (AUTHORIZE REQUEST)
            return linkpointOrder.toXML();
        }

        #endregion


        #region ProcessResponse

        private string ParseTag(string tag, string providerResponse)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.AppendFormat("<{0}>", tag);
            int len = sb.Length;
            int idxSt = -1, idxEnd = -1;
            if (-1 == (idxSt = providerResponse.IndexOf(sb.ToString())))
            { return ""; }
            idxSt += len;
            sb.Remove(0, len);
            sb.AppendFormat("</{0}>", tag);
            if (-1 == (idxEnd = providerResponse.IndexOf(sb.ToString(), idxSt)))
            { return ""; }
            return providerResponse.Substring(idxSt, idxEnd - idxSt);
        }

        //PARSE THE PROVIER AVS RESULT INTO THE STANDARD ABLECOMMERCE CODE
        private string ParseAVS(string providerAvsResult)
        {
            //IF THE INPUT IS NOT A THREE CHARACTER STRING RETURN UNKNOWN RESULT
            if (string.IsNullOrEmpty(providerAvsResult) || providerAvsResult.Length != 3) return "U";
            string addressMatch = providerAvsResult.Substring(0, 1);
            string zipMatch = providerAvsResult.Substring(1, 1);
            //INSUFFICIENT INFO, RETURN UNKNOWN RESULT
            if (addressMatch.Equals("X") && zipMatch.Equals("X")) return "U";
            if (addressMatch.Equals("Y"))
            {
                //ADDRESS MATCH
                if (zipMatch.Equals("Y"))
                {
                    //ZIP MATCH
                    return "Y";
                }
                else
                {
                    //ZIP NO MATCH
                    return "A";
                }
            }
            else
            {
                //ADDRESS NO MATCH
                if (zipMatch.Equals("Y"))
                {
                    //ZIP MATCH
                    return "Z";
                }
                else
                {
                    //ZIP NO MATCH
                    return "N";
                }
            }
        }

        private Transaction ProcessProviderResponse_Authorize(AuthorizeTransactionRequest authorizeRequest, string providerResponse)
        {
            //VERIFY OR CREATE THE PAYMENT OBJECT
            Payment payment = authorizeRequest.Payment;
            if (payment == null)
            {
                //NO PAYMENT CURRENTLY EXISTS, ADD A NEW PAYMENT TO THE ORDER
                throw new PaymentGatewayProviderException("Payment is not defined!");
            }
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = (authorizeRequest.Capture ? TransactionType.AuthorizeCapture : TransactionType.Authorize);
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            string oid = ParseTag("r_ordernum", providerResponse);
            string tdate = ParseTag("r_tdate", providerResponse);
            if (!string.IsNullOrEmpty(oid))
            {
                if (!string.IsNullOrEmpty(tdate)) transaction.ProviderTransactionId = oid + ":" + tdate;
                else transaction.ProviderTransactionId = oid;
            }
            transaction.TransactionDate = AlwaysConvert.ToDateTime(ParseTag("r_time", providerResponse), DateTime.UtcNow).ToUniversalTime();
            transaction.Amount = authorizeRequest.Amount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            //CHECK THE TRANSACTION RESULT
            string decision = ParseTag("r_approved", providerResponse);
            transaction.ResponseMessage = StringHelper.Truncate(ParseTag("r_error", providerResponse), 250);
            if (decision.Equals("APPROVED"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.AuthorizationCode = ParseTag("r_code", providerResponse);
                string avsCode = ParseTag("r_avs", providerResponse);
                if (!string.IsNullOrEmpty(avsCode) && ((avsCode.Length == 3) || (avsCode.Length == 4)))
                {
                    string cvvCode = string.Empty;
                    if (avsCode.Length == 4)
                    {
                        cvvCode = avsCode.Substring(3, 1);
                        avsCode = avsCode.Substring(0, 3);
                    }
                    transaction.AVSResultCode = ParseAVS(avsCode);
                    transaction.CVVResultCode = cvvCode;
                }
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessProviderResponse_AuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest, string providerResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.AuthorizeRecurring;
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            string oid = ParseTag("r_ordernum", providerResponse);
            string tdate = ParseTag("r_tdate", providerResponse);
            if (!string.IsNullOrEmpty(oid))
            {
                if (!string.IsNullOrEmpty(tdate)) transaction.ProviderTransactionId = oid + ":" + tdate;
                else transaction.ProviderTransactionId = oid;
            }
            transaction.TransactionDate = AlwaysConvert.ToDateTime(ParseTag("r_time", providerResponse), LocaleHelper.LocalNow).ToUniversalTime();
            transaction.Amount = authorizeRequest.RecurringChargeSpecified ? authorizeRequest.RecurringCharge : authorizeRequest.Amount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            //CHECK THE TRANSACTION RESULT
            string decision = ParseTag("r_approved", providerResponse);
            transaction.ResponseMessage = StringHelper.Truncate(ParseTag("r_error", providerResponse), 250);
            if (decision.Equals("APPROVED"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.AuthorizationCode = ParseTag("r_code", providerResponse);
                string avsCode = ParseTag("r_avs", providerResponse);
                if (!string.IsNullOrEmpty(avsCode) && ((avsCode.Length == 3) || (avsCode.Length == 4)))
                {
                    string cvvCode = string.Empty;
                    if (avsCode.Length == 4)
                    {
                        cvvCode = avsCode.Substring(3, 1);
                        avsCode = avsCode.Substring(0, 3);
                    }
                    transaction.AVSResultCode = ParseAVS(avsCode);
                    transaction.CVVResultCode = cvvCode;
                }
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessProviderResponse_Capture(CaptureTransactionRequest captureRequest, string providerResponse)
        {
            //VERIFY OR CREATE THE PAYMENT OBJECT
            Payment payment = captureRequest.Payment;
            if (payment == null)
            {
                //NO PAYMENT CURRENTLY EXISTS, ADD A NEW PAYMENT TO THE ORDER
                throw new PaymentGatewayProviderException("Payment is not defined!");
            }
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Capture;
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            string oid = ParseTag("r_ordernum", providerResponse);
            string tdate = ParseTag("r_tdate", providerResponse);
            if (!string.IsNullOrEmpty(oid))
            {
                if (!string.IsNullOrEmpty(tdate)) transaction.ProviderTransactionId = oid + ":" + tdate;
                else transaction.ProviderTransactionId = oid;
            }
            transaction.TransactionDate = AlwaysConvert.ToDateTime(ParseTag("r_time", providerResponse), DateTime.UtcNow).ToUniversalTime();
            transaction.Amount = captureRequest.Amount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            //CHECK THE TRANSACTION RESULT
            string decision = ParseTag("r_approved", providerResponse);
            transaction.ResponseMessage = StringHelper.Truncate(ParseTag("r_error", providerResponse), 250);
            if (decision.Equals("APPROVED"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessProviderResponse_Credit(RefundTransactionRequest creditRequest, string providerResponse)
        {
            //VERIFY OR CREATE THE PAYMENT OBJECT
            Payment payment = creditRequest.Payment;
            if (payment == null)
            {
                //NO PAYMENT CURRENTLY EXISTS, ADD A NEW PAYMENT TO THE ORDER
                throw new PaymentGatewayProviderException("Payment is not defined!");
            }
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = creditRequest.TransactionType;
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            string oid = ParseTag("r_ordernum", providerResponse);
            string tdate = ParseTag("r_tdate", providerResponse);
            if (!string.IsNullOrEmpty(oid))
            {
                if (!string.IsNullOrEmpty(tdate)) transaction.ProviderTransactionId = oid + ":" + tdate;
                else transaction.ProviderTransactionId = oid;
            }
            transaction.TransactionDate = AlwaysConvert.ToDateTime(ParseTag("r_time", providerResponse), DateTime.UtcNow).ToUniversalTime();
            transaction.Amount = creditRequest.Amount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            //CHECK THE TRANSACTION RESULT
            string decision = ParseTag("r_approved", providerResponse);
            transaction.ResponseMessage = StringHelper.Truncate(ParseTag("r_error", providerResponse), 250);
            if (decision.Equals("APPROVED"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessProviderResponse_Void(VoidTransactionRequest voidRequest, string providerResponse)
        {
            //VERIFY OR CREATE THE PAYMENT OBJECT
            Payment payment = voidRequest.Payment;
            if (payment == null)
            {
                //NO PAYMENT CURRENTLY EXISTS, ADD A NEW PAYMENT TO THE ORDER
                throw new PaymentGatewayProviderException("Payment is not defined!");
            }
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            string oid = ParseTag("r_ordernum", providerResponse);
            string tdate = ParseTag("r_tdate", providerResponse);
            if (!string.IsNullOrEmpty(oid))
            {
                if (!string.IsNullOrEmpty(tdate)) transaction.ProviderTransactionId = oid + ":" + tdate;
                else transaction.ProviderTransactionId = oid;
            }
            transaction.TransactionDate = AlwaysConvert.ToDateTime(ParseTag("r_time", providerResponse), DateTime.UtcNow).ToUniversalTime();
            transaction.Amount = voidRequest.Amount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            //CHECK THE TRANSACTION RESULT
            string decision = ParseTag("r_approved", providerResponse);
            transaction.ResponseMessage = StringHelper.Truncate(ParseTag("r_error", providerResponse), 250);
            if (decision.Equals("APPROVED"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }


        #endregion

        private string SendRequestToProvider(string providerRequest)
        {
            LinkPointTxn LPTxn = new LinkPointTxn();
            return LPTxn.send(this.CertificateFile, ((!this.IsTestAccount) ? LIVEHOST : TESTHOST), 1129, providerRequest);
        }

        public enum GatewayModeType
        {
            LIVE, GOOD, DECLINE, DUPLICATE
        }

    }
}
