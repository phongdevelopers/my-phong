using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Payments;
using System.Text.RegularExpressions;
using CommerceBuilder.Payments.Providers.PaymentechOrbital.RequestSchema;
using CommerceBuilder.Payments.Providers.PaymentechOrbital.ResponseSchema;
using CommerceBuilder.Stores;
using CommerceBuilder.Web.UI.WebControls;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    public class PaymentechOrbital : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        private const string PRIMARY_LIVEURL = "https://orbital1.paymentech.net/authorize";
        private const string SECONDARY_LIVEURL = "https://orbital2.paymentech.net/authorize";
        private const string PRIMARY_TESTURL = "https://orbitalvar1.paymentech.net/authorize";
        private const string SECONDARY_TESTURL = "https://orbitalvar2.paymentech.net/authorize";

        public override string Name
        {
            get { return "Chase Paymentech Orbital"; }
        }

        public override string Description
        {
            get { return "Chase Paymentech Orbital"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.PaymentechOrbital.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "Orbital Version 4.6 (PTI46)"; }
        }

        string _MerchantId = string.Empty;
        public string MerchantId
        {
            get { return _MerchantId; }
            set { _MerchantId = value; }
        }

        bool _UseAuthCapture = false;
        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }

        bool _UseTestMode = true;
        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        private validroutingbins _RoutingBin = validroutingbins.Item000002;
        public validroutingbins RoutingBin
        {
            get { return _RoutingBin; }
            set { _RoutingBin = value; }
        }

        private string _TerminalId = "001";
        public string TerminalId
        {
            get { return _TerminalId; }
            set { _TerminalId = value; }
        }

        private int _CurrencyCode = 840;
        public int CurrencyCode
        {
            get { return _CurrencyCode; }
            set { _CurrencyCode = value; }
        }

        public override string ConfigReference
        {
            get { return MerchantId; }
        }

        public override bool RefundRequiresAccountData
        {
            get
            {
                return true;
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
                    | SupportedTransactions.Void);
            }
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
            currentCell.Attributes.Add("style", "text-align:left;");
            currentCell.ColSpan = 2;
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = this.Name;
            gatewayLink.NavigateUrl = "http://www.chasepaymentech.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD VALIDATION SUMMARY
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            ValidationSummary validationSummary = new ValidationSummary();
            validationSummary.ShowMessageBox = true;
            currentCell.Controls.Add(validationSummary);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //display assembly information
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(new LiteralControl("Assembly:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            Label lblAssembly = new Label();
            lblAssembly.ID = "AssemblyInfo";
            lblAssembly.Text = this.GetType().Assembly.GetName().Name.ToString() + "&nbsp;(v" + this.GetType().Assembly.GetName().Version.ToString() + ")";
            currentCell.Controls.Add(lblAssembly);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //display assembly information
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.VAlign = "Top";
            currentCell.Controls.Add(new LiteralControl("IP Security:"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.Controls.Add(new LiteralControl("This gateway uses IP based security.  In addition to completing this form, your IP address must be registered with Paymentech. Transactions will be rejected if they do not come from a registered IP."));
            string myIP = Misc.WhatIsMyIP();
            if (!string.IsNullOrEmpty(myIP))
            {
                currentCell.Controls.Add(new LiteralControl("<br /><br />Your external IP appears to be: <b>" + myIP + "</b>"));
            }
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Account Token
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Merchant ID:", "Your 6 or 12 digit Paymentech Merchant ID is required."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            TextBox txtMerchantId = new TextBox();
            txtMerchantId.ID = "Config_MerchantId";
            txtMerchantId.Width = new Unit("100px");
            txtMerchantId.MaxLength = 12;
            txtMerchantId.Text = this.MerchantId;
            currentCell.Controls.Add(txtMerchantId);
            RequiredFieldValidator midRequired = new RequiredFieldValidator();
            midRequired.ControlToValidate = txtMerchantId.ID;
            midRequired.Text = "*";
            midRequired.Display = ValidatorDisplay.Dynamic;
            midRequired.ErrorMessage = "Merchant ID is required.";
            currentCell.Controls.Add(midRequired);
            RegularExpressionValidator midValidator = new RegularExpressionValidator();
            midValidator.ValidationExpression = "^(\\d{6}|\\d{12})$";
            midValidator.ControlToValidate = txtMerchantId.ID;
            midValidator.Text = "*";
            midValidator.Display = ValidatorDisplay.Dynamic;
            midValidator.ErrorMessage = "Merchant ID must be a 6 or 12 digit number.";
            currentCell.Controls.Add(midValidator);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get Routing Bin
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("BIN:", "Select the Transaction Routing BIN assigned by Paymentech."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            DropDownList ddlRoutingBin = new DropDownList();
            ddlRoutingBin.ID = "Config_RoutingBin";
            foreach (validroutingbins rbin in Enum.GetValues(typeof(validroutingbins)))
            {

                string binValue = rbin.ToString();
                string binDisplayName;
                if (binValue.EndsWith("000001")) binDisplayName = "000001 (Salem)";
                else if (binValue.EndsWith("000002")) binDisplayName = "000002 (Tampa)";
                else binDisplayName = binValue;
                ListItem newItem = new ListItem(binDisplayName, binValue);
                if (rbin == this.RoutingBin)
                {
                    newItem.Selected = true;
                }
                ddlRoutingBin.Items.Add(newItem);
            }
            currentCell.Controls.Add(ddlRoutingBin);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get Terminal Id
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Terminal ID:", "Your Terminal ID assigned by Paymentech."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            TextBox txtTerminalId = new TextBox();
            txtTerminalId.ID = "Config_TerminalId";
            txtTerminalId.Width = new Unit("40px");
            txtTerminalId.MaxLength = 3;
            txtTerminalId.Text = this.TerminalId;
            currentCell.Controls.Add(txtTerminalId);
            RequiredFieldValidator tidRequired = new RequiredFieldValidator();
            tidRequired.ControlToValidate = txtTerminalId.ID;
            tidRequired.Text = "*";
            tidRequired.Display = ValidatorDisplay.Dynamic;
            tidRequired.ErrorMessage = "Terminal ID is required.";
            currentCell.Controls.Add(tidRequired);
            RegularExpressionValidator tidValidator = new RegularExpressionValidator();
            tidValidator.ValidationExpression = "^\\d{3}$";
            tidValidator.ControlToValidate = txtTerminalId.ID;
            tidValidator.Text = "*";
            tidValidator.Display = ValidatorDisplay.Dynamic;
            tidValidator.ErrorMessage = "Terminal ID must be a 3 digit number.";
            currentCell.Controls.Add(tidValidator);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //get Currency 
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Currency:", "Select the currency your Paymentech account is set up for.  Your store base currency must also be set to this value."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            DropDownList ddlCurrencyCode = new DropDownList();
            ddlCurrencyCode.ID = "Config_CurrencyCode";            
            foreach (CurrencyData cdata in CurrencyCodes.Items)
            {
                ListItem li = new ListItem(cdata.ISOCode, (cdata.numericCode).ToString());
                if (CurrencyCode == cdata.numericCode)
                {
                    li.Selected = true;
                }
                ddlCurrencyCode.Items.Add(li);
            }
            currentCell.Controls.Add(ddlCurrencyCode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE AUTHORIZATION MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Authorization&nbsp;Mode:", "Use \"Authorize\" to request authorization without capturing funds at the time of purchase. You can capture authorized transactions through the order admin interface. Use \"Authorize & Capture\" to capture funds immediately at the time of purchase."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            DropDownList rblTransactionType = new DropDownList();
            rblTransactionType.ID = "Config_UseAuthCapture";
            rblTransactionType.Items.Add(new ListItem("Authorize (recommended)", "false"));
            rblTransactionType.Items.Add(new ListItem("Authorize & Capture", "true"));
            rblTransactionType.Items[UseAuthCapture ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblTransactionType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get gateway mode
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Gateway Mode:", "You can configure to use the gateway in Live mode or Test/Certification mode."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            DropDownList rblGatewayMode = new DropDownList();
            rblGatewayMode.ID = "Config_UseTestMode";
            rblGatewayMode.Items.Add(new ListItem("Live Mode", "false"));
            rblGatewayMode.Items.Add(new ListItem("Test Mode", "true"));
            rblGatewayMode.Items[UseTestMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblGatewayMode);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            currentCell.Controls.Add(new LiteralControl("<br />"));

            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Controls.Add(GetToolTipLabel("Debug Mode:", "When debug mode is enabled, communication between your store and the gateway is recorded in the \"logs\" folder. Sensitive information is stripped from the log entries."));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            DropDownList rblDebugMode = new DropDownList();
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

        private ToolTipLabel GetToolTipLabel(string label, string helpText)
        {
            ToolTipLabel ttLabel = new ToolTipLabel();
            ttLabel.Text = label;
            ttLabel.ToolTip = helpText;
            return ttLabel;
        }
        
        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("MerchantId")) MerchantId = ConfigurationData["MerchantId"];
            if (ConfigurationData.ContainsKey("RoutingBin")) RoutingBin = (validroutingbins) AlwaysConvert.ToEnum(typeof(validroutingbins), ConfigurationData["RoutingBin"],validroutingbins.Item000001);
            if (ConfigurationData.ContainsKey("TerminalId")) TerminalId = ConfigurationData["TerminalId"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("CurrencyCode")) CurrencyCode = AlwaysConvert.ToInt(ConfigurationData["CurrencyCode"], CurrencyCode);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            // VALIDATE CONFIG AND INPUT PARAMETERS
            VerifyGatewayConfig();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");

            // PARSE THE PAYMENT DATA
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            PaymentData paymentData = GetPaymentData(accountData, payment.PaymentMethod, sensitiveData);

            // ADJUST THE CAPTURE NODE
            if (this.UseAuthCapture) authorizeRequest.Capture = true;

            // CREATE THE REQUEST
            Request request = BuildAuthRequest(order, paymentData, authorizeRequest);
            Response resp = SendRequestToGateway(request, sensitiveData);
            
            // PARSE THE RESPONSE
            return ProcessNewOrderResponse(resp.Item, authorizeRequest.Amount, authorizeRequest.Capture ? TransactionType.AuthorizeCapture : TransactionType.Authorize, order);
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            // VALIDATE CONFIG AND INPUT PARAMETERS
            VerifyGatewayConfig();
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            
            Transaction authTrans = captureRequest.AuthorizeTransaction;
            if (authTrans == null) throw new ArgumentNullException("captureRequest.AuthorizeTransaction");

            // CREATE THE REQUEST
            Request request = BuildCaptureRequest(order.OrderNumber, captureRequest.Amount, authTrans.ProviderTransactionId);
            Response resp = SendRequestToGateway(request, null);

            // PARSE THE RESPONSE
            return ProcessCaptureResponse(resp.Item, captureRequest.Amount, order);
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            // VALIDATE CONFIG AND INPUT PARAMETERS
            VerifyGatewayConfig();
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            Transaction captTrans = creditRequest.CaptureTransaction;
            if (captTrans == null) throw new ArgumentNullException("creditRequest.CaptureTransaction");

            // WE MUST CREATE THE ACCOUNT DATA DICTIONARY MANUALLY
            AccountDataDictionary accountData = new AccountDataDictionary();

            // NON CREDIT CARD PAYMENT DATA IS IN EXTENDED PROPERTIES FIELD
            foreach(string key in creditRequest.ExtendedProperties.Keys) accountData[key] = creditRequest.ExtendedProperties[key];

            // ADD IN CREDIT CARD PAYMENT DATA
            if (!accountData.ContainsKey("AccountNumber")) accountData["AccountNumber"] = creditRequest.CardNumber;
            accountData["ExpirationMonth"] = creditRequest.ExpirationMonth.ToString();
            accountData["ExpirationYear"] = creditRequest.ExpirationYear.ToString();

            // NOW PARSE THE PAYMENT DATA
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            PaymentData paymentData = GetPaymentData(accountData, payment.PaymentMethod, sensitiveData);

            // CREATE THE REQUEST
            Request request = BuildRefundRequest(order, paymentData, creditRequest);
            Response resp = SendRequestToGateway(request, sensitiveData);

            // PARSE THE RESPONSE
            return ProcessNewOrderResponse(resp.Item, creditRequest.Amount, TransactionType.Refund, order);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            // VALIDATE CONFIG AND INPUT PARAMETERS
            VerifyGatewayConfig();
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            Transaction authTrans = voidRequest.AuthorizeTransaction;
            if (authTrans == null) throw new ArgumentNullException("captureRequest.AuthorizeTransaction");

            // CREATE AND SEND THE REQUEST
            Request request = BuildVoidRequest(authTrans, voidRequest.Amount, order.OrderNumber);
            Response resp = SendRequestToGateway(request, null);

            // PARSE THE RESPONSE
            return ProcessVoidResponse(resp.Item, voidRequest.Amount, order);
        }

        /// <summary>
        /// Serializes the request witout namespaces in the document element
        /// </summary>
        /// <param name="request">Request to serialize</param>
        /// <returns>Array of bytes of the serialized request</returns>
        private static byte[] SerializeRequest(Request request)
        {
            string xml = Encoding.UTF8.GetString(XmlUtility.Serialize(request, Encoding.UTF8));
            Match match = Regex.Match(xml, "<Request [^>]*>");
            if (match.Success)
            {
                xml = xml.Replace(match.Groups[0].Value, "<Request>");
            }
            int startIndex = xml.IndexOf("<?xml");
            if (startIndex > 0) xml = xml.Substring(startIndex);
            return Encoding.UTF8.GetBytes(xml);
        }

        private void VerifyGatewayConfig()
        {
            if (string.IsNullOrEmpty(this.MerchantId))
            {                
                throw new InvalidOperationException("MerchantId missing");
            }
            if (string.IsNullOrEmpty(TerminalId))
            {
                throw new InvalidOperationException("TerminalId missing");
            }
        }

        #region Process Gateway Response

        /// <summary>
        /// Processes response messages that are returned for authorizations or refunds
        /// </summary>
        /// <param name="newOrderResp">The new order response from the gateway</param>
        /// <param name="amount">The amount of the transaction</param>
        /// <param name="transactionType">Should only be Authorize, AuthorizeCapture, or Refund</param>
        /// <returns>A transaction populated with the response data</returns>
        private Transaction ProcessNewOrderResponse(object response, LSDecimal amount, TransactionType transactionType, Order order)
        {
            // VALIDATE TRANSACTION TYPE
            if (transactionType != TransactionType.Authorize && transactionType != TransactionType.AuthorizeCapture && transactionType != TransactionType.Refund)
            {
                throw new ArgumentException("Only Authorize, AuthorizeCapture, and Refund transaction types are supported.", "transactionType");
            }

            // CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;
            transaction.Amount = amount;

            quickRespType quickResp = response as quickRespType;
            if (quickResp == null)
            {
                newOrderRespType newOrderResp = (newOrderRespType)response;

                // TxRefNum: Gateway transaction reference number
                transaction.ProviderTransactionId = newOrderResp.TxRefNum;

                // SEE WHETHER REQUEST WAS ACCEPTED BY THE GATEWAY
                if (newOrderResp.ProcStatus != "0")
                {
                    // THE REQUEST FAILED TO PROCESSS
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    // ProcStatus: Process Status: 0 - success
                    transaction.ResponseCode = newOrderResp.ProcStatus;
                    // StatusMsg: Text message associated with ProcStatus value.
                    transaction.ResponseMessage = newOrderResp.StatusMsg;
                }
                else
                {
                    // THE REQUEST WAS PROCESSED, CHECK FOR APPROVAL
                    // ApprovalStatus: 0 – Decline, 1 – Approved, 2 – Message/System Error
                    if (newOrderResp.ApprovalStatus == "1")
                    {
                        // REQUEST WAS APPROVED
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        // RespMsg: Message associated with response code (code = 0 on approval)
                        transaction.ResponseMessage = newOrderResp.RespMsg;

                        if (this.UseTestMode)
                        {
                            // WRITE THE CERT DATA
                            WriteCertData(newOrderResp.RespTime, this.MerchantId, newOrderResp.OrderID, this.CurrencyCode.ToString(), order.BillToPhone, order.BillToFirstName + " " + order.BillToLastName, order.BillToAddress1, newOrderResp.AuthCode, newOrderResp.AVSRespCode, newOrderResp.TxRefNum, string.Empty);
                        }
                    }
                    else
                    {
                        // REQUEST WAS DECLINED OR HAD AN ERROR
                        transaction.TransactionStatus = TransactionStatus.Failed;
                        // RespCode: Response code
                        transaction.ResponseCode = newOrderResp.RespCode;
                        // RespMsg: Message associated with response code
                        transaction.ResponseMessage = newOrderResp.RespMsg;
                        if (string.IsNullOrEmpty(transaction.ResponseMessage))
                            transaction.ResponseMessage = newOrderResp.StatusMsg;
                    }

                    // POPULATE DATA SPECIFIC TO AUTHORIZE TYPE TRANSACTIONS
                    if (transactionType != TransactionType.Refund)
                    {
                        // AuthCode: Issuer approval code
                        transaction.AuthorizationCode = newOrderResp.AuthCode;
                        // AVSRespCode: Address verification request response
                        transaction.AVSResultCode = TranslateAvsCode(newOrderResp.AVSRespCode);
                        // CVV2RespCode: Card verification value request response
                        transaction.CVVResultCode = TranslateCvvCode(newOrderResp.CVV2RespCode);
                    }
                }
            }
            else
            {
                // THE REQUEST FAILED TO PROCESSS
                transaction.TransactionStatus = TransactionStatus.Failed;
                // ProcStatus: Process Status: 0 - success
                transaction.ResponseCode = quickResp.ProcStatus;
                // StatusMsg: Text message associated with ProcStatus value.
                transaction.ResponseMessage = quickResp.StatusMsg;
            }

            // SET VARIABLES FROM HTTPCONTEXT IF POSSIBLE
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            // RETURN THE PREPARED TRANSACTION
            return transaction;
        }

        /// <summary>
        /// Converts the AVS code from the provider specific value to one recognized by AbleCommerce
        /// </summary>
        /// <param name="providerAvsCode">The avs code returned by the provider gateway</param>
        /// <returns>The AbleCommerce specific AVS code</returns>
        private string TranslateAvsCode(string providerAvsCode)
        {
            string testVal = string.Empty;
            if (!string.IsNullOrEmpty(providerAvsCode)) testVal = providerAvsCode.Trim().ToUpperInvariant();
            switch (providerAvsCode.ToUpperInvariant())
            {
                case "1":
                case "2":
                case "5":
                    // INVALID DATA OR ERROR
                    return "E";
                case "3":
                case "7":
                case "8":
                case "UK":
                case "Y":
                    // UNAVAILABLE
                    return "U";
                case "4":
                case "R":
                    // NOT SUPPORTED
                    return "S";
                case "6":
                    // TIMEOUT OR SYSTEM UNAVAILABLE
                    return "R";
                case "9":
                case "X":
                    // STREET AND NINE DIGIT ZIP MATCH
                    return "X";
                case "A":
                    // NINE DIGIT ZIP MATCHES, STREET DOES NOT
                    return "W";
                case "B":
                case "H":
                    // STREET ADDRESS AND FIVE DIGIT ZIP MATCH
                    return "Y";
                case "C":
                case "Z":
                case "N7":
                    // FIVE DIGIT ZIP MATCHES, STREET DOES NOT
                    return "Z";
                case "D":
                case "F":
                case "N3":
                    // STREET ADDRESS MATCHES, ZIP DOES NOT
                    return "A";
                case "E":
                case "G":
                    // NO MATCH
                    return "N";
                case "J":
                    // NOT SUPPORTED (INTL)
                    return "G";
                case "JA":
                case "M2":
                case "M5":
                case "N4":
                case "N6":
                case "N8":
                    // STREET ADDRESS AND POSTAL CODE MATCH (INTL)
                    return "M";
                case "JB":
                case "M4":
                case "M6":
                case "M7":
                    // STREET ADDRESS MATCHES, POSTAL CODE NOT VERIFIED (INTL)
                    return "B";
                case "JC":
                    // STREET ADDRESS AND POSTAL CODE NOT VERIFIED (INTL)
                    return "C";
                case "JD":
                case "M3":
                    // POSTAL CODE MATCHES, STREET ADDRESS NOT VERIFIED (INTL)
                    return "P";
                case "M1":
                case "M8":
                case "N5":
                    // ADDRESS INFORMATION NOT VERIFIED (INTL)
                    return "I";
                default:
                    // FOR LACK OF A BETTER CODE, RETURN UNAVAILABLE
                    return "U";
            }
        }

        /// <summary>
        /// Converts the CVV code from the provider specific value to one recognized by AbleCommerce
        /// </summary>
        /// <param name="providerCvvCode">The CVV code returned by the provider gateway</param>
        /// <returns>The AbleCommerce specific CVV code</returns>
        private string TranslateCvvCode(string providerCvvCode)
        {
            // Paymentech CVV codes are roughly equivalent to AbleCommerce
            string testVal = string.Empty;
            if (!string.IsNullOrEmpty(providerCvvCode)) testVal = providerCvvCode.Trim().ToUpperInvariant();
            // MAKE SURE A BLANK CVV RESPONSE IS INDICATED AS NOT PERFORMED
            if (string.IsNullOrEmpty(testVal)) return "P";
            return testVal;
        }

        /// <summary>
        /// Processes response messages that are returned for captures
        /// </summary>
        /// <param name="captureResp">The capture response from the gateway</param>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>A transaction populated with the response data</returns>
        private Transaction ProcessCaptureResponse(object response, LSDecimal amount, Order order)
        {
            // CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Capture;
            transaction.Amount = amount;

            quickRespType quickResp = response as quickRespType;
            if (quickResp == null)
            {
                markForCaptureRespType captureResp = (markForCaptureRespType)response;

                // TxRefNum: Gateway transaction reference number
                transaction.ProviderTransactionId = captureResp.TxRefNum;

                // SEE WHETHER REQUEST WAS ACCEPTED BY THE GATEWAY
                if (captureResp.ProcStatus != "0")
                {
                    // THE REQUEST FAILED TO PROCESSS
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    // ProcStatus: Process Status: 0 - success
                    transaction.ResponseCode = captureResp.ProcStatus;
                    // StatusMsg: Text message associated with ProcStatus value.
                    transaction.ResponseMessage = captureResp.StatusMsg;
                }
                else
                {
                    // THE REQUEST WAS PROCESSED, CHECK FOR APPROVAL
                    // ApprovalStatus: 0 – Decline, 1 – Approved, 2 – Message/System Error
                    if (captureResp.ApprovalStatus == "1")
                    {
                        // REQUEST WAS APPROVED
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        // RespMsg: Message associated with response code (code = 0 on approval)
                        transaction.ResponseMessage = captureResp.RespMsg;
                        // AuthCode: Issuer approval code
                        transaction.AuthorizationCode = captureResp.AuthCode;

                        if (this.UseTestMode)
                        {
                            // WRITE THE CERT DATA
                            WriteCertData(captureResp.RespTime, this.MerchantId, captureResp.OrderID, this.CurrencyCode.ToString(), order.BillToPhone, order.BillToFirstName + " " + order.BillToLastName, order.BillToAddress1, captureResp.AuthCode, captureResp.AVSRespCode, captureResp.TxRefNum, string.Empty);
                        }
                    }
                    else
                    {
                        // REQUEST WAS DECLINED OR HAD AN ERROR
                        transaction.TransactionStatus = TransactionStatus.Failed;
                        // RespCode: Response code
                        transaction.ResponseCode = captureResp.RespCode;
                        // RespMsg: Message associated with response code
                        transaction.ResponseMessage = captureResp.RespMsg;
                    }
                }
            }
            else
            {
                // THE REQUEST FAILED TO PROCESSS
                transaction.TransactionStatus = TransactionStatus.Failed;
                // ProcStatus: Process Status: 0 - success
                transaction.ResponseCode = quickResp.ProcStatus;
                // StatusMsg: Text message associated with ProcStatus value.
                transaction.ResponseMessage = quickResp.StatusMsg;
            }

            // SET VARIABLES FROM HTTPCONTEXT IF POSSIBLE
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            // RETURN THE PREPARED TRANSACTION
            return transaction;
        }

        /// <summary>
        /// Processes response messages that are returned for voids (reversals)
        /// </summary>
        /// <param name="reversalResp">The reversal response from the gateway</param>
        /// <param name="amount">The amount of the transaction</param>
        /// <returns>A transaction populated with the response data</returns>
        private Transaction ProcessVoidResponse(object response, LSDecimal amount, Order order)
        {
            // CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            transaction.Amount = amount;

            quickRespType quickResp = response as quickRespType;
            if (quickResp == null)
            {
                reversalRespType reversalResp = (reversalRespType)response;

                // TxRefNum: Gateway transaction reference number
                transaction.ProviderTransactionId = reversalResp.TxRefNum;

                // SEE WHETHER REQUEST WAS ACCEPTED BY THE GATEWAY
                if (reversalResp.ProcStatus != "0")
                {
                    // THE REQUEST FAILED TO PROCESSS
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    // ProcStatus: Process Status: 0 - success
                    transaction.ResponseCode = reversalResp.ProcStatus;
                    // StatusMsg: Text message associated with ProcStatus value.
                    transaction.ResponseMessage = reversalResp.StatusMsg;
                }
                else
                {
                    // REQUEST WAS PROCESSED WITHOUT ERRORS
                    transaction.TransactionStatus = TransactionStatus.Successful;

                    if (this.UseTestMode)
                    {
                        // WRITE THE CERT DATA
                        WriteCertData(reversalResp.RespTime, this.MerchantId, reversalResp.OrderID, this.CurrencyCode.ToString(), order.BillToPhone, order.BillToFirstName + " " + order.BillToLastName, order.BillToAddress1, string.Empty, string.Empty, reversalResp.TxRefNum, string.Empty);
                    }
                }

                // SET VARIABLES FROM HTTPCONTEXT IF POSSIBLE
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }
            else
            {
                // THE REQUEST FAILED TO PROCESSS
                transaction.TransactionStatus = TransactionStatus.Failed;
                // ProcStatus: Process Status: 0 - success
                transaction.ResponseCode = quickResp.ProcStatus;
                // StatusMsg: Text message associated with ProcStatus value.
                transaction.ResponseMessage = quickResp.StatusMsg;
            }

            // RETURN THE PREPARED TRANSACTION
            return transaction;
        }

        #endregion

        #region Build Requests

        #region BuildAuthRequest
        /// <summary>
        /// Builds an authorization request for the gateway
        /// </summary>
        /// <param name="order">The order associated with the auth reqeust</param>
        /// <param name="paymentData">The payment instrument used for the auth</param>
        /// <param name="paymentAmount">The amount for the authorization</param>
        /// <param name="capture">If true, an authcapture request is generated. If false, the request is authorize only.</param>
        /// <returns>A Paymentech request object suitable for transmission to the gateway</returns>
        private Request BuildAuthRequest(Order order, PaymentData paymentData, AuthorizeTransactionRequest authorizeRequest)
        {
            // CREATE THE ORDER CONTAINER
            newOrderType newOrder = new newOrderType();

            // TODO: OrbitalConnectionUsername / OrbitalConnectionPassword

            // IndustryType: Defines the Industry type of the transaction
            // EC: eCommerce transaction
            newOrder.IndustryType = validindustrytypes.EC;

            // MessageType: Defines the transaction type
            newOrder.MessageType = authorizeRequest.Capture ? validtranstypes.AC : validtranstypes.A;

            // BIN: Transaction Routing Definition.  000001 = Salem, 000002 = Tampa
            newOrder.BIN = this.RoutingBin;

            // MerchantID: Gateway merchant account number assigned by Chase Paymentech.
            // 6 digits for Salem, 12 digits for Tampa.
            newOrder.MerchantID = this.MerchantId;

            //TerminalID: Merchant Terminal ID assigned by Chase Paymentech
            // For Salem, must be 001.  For Tampa can be 001 to 999, but 001 is standard.
            newOrder.TerminalID = this.TerminalId;

            //CardBrand: Defines the Card Type / Brand for the Transaction:
            //Required for: SW – Switch / Solo, EC – Electronic Check
            if (paymentData.CardBrand != CardBrand.CC)
            {
                newOrder.CardBrand = paymentData.CardBrand.ToString();
            }

            // (AccountNum, Exp, CardSecVal, etc...)
            BuildAuthRequest_SetPaymentDataData(newOrder, paymentData);

            //CurrencyCode: Defines the transaction currency:
            //Bin 000002 only supports the US Dollar [‘840’] and Canadian Dollar [‘124’].
            string currencyCode = this.CurrencyCode.ToString("000");
            newOrder.CurrencyCode = currencyCode;
            newOrder.CurrencyExponent = CurrencyCodes.GetExponent(this.CurrencyCode).ToString();

            // SET THE AVS DATA FOR THIS REQUEST
            BuildAuthRequest_SetAVSData(newOrder, order);

            //OrderID : Merchant Defined Order Number:
            // first 8 characters should be unique
            newOrder.OrderID = order.OrderNumber.ToString();

            // Amount: Transaction Amount, this is a whole integer including implied decimal
            // in other words, $100.00 should be transmitted as 10000
            newOrder.Amount = ((int)(authorizeRequest.Amount * 100)).ToString();

            // CREATE AND RETURN THE REQUEST
            Request request = new Request();
            request.Item = newOrder;
            return request;
        }

        // THIS METHOD IS USED TO BUILD BOTH AUTHORIZATIONS AND REFUNDS
        private void BuildAuthRequest_SetAVSData(newOrderType newOrder, Order order)
        {
            //AVSname: Cardholder Billing Name
            newOrder.AVSname = order.BillToFirstName + " " + order.BillToLastName;

            //AVSaddress1: Cardholder Billing Address line 1
            newOrder.AVSaddress1 = StringHelper.Truncate(order.BillToAddress1, 30);

            //AVSaddress2: Cardholder Billing Address Line 2
            newOrder.AVSaddress2 = StringHelper.Truncate(order.BillToAddress2, 30);

            //AVScity: Cardholder Billing City
            newOrder.AVScity = StringHelper.Truncate(order.BillToCity, 20);

            //AVSstate: Cardholder Billing State
            newOrder.AVSstate = StringHelper.Truncate(order.BillToProvince, 2);

            //AVSzip: Cardholder Billing Address Zip Code. Required for Bill Me Later sale transactions
            newOrder.AVSzip = StringHelper.Truncate(order.BillToPostalCode, 10);

            //AVScountryCode: Cardholder Billing Address Country Code
            newOrder.AVScountryCode = order.BillToCountryCode;

            //AVSphoneNum NewOrder Cardholder Billing Phone Number
            newOrder.AVSphoneNum = StringHelper.Truncate(order.BillToPhone, 14);
        }

        // THIS METHOD IS USED TO BUILD BOTH AUTHORIZATIONS AND REFUNDS
        private void BuildAuthRequest_SetPaymentDataData(newOrderType newOrder, PaymentData paymentData)
        {
            if (paymentData is CreditCardData)
            {
                CreditCardData creditCard = paymentData as CreditCardData;

                // AccountNum: Card Number
                newOrder.AccountNum = creditCard.CardNumber;

                // Exp: Card Expiration Date (MMYY)
                newOrder.Exp = creditCard.ExpirationDate;

                if (!string.IsNullOrEmpty(creditCard.SecurityCode))
                {
                    // CardSecVal: Card Verification Number
                    newOrder.CardSecVal = creditCard.SecurityCode;
                }
            }
            else if (paymentData is DebitCardData)
            {
                DebitCardData debitCard = paymentData as DebitCardData;

                // AccountNum: Card Number
                newOrder.AccountNum = debitCard.CardNumber;

                // Exp: Card Expiration Date (MMYY)
                newOrder.Exp = debitCard.ExpirationDate;

                if (!string.IsNullOrEmpty(debitCard.SecurityCode))
                {
                    // CardSecVal: Card Verification Number
                    newOrder.CardSecVal = debitCard.SecurityCode;
                }

                // DebitCardIssueNum: Switch/Solo incremental counter for lost or replacement cards.
                // is required, but can be null if only start date appears
                newOrder.DebitCardIssueNum = debitCard.IssueNumber;

                // DebitCardStartDate: Switch/Solo card start date
                // is required, but can be null if only issue number appears
                newOrder.DebitCardStartDate = debitCard.StartDate;
            }
            else if (paymentData is ECheckData)
            {
                ECheckData eCheck = (ECheckData)paymentData;

                // BCRtNum: Bank routing and transit number for the customer.
                newOrder.BCRtNum = eCheck.RoutingNumber;

                // CheckDDA: Checking account number
                newOrder.CheckDDA = eCheck.AccountNumber;

                // BankAccountType: Defines the deposit account type. C = Consumer Checking (US/CA), S = Consumer Savings (US), X = Commercial Checking (US)
                newOrder.BankAccountType = eCheck.BankAccountType.ToString();

                // ECPAuthMethod: Code used to identify the method used by consumers to authorize debits
                // I = Internet (Web)
                newOrder.ECPAuthMethod = "I";

                // BankPmtDelv : Defines the ECP payment delivery method. B = Best Possible (US), A = ACH (US/CA)
                newOrder.BankPmtDelv = "B";
            }
        }

        private PaymentData GetPaymentData(AccountDataDictionary accountData, PaymentMethod method, Dictionary<string, string> sensitiveData)
        {
            if (method == null || method.IsCreditCard())
            {
                CreditCardData creditCard = new CreditCardData();
                creditCard.CardNumber = accountData.GetValue("AccountNumber");
                sensitiveData[creditCard.CardNumber] = this.MakeReferenceNumber(creditCard.CardNumber);
                creditCard.ExpirationMonth = AlwaysConvert.ToInt(accountData.GetValue("ExpirationMonth"));
                creditCard.ExpirationYear = AlwaysConvert.ToInt(accountData.GetValue("ExpirationYear"));
                creditCard.SecurityCode = accountData.GetValue("SecurityCode");
                if (!string.IsNullOrEmpty(creditCard.SecurityCode))
                {
                    sensitiveData["CardSecVal>" + creditCard.SecurityCode] = "CardSecVal>" + (new string('x', creditCard.SecurityCode.Length));
                }
                return creditCard;
            }
            else if (method.PaymentInstrument == PaymentInstrument.Check)
            {
                ECheckData eCheck = new ECheckData();
                eCheck.RoutingNumber = accountData.GetValue("RoutingNumber");
                sensitiveData[eCheck.RoutingNumber] = this.MakeReferenceNumber(eCheck.RoutingNumber);
                eCheck.AccountNumber = accountData.GetValue("AccountNumber");
                sensitiveData[eCheck.AccountNumber] = this.MakeReferenceNumber(eCheck.AccountNumber);
                string accountType = accountData.GetValue("AccountType");
                if (accountType.ToLowerInvariant() == "savings")
                {
                    eCheck.BankAccountType = BankAccountType.S;
                }
                else if (accountType.ToLowerInvariant() == "business")
                {
                    eCheck.BankAccountType = BankAccountType.X;
                }
                return eCheck;
            }
            else if (method.IsIntlDebitCard())
            {
                DebitCardData debitCard = new DebitCardData();
                debitCard.CardNumber = accountData.GetValue("AccountNumber");
                sensitiveData[debitCard.CardNumber] = this.MakeReferenceNumber(debitCard.CardNumber);
                debitCard.ExpirationMonth = AlwaysConvert.ToInt(accountData.GetValue("ExpirationMonth"));
                debitCard.ExpirationYear = AlwaysConvert.ToInt(accountData.GetValue("ExpirationYear"));
                debitCard.IssueNumber = accountData.GetValue("IssueNumber");
                string startDateMonth = accountData.GetValue("StartDateMonth");
                string startDateYear = accountData.GetValue("StartDateYear");
                if (!string.IsNullOrEmpty(startDateMonth) && !string.IsNullOrEmpty(startDateYear))
                {
                    debitCard.StartMonth = AlwaysConvert.ToInt(startDateMonth);
                    debitCard.StartYear = AlwaysConvert.ToInt(startDateYear);
                }
                debitCard.SecurityCode = accountData.GetValue("SecurityCode");
                if (!string.IsNullOrEmpty(debitCard.SecurityCode))
                {
                    sensitiveData["CardSecVal>" + debitCard.SecurityCode] = "CardSecVal>" + (new string('x', debitCard.SecurityCode.Length));
                }
                return debitCard;
            }
            else
            {
                throw new ArgumentException("Payment method is not known to be supported by this gateway.", "method");
            }
        }
        #endregion

        private Request BuildCaptureRequest(int orderNumber, LSDecimal captureAmount, string txRefNum)
        {
            // CREATE THE ORDER CONTAINER
            markForCaptureType captureRequest = new markForCaptureType();

            // TODO: OrbitalConnectionUsername / OrbitalConnectionPassword

            //OrderID : Merchant Defined Order Number:
            // first 8 characters should be unique
            captureRequest.OrderID = orderNumber.ToString();

            // Amount: Transaction Amount, this is a whole integer including implied decimal
            // in other words, $100.00 should be transmitted as 10000
            captureRequest.Amount = ((int)(captureAmount * 100)).ToString();

            // BIN: Transaction Routing Definition.  000001 = Salem, 000002 = Tampa
            captureRequest.BIN = this.RoutingBin;

            // MerchantID: Gateway merchant account number assigned by Chase Paymentech.
            // 6 digits for Salem, 12 digits for Tampa.
            captureRequest.MerchantID = this.MerchantId;

            //TerminalID: Merchant Terminal ID assigned by Chase Paymentech
            // For Salem, must be 001.  For Tampa can be 001 to 999, but 001 is standard.
            captureRequest.TerminalID = this.TerminalId;

            //TxRefNum: Gateway transaction reference number
            captureRequest.TxRefNum = txRefNum;

            // CREATE AND RETURN THE REQUEST
            Request request = new Request();
            request.Item = captureRequest;
            return request;
        }

        private Request BuildVoidRequest(Transaction authTrans, LSDecimal voidAmount, int orderNumber)
        {
            // CREATE THE ORDER CONTAINER
            reversalType reversalRequest = new reversalType();

            // TODO: OrbitalConnectionUsername / OrbitalConnectionPassword

            //TxRefNum: Gateway transaction reference number
            reversalRequest.TxRefNum = authTrans.ProviderTransactionId;

            // Amount: Transaction Amount, this is a whole integer including implied decimal
            // in other words, $100.00 should be transmitted as 10000
            reversalRequest.AdjustedAmt = ((int)(voidAmount * 100)).ToString();

            //OrderID : Merchant Defined Order Number:
            // first 8 characters should be unique
            reversalRequest.OrderID = orderNumber.ToString();

            // BIN: Transaction Routing Definition.  000001 = Salem, 000002 = Tampa
            reversalRequest.BIN = this.RoutingBin;

            // MerchantID: Gateway merchant account number assigned by Chase Paymentech.
            // 6 digits for Salem, 12 digits for Tampa.
            reversalRequest.MerchantID = this.MerchantId;

            //TerminalID: Merchant Terminal ID assigned by Chase Paymentech
            // For Salem, must be 001.  For Tampa can be 001 to 999, but 001 is standard.
            reversalRequest.TerminalID = this.TerminalId;

            // CREATE AND RETURN THE REQUEST
            Request request = new Request();
            request.Item = reversalRequest;
            return request;
        }

        // THIS METHOD IS NEARLY IDENTICAL TO BUILDAUTHREQUEST
        // ONLY DIFFERENCES ARE TO USE A REFUNDTRANSACTIONREQUEST PARAMETER AND TO SET MESSAGE TYPE TO R
        private Request BuildRefundRequest(Order order, PaymentData paymentData, RefundTransactionRequest refundRequest)
        {
            // CREATE THE ORDER CONTAINER
            newOrderType newOrder = new newOrderType();

            // TODO: OrbitalConnectionUsername / OrbitalConnectionPassword

            // IndustryType: Defines the Industry type of the transaction
            // EC: eCommerce transaction
            newOrder.IndustryType = validindustrytypes.EC;

            // MessageType: Defines the transaction type
            // R = Reversal
            newOrder.MessageType = validtranstypes.R;

            // BIN: Transaction Routing Definition.  000001 = Salem, 000002 = Tampa
            newOrder.BIN = this.RoutingBin;

            // MerchantID: Gateway merchant account number assigned by Chase Paymentech.
            // 6 digits for Salem, 12 digits for Tampa.
            newOrder.MerchantID = this.MerchantId;

            //TerminalID: Merchant Terminal ID assigned by Chase Paymentech
            // For Salem, must be 001.  For Tampa can be 001 to 999, but 001 is standard.
            newOrder.TerminalID = this.TerminalId;

            //CardBrand: Defines the Card Type / Brand for the Transaction:
            //Required for: SW – Switch / Solo, EC – Electronic Check
            if (paymentData.CardBrand != CardBrand.CC)
            {
                newOrder.CardBrand = paymentData.CardBrand.ToString();
            }

            // (AccountNum, Exp, CardSecVal, etc...)
            BuildAuthRequest_SetPaymentDataData(newOrder, paymentData);

            //CurrencyCode: Defines the transaction currency:
            //Bin 000002 only supports the US Dollar [‘840’] and Canadian Dollar [‘124’].
            string currencyCode = this.CurrencyCode.ToString("000");
            newOrder.CurrencyCode = currencyCode;
            newOrder.CurrencyExponent = CurrencyCodes.GetExponent(this.CurrencyCode).ToString();

            // SET THE AVS DATA FOR THIS REQUEST
            BuildAuthRequest_SetAVSData(newOrder, order);

            //OrderID : Merchant Defined Order Number:
            // first 8 characters should be unique
            newOrder.OrderID = order.OrderNumber.ToString();
            
            // Amount: Transaction Amount, this is a whole integer including implied decimal
            // in other words, $100.00 should be transmitted as 10000
            newOrder.Amount = ((int)(refundRequest.Amount * 100)).ToString();

            // CREATE AND RETURN THE REQUEST
            Request request = new Request();
            request.Item = newOrder;
            return request;
        }

        #endregion

        /// <summary>
        /// Sends the request object to the gateway
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="sensitiveData">Any sensitive data replacements that should be processed for debug data.  Can be null.</param>
        /// <returns>The response returned by the gateway</returns>
        private Response SendRequestToGateway(Request request, Dictionary<string, string> sensitiveData)
        {
            // SERIALIZE THE REQUEST FOR TRANSMISSION
            byte[] requestBytes = SerializeRequest(request);

            // DETERMINE THE DESTINATION URL
            string destUrl = this.UseTestMode ? PRIMARY_TESTURL : PRIMARY_LIVEURL;

            // FOR DEBUG, RECORD THE SEND
            if (this.UseDebugMode)
            {
                RecordCommunication("PaymentechOrbital", CommunicationDirection.Send, destUrl + "\r\n" + Encoding.UTF8.GetString(requestBytes), sensitiveData);
            }

            // CREATE THE WEB REQUEST
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(destUrl);
            httpWebRequest.Method = "POST";            
            httpWebRequest.ContentType = "application/PTI46";
            httpWebRequest.Headers.Add("MIME-Version", "1.1");
            httpWebRequest.Headers.Add("Content-transfer-encoding", "text");
            httpWebRequest.Headers.Add("Request-number", "1");
            httpWebRequest.Headers.Add("Document-type", "Request");
            httpWebRequest.Headers.Add("Interface-Version", "Ablecommerce 7");
            httpWebRequest.ContentLength = requestBytes.Length;

            // SEE IF WE CAN SET THE REFERRER FOR THE REQUEST
            HttpRequest httpRequestContext = HttpContextHelper.SafeGetRequest();
            if (httpRequestContext != null)
            {
                string referer = httpRequestContext.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }

            // SEND THE REQUEST
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }

            // READ RESPONSEDATA FROM STREAM
            string responseData;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), Encoding.UTF8))
            {
                responseData = responseStream.ReadToEnd();
                responseStream.Close();
            }

            // FOR DEBUG, RECORD THE RECEIVE (THE XML RETURNED BY THE GATEWAY IS NOT PRETTY PRINTED)
            if (this.UseDebugMode)
            {
                RecordCommunication("PaymentechOrbital", CommunicationDirection.Receive, FormatXml(responseData), sensitiveData);
            }

            // DESERIALIZE THE RESPONSE OBJECT
            Response response = (Response)XmlUtility.Deserialize(responseData, typeof(Response));

            // RETURN THE RESPONSE 
            return response;
        }

        /// <summary>
        /// Attempts to reformat an XML document for visual appeal
        /// </summary>
        /// <param name="xml">XML document string</param>
        /// <returns>Formatted XML document string</returns>
        private string FormatXml(string xml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlTextWriter tw = new XmlTextWriter(sw))
                    {
                        tw.Formatting = Formatting.Indented;
                        tw.Indentation = 2;
                        tw.IndentChar = ' ';
                        xmlDoc.WriteTo(tw);
                        tw.Flush();
                    }
                    return sw.ToString();
                }
            }
            catch
            {
                // SOMETHING WENT WRONG, JUST ECHO THE INPUT
                return xml;
            }
        }

        private static void WriteCertData(string timestamp, string merchantId, string orderId, string currency, string phone, string name, string address, string authCode, string avsCode, string txRefNum, string batch)
        {
            string newtimestamp = DateTime.Now.ToShortDateString() + " " + timestamp.Substring(0, 2) + ":" + timestamp.Substring(2, 2) + ":" + timestamp.Substring(4, 2);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\PaymenTech_Cert.csv");
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("Date/Time,MerchantId,OrderId,IndustryType,Currency,Phone,Name,Address,PurchCard2,AuthCode,AvsCode,TxRefNum,Batch");
                    sw.Flush();
                    sw.Close();
                }
            }
            using (StreamWriter sw = File.AppendText(filePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(newtimestamp);
                sb.Append(",");
                sb.Append(merchantId);
                sb.Append(",");
                sb.Append(orderId);
                sb.Append(",");
                sb.Append("EC");
                sb.Append(",");
                sb.Append(currency);
                sb.Append(",");
                sb.Append(phone);
                sb.Append(",");
                sb.Append(name);
                sb.Append(",");
                sb.Append(address);
                sb.Append(",");
                sb.Append(string.Empty);
                sb.Append(",");
                sb.Append(authCode);
                sb.Append(",");
                sb.Append(avsCode);
                sb.Append(",");
                sb.Append(txRefNum);
                sb.Append(",");
                sb.Append(batch);
                sw.WriteLine(sb.ToString());
                sw.Flush();
                sw.Close();
            }
        }
    }
}
