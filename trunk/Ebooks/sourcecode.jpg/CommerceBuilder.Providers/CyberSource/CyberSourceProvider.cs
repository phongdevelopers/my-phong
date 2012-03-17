using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CyberSource.Clients;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Design;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using CommerceBuilder.Common;
using CyberSource.Clients.SoapWebReference;

namespace CommerceBuilder.Payments.Providers.CyberSource
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        private const string LIVEURL = "https://ics2ws.ic3.com/commerce/1.x/transactionProcessor";
        private const string TESTURL = "https://ics2wstest.ic3.com/commerce/1.x/transactionProcessor";

        private const String LIB_VERSION = "3.0";  // WSE version
        private const String POLICY_NAME = "CyberSource";

        string _MerchantId = string.Empty;
        string _TransactionKey = string.Empty;

        bool _UseAuthCapture = false;
        bool _UseTestMode = true;
        bool _IgnoreAVS = false;

        public override string Name
        {
            get { return "CyberSource"; }
        }

        public override string Description
        {
            get { return "CyberSource provides fast, reliable, and secure electronic credit card processing."; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.CyberSource.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "SOAP Toolkit API 1.29"; }
        }

        public string MerchantId
        {
            get { return _MerchantId; }
            set { _MerchantId = value; }
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

        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        public bool IgnoreAVS
        {
            get { return _IgnoreAVS; }
            set { _IgnoreAVS = value; }
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.Refund
                    | SupportedTransactions.Void
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
            gatewayLink.NavigateUrl = "http://www.cybersource.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable CyberSource, you must provide your Merchant Id.  You will also need to have your transaction security key generated from the CyberSource merchant interface. </p>"));
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

            //GET THE MERCHANT ID
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Id:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your CyberSource Merchant Id is required.</span>"));
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Transaction Key is the Security Key for the SOAP Toolkit API. You can generate this key from Cybersource merchant interface.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantKey = new TextBox();
            txtMerchantKey.ID = "Config_TransactionKey";
            txtMerchantKey.Columns = 40;
            txtMerchantKey.Rows = 5;
            txtMerchantKey.TextMode = TextBoxMode.MultiLine;
            txtMerchantKey.Text = this.TransactionKey;
            currentCell.Controls.Add(txtMerchantKey);
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

            //GET THE IGNORE AVS RESULT SETTING
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Ignore AVS:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">CyberSource performs an AVS check and may automatically deny payments based on the result.  Check this box to authorize these payments anyway.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            CheckBox cbIgnoreAVS = new CheckBox();
            cbIgnoreAVS.ID = "Config_IgnoreAVS";
            cbIgnoreAVS.Checked = this.IgnoreAVS;
            currentCell.Controls.Add(cbIgnoreAVS);
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure CyberSource to run live or test transactions.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblUseTestMode = new RadioButtonList();
            rblUseTestMode.ID = "Config_UseTestMode";
            rblUseTestMode.Items.Add(new ListItem("Live Mode", false.ToString()));
            rblUseTestMode.Items.Add(new ListItem("Test Mode", true.ToString()));
            rblUseTestMode.Items[this.UseTestMode ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblUseTestMode);
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between your store and CyberSource is recorded in the \"App_Data\\logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
            //Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //REQUEST AUTH/CAPTURE IF SO CONFIGURED
            if (this.UseAuthCapture) authorizeRequest.Capture = true;
            //BUILD THE REQUEST            
            RequestMessage request = BuildProviderRequest_Authorize(authorizeRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, GetSendDebugData(request,CybRequestType.Authorize) , null);
            //SEND REQUEST
            ReplyMessage reply = this.SendRequest(request);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, GetReceiveDebugData(reply,CybRequestType.Authorize) , null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Authorize(authorizeRequest, reply);
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            //BUILD THE REQUEST
            RequestMessage request = BuildProviderRequest_Capture(captureRequest);            
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, GetSendDebugData(request,CybRequestType.Capture), null);
            //SEND REQUEST
            ReplyMessage reply = this.SendRequest(request);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, GetReceiveDebugData(reply, CybRequestType.Capture), null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Capture(captureRequest, reply);
        }

        public override Transaction DoRefund(RefundTransactionRequest refundRequest)
        {
            //BUILD THE REQUEST
            RequestMessage request = BuildProviderRequest_Refund(refundRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, GetSendDebugData(request,CybRequestType.Refund), null);
            //SEND REQUEST
            ReplyMessage reply = this.SendRequest(request);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, GetReceiveDebugData(reply, CybRequestType.Refund), null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Refund(refundRequest, reply);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            //BUILD THE REQUEST
            RequestMessage request = BuildProviderRequest_Void(voidRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, GetSendDebugData(request, CybRequestType.Void), null);
            //SEND REQUEST
            ReplyMessage reply = this.SendRequest(request);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, GetReceiveDebugData(reply, CybRequestType.Void), null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessProviderResponse_Void(voidRequest, reply);
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("MerchantId")) MerchantId = ConfigurationData["MerchantId"];
            if (ConfigurationData.ContainsKey("TransactionKey")) TransactionKey = ConfigurationData["TransactionKey"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], false);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("IgnoreAVS")) IgnoreAVS = ConfigurationData["IgnoreAVS"].ToLowerInvariant().Equals("on");
        }

        #region BuildRequest

        private RequestMessage BuildProviderRequest_Authorize(AuthorizeTransactionRequest authorizeRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            //GENERATE REQUEST
            RequestMessage request = new RequestMessage();
            SetBasicInfo(request);
            //request.merchantReferenceCode = Guid.NewGuid().ToString();
            request.merchantReferenceCode = order.OrderNumber.ToString();

            request.ccAuthService = new CCAuthService();
            request.ccAuthService.run = "true";
            request.ccAuthService.commerceIndicator = "internet";

            if (authorizeRequest.Capture)
            {
                request.ccCaptureService = new CCCaptureService();
                request.ccCaptureService.run = "true";
            }

            SetBillingInfo(request, order, accountData, authorizeRequest.RemoteIP);

            request.item = new Item[1];
            Item item = new Item();
            item.id = "0";
            item.unitPrice = authorizeRequest.Amount.ToString();
            request.item[0] = item;

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.grandTotalAmount = authorizeRequest.Amount.ToString();
            purchaseTotals.currency = authorizeRequest.CurrencyCode;
            request.purchaseTotals = purchaseTotals;

            SetCCInfo(request, accountData, payment.PaymentMethod);

            if (this.IgnoreAVS)
            {
                request.businessRules = new BusinessRules();
                request.businessRules.ignoreAVSResult = "true";
            }

            return request;
        }

        private RequestMessage BuildProviderRequest_Capture(CaptureTransactionRequest captureRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("transactionRequest.Payment");
            Transaction authorizeTransaction = captureRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            //GENERATE REQUEST
            RequestMessage request = new RequestMessage();
            SetBasicInfo(request);
            //request.merchantReferenceCode = Guid.NewGuid().ToString();
            request.merchantReferenceCode = order.OrderNumber.ToString();

            request.ccCaptureService = new CCCaptureService();
            request.ccCaptureService.run = "true";            
            request.ccCaptureService.authRequestID = authorizeTransaction.ProviderTransactionId;

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.grandTotalAmount = captureRequest.Amount.ToString();
            purchaseTotals.currency = captureRequest.CurrencyCode;
            request.purchaseTotals = purchaseTotals;

            if (this.IgnoreAVS)
            {
                request.businessRules = new BusinessRules();
                request.businessRules.ignoreAVSResult = "true";
            }
            return request;
        }

        private RequestMessage BuildProviderRequest_Void(VoidTransactionRequest voidRequest)
        {
            //GENERATE REQUEST
            RequestMessage request = new RequestMessage();
            SetBasicInfo(request);
            //request.merchantReferenceCode = Guid.NewGuid().ToString();
            request.merchantReferenceCode = voidRequest.Payment.Order.OrderNumber.ToString();

            request.ccAuthReversalService = new CCAuthReversalService();
            request.ccAuthReversalService.run = "true";
            request.ccAuthReversalService.authRequestID = voidRequest.AuthorizeTransaction.ProviderTransactionId;

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.grandTotalAmount = voidRequest.Amount.ToString();
            purchaseTotals.currency = voidRequest.CurrencyCode;
            request.purchaseTotals = purchaseTotals;

            return request;
        }

        private RequestMessage BuildProviderRequest_Refund(RefundTransactionRequest creditRequest)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("creditRequest.Payment");
            Transaction captureTransaction = creditRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("refund.CaptureTransaction");

            //GENERATE REQUEST
            RequestMessage request = new RequestMessage();
            SetBasicInfo(request);
            //request.merchantReferenceCode = Guid.NewGuid().ToString();
            request.merchantReferenceCode = creditRequest.Payment.Order.OrderNumber.ToString();

            request.ccCreditService = new CCCreditService();
            request.ccCreditService.run = "true";
            request.ccCreditService.captureRequestID = creditRequest.CaptureTransaction.ProviderTransactionId;

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.grandTotalAmount = creditRequest.Amount.ToString();
            purchaseTotals.currency = creditRequest.CurrencyCode;
            request.purchaseTotals = purchaseTotals;

            return request;
        }

        #endregion

        #region ProcessResponse

        private Transaction ProcessProviderResponse_Authorize(AuthorizeTransactionRequest authorizeRequest, ReplyMessage reply)
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
            transaction.ProviderTransactionId = reply.requestID;
            if (reply.ccAuthReply != null)
            {
                transaction.TransactionDate = AlwaysConvert.ToDateTime(reply.ccAuthReply.authorizedDateTime, DateTime.UtcNow).ToUniversalTime();
                transaction.Amount = AlwaysConvert.ToDecimal(reply.ccAuthReply.amount, (Decimal)authorizeRequest.Amount);
            }
            else
            {
                transaction.Amount = authorizeRequest.Amount;
            }
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            else
            {
                transaction.RemoteIP = authorizeRequest.RemoteIP;
            }

            //CHECK THE TRANSACTION RESULT
            string decision = AlwaysConvert.ToString(reply.decision).ToUpper();
            int reasonCode = AlwaysConvert.ToInt(reply.reasonCode);
            transaction.ResponseCode = reasonCode.ToString();
            transaction.ResponseMessage = TranslateResponseCode(reasonCode, reply);
            if (decision.Equals("ACCEPT"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.AuthorizationCode = reply.ccAuthReply.authorizationCode;
                transaction.AVSResultCode = reply.ccAuthReply.avsCode;
                if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = GetCvvCode(reply.ccAuthReply.cvCode);
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private string GetCvvCode(string cvvResult)
        {
            switch (cvvResult)
            {
                case "M":
                case "N":
                case "S":
                case "I":
                case "U":
                case "P":
                    return cvvResult;
            }
            return "X";
        }

        private Transaction ProcessProviderResponse_Capture(CaptureTransactionRequest captureRequest, ReplyMessage reply)
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
            transaction.ProviderTransactionId = reply.requestID;
            if (reply.ccCaptureReply != null)
            {
                transaction.TransactionDate = AlwaysConvert.ToDateTime(reply.ccCaptureReply.requestDateTime, DateTime.UtcNow).ToUniversalTime();
                transaction.Amount = AlwaysConvert.ToDecimal(reply.ccCaptureReply.amount, (Decimal)captureRequest.Amount);
            }
            else
            {
                transaction.Amount = captureRequest.Amount;
            }
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            else
            {
                transaction.RemoteIP = captureRequest.RemoteIP;
            }
            //CHECK THE TRANSACTION RESULT
            string decision = AlwaysConvert.ToString(reply.decision).ToUpper();
            int reasonCode = AlwaysConvert.ToInt(reply.reasonCode);
            transaction.ResponseCode = reasonCode.ToString();
            transaction.ResponseMessage = TranslateResponseCode(reasonCode, reply);
            if (decision.Equals("ACCEPT"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.AuthorizationCode = AlwaysConvert.ToString(reply.ccCaptureReply.reconciliationID);
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessProviderResponse_Void(VoidTransactionRequest voidRequest, ReplyMessage reply)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            transaction.ProviderTransactionId = reply.requestID;

            if (reply.voidReply != null)
            {
                transaction.TransactionDate = AlwaysConvert.ToDateTime(reply.voidReply.requestDateTime, DateTime.UtcNow).ToUniversalTime();
                transaction.Amount = AlwaysConvert.ToDecimal(reply.voidReply.amount, (Decimal)voidRequest.Amount);
            }
            else
            {
                transaction.Amount = voidRequest.Amount;
            }
            transaction.RemoteIP = voidRequest.RemoteIP;
            string decision = AlwaysConvert.ToString(reply.decision).ToUpper();
            int reasonCode = AlwaysConvert.ToInt(reply.reasonCode);
            transaction.ResponseCode = reasonCode.ToString();
            transaction.ResponseMessage = TranslateResponseCode(reasonCode, reply);

            bool successful = decision.Equals("ACCEPT");
            if (successful)
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
            }

            return transaction;
        }

        private Transaction ProcessProviderResponse_Refund(RefundTransactionRequest refundRequest, ReplyMessage reply)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Refund;
            transaction.ProviderTransactionId = reply.requestID;

            if (reply.ccCreditReply != null)
            {
                transaction.TransactionDate = AlwaysConvert.ToDateTime(reply.ccCreditReply.requestDateTime, DateTime.UtcNow).ToUniversalTime();
                transaction.Amount = AlwaysConvert.ToDecimal(reply.ccCreditReply.amount, (Decimal)refundRequest.Amount);
            }
            else
            {
                transaction.Amount = refundRequest.Amount;
            }
            transaction.RemoteIP = refundRequest.RemoteIP;
            string decision = AlwaysConvert.ToString(reply.decision).ToUpper();
            int reasonCode = AlwaysConvert.ToInt(reply.reasonCode);
            transaction.ResponseCode = reasonCode.ToString();
            transaction.ResponseMessage = TranslateResponseCode(reasonCode, reply);

            bool successful = decision.Equals("ACCEPT");
            if (successful)
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }
        #endregion

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();
            AuthorizeTransactionRequest authRequest;

            // VALIDATE THE PAYMENT PERIOD
            string payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == string.Empty)
            {
                // THE PAYMENT PERIOD IS INVALID
                Transaction errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The specified payment interval is not valid for this processor.");
                return new AuthorizeRecurringTransactionResponse(errTrans);
            }

            // SEE IF THERE IS A DIFFERENT INITIAL PAYMENT AND RECURRING AMOUNT
            // AND IF SO, SEE IF THE INITIAL PAYMENT IS GREATER THAN ZERO
            if (authorizeRequest.RecurringChargeSpecified && authorizeRequest.Amount >0)
            {
                // AN INITIAL PAYMENT REQUEST MUST BE RUN
                authRequest = new AuthorizeTransactionRequest(authorizeRequest.Payment, authorizeRequest.RemoteIP);
                authRequest.Amount = authorizeRequest.Amount;
                authRequest.Capture = true;
                Transaction initialTransaction = DoAuthorize(authRequest);

                // IF OUR TRAIL PAYMENT FAILS, WE NEED TO EXIT THIS SEQUENCE
                if (initialTransaction.TransactionStatus != TransactionStatus.Successful)
                {
                    initialTransaction.TransactionType = TransactionType.AuthorizeRecurring;
                    return new AuthorizeRecurringTransactionResponse(initialTransaction);
                }

                // OUR TRIAL PAYMENT SUCCEEDED, SO ADD TO THE COLLECTION
                response.AddTransaction(initialTransaction);
            }
          
            //BUILD REQUEST
            RequestMessage request = BuildProviderRequest_RecurAuthorize(authorizeRequest, payPeriod);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, GetSendDebugData(request, CybRequestType.RecurringAuthorize), null);
            //SEND REQUEST
            ReplyMessage reply = this.SendRequest(request);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, GetReceiveDebugData(reply, CybRequestType.RecurringAuthorize), null);
            Transaction authRecTrans = this.ProcessProviderResponse_RecAuthorize(authorizeRequest, reply, TransactionType.AuthorizeRecurring);

            response.Status = authRecTrans.TransactionStatus;
            response.AddTransaction(authRecTrans);
            return response;
        }

        private RequestMessage BuildProviderRequest_RecurAuthorize(AuthorizeRecurringTransactionRequest authorizeRequest, string payPeriod)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            // SET THE MERCHANT LEVEL DATA
            RequestMessage request = new RequestMessage();
            SetBasicInfo(request);
            request.merchantReferenceCode = order.OrderNumber.ToString();

            // SET THE BILLING AND PAYMENT DATA
            SetBillingInfo(request, order, accountData, authorizeRequest.RemoteIP);
            SetCCInfo(request, accountData, payment.PaymentMethod);

            // INITIALIZE THE PURCHASE TOTAL (NO SETUP FEE)
            request.purchaseTotals = new PurchaseTotals();
            request.purchaseTotals.grandTotalAmount = "0.00";
            request.purchaseTotals.currency = authorizeRequest.CurrencyCode;

            // DETERMINE THE RECURRING CHARGE, START DATE AND NUMBER OF PAYMENTS
            LSDecimal recurringCharge = authorizeRequest.Amount;
            int remainingPayments = authorizeRequest.NumberOfPayments;
            DateTime subscriptionStartDate = LocaleHelper.LocalNow;
            if (authorizeRequest.RecurringChargeSpecified)
            {
                recurringCharge = authorizeRequest.RecurringCharge;
                remainingPayments--;
                subscriptionStartDate = GetNextPaymentDate(payPeriod);

                /*
                // WE ARE HANDLING THIS WITH A SEPARATE AUTH TRANSACTION
                // INCLUDE A SETUP FEE / INITIAL CHARGE
                if (authorizeRequest.Amount > 0)
                {
                    // CONFIGURE THE ITEM
                    request.item = new Item[1];
                    Item item = new Item();
                    item.productName = authorizeRequest.SubscriptionName;
                    item.unitPrice = authorizeRequest.Amount.ToString("F2");
                    request.item[0] = item;
                    // SET THE TOTAL
                    request.purchaseTotals.grandTotalAmount = item.unitPrice;
                }
                */
            }

            // RUN THE PAYMENT SUBSCRIPTION SERVICE
            request.paySubscriptionCreateService = new PaySubscriptionCreateService();
            request.paySubscriptionCreateService.run = "true";

            // SET UP THE SUBSCRIPTION PROFILE
            request.subscription = new global::CyberSource.Clients.SoapWebReference.Subscription();
            request.subscription.title = authorizeRequest.SubscriptionName;
            request.subscription.paymentMethod = "credit card";

            // PROVIDE THE RECURRING SUBSCRIPTION DETAILS
            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.amount = recurringCharge.ToString("F2");
            request.recurringSubscriptionInfo.numberOfPayments = remainingPayments.ToString();
            request.recurringSubscriptionInfo.frequency = payPeriod;
            request.recurringSubscriptionInfo.automaticRenew = "false";
            request.recurringSubscriptionInfo.startDate = subscriptionStartDate.ToString("yyyyMMdd");

            return request;
        }

        private Transaction ProcessProviderResponse_RecAuthorize(AuthorizeRecurringTransactionRequest authorizeRequest, ReplyMessage reply, TransactionType transType)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transType;
            //SET TRANSACTION ELEMENTS COMMON TO ALL RESULTS
            transaction.ProviderTransactionId = reply.requestID;
            transaction.Amount = (authorizeRequest.RecurringChargeSpecified ? authorizeRequest.RecurringCharge : authorizeRequest.Amount);
            //CHECK THE TRANSACTION RESULT
            string decision = AlwaysConvert.ToString(reply.decision).ToUpper();
            int reasonCode = AlwaysConvert.ToInt(reply.reasonCode);
            transaction.ResponseCode = reasonCode.ToString();
            transaction.ResponseMessage = TranslateResponseCode(reasonCode, reply);
            if (decision.Equals("ACCEPT"))
            {
                //PROCESS SUCCESS
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = AlwaysConvert.ToString(reply.paySubscriptionCreateReply.subscriptionID);
            }
            else
            {
                //PROCESS ERROR
                transaction.TransactionStatus = TransactionStatus.Failed;
            }

            return transaction;
        }

        private void SetBasicInfo(RequestMessage request)
        {
            request.clientApplication = "AbleCommerce";
            request.clientApplicationVersion = "7.0";
            request.merchantID = this.MerchantId;
            request.clientLibrary = ".NET C# WSE";
            request.clientLibraryVersion = LIB_VERSION;
            request.clientEnvironment = Environment.OSVersion.Platform +
                Environment.OSVersion.Version.ToString() + "-CLR" +
                Environment.Version.ToString();
        }

        private void SetBillingInfo(RequestMessage request, Order order, AccountDataDictionary accountData, string remoteIP)
        {
            BillTo billTo = new BillTo();
            if (accountData != null && accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                string[] names = accountData["AccountName"].Split(" ".ToCharArray());
                billTo.firstName = names[0];
                if (names.Length > 1)
                {
                    billTo.lastName = string.Join(" ", names, 1, names.Length - 1);
                }
            }
            else
            {
                billTo.firstName = order.BillToFirstName;
                billTo.lastName = order.BillToLastName;
            }

            billTo.customerID = order.UserId.ToString();
            billTo.street1 = order.BillToAddress1;
            billTo.street2 = order.BillToAddress2;
            billTo.city = order.BillToCity;
            billTo.state = order.BillToProvince;
            billTo.postalCode = order.BillToPostalCode;
            billTo.country = order.BillToCountryCode;
            billTo.phoneNumber = order.BillToPhone;
            billTo.email = order.BillToEmail;
            billTo.ipAddress = remoteIP;

            request.billTo = billTo;
        }

        private void SetCCInfo(RequestMessage request, AccountDataDictionary accountData, PaymentMethod payMethod)
        {
            // TODO: HANDLE ECHECK?
            if (!payMethod.IsCreditOrDebitCard())
            {
                throw new ArgumentException("The CyberSource integration does not support the requested payment instrument: " + payMethod.PaymentInstrument.ToString());
            }

            // POPULATE THE CREDIT CARD DATA
            Card card = new Card();
            card.cardType = PaymentProvider.GetCybCreditCardType(payMethod.PaymentInstrument);
            card.accountNumber = accountData.GetValue("AccountNumber");
            card.expirationMonth = accountData.GetValue("ExpirationMonth");
            if (card.expirationMonth.Length == 1) card.expirationMonth.Insert(0, "0");
            card.expirationYear = accountData.GetValue("ExpirationYear");
            card.cvNumber = accountData.GetValue("SecurityCode");

            // ADDITIONAL DATA NEEDED FOR MAESTRO / SOLO
            if (payMethod.PaymentInstrument == PaymentInstrument.SwitchSolo
                || payMethod.PaymentInstrument == PaymentInstrument.Maestro)
            {
                string issueNumber = accountData.GetValue("IssueNumber");
                if (!string.IsNullOrEmpty(issueNumber))
                {
                    card.issueNumber = issueNumber;
                }
                string startDateMonth = accountData.GetValue("StartDateMonth");
                string startDateYear = accountData.GetValue("StartDateYear");
                if (!string.IsNullOrEmpty(startDateMonth) && !string.IsNullOrEmpty(startDateYear))
                {
                    if (startDateMonth.Length == 1) startDateMonth.Insert(0, "0");
                    if (startDateYear.Length > 2) startDateYear = StringHelper.Right(startDateYear, 2);
                    card.startMonth = startDateMonth;
                    card.startYear = startDateYear;
                }
            }

            // SET THE REQUEST CARD DATA INSTANCE
            request.card = card;
        }

        private static string GetCybCreditCardType(PaymentInstrument instrument)
        {
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                    return "003";
                case PaymentInstrument.Discover:
                    return "004";
                case PaymentInstrument.MasterCard:
                    return "002";
                case PaymentInstrument.Visa:
                    return "001";
                case PaymentInstrument.DinersClub:
                    return "005";
                case PaymentInstrument.JCB:
                    return "007";
                case PaymentInstrument.Maestro:
                    return "042";
                case PaymentInstrument.SwitchSolo:
                    return "024";
                default:
                    return string.Empty;
            }
        }

        private string TranslateResponseCode(int reasonCode, ReplyMessage reply)
        {
            switch (reasonCode)
            {
                case 100:
                    //SUCCESS, RETURN EMPTY STRING AS NO MESSAGE IS NECESSARY
                    return string.Empty;
                case 101:
                    //Missing field(s)
                    return "The following required field(s) are missing:" + EnumerateValues(reply.missingField);
                case 102:
                    //Invalid field(s)
                    return "The following required field(s) are invalid:" + EnumerateValues(reply.invalidField);
                case 104:
                    //possible duplicate
                    return "The requested transaction appears to be a duplicate.";
                case 150:
                    return "Payment system failure. Please wait a few minutes and resend the request.";
                case 151:
                    return "Timeout error. Please wait a few minutes and resend the request.";
                case 152:
                    return "Timeout error. Please wait a few minutes and resend the request.";
                case 201:
                case 203:
                    //voice authorization required (201), general decline (203)
                    return "Card declined.  Please use a different card or select another form of payment.";
                case 202:
                    //expired card
                    return "Card expired.  Please use a different card or select another form of payment.";
                case 204:
                case 210:
                    //Insufficient funds
                    return "Insufficient funds in the account.  Please use a different card or select another form of payment.";
                case 205:
                    //STOLEN CARD!
                    return "Card declined.  Please use a different card or select another form of payment.";
                //SysUtils.LogEvent("Use of stolen or lost card attempted." & vbCrLf & "Instance: " & objToken.InstallPrefix & vbCrLf & "Order_ID: " & intOrderID, EventLogEntryType.Warning);
                case 207:
                    return "Issuing bank unavailable. Please wait a few minutes and try again.";
                case 208:
                    return "Card inactive or not authorized for card-not-present transactions.  Please use a different card or select another form of payment.";
                case 211:
                    return "Invalid card verification number.  Please check your entries and try again.";
                case 221:
                case 233:
                    //221 = processors negative file, 233 = general processor decline
                    return "Payment transaction declined.  Please use a different card or select another form of payment.";
                case 231:
                    return "Invalid account number.  Please use a different card or select another form of payment.";
                case 232:
                    return "Card type is not accepted.  Please use a different card or select another form of payment.";
                case 234:
                    return "There is a problem with your CyberSource merchant configuration. Possible action: Do not resend the request. Contact Customer Support to correct the configuration problem.";
                case 235:
                    return "The requested amount exceeds the originally authorized amount. Occurs, for example, if you try to capture an amount larger than the original authorization amount. This reason code applies if you are processing a capture through the API. Possible action: Issue a new authorization and capture request for the new amount.";
                case 236:
                    return "Payment processing system is unavailable. Please wait a few minutes and try again.";
                case 238:
                    return "The authorization has already been captured. This reason code applies if you are processing a capture through the API. Possible action: No action required.";
                case 239:
                    return "The requested transaction amount must match the previous transaction amount. Possible action: Correct the amount and resend the request.";
                case 240:
                    return "The card type sent is invalid or does not correlate with the credit card number. Possible action: Ask your customer to verify that the card is really the type that they indicated in your Web store, then resend the request.";
                case 241:
                    return "The request ID is invalid. This reason code applies when you are processing a capture or credit through the API. Possible action: Request a new authorization, and if successful, proceed with the capture.";
                case 242:
                    return "You requested a capture through the API, but there is no corresponding, unused authorization record. Occurs if there was not a previously successful authorization request or if the previously successful authorization has already been used by another capture request.";
                case 250:
                    return "Timeout error. Please wait a few minutes and resend the request.";
            }
            return "Unknown error occurred processing transaction. (Reason code: " + reasonCode + ")";
        }

        private string GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 7) return "weekly";
                    if (authorizeRequest.PaymentFrequency == 14) return "bi-weekly";
                    if (authorizeRequest.PaymentFrequency == 28) return "quad-weekly";
                    break;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return "monthly";
                    if (authorizeRequest.PaymentFrequency == 3) return "quarterly";
                    if (authorizeRequest.PaymentFrequency == 6) return "semi-annually";
                    if (authorizeRequest.PaymentFrequency == 12) return "annually";
                    break;
            }
            return string.Empty;
        }

        private DateTime GetNextPaymentDate(string payPeriod)
        {
            switch (payPeriod)
            {
                case "weekly":
                    return LocaleHelper.LocalNow.AddDays(7);
                case "bi-weekly":
                    return LocaleHelper.LocalNow.AddDays(14);
                case "quad-weekly":
                    return LocaleHelper.LocalNow.AddDays(28);
                case "monthly":
                    return LocaleHelper.LocalNow.AddMonths(1);
                case "quarterly":
                    return LocaleHelper.LocalNow.AddMonths(3);
                case "semi-annually":
                    return LocaleHelper.LocalNow.AddMonths(6);
                case "annually":
                    return LocaleHelper.LocalNow.AddYears(1);
            }
            throw new ArgumentException("The specified payPeriod (" + payPeriod + ")is not valid for this processor.", "payPeriod");
        }

        private ReplyMessage SendRequest(RequestMessage request)
        {
            string url = this.UseTestMode ? TESTURL : LIVEURL;
            TransactionProcessorWse proc = new TransactionProcessorWse(url);
            proc.SetPolicy(POLICY_NAME);
            proc.SetClientCredential<UsernameToken>(new UsernameToken(request.merchantID, this.TransactionKey, PasswordOption.SendPlainText));
            return proc.runTransaction(request);
        }

        private static string EnumerateValues(string[] fields)
        {
            if (fields == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string field in fields)
            {                
                sb.Append(field + "\r\n");
            }
            return sb.ToString();
        }

        private string GetSendDebugData(RequestMessage request, CybRequestType type)
        {
            StringBuilder debug = new StringBuilder();
            debug.Append("\r\nRequest Type: " + type.ToString());
            debug.Append("\r\nMerchant ID: " + MerchantId);
            debug.Append("\r\nPolicy Name: " + POLICY_NAME);
            debug.Append("\r\nVersion: " + Version);
            debug.Append("\r\nRequest URL: " + (this.UseTestMode ? TESTURL : LIVEURL));
            debug.Append("\r\nclient Application: " + request.clientApplication);
            debug.Append("\r\nclientApplicationVersion: " + request.clientApplicationVersion);
            debug.Append("\r\nclientLibrary: " + request.clientLibrary);
            debug.Append("\r\nclientLibraryVersion: " + request.clientLibraryVersion);
            debug.Append("\r\nclientEnvironment: " + request.clientEnvironment);
            debug.Append("\r\nmerchantReferenceCode: " + request.merchantReferenceCode);
            debug.Append("\r\npurchaseTotals.grandTotalAmount: " + request.purchaseTotals.grandTotalAmount);
            debug.Append("\r\nrequest.purchaseTotals.currency: " + request.purchaseTotals.currency);
            
            if(request.businessRules != null)
                debug.Append("\r\nrequest.businessRules.ignoreAVSResult: " + request.businessRules.ignoreAVSResult); 
            
            if (type == CybRequestType.Authorize)
            {
                if (request.ccAuthService != null)
                {
                    debug.Append("\r\nccAuthService.run: " + request.ccAuthService.run);
                    debug.Append("\r\nccAuthService.commerceIndicator: " + request.ccAuthService.commerceIndicator);
                }
                if (request.ccCaptureService != null)
                {
                    debug.Append("\r\nccCaptureService.run: " + request.ccCaptureService.run);
                }
            } 
            else if (type == CybRequestType.RecurringAuthorize)
            {
                debug.Append("\r\npaySubscriptionCreateService: " + request.paySubscriptionCreateService);
                debug.Append("\r\npaySubscriptionCreateService.run: " + request.paySubscriptionCreateService.run);
                debug.Append("\r\nsubscription.title: " + request.subscription.title);
                debug.Append("\r\nsubscription.paymentMethod: " + request.subscription.paymentMethod);
                debug.Append("\r\nrecurringSubscriptionInfo.frequency: " + request.recurringSubscriptionInfo.frequency);
                debug.Append("\r\nrecurringSubscriptionInfo.amount: " + request.recurringSubscriptionInfo.amount);
                debug.Append("\r\nrecurringSubscriptionInfo.numberOfPayments: " + request.recurringSubscriptionInfo.numberOfPayments);
                debug.Append("\r\nrecurringSubscriptionInfo.automaticRenew: " + request.recurringSubscriptionInfo.automaticRenew);
                debug.Append("\r\nrecurringSubscriptionInfo.startDate: " + request.recurringSubscriptionInfo.startDate);
            }
            else if (type == CybRequestType.Capture)
            {
                debug.Append("\r\nccCaptureService.run: " + request.ccCaptureService.run);
                debug.Append("\r\nccCaptureService.authRequestID: " + request.ccCaptureService.authRequestID);
            }
            else if (type == CybRequestType.Refund)
            {
                debug.Append("\r\nccCreditService.run: " + request.ccCreditService.run);
                debug.Append("\r\nccCreditService.captureRequestID: " + request.ccCreditService.captureRequestID);
            }
            else if (type == CybRequestType.Void)
            {
                debug.Append("\r\nrequest.ccAuthReversalService.run: " + request.ccAuthReversalService.run);
                debug.Append("\r\nccAuthReversalService.authRequestID: " + request.ccAuthReversalService.authRequestID);
            }

            if (type == CybRequestType.Authorize || type == CybRequestType.RecurringAuthorize)
            {
                //billing info
                debug.Append("\r\nbillTo.firstName: " + request.billTo.firstName);
                debug.Append("\r\nbillTo.lastName: " + request.billTo.lastName);
                debug.Append("\r\nbillTo.customerID: " + request.billTo.customerID);
                debug.Append("\r\nbillTo.street1: " + request.billTo.street1);
                debug.Append("\r\nbillTo.street2: " + request.billTo.street2);
                debug.Append("\r\nbillTo.city: " + request.billTo.city);
                debug.Append("\r\nbillTo.state: " + request.billTo.state);
                debug.Append("\r\nbillTo.postalCode: " + request.billTo.postalCode);
                debug.Append("\r\nbillTo.country: " + request.billTo.country);
                debug.Append("\r\nbillTo.phoneNumber: " + request.billTo.phoneNumber);
                debug.Append("\r\nbillTo.email: " + request.billTo.email);
                debug.Append("\r\nbillTo.ipAddress: " + request.billTo.ipAddress);

                //cc info                
                debug.Append("\r\ncard.accountNumber: " + MakeReferenceNumber(request.card.accountNumber));
                debug.Append("\r\ncard.expirationMonth: " + request.card.expirationMonth);
                debug.Append("\r\ncard.expirationYear: " + request.card.expirationYear);
                debug.Append("\r\ncard.cvNumber: " + "xxxx");

                if (request.item != null)
                {
                    int index = 0;
                    foreach (Item item in request.item)
                    {
                        debug.Append("\r\nitem[" + index + "].productName: " + item.productName);
                        debug.Append("\r\nitem[" + index + "].unitPrice: " + item.unitPrice);
                        index++;
                    }
                }
            }

            return debug.ToString();
        }

        private string GetReceiveDebugData(ReplyMessage reply, CybRequestType type)
        {
            StringBuilder debug = new StringBuilder();
            debug.Append("\r\nRequest Type: " + type.ToString());
            debug.Append("\r\nMerchant ID: " + MerchantId);
            debug.Append("\r\nPolicy Name: " + POLICY_NAME);
            debug.Append("\r\nVersion: " + Version);
            debug.Append("\r\nrequestID: " + reply.requestID);
            debug.Append("\r\ndecision: " + reply.decision);
            debug.Append("\r\nreasonCode: " + reply.reasonCode);

            if (type == CybRequestType.Authorize || type == CybRequestType.RecurringAuthorize)
            {
                if (reply.ccAuthReply != null)
                {
                    debug.Append("\r\nrccAuthReply.authorizedDateTime: " + reply.ccAuthReply.authorizedDateTime);
                    debug.Append("\r\nccAuthReply.amount: " + reply.ccAuthReply.amount);
                    debug.Append("\r\nccAuthReply.authorizationCode: " + reply.ccAuthReply.authorizationCode);
                    debug.Append("\r\nccAuthReply.avsCode: " + reply.ccAuthReply.avsCode);
                    debug.Append("\r\nccAuthReply.cvCode: " + reply.ccAuthReply.cvCode);
                }
            }
            else if (type == CybRequestType.Capture)
            {
                if (reply.ccCaptureReply != null)
                {
                    debug.Append("\r\nccCaptureReply.requestDateTime: " + reply.ccCaptureReply.requestDateTime);
                    debug.Append("\r\nccCaptureReply.amount: " + reply.ccCaptureReply.amount);
                }
            }
            else if (type == CybRequestType.Refund)
            {
                if (reply.ccCreditReply != null)
                {
                    debug.Append("\r\nccCreditReply.requestDateTime: " + reply.ccCreditReply.requestDateTime);
                    debug.Append("\r\nccCreditReply.amount: " + reply.ccCreditReply.amount);
                }
            }
            else if (type == CybRequestType.Void)
            {
                if (reply.ccAuthReversalReply != null)
                {
                    debug.Append("\r\nccAuthReversalReply.requestDateTime: " + reply.ccAuthReversalReply.requestDateTime);
                    debug.Append("\r\nccAuthReversalReply.amount: " + reply.ccAuthReversalReply.amount);
                    debug.Append("\r\nccAuthReversalReply.authorizationCode: " + reply.ccAuthReversalReply.authorizationCode);
                    debug.Append("\r\nccAuthReversalReply.processorResponse: " + reply.ccAuthReversalReply.processorResponse);
                }
            }

            if (type == CybRequestType.RecurringAuthorize)
            {
                if(reply.paySubscriptionCreateReply!=null) 
                {
                    debug.Append("\r\npaySubscriptionCreateReply.subscriptionID:" + reply.paySubscriptionCreateReply.subscriptionID);
                }
            }

            return debug.ToString();
        }

        private enum CybRequestType
        {
            Authorize, Capture, Refund, Void, RecurringAuthorize
        }
    }
}
