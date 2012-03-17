using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;
using CommerceBuilder.Payments;
using System.Collections.Specialized;

namespace CommerceBuilder.Payments.Providers.PayJunction
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {               
        bool _UseAuthCapture = false;
        bool _UseTestMode = true;

        string _LoginName = "";
        string _Password = "";

        public override string Name
        {
            get { return "PayJunction"; }
        }

        public override string Description
        {
            get { return "PayJunction Trinity"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.PayJunction.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "PayJunction Trinity Gateway API 1.2"; }
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

        public string LoginName
        {
            get { return _LoginName; }
            set { _LoginName = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public override string ConfigReference
        {
            get { return _LoginName; }
        }

        public override bool RefundRequiresAccountData
        {
            get
            {
                return true;
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
            currentCell.ColSpan = 2;
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = this.Name;
            gatewayLink.NavigateUrl = "http://www.payjunction.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable PayJunction, you must provide your Login Name and Password.</p>"));
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

            //get Login Name
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Login Name:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your PayJunction account login name.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtLoginName = new TextBox();
            txtLoginName.ID = "Config_LoginName";
            txtLoginName.Columns = 40;
            txtLoginName.MaxLength = 280;
            txtLoginName.Text = this.LoginName;
            currentCell.Controls.Add(txtLoginName);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Password
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Password:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your PayJunction account password.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtPassword = new TextBox();
            txtPassword.TextMode = TextBoxMode.Password;
            txtPassword.ID = "Config_Password";
            txtPassword.Columns = 40;
            txtPassword.MaxLength = 280;
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

            currentCell.Controls.Add(new LiteralControl("<br />"));

            //GET THE DEBUG MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Debug Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and PayJunction is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("LoginName")) LoginName = ConfigurationData["LoginName"];
            if (ConfigurationData.ContainsKey("Password")) Password = ConfigurationData["Password"];            
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
            //BUILD REQUEST
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            bool capture = this.UseAuthCapture || authorizeRequest.Capture;
            string requestData = InitializeAuthRequest(payment, order, user, sensitiveData, capture);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            }
            //SEND REQUEST AND GET RESPONSE
            string responseData = SendRequestToGateway(requestData);
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                }
                return ProcessResponse(payment, responseData, capture ? TransactionType.AuthorizeCapture : TransactionType.Authorize, authorizeRequest.Amount);
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

            //YOU CAN ONLY CAPTURE THE AMOUNT OF THE AUTHORIZATION
            LSDecimal totalAuthorized = payment.Transactions.GetTotalAuthorized();
            if (captureRequest.Amount != totalAuthorized)
            {
                Transaction transaction = new Transaction();
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.TransactionType = captureRequest.TransactionType;
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = captureRequest.Amount;
                transaction.ResponseMessage = "You can only capture the authorization amount.  Capture amount must be " + totalAuthorized.ToString("F2") + ".";
                return transaction;
            }

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            string requestData = InitializeCaptureRequest(payment, authorizeTransaction, captureRequest.Amount, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            }

            string responseData = SendRequestToGateway(requestData);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                Transaction transaction = ProcessResponse(payment, responseData, captureRequest.TransactionType, captureRequest.Amount);
                //PAYJUNCTION DOES NOT INCLUDE TRANSACTION ID WITH CAPTURES
                //IT IS THE SAME TRANSACTION ID AS AUTHORIZATION
                if (string.IsNullOrEmpty(transaction.ProviderTransactionId))
                    transaction.ProviderTransactionId = authorizeTransaction.ProviderTransactionId;
                return transaction;
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

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            string requestData = InitializeRefundRequest(payment, creditRequest, captureTransaction, creditRequest.Amount, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
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

            //HAVE WE CAPTURED ANY?
            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
            if (capturedAmount > 0)
            {
                //PAYJUNCTION CANNOT 'PARTIAL VOID'
                //JUST RETURN A SUCCESSFUL RESPONSE, THE REMAINING AUTHORIZATION WILL BE 
                //RELEASED BY THE BANK WHEN IT IS NOT CAPTURED
                Transaction transaction = new Transaction();
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.ProviderTransactionId = authorizeTransaction.ProviderTransactionId;
                transaction.TransactionType = voidRequest.TransactionType;
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = voidRequest.Amount;
                return transaction;
            }

            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            string requestData = InitializeVoidRequest(payment, authorizeTransaction, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            }

            string responseData = SendRequestToGateway(requestData);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                Transaction transaction = ProcessResponse(payment, responseData, TransactionType.Void, voidRequest.Amount);
                //PAYJUNCTION DOES NOT INCLUDE TRANSACTION ID WITH CAPTURES
                //IT IS THE SAME TRANSACTION ID AS AUTHORIZATION
                if (string.IsNullOrEmpty(transaction.ProviderTransactionId))
                    transaction.ProviderTransactionId = authorizeTransaction.ProviderTransactionId;
                return transaction;
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            VerifyStatus();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");

            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();
            AuthorizeTransactionRequest authRequest;
            Transaction tr1, tr2, errTrans;

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
            string requestData = InitializeAuthRecurringRequest(authorizeRequest, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, sensitiveData);
            }

            string responseData = SendRequestToGateway(requestData);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                }

                tr2 = ProcessRecurringResponse(authorizeRequest, responseData);
                response.AddTransaction(tr2);
                response.Status = tr2.TransactionStatus;

                return response;
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }

        }

        private string InitializeAuthRequest(Payment payment, Order order, User user, Dictionary<string,string> sensitiveData, bool capture)
        {
            VerifyPaymentInstrument(payment);
            //Address address = user.PrimaryAddress;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            string expireDate = expirationMonth + expirationYear;
            string amount = String.Format("{0:F2}", payment.Amount);
            string securityCode = accountData.GetValue("SecurityCode");
                        
            string data = "dc_logon=" + HttpUtility.UrlEncode(this.LoginName, System.Text.Encoding.UTF8);
            data += Encode("dc_password", this.Password);
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            if (capture)
            {
                data += Encode("dc_transaction_type", "AUTHORIZATION_CAPTURE");
            }
            else
            {
                data += Encode("dc_transaction_type", "AUTHORIZATION");
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

            data += Encode("dc_version", "1.2");
            data += Encode("dc_address", order.BillToAddress1 + " " + order.BillToAddress2);
            data += Encode("dc_city", order.BillToCity);
            data += Encode("dc_zip", order.BillToPostalCode);
            data += Encode("dc_country", order.BillToCountryCode);
            data += Encode("dc_transaction_amount", amount);
            data += Encode("dc_name", fullName);
            data += Encode("dc_number", accountNumber);
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            data += Encode("dc_expiration_month", expirationMonth);
            data += Encode("dc_expiration_year", expirationYear);
            
            if (!string.IsNullOrEmpty(securityCode))
            {
                data += Encode("dc_verification_number", securityCode);
                if (this.UseDebugMode) sensitiveData["dc_verification_number=" + securityCode] = "dc_verification_number=" + (new string('x', securityCode.Length));
            }

            return data;
        }

        private string InitializeCaptureRequest(Payment payment, Transaction authorizeTransaction, LSDecimal dAmount, Dictionary<string,string> sensitiveData)
        {
            string data = "dc_logon=" + HttpUtility.UrlEncode(this.LoginName, System.Text.Encoding.UTF8);
            data += Encode("dc_password", this.Password);
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            data += Encode("dc_transaction_type", "update");
            data += Encode("dc_version", "1.2");
            data += Encode("dc_transaction_amount", string.Format("{0:F2}", dAmount));
            data += Encode("dc_transaction_id", authorizeTransaction.ProviderTransactionId);
            data += Encode("dc_posture", "capture");
            return data;
        }

        private string InitializeRefundRequest(Payment payment, RefundTransactionRequest refundRequest, Transaction authorizeTransaction, LSDecimal dAmount, Dictionary<string, string> sensitiveData)
        {
            Order order = payment.Order;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = refundRequest.CardNumber;
            string expirationMonth = refundRequest.ExpirationMonth.ToString();
            string expirationYear = refundRequest.ExpirationYear.ToString();
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            string expireDate = expirationMonth + expirationYear;
            string amount = String.Format("{0:F2}", dAmount);
            string securityCode = accountData.GetValue("SecurityCode");

            string fullName;
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                fullName = accountData["AccountName"];
            }
            else
            {
                fullName = order.BillToFirstName + " " + order.BillToLastName;
            }

            string data = "dc_logon=" + HttpUtility.UrlEncode(this.LoginName, System.Text.Encoding.UTF8);
            data += Encode("dc_password", this.Password);
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            data += Encode("dc_transaction_type", "CREDIT");
            data += Encode("dc_version", "1.2");            
            data += Encode("dc_transaction_id", authorizeTransaction.ProviderTransactionId);
            data += Encode("dc_address", order.BillToAddress1 + " " + order.BillToAddress2);
            data += Encode("dc_city", order.BillToCity);
            data += Encode("dc_zip", order.BillToPostalCode);
            data += Encode("dc_transaction_amount", amount);
            data += Encode("dc_name", fullName);
            data += Encode("dc_number", accountNumber);
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            data += Encode("dc_expiration_month", expirationMonth);
            data += Encode("dc_expiration_year", expirationYear);
            if (!string.IsNullOrEmpty(securityCode))
            {
                data += Encode("dc_verification_number", securityCode);
                if (this.UseDebugMode) sensitiveData["dc_verification_number=" + securityCode] = "dc_verification_number=" + (new string('x', securityCode.Length));
            }

            return data;
        }

        private string InitializeVoidRequest(Payment payment, Transaction authorizeTransaction, Dictionary<string, string> sensitiveData)
        {
            string data = "dc_logon=" + HttpUtility.UrlEncode(LoginName, System.Text.Encoding.UTF8);
            data += Encode("dc_password", this.Password);
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            data += Encode("dc_transaction_type", "update");
            data += Encode("dc_version", "1.2");
            //data += Encode("dc_transaction_amount", amount);
            data += Encode("dc_transaction_id", authorizeTransaction.ProviderTransactionId);
            data += Encode("dc_posture", "void");
            return data;
        }

        private string InitializeAuthRecurringRequest(AuthorizeRecurringTransactionRequest authRequest, Dictionary<string, string> sensitiveData)
        {
            Payment payment = authRequest.Payment;
            User user = payment.Order.User;
            Order order = payment.Order;
            
            VerifyPaymentInstrument(payment);            
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            string expireDate = expirationMonth + expirationYear;
            string amount = String.Format("{0:F2}", authRequest.RecurringChargeSpecified? authRequest.RecurringCharge : authRequest.Amount);
            string securityCode = accountData.GetValue("SecurityCode");

            //authentication data
            string data = "dc_logon=" + HttpUtility.UrlEncode(this.LoginName, System.Text.Encoding.UTF8);
            data += Encode("dc_password", this.Password);
            if (this.UseDebugMode) sensitiveData[this.Password] = "xxxxxxxx";
            
            //address data
            data += Encode("dc_address", order.BillToAddress1 + " " + order.BillToAddress2);
            data += Encode("dc_city", order.BillToCity);
            data += Encode("dc_zip", order.BillToPostalCode);
            data += Encode("dc_country", order.BillToCountryCode);

            //transaction data
            data += Encode("dc_transaction_type", "AUTHORIZATION_CAPTURE");
            data += Encode("dc_transaction_amount", amount);
            data += Encode("dc_version", "1.2");

            //cc data
            string fullName;
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                fullName = accountData["AccountName"];
            }
            else
            {
                fullName = order.BillToFirstName + " " + order.BillToLastName;
            }
            data += Encode("dc_name", fullName);
            data += Encode("dc_number", accountNumber);
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            data += Encode("dc_expiration_month", expirationMonth);
            data += Encode("dc_expiration_year", expirationYear);

            if (!string.IsNullOrEmpty(securityCode))
            {
                data += Encode("dc_verification_number", securityCode);
                if (this.UseDebugMode) sensitiveData["dc_verification_number=" + securityCode] = "dc_verification_number=" + (new string('x', securityCode.Length));
            }

            //schedule data
            data += Encode("dc_schedule_create", "true");
            int remainingPayments = authRequest.NumberOfPayments;
            if (authRequest.RecurringChargeSpecified) remainingPayments -= 1;
            data += Encode("dc_schedule_limit", remainingPayments.ToString());
            if (authRequest.PaymentFrequencyUnit == CommerceBuilder.Products.PaymentFrequencyUnit.Day)
            {
                data += Encode("dc_schedule_periodic_type", "day");
            }
            else
            {
                data += Encode("dc_schedule_periodic_type", "month");                
                
            }
            data += Encode("dc_schedule_periodic_number", authRequest.PaymentFrequency.ToString());

            DateTime startDt = GetNextPaymentDate(authRequest);

            data += Encode("dc_schedule_start", startDt.ToString("yyyy-MM-dd"));

            return data;
        }

        private DateTime GetNextPaymentDate(AuthorizeRecurringTransactionRequest authRequest)
        {
            if (authRequest.RecurringChargeSpecified)
            {
                if (authRequest.PaymentFrequencyUnit == CommerceBuilder.Products.PaymentFrequencyUnit.Month)
                {
                    return LocaleHelper.LocalNow.AddMonths(authRequest.PaymentFrequency);
                }
                else
                {
                    return LocaleHelper.LocalNow.AddDays(authRequest.PaymentFrequency);
                }
            }
            else
            {
                return LocaleHelper.LocalNow;
            }
        }

        private Transaction ProcessResponse(Payment payment, String responseData, TransactionType transactionType, LSDecimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;

            NameValueCollection response = ParseResponseString(responseData);

            bool success = false;
            string queryStatus = response.Get("dc_query_status");
            if (!string.IsNullOrEmpty(queryStatus))
            {
                success = AlwaysConvert.ToBool(queryStatus, false);
            }
            else
            {
                int responseCode = AlwaysConvert.ToInt(response.Get("dc_response_code"), -9999);
                success = (responseCode == 0 || responseCode == 85);
            }

            transaction.TransactionDate = DateTime.UtcNow;
            transaction.Amount = transactionAmount;
            if (!success)
            {
                //failed
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = response.Get("dc_response_code");
                transaction.ResponseMessage = response.Get("dc_response_message");
            }
            else
            {
                //successful
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = response.Get("dc_transaction_id");
                transaction.AuthorizationCode = response.Get("dc_approval_code");
                if (transaction.AuthorizationCode == "null") transaction.AuthorizationCode = string.Empty;
                //no avs or cvv in response? return unavailable codes
                transaction.AVSResultCode = "U";
                transaction.CVVResultCode = "X";

                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }

            return transaction;
        }

        private Transaction ProcessRecurringResponse(AuthorizeRecurringTransactionRequest authRequest, string responseData)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.AuthorizeRecurring;
            LSDecimal transAmount = authRequest.RecurringChargeSpecified ? authRequest.RecurringCharge : authRequest.Amount;
            transaction.Amount = transAmount;

            NameValueCollection response = ParseResponseString(responseData);
            
            int responseCode = AlwaysConvert.ToInt(response.Get("dc_response_code"), -9999);
            if (responseCode == 0 || responseCode == 85)
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = response.Get("dc_transaction_id");
                transaction.TransactionDate = LocaleHelper.LocalNow;
                transaction.ResponseCode = response.Get("dc_response_code");
                transaction.ResponseMessage = response.Get("dc_response_message");
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = response.Get("dc_response_code");
                transaction.ResponseMessage = response.Get("dc_response_message");
            }

            return transaction;
        }

        private string SendRequestToGateway(string requestData)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            string gatewayUrl = this.UseTestMode ? "https://demo.payjunction.com/quick_link" : "https://payjunction.com/quick_link";
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(gatewayUrl);
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

        private NameValueCollection ParseResponseString(string responseData) 
        {
            NameValueCollection resp = new NameValueCollection();

            string key, value;
            string[] tokens = responseData.Split(new char[] {(char)0x1C});
            foreach(string token in tokens) {
                string[] nvpair = token.Split(new char[]{'='});
                if (nvpair != null && nvpair.Length > 0)
                {
                    key = nvpair[0];
                    if(nvpair.Length > 1) {
                        value= nvpair[1];
                        if (value == null) value = "";
                    }else{
                        value = "";
                    }
                    if (key != null)
                    {
                        resp.Add(key, value);
                    }
                }
            }
            return resp;
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

        private void VerifyStatus()
        {
            if(string.IsNullOrEmpty(this.LoginName) ||
                string.IsNullOrEmpty(this.Password) )
            {
                throw new InvalidOperationException("One of the required parameters is not set : LoginName and/or Password");
            }  
        }

        // this is the method that will url encode the values before sending 
        private string Encode(string str_Name, string str_Value)
        {
            string str_ReturnValue = string.Empty;

            try
            {
                str_ReturnValue = HttpUtility.UrlEncode(str_Value, System.Text.Encoding.UTF8);
            }

            catch
            {
                throw new Exception("Encoding failure: Field - " + str_Name + " Value - " + str_Value);
            }

            return "&" + str_Name + "=" + str_ReturnValue;
        }// end of the Encode method

    }
}
