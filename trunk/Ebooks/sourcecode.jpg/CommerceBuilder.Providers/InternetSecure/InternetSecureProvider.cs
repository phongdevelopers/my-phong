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


namespace CommerceBuilder.Payments.Providers.InternetSecure
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        bool _UseTestMode = true;
        bool _IncludeOrderItems = false;
        bool _IsUSD = true;

        private string _MerchantNumber = "";

        public override string Name
        {
            get { return "InternetSecure"; }
        }

        public override string Description
        {
            get { return "InternetSecure Description here"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.InternetSecure.Logo.gif");
            return string.Empty;
        }
        public override string Version
        {
            get { return "Merchant Direct (2004-NOV-14)"; }
        }

        public bool IsUSD
        {
            get { return _IsUSD; }
            set { _IsUSD = value; }
        }

        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        public bool IncludeOrderItems
        {
            get { return _IncludeOrderItems; }
            set { _IncludeOrderItems = value; }
        }

        public string MerchantNumber
        {
            get { return _MerchantNumber; }
            set { _MerchantNumber = value; }
        }

        public override string ConfigReference
        {
            get { return MerchantNumber; }
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
            gatewayLink.NavigateUrl = "http://www.internetsecure.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable InternetSecure, you must provide your Merchant Number.  Also be aware that you must provide the IP address of your web server to InternetSecure.  Transactions will be rejected if they do not come from a registered IP.</p>"));
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

            //get Account Token
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Merchant Number:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your Internet Secure Merchant Number is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtMerchantNumber = new TextBox();
            txtMerchantNumber.ID = "Config_MerchantNumber";
            txtMerchantNumber.Columns = 70;
            txtMerchantNumber.MaxLength = 280;
            txtMerchantNumber.Text = this.MerchantNumber;
            currentCell.Controls.Add(txtMerchantNumber);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Currency Type
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Currency:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Select the Currency your InternetSecure account is setup for.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblCurrencyType = new RadioButtonList();
            rblCurrencyType.ID = "Config_IsUSD";
            rblCurrencyType.Items.Add(new ListItem("US Dollar (USD)", "true"));
            rblCurrencyType.Items.Add(new ListItem("Canadian Dollar (CAD)", "false"));
            rblCurrencyType.Items[IsUSD ? 0 : 1].Selected = true;
            currentCell.Controls.Add(rblCurrencyType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Include Order Items
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Include Order Items:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Include Order Items detail in the transaction request sent to provider.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblIncludeOrderItems = new RadioButtonList();
            rblIncludeOrderItems.ID = "Config_IncludeOrderItems";
            rblIncludeOrderItems.Items.Add(new ListItem("Yes", "true"));
            rblIncludeOrderItems.Items.Add(new ListItem("No", "false"));
            rblIncludeOrderItems.Items[IncludeOrderItems ? 0 : 1].Selected = true;
            currentCell.Controls.Add(rblIncludeOrderItems);
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

            currentCell.Controls.Add(new LiteralControl("<br />"));

            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and InternetSecure is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
                return (SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.RecurringBilling
                    | SupportedTransactions.Refund);
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("MerchantNumber")) MerchantNumber = ConfigurationData["MerchantNumber"];
            if (ConfigurationData.ContainsKey("IncludeOrderItems")) IncludeOrderItems = AlwaysConvert.ToBool(ConfigurationData["IncludeOrderItems"], false);
            if (ConfigurationData.ContainsKey("IsUSD")) IsUSD = AlwaysConvert.ToBool(ConfigurationData["IsUSD"], true);
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            VerifyGatewayConfig();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            String requestData = InitializeAuthRequest(payment, order, user, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            //send request and get response
            String responseData = SendRequest(requestData);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
            return ProcessResponse(payment, responseData, TransactionType.AuthorizeCapture);
        }

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            //VALIDATE THE PAYMENT PERIOD
            string payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == string.Empty)
            {
                Transaction errTrans = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The specified payment interval is not valid for this processor.");
                return new AuthorizeRecurringTransactionResponse(errTrans);
            }

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            String requestData = InitializeAuthRecurringRequest(authorizeRequest, payPeriod, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            //send request and get response
            String responseData = SendRequest(requestData);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);

            return ProcessRecurringResponse(authorizeRequest, responseData);
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            throw new Exception("Capture is not supported by the InternetSecure gateway.");
        }

        #region Refund
        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            VerifyGatewayConfig();
            //MAKE SURE WE HAVE THE VALID TRANSACTION ID TO PROCESS REFUND
            bool haveTransactionTokens = false;
            Transaction captureTransaction = creditRequest.CaptureTransaction;
            if (captureTransaction != null)
            {
                string authCode = captureTransaction.AuthorizationCode;
                haveTransactionTokens = authCode.Contains(":");
            }
            if (!haveTransactionTokens) 
                return Transaction.CreateErrorTransaction(this.PaymentGatewayId, TransactionType.Refund, creditRequest.Amount,  string.Empty, "AbleCommerce does not have enough information to process the refund request.", creditRequest.RemoteIP);
            //BUILD THE REFUND REQUEST
            string requestData = BuildRefundRequest(creditRequest);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, null);
            //SEND REQUEST AND GET RESPONE
            String responseData = SendRequest(requestData);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
            //PARSE RESPONSE AND RETURN RESULT
            return ProcessRefundResponse(responseData, creditRequest.Amount);
        }

        private string BuildRefundRequest(RefundTransactionRequest creditRequest)
        {
            //INITIALIZE REQUEST FLAGS
            //BUILD THE XML REQUEST
            StringBuilder refundXml = new StringBuilder();
            refundXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><TranxRequest>");
            refundXml.Append("<xxxTransType>11</xxxTransType>");
            refundXml.Append("<MerchantNumber>" + this.MerchantNumber + "</MerchantNumber>");
            refundXml.Append("<xxxInitialRequest>Y</xxxInitialRequest>");
            refundXml.Append("<xxxAttemptVoid>Y</xxxAttemptVoid>");
            refundXml.Append("</TranxRequest>");
            //RETURN THE REQUEST PROPERLY ENCODED INTO FORM DATA FOR POSTING
            Transaction captureTransaction = creditRequest.CaptureTransaction;
            StringBuilder formData = new StringBuilder();
            string receiptNumber = captureTransaction.ProviderTransactionId;
            string[] authCode = captureTransaction.AuthorizationCode.Split(":".ToCharArray());
            string txGuid = authCode[1];
            formData.Append("xxxRequestMode=X&TRX=" + receiptNumber + "&GUID=" + txGuid + "&xxxRequestData=" + HttpUtility.UrlEncode(refundXml.ToString()));
            return formData.ToString();
        }

        private Transaction ProcessRefundResponse(string responseData, LSDecimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Refund;
            //LOAD THE RESPONSE XML
            XmlDocument xmlresponse = new XmlDocument();
            xmlresponse.LoadXml(responseData);
            string resultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Page");
            transaction.TransactionStatus = (resultCode == "11000" ? TransactionStatus.Successful : TransactionStatus.Failed);
            transaction.ProviderTransactionId = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ReceiptNumber");
            transaction.AuthorizationCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ApprovalCode") + ":" + XmlUtility.GetElementValue(xmlresponse.DocumentElement, "GUID");
            transaction.TransactionDate = DateTime.UtcNow;
            string amount = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "TotalAmount");
            if (string.IsNullOrEmpty(amount)) transaction.Amount = transactionAmount;
            else transaction.Amount = AlwaysConvert.ToDecimal(amount, (decimal)transactionAmount);
            if (transaction.TransactionStatus != TransactionStatus.Successful)
            {
                string errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Error");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Verbiage");
                }
                transaction.ResponseCode = resultCode;
                transaction.ResponseMessage = errorMessage;
            }
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            return transaction;
        }
        #endregion



        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            throw new Exception("Void is not supported by the InternetSecure gateway.");
        }

        private string InitializeAuthRequest(Payment payment, Order order, User user, Dictionary<string, string> sensitiveData)
        {
            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><TranxRequest />");

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "MerchantNumber", MerchantNumber);

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            /*string sCurrencyCode = payment.CurrencyCode;
            if (!("USD".Equals(sCurrencyCode)))
            {
                sCurrencyCode = "CAD";
            }*/
            string sCurrencyCode = "CAD";
            if (IsUSD)
            {
                sCurrencyCode = "USD";
            }

            string ccFlag = "{" + sCurrencyCode + "}";
            string testFlag = (UseTestMode ? "{TEST}" : "");

            string sFlags = ccFlag + testFlag;
            string sDescription = "(None)";//order.Notes;
            string sAmount = String.Format("{0:F2}", payment.Amount);

            string pline;
            System.Collections.Generic.List<string> arrProducts = new System.Collections.Generic.List<string>();
            if (IncludeOrderItems)
            {
                OrderItemCollection orderItems = order.Items;
                if (orderItems != null && orderItems.Count > 0)
                {
                    foreach (OrderItem orderItem in orderItems)
                    {
                        if (orderItem.OrderItemType == OrderItemType.Product)
                        {
                            pline = "0.00::" + orderItem.Quantity + "::" + MakeSafe(orderItem.Sku, 30) + "::" + MakeSafe(orderItem.Name, 150) + "::" + sFlags;
                            arrProducts.Add(pline);
                        }
                    }
                }
            }

            pline = sAmount + "::1::" + order.OrderNumber + "::" + sDescription + "::" + sFlags;
            arrProducts.Add(pline);

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "Products", string.Join("|", (string[])arrProducts.ToArray()));
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxName", accountData["AccountName"]);
            }
            else
            {
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxName", order.BillToFirstName + " " + order.BillToLastName);
            }
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCompany", order.BillToCompany);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxAddress", order.BillToAddress1);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCity", order.BillToCity);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxProvince", order.BillToProvince);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxPostal", order.BillToPostalCode);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCountry", order.BillToCountryCode);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxPhone", order.BillToPhone);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxEmail", order.BillToEmail);

            SetupCreditCardData(xmlRequest, payment, accountData, sensitiveData);

            StringBuilder formData = new StringBuilder();
            formData.Append("xxxRequestMode=X&xxxRequestData=" + HttpUtility.UrlEncode(XmlToString(xmlRequest)));

            return formData.ToString();
        }

        private string InitializeAuthRecurringRequest(AuthorizeRecurringTransactionRequest authRequest, string payPeriod, Dictionary<string, string> sensitiveData)
        {
            Payment payment = authRequest.Payment;
            Order order = payment.Order;
            User user = order.User;

            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><TranxRequest />");

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "MerchantNumber", MerchantNumber);

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            /*string sCurrencyCode = payment.CurrencyCode;
            if (!("USD".Equals(sCurrencyCode)))
            {
                sCurrencyCode = "CAD";
            }*/
            string sCurrencyCode = "CAD";
            if (IsUSD)
            {
                sCurrencyCode = "USD";
            }

            string recurrFlag = "{RB ";
            LSDecimal recurAmount = authRequest.RecurringChargeSpecified ? authRequest.RecurringCharge : authRequest.Amount;
            string startDtString = GetNextPaymentDateStr(authRequest);
            string duration = ((int)(authRequest.NumberOfPayments - 1)).ToString();
            recurrFlag += recurAmount.ToString("F2") + " " + startDtString + " " + payPeriod + " " + duration + " email=2}";

            string ccFlag = "{" + sCurrencyCode + "}";
            string testFlag = (UseTestMode ? "{TEST}" : "");

            string sFlags = ccFlag + testFlag + recurrFlag;
            string sDescription = authRequest.SubscriptionName; //order.Notes;
            string sAmount = String.Format("{0:F2}", authRequest.Amount);

            string pline;
            System.Collections.Generic.List<string> arrProducts = new System.Collections.Generic.List<string>();
            if (IncludeOrderItems)
            {
                OrderItemCollection orderItems = order.Items;
                if (orderItems != null && orderItems.Count > 0)
                {
                    foreach (OrderItem orderItem in orderItems)
                    {
                        if (orderItem.OrderItemType == OrderItemType.Product)
                        {
                            pline = "0.00::" + orderItem.Quantity + "::" + MakeSafe(orderItem.Sku, 30) + "::" + MakeSafe(orderItem.Name, 150) + "::" + sFlags;
                            arrProducts.Add(pline);
                        }
                    }
                }
            }

            pline = sAmount + "::1::" + order.OrderNumber + "::" + sDescription + "::" + sFlags;
            arrProducts.Add(pline);

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "Products", string.Join("|", (string[])arrProducts.ToArray()));
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxName", accountData["AccountName"]);
            }
            else
            {
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxName", order.BillToFirstName + " " + order.BillToLastName);
            }

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCompany", order.BillToCompany);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxAddress", order.BillToAddress1);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCity", order.BillToCity);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxProvince", order.BillToProvince);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxPostal", order.BillToPostalCode);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCountry", order.BillToCountryCode);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxPhone", order.BillToPhone);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxEmail", order.BillToEmail);


            SetupCreditCardData(xmlRequest, payment, accountData, sensitiveData);

            StringBuilder formData = new StringBuilder();
            formData.Append("xxxRequestMode=X&xxxRequestData=" + HttpUtility.UrlEncode(XmlToString(xmlRequest)));

            return formData.ToString();
        }

        private string GetNextPaymentDateStr(AuthorizeRecurringTransactionRequest authRequest)
        {
            switch (authRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    return "startday=+" + authRequest.PaymentFrequency;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    return "startmonth=+" + authRequest.PaymentFrequency;
            }
            return "";
        }

        private string GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 1) return "daily";
                    if (authorizeRequest.PaymentFrequency == 7) return "weekly";
                    if (authorizeRequest.PaymentFrequency == 14) return "biweekly";
                    break;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return "monthly";
                    if (authorizeRequest.PaymentFrequency == 2) return "bimonthly";
                    if (authorizeRequest.PaymentFrequency == 3) return "quarterly";
                    if (authorizeRequest.PaymentFrequency == 6) return "semiannually";
                    if (authorizeRequest.PaymentFrequency == 12) return "annually";
                    break;
            }
            return string.Empty;
        }

        private String SendRequest(String requestData)
        {
            byte[] arrRequest = System.Text.Encoding.UTF8.GetBytes(requestData);

            HttpWebRequest srvXMLHttp;
            srvXMLHttp = ((HttpWebRequest)(WebRequest.Create("https://secure.internetsecure.com/process.cgi")));
            srvXMLHttp.Method = "POST";
            srvXMLHttp.ContentType = "application/x-www-form-urlencoded";

            HttpContext context = HttpContext.Current;
            string referer = null;
            if (context != null)
            {
                referer = context.Request.ServerVariables["HTTP_REFERER"];
            }
            if (referer != null && referer.Length > 0)
            {
                srvXMLHttp.Referer = referer;
            }

            srvXMLHttp.ContentLength = arrRequest.Length;
            Stream RequestStream = srvXMLHttp.GetRequestStream();
            RequestStream.Write(arrRequest, 0, arrRequest.Length);
            RequestStream.Close();

            StreamReader ResponseStream = new StreamReader(srvXMLHttp.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8);
            string sResponse = ResponseStream.ReadToEnd();
            ResponseStream.Close();
            return sResponse;
        }

        private void SetupCreditCardData(XmlDocument xmlRequest, Payment payment, AccountDataDictionary accountData, Dictionary<string, string> sensitiveData)
        {
            //set credit card data            
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
            string expirationYear = accountData.GetValue("ExpirationYear");

            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCard_Number", accountNumber);
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCCMonth", expirationMonth);
            XmlUtility.SetElementValue(xmlRequest.DocumentElement, "xxxCCYear", expirationYear);

            string securityCode = accountData.GetValue("SecurityCode");
            if (!string.IsNullOrEmpty(securityCode))
            {
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "CVV2Indicator", "0");
                XmlUtility.SetElementValue(xmlRequest.DocumentElement, "CVV2", securityCode);
                if (this.UseDebugMode)
                {
                    sensitiveData["CVV2%3E" + securityCode] = "CVV2%3E" + (new string('x', securityCode.Length));
                    sensitiveData["CVV2%3e" + securityCode] = "CVV2%3e" + (new string('x', securityCode.Length));
                }
            }
        }

        private Transaction ProcessResponse(Payment payment, String sResponse, TransactionType transactionType)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;

            XmlDocument xmlresponse = new XmlDocument();
            xmlresponse.LoadXml(sResponse);

            string resultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Page");
            transaction.TransactionStatus = ((resultCode == "2000") || (resultCode == "90000")) ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.ProviderTransactionId = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ReceiptNumber");
            transaction.TransactionDate = DateTime.UtcNow;
            string amount =XmlUtility.GetElementValue(xmlresponse.DocumentElement, "TotalAmount");
            if (string.IsNullOrEmpty(amount)) transaction.Amount = payment.Amount;
            else transaction.Amount = AlwaysConvert.ToDecimal(amount, (decimal)payment.Amount);
            if (transaction.TransactionStatus != TransactionStatus.Successful)
            {
                string errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Error");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Verbiage");
                }
                transaction.ResponseCode = resultCode;
                transaction.ResponseMessage = errorMessage;
            }
            transaction.AuthorizationCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ApprovalCode") + ":" + XmlUtility.GetElementValue(xmlresponse.DocumentElement, "GUID");
            transaction.AVSResultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "AVSResponseCode");
            if (string.IsNullOrEmpty(transaction.AVSResultCode)) transaction.AVSResultCode = "U";
            transaction.CVVResultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "CVV2ResponseCode");
            if (string.IsNullOrEmpty(transaction.CVVResultCode)) transaction.CVVResultCode= "X";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            return transaction;
        }

        private AuthorizeRecurringTransactionResponse ProcessRecurringResponse(AuthorizeRecurringTransactionRequest authRequest, String sResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.AuthorizeRecurring;
            LSDecimal transAmount = authRequest.RecurringChargeSpecified ? authRequest.RecurringCharge : authRequest.Amount;
            transaction.Amount = transAmount;

            XmlDocument xmlresponse = new XmlDocument();
            xmlresponse.LoadXml(sResponse);

            string resultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Page");
            transaction.TransactionStatus = ((resultCode == "2000") || (resultCode == "90000")) ? TransactionStatus.Successful : TransactionStatus.Failed;
            transaction.ProviderTransactionId = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ReceiptNumber");
            transaction.TransactionDate = DateTime.UtcNow;
            string amount = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "TotalAmount");
            if (string.IsNullOrEmpty(amount)) transaction.Amount = transAmount;
            else transaction.Amount = AlwaysConvert.ToDecimal(amount, (decimal)transAmount);
            if (transaction.TransactionStatus != TransactionStatus.Successful)
            {
                string errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Error");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "Verbiage");
                }
                transaction.ResponseCode = resultCode;
                transaction.ResponseMessage = errorMessage;
            }
            transaction.AuthorizationCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "ApprovalCode");
            transaction.AVSResultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "AVSResponseCode");
            if (string.IsNullOrEmpty(transaction.AVSResultCode)) transaction.AVSResultCode = "U";
            transaction.CVVResultCode = XmlUtility.GetElementValue(xmlresponse.DocumentElement, "CVV2ResponseCode");
            if (string.IsNullOrEmpty(transaction.CVVResultCode)) transaction.CVVResultCode = "X";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            return new AuthorizeRecurringTransactionResponse(transaction);
        }

        private void VerifyGatewayConfig()
        {
            if (string.IsNullOrEmpty(this.MerchantNumber))
            {
                throw new InvalidOperationException("Merchant Number missing");
            }
        }

        private string MakeSafe(string sInput, int iLen)
        {
            string sReturn = Utility.StringHelper.Replace(sInput, "|", "",StringComparison.InvariantCultureIgnoreCase);
            sReturn = Utility.StringHelper.Replace(sReturn, "\"", "", StringComparison.InvariantCultureIgnoreCase);
            sReturn = Utility.StringHelper.Replace(sReturn, "'", "", StringComparison.InvariantCultureIgnoreCase);
            sReturn = Utility.StringHelper.Replace(sReturn, ":", "", StringComparison.InvariantCultureIgnoreCase);
            return Utility.StringHelper.Truncate(sReturn, iLen);
        }

        private string CleanOptions(string sInput)
        {
            ArrayList arrOptions = new ArrayList();
            MatchCollection colMatches = Regex.Matches(sInput, "<span class=\"name\">([^<]*)</span>");
            //Match objMatch;
            foreach (Match objMatch in colMatches)
            {
                //arrOptions.Add(objMatch.Groups.Item(0).ToString());
                arrOptions.Add(objMatch.Groups.GetEnumerator().Current.ToString());
            }
            if (arrOptions.Count == 0)
            {
                return "";
            }
            return string.Join(",", (string[])arrOptions.ToArray());
        }

        private static string XmlToString(XmlDocument sLogData)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriterWithEncoding strWriter = new StringWriterWithEncoding(strBuilder, System.Text.Encoding.UTF8);
            System.Xml.XmlTextWriter objXMLWriter = new System.Xml.XmlTextWriter(strWriter);
            objXMLWriter.Formatting = Formatting.Indented;
            objXMLWriter.IndentChar = (char)9;
            objXMLWriter.Indentation = 1;
            sLogData.Save(objXMLWriter);
            objXMLWriter.Flush();
            objXMLWriter.Close();
            objXMLWriter = null;
            return strBuilder.ToString();
        }

        private class StringWriterWithEncoding : StringWriter
        {
            private Encoding m_encoding;

            public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
                : base(sb)
            {
                //base(sb);// base.ne.New(sb);
                m_encoding = encoding;
            }

            public override Encoding Encoding
            {
                get
                {
                    return m_encoding;
                }
            }
        }

    }
}