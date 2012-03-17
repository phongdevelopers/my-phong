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
using System.Text.RegularExpressions;


namespace CommerceBuilder.Payments.Providers.SkipJack
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {

        bool _UseTestMode = true;

        string _SerialNumber = "";
        string _DeveloperSerialNumber = "";

        public override string Name
        {
            get { return "SkipJack"; }
        }

        public override string Description
        {
            get { return "SkipJack Description here"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.SkipJack.Logo.gif");
            return string.Empty;
        }

        public override string Version
        {
            get { return "SkipJack 1.0"; }
        }

        public bool UseTestMode
        {
            get { return _UseTestMode; }
            set { _UseTestMode = value; }
        }

        public string SerialNumber
        {
            get { return _SerialNumber; }
            set { this._SerialNumber = value; }
        }

        public string DeveloperSerialNumber
        {
            get { return _DeveloperSerialNumber; }
            set { this._DeveloperSerialNumber = value; }
        }

        public override string ConfigReference
        {
            get { return this.SerialNumber; }
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
            gatewayLink.NavigateUrl = "http://www.skipjack.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable SkipJack you should provide both the HTML and the developer serial numbers given to you with your merchant account.  This applies to test and live accounts equally.  For a description of these serial numbers, <a href=\"http://www.skipjack.com/merchants.aspx?cmsphid=31794473|5333904|7709716\" target=\"blank\">see here</a>.</p>"));
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

            //get Serial Number
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("HTML Serial Number:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">This is the HTML serial number associated with your Skipjack account.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtSerialNumber = new TextBox();
            txtSerialNumber.ID = "Config_SerialNumber";
            txtSerialNumber.Columns = 50;
            txtSerialNumber.MaxLength = 280;
            txtSerialNumber.Text = this.SerialNumber;
            currentCell.Controls.Add(txtSerialNumber);
            currentCell.Controls.Add(new LiteralControl("<br />"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //get Developer Serial Number
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Developer Serial Number:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">This is the developer serial number associated with your Skipjack account.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtDeveloperSerialNumber = new TextBox();
            txtDeveloperSerialNumber.ID = "Config_DeveloperSerialNumber";
            txtDeveloperSerialNumber.Columns = 50;
            txtDeveloperSerialNumber.MaxLength = 280;
            txtDeveloperSerialNumber.Text = this.DeveloperSerialNumber;
            currentCell.Controls.Add(txtDeveloperSerialNumber);
            currentCell.Controls.Add(new LiteralControl("<br />"));
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and SkipJack is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
                    | SupportedTransactions.Capture
                    | SupportedTransactions.PartialCapture
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Refund
                    | SupportedTransactions.Void
                    | SupportedTransactions.RecurringBilling);
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"], true);
            if (ConfigurationData.ContainsKey("SerialNumber")) SerialNumber = ConfigurationData["SerialNumber"];
            if (ConfigurationData.ContainsKey("DeveloperSerialNumber")) DeveloperSerialNumber = ConfigurationData["DeveloperSerialNumber"];
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            //VALIDATE THE PARAMETERS
            VerifyGatewayConfig();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");
            //BUILD THE REQUEST
            string requestDebug;
            string requestData = InitializeAuthRequest(authorizeRequest, out requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }
            //SEND REQUEST TO GATEWAY
            TransactionType transType = TransactionType.Authorize;
            string responseData = SendRequestToGateway(requestData, transType);
            //CHECK THE RESPONSE
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                }
                //PARSE THE RESPONISE
                return ProcessAuthResponse(authorizeRequest, responseData, transType);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            VerifyGatewayConfig();
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = captureRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            string requestDebug;
            string requestData = InitializeCaptureRequest(payment, authorizeTransaction, captureRequest, out requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestData, null);
            }

            string responseData = SendRequestToGateway(requestData,TransactionType.Capture);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                return ProcessChangeStatusResponse(payment, responseData, captureRequest.TransactionType, captureRequest.Amount);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        private SJTransactionStatus GetTransactionStatus(Payment payment, Transaction transaction)
        {
            //BUILD THE QUERY REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("szSerialNumber=" + this.SerialNumber);
            data.Append(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
            data.Append(Encode("szOrderNumber", payment.Order.OrderNumber.ToString()));
            //data.Append(Encode("szDate", transaction.TransactionDate.ToString("MM/dd/yyyy")));
            //RECORD THE REQUEST
            if (this.UseDebugMode)
            {
                Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
                sensitiveData.Add("=" + this.SerialNumber, "=" + StringHelper.MakeReferenceNumber(this.SerialNumber));
                sensitiveData.Add("=" + this.DeveloperSerialNumber, "=" + StringHelper.MakeReferenceNumber(this.DeveloperSerialNumber));
                this.RecordCommunication(this.Name, CommunicationDirection.Send, data.ToString(), sensitiveData);
            }
            //GET URL FOR REQUEST
            string testURL = "https://developer.skipjackic.com/scripts/evolvcc.dll?SJAPI_TransactionStatusRequest";
            string liveURL = "https://www.skipjackic.com/scripts/evolvcc.dll?SJAPI_TransactionStatusRequest";
            string serverURL = (this.UseTestMode ? testURL : liveURL);
            //SEND REQUEST
            string responseData = SendRequestToGateway(data.ToString(), serverURL);
            if (responseData != null)
            {
                if (this.UseDebugMode) 
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                //BREAK RESPONSE INTO LINES
                if (responseData.EndsWith("\r\n")) responseData = responseData.Substring(0, responseData.Length - 2);
                string[] responseLines = Regex.Split(responseData, "\r\n");
                string headerLine = responseLines[0];
                string[] headerValues = headerLine.Split(',');
                int errorCode = Convert.ToInt32(headerValues[1].Replace("\"", string.Empty));
                if (errorCode == 0)
                {
                    string[] lastRecordValues = responseLines[responseLines.Length - 1].Split(',');
                    string statusCode = lastRecordValues[2].Replace("\"", string.Empty);
                    int currentStatusCode = Convert.ToInt32(statusCode.Substring(0, 1));
                    int pendingStatusCode = Convert.ToInt32(statusCode.Substring(1, 1));
                    SJTransactionStatus status = new SJTransactionStatus();
                    status.CurrentStatus = (SJTransactionStatus.SJCurrentStatus)currentStatusCode;
                    status.PendingStatus = (SJTransactionStatus.SJPendingStatus)pendingStatusCode;
                    status.TransactionId = lastRecordValues[6].Replace("\"", string.Empty);
                    return status;
                }
            }
            //NO DATA AVAILABLE
            return null;

        }

        private class SJTransactionStatus
        {
            private SJCurrentStatus _CurrentStatus;
            private SJPendingStatus _PendingStatus;
            private string _TransactionId;
            public SJCurrentStatus CurrentStatus
            {
                get { return _CurrentStatus; }
                set { _CurrentStatus = value; }
            }
            public SJPendingStatus PendingStatus
            {
                get { return _PendingStatus; }
                set { _PendingStatus = value; }
            }
            public string TransactionId
            {
                get { return _TransactionId; }
                set { _TransactionId = value; }
            }
            public enum SJCurrentStatus
            {
                Idle, Authorized, Denied, Settled, Credited, Deleted, Archived, PreAuthorized, SplitSettled
            }
            public enum SJPendingStatus
            {
                Idle, PendingCredit, PendingSettlement, PendingDelete, PendingAuthorization, PendingManualSettlement, PendingRecurring, SubmittedForSettlement
            }
        }

        private bool HasMultipleCaptures(TransactionCollection transactions)
        {
            bool captureFound = false;
            foreach (Transaction t in transactions)
            {
                if ((t.TransactionStatus == TransactionStatus.Successful) &&
                    ((t.TransactionType == TransactionType.Capture) || (t.TransactionType == TransactionType.PartialCapture)))
                {
                    if (captureFound) return true;
                    captureFound = true;
                }
            }
            return false;
        }

        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            VerifyGatewayConfig();
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction captureTransaction = creditRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            //DOES THE PAYMENT CONTAIN MULTIPLE CAPTURES?
            if (HasMultipleCaptures(payment.Transactions))
            {
                //WE WILL NOT SUPPORT THIS KIND OF REFUND THROUGH AC
                return Transaction.CreateErrorTransaction(this.PaymentGatewayId, TransactionType.Refund, creditRequest.Amount, string.Empty, "Payments that include multiple captures must be refunded through the SkipJack merchant interface.", creditRequest.RemoteIP);
            }

            //TRACK THE TRANSACTION ID WE WANT TO REFUND
            string sjTransactionId = captureTransaction.ProviderTransactionId;

            //NEED TO GET CURRENT TRANSACTION STATUS
            SJTransactionStatus currentStatus = GetTransactionStatus(payment, captureTransaction);
            if (currentStatus != null)
            {
                if (currentStatus.PendingStatus == SJTransactionStatus.SJPendingStatus.PendingSettlement)
                {
                    if (creditRequest.Amount == payment.Transactions.GetTotalCaptured())
                    {
                        //THE PAYMENT IS NOT SETTLED, WE SHOULD RUN A VOID INSTEAD
                        VoidTransactionRequest voidRequest = new VoidTransactionRequest(payment, creditRequest.RemoteIP);
                        //WE NEED TO VOID THE TRANSACTION ID OF THE LAST CAPTURE
                        Transaction t = DoVoid(voidRequest, true, currentStatus.TransactionId);
                        //MIMIC A REFUND TRANSACTION
                        t.TransactionType = TransactionType.Refund;
                        return t;
                    }
                    else
                    {
                        //CANNOT PROCESS PARTIAL REFUNDS UNTIL AFTER SETTLEMENT
                        return Transaction.CreateErrorTransaction(this.PaymentGatewayId, TransactionType.Refund, creditRequest.Amount, string.Empty, "You cannot process a partial refund until this transaction is settled.", creditRequest.RemoteIP);
                    }
                }
                else
                {
                    //GET THE MOST RECENT TRANSACTION THAT SHOULD REPRESENT THE ID OF THE SETTLEMENT
                    //THIS IS A BEST GUESS AS WE DO NOT HAVE A LIVE ACCOUNT TO RESEARCH AND TEST PROPERLY
                    sjTransactionId = currentStatus.TransactionId;
                }
            }

            string requestDebug;
            string requestData = InitializeRefundRequest(payment, captureTransaction, creditRequest.Amount, sjTransactionId, out requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData, TransactionType.Refund);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);

                return ProcessChangeStatusResponse(payment, responseData, creditRequest.TransactionType, creditRequest.Amount);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            return DoVoid(voidRequest, false, voidRequest.AuthorizeTransaction.ProviderTransactionId);
        }

        /// <summary>
        /// Process a void on the SkipJack gateway
        /// </summary>
        /// <param name="voidRequest">The void request details</param>
        /// <param name="isRefund">If true, void is acting as a refund of an unsettled catpure.</param>
        /// <param name="sjTransactionId">This is the transaction ID to be voided, if available.</param>
        /// <returns>The transaction result</returns>
        private Transaction DoVoid(VoidTransactionRequest voidRequest, bool isRefund, string sjTransactionId)
        {
            VerifyGatewayConfig();
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("transactionRequest.AuthorizeTransaction");

            //IF WE HAVE CAPTURED, BUT IT IS LESS THAN THE TOTAL PAYMENT AMOUNT
            //THIS IS A PARTIAL VOID.  IF WE HAVE CAPTURED BUT EQUAL TO THE PAYMENT
            LSDecimal capturedAmount = payment.Transactions.GetTotalCaptured();
            if ((capturedAmount > 0) && (!isRefund))
            {
                //SKIPJACK CANNOT 'PARTIAL VOID'
                //JUST RETURN A SUCCESSFUL RESPONSE, THE REMAINING AUTHORIZATION WILL BE 
                //RELEASED BY THE BANK WHEN IT IS NOT CAPTURED
                Transaction transaction = new Transaction();
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.ProviderTransactionId = sjTransactionId;
                transaction.TransactionType = voidRequest.TransactionType;
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = voidRequest.Amount;
                return transaction;
            }

            string requestDebug;
            string requestData = InitializeVoidRequest(payment, authorizeTransaction, sjTransactionId, out requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            }

            string responseData = SendRequestToGateway(requestData,TransactionType.Void);

            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);

                return ProcessChangeStatusResponse(payment, responseData, TransactionType.Void, voidRequest.Amount);
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        #region Recurring Auth

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            //CREATE THE RESPONSE OBJECT FOR THIS REQUEST
            AuthorizeRecurringTransactionResponse response = new AuthorizeRecurringTransactionResponse();
            //VALIDATE THE PAYMENT PERIOD
            string payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == string.Empty)
            {
                Transaction errorTx = Transaction.CreateErrorTransaction(this.PaymentGatewayId, authorizeRequest, "E", "The specified payment interval is not valid for this processor.");
                return new AuthorizeRecurringTransactionResponse(errorTx);
            }
            //SKIPJACK RECURRING BILLING DOES NOT RUN THE CARD UNTIL THE FIRST SCHEDULED PAYMENT
            //TO GET AROUND THIS WE SHOULD RUN A STANDARD AUTH/CAPTURE SEQUENCE FOR THE FIRST PAYMENT
            //THEN IF IT SUCCEEDS WE CAN ISSUE A RECURRING PAYMENT FOR THE REMAINING CHARGES
            //CREATE AN AUTHORIZATION REQUEST FOR THE INITIAL CHARGE
            AuthorizeTransactionRequest initialAuthRequest = new AuthorizeTransactionRequest(authorizeRequest.Payment, authorizeRequest.RemoteIP);
            //SET THE AMOUNT TO THE INITIAL PAYMENT AND RUN THE AUTHORIZATION
            initialAuthRequest.Amount = authorizeRequest.Amount;
            Transaction initialAuthTx = DoAuthorize(initialAuthRequest);
            //CHECK WHETHER THE INITIAL AUTHORIZATION WAS SUCCESSFUL
            if (initialAuthTx.TransactionStatus == TransactionStatus.Successful)
            {
                response.AddTransaction(initialAuthTx);
                //AUTHORIZATION SUCCESSFUL, ATTEMPT TO CAPTURE THE PAYMENT
                CaptureTransactionRequest initialCaptureRequest = new CaptureTransactionRequest(authorizeRequest.Payment, authorizeRequest.RemoteIP);
                initialCaptureRequest.Amount = initialAuthTx.Amount;
                initialCaptureRequest.AuthorizeTransaction = initialAuthTx;
                //WE SHOULD IGNORE FAILURE TO CAPTURE THE AUTHORIZATION
                //BECAUSE THIS WILL LEAVE AN OPEN AUTHORIZATION ASSOCIATED WITH THIS PAYMENT
                Transaction initialCaptureTx = DoCapture(initialCaptureRequest);
                response.AddTransaction(initialCaptureTx);
            }
            else
            {
                //THE INITIAL CHARGE COULD NOT BE AUTHORIZED
                //RETURN AN ERROR AND STOP PROCESSING RECURRING AUTHORIZATION
                response.AddTransaction(initialAuthTx);
                response.Status = TransactionStatus.Failed;
                return response;
            }

            //NOW TIME TO SET UP THE RECURRING BILLING TRANSACTION
            //BUILD THE RECURRING AUTH REQUEST
            string requestDebug;
            string requestData = BuildRecurringAuthRequest(authorizeRequest, payPeriod, out requestDebug);
            //RECORD REQUEST
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Send, requestDebug, null);
            //SEND REQUEST AND GET RESPONSE
            string responseData = SendRequestToGateway(requestData, TransactionType.AuthorizeRecurring);
            if (responseData != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, responseData, null);
                Transaction recurringTx = ProcessRecurringResponse(authorizeRequest, responseData);
                response.AddTransaction(recurringTx);
                response.Status = recurringTx.TransactionStatus;
                return response;
            }
            else
            {
                Transaction errorTx = Transaction.CreateErrorTransaction(PaymentGatewayId, authorizeRequest, "E", "Null response from gateway.");
                errorTx.TransactionType = TransactionType.AuthorizeRecurring;
                response.AddTransaction(errorTx);
                response.Status = TransactionStatus.Failed;
                return response;
            }
        }

        private string BuildRecurringAuthRequest(AuthorizeRecurringTransactionRequest authorizeRequest, string payPeriod, out string requestDebug)
        {
            VerifyGatewayConfig();
            //GET VARIABLES FOR BUILDING REQUEST
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            VerifyPaymentInstrument(payment);
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if (user == null) throw new ArgumentNullException("request.Payment.Order.User");
            Address address = user.PrimaryAddress;
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) expirationMonth = "0" + expirationMonth;
            if (expirationYear.Length == 2) expirationYear = "20" + expirationYear;
            else if (expirationYear.Length == 1) expirationYear = "200" + expirationYear;
            //BUILD THE RECURRING AUTH REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
            data.Append(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
            data.Append(Encode("rtOrderNumber", order.OrderNumber.ToString()));
            data.Append(Encode("rtAddress1", address.Address1));
            data.Append(Encode("rtAddress2", address.Address2));
            data.Append(Encode("rtCity", address.City));
            data.Append(Encode("rtPostalCode", address.PostalCode));
            data.Append(Encode("rtName", address.FullName));
            data.Append(Encode("rtState", address.Province));
            data.Append(Encode("rtPhone", address.Phone));
            data.Append(Encode("rtFax", address.Fax));
            data.Append(Encode("rtCountry", address.CountryCode));
            data.Append(Encode("rtEmail", string.IsNullOrEmpty(address.Email) ? user.Email : address.Email));
            data.Append(Encode("rtItemNumber", order.OrderNumber.ToString()));
            data.Append(Encode("rtItemDescription", authorizeRequest.SubscriptionName));
            //ADD IN CREDIT CARD DATA
            data.Append(Encode("rtAccountNumber", accountNumber));
            data.Append(Encode("rtExpMonth", expirationMonth));
            data.Append(Encode("rtExpYear", expirationYear));
            //IF A RECURRING CHARGE IS SPECIFIED, WE SHOULD USE THIS VALUE
            LSDecimal amount = (authorizeRequest.RecurringChargeSpecified ?  authorizeRequest.RecurringCharge : authorizeRequest.Amount);
            data.Append(Encode("rtAmount", amount.ToString("F2")));
            DateTime startDt = GetNextPaymentDate(payPeriod);
            data.Append(Encode("rtStartingDate", startDt.ToString("MM/dd/yyyy")));
            data.Append(Encode("rtFrequency", payPeriod));
            //WE HAVE ALREADY RUN AN AUTH FOR THE FIRST PAYMENT, SO REDUCE TOTAL BY ONE
            int remainingPayments = authorizeRequest.NumberOfPayments - 1;
            data.Append(Encode("rtTotalTransactions", remainingPayments.ToString()));
            //PREPARE THE DEBUG STRING
            if (this.UseDebugMode)
            {
                List<string> debugFind = new List<string>();
                List<string> debugReplace = new List<string>();
                debugFind.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
                debugReplace.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.MakeReferenceNumber(this.SerialNumber)));
                debugFind.Add(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
                debugReplace.Add(Encode("szDeveloperSerialNumber", this.MakeReferenceNumber(this.DeveloperSerialNumber)));
                debugFind.Add(Encode("rtAccountNumber", accountNumber));
                debugReplace.Add(Encode("rtAccountNumber", this.MakeReferenceNumber(accountNumber)));
                requestDebug = data.ToString();
                for (int i = 0; i < debugFind.Count; i++)
                {
                    requestDebug = requestDebug.Replace(debugFind[i], debugReplace[i]);
                }
            }
            else requestDebug = string.Empty;
            //RETURN THE RECURRING AUTH REQUEST
            return data.ToString();
        }

        private Transaction ProcessRecurringResponse(AuthorizeRecurringTransactionRequest authTx, string responseData)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.AuthorizeRecurring;
            transaction.Amount = (authTx.RecurringChargeSpecified ? authTx.RecurringCharge : authTx.Amount);
            //PARSE THE RESPONSE
            string[] responseArray = ParseResponseRecord(responseData);
            if (responseArray == null || responseArray.Length < 5)
            {
                //error in data
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = "E";
                transaction.ResponseMessage = "Null or invalid length response.";
                return transaction;
            }
            else
            {
                int responseCode = AlwaysConvert.ToInt(responseArray[1], 1);
                if (responseCode == 0)
                {
                    //success
                    transaction.TransactionStatus = TransactionStatus.Successful;
                    transaction.ProviderTransactionId = responseArray[3];
                    transaction.AVSResultCode = "U"; // authTx.AVSResultCode;
                    transaction.CVVResultCode = "X"; // authTx.CVVResultCode;
                }
                else
                {
                    //failure
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.ResponseCode = responseCode.ToString();
                    transaction.ResponseMessage = GetResponseMessage(responseCode);
                }
                return transaction;
            }
        }

#endregion

        private string InitializeAuthRequest(AuthorizeTransactionRequest authRequest, out string requestDebug)
        {
            Payment payment = authRequest.Payment;
            VerifyPaymentInstrument(payment);
            Order order = payment.Order;
            User user = order.User;
            List<string> debugFind = new List<string>();
            List<string> debugReplace = new List<string>();
            //FORMAT REQUEST PARAMETERS FOR USE IN BUILDING REQUEST            
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationMonth.Length == 1) { expirationMonth = "0" + expirationMonth; }
            if (expirationYear.Length > 2) { expirationYear = expirationYear.Substring(expirationYear.Length - 2); }
            string expireDate = expirationMonth + expirationYear;
            string amount = String.Format("{0:F2}", authRequest.Amount);
            string securityCode = accountData.GetValue("SecurityCode");
            string userEmail = string.IsNullOrEmpty(order.BillToEmail) ? user.Email : order.BillToEmail;
            string fullName;
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                fullName = accountData["AccountName"];
            }
            else
            {
                fullName = order.BillToFirstName + " " + order.BillToLastName;
            }

            String shippingAmount = String.Format("{0:F2}", order.Items.TotalPrice(OrderItemType.Shipping, OrderItemType.Handling));
            String customerTax = String.Format("{0:F2}", order.Items.TotalPrice(OrderItemType.Tax));

            //BUILD THE AUTHORIZATION REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("serialnumber=" + HttpUtility.UrlEncode(this.SerialNumber));
            data.Append(Encode("streetaddress", order.BillToAddress1));
            data.Append(Encode("city", order.BillToCity));
            data.Append(Encode("zipcode", order.BillToPostalCode));
            data.Append(Encode("transactionAmount", amount));
            data.Append(Encode("sjname", fullName));
            data.Append(Encode("state", order.BillToProvince));
            data.Append(Encode("phone", order.BillToPhone));
            data.Append(Encode("fax", order.BillToFax));
            data.Append(Encode("email", userEmail));
            data.Append(Encode("country", order.BillToCountryCode));
            data.Append(Encode("ordernumber", order.OrderNumber.ToString()));
            data.Append(Encode("accountnumber", accountNumber));
            data.Append(Encode("month", expirationMonth));
            data.Append(Encode("year", expirationYear));
            if (!string.IsNullOrEmpty(securityCode)) data.Append(Encode("cvv2", securityCode));
            data.Append(Encode("orderstring", "00001~" + "order~" + amount + "~1~N~||"));
            data.Append(Encode("shiptophone", order.BillToPhone));
            data.Append(Encode("shippingamount", shippingAmount));
            data.Append(Encode("customertax", customerTax));
            //IF DEBUG IS ENABLED, BUILD THE DEBUG DATA STRING
            if (this.UseDebugMode)
            {
                //REGISTER VALUES TO BE SCRUBBED FROM DEBUG
                debugFind.Add("serialnumber=" + HttpUtility.UrlEncode(this.SerialNumber));
                debugReplace.Add("serialnumber=" + HttpUtility.UrlEncode(this.MakeReferenceNumber(this.SerialNumber)));
                debugFind.Add(Encode("accountnumber", accountNumber));
                debugReplace.Add(Encode("accountnumber", this.MakeReferenceNumber(accountNumber)));
                if (!string.IsNullOrEmpty(securityCode))
                {
                    debugFind.Add(Encode("cvv2", securityCode));
                    debugReplace.Add(Encode("cvv2", "XXX"));
                }
                //INITIALIZE THE DEBUG DATA
                requestDebug = data.ToString();
                //SCRUB THE SENSITIVE DATA
                for (int i = 0; i < debugFind.Count; i++)
                {
                    requestDebug = requestDebug.Replace(debugFind[i], debugReplace[i]);
                }
            }
            else requestDebug = string.Empty;
            //RETURN THE AUTHORIZE REQUEST
            return data.ToString();
        }

        private string InitializeCaptureRequest(Payment payment, Transaction authorizeTransaction, CaptureTransactionRequest captureRequest, out string requestDebug)
        {
            //BUILD CAPTURE REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
            data.Append(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
            data.Append(Encode("szOrderNumber", payment.Order.OrderNumber.ToString()));
            data.Append(Encode("szTransactionId", authorizeTransaction.ProviderTransactionId));
            LSDecimal totalCaptured = payment.Transactions.GetTotalCaptured();
            if (captureRequest.IsFinal && (totalCaptured == 0))
            {
                data.Append(Encode("szDesiredStatus", "SETTLE"));
            }
            else
            {
                data.Append(Encode("szDesiredStatus", "SPLITSETTLE"));
            }
            data.Append(Encode("szAmount", String.Format("{0:F0}", (captureRequest.Amount * 100))));
            //PREPARE THE DEBUG STRING
            if (this.UseDebugMode)
            {
                List<string> debugFind = new List<string>();
                List<string> debugReplace = new List<string>();
                debugFind.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
                debugReplace.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.MakeReferenceNumber(this.SerialNumber)));
                debugFind.Add(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
                debugReplace.Add(Encode("szDeveloperSerialNumber", this.MakeReferenceNumber(this.DeveloperSerialNumber)));
                requestDebug = data.ToString();
                for (int i = 0; i < debugFind.Count; i++)
                {
                    requestDebug = requestDebug.Replace(debugFind[i], debugReplace[i]);
                }
            }
            else requestDebug = string.Empty;
            //RETURN THE CAPTURE REQUEST
            return data.ToString();
        }

        private string InitializeRefundRequest(Payment payment, Transaction authorizeTransaction, LSDecimal refundAmount, string sjTransactionId, out string requestDebug)
        {
            //BUILD THE REFUND REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
            data.Append(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
            data.Append(Encode("szAmount", String.Format("{0:F2}", refundAmount)));
            data.Append(Encode("szOrderNumber", payment.Order.OrderNumber.ToString()));
            data.Append( Encode("szDesiredStatus", "CREDIT"));
            data.Append( Encode("szTransactionId", sjTransactionId));
            //PREPARE THE DEBUG STRING
            if (this.UseDebugMode)
            {
                List<string> debugFind = new List<string>();
                List<string> debugReplace = new List<string>();
                debugFind.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
                debugReplace.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.MakeReferenceNumber(this.SerialNumber)));
                debugFind.Add(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
                debugReplace.Add(Encode("szDeveloperSerialNumber", this.MakeReferenceNumber(this.DeveloperSerialNumber)));
                requestDebug = data.ToString();
                for (int i = 0; i < debugFind.Count; i++)
                {
                    requestDebug = requestDebug.Replace(debugFind[i], debugReplace[i]);
                }
            }
            else requestDebug = string.Empty;
            //RETURN THE REFUND REQUEST
            return data.ToString();
        }

        private string InitializeVoidRequest(Payment payment, Transaction authorizeTransaction, string sjTransactionId, out string requestDebug)
        {
            //BUILD THE VOID REQUEST
            StringBuilder data = new StringBuilder();
            data.Append("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
            data.Append(Encode("szDeveloperSerialNumber", HttpUtility.UrlEncode(this.DeveloperSerialNumber)));
            data.Append(Encode("szOrderNumber", payment.Order.OrderNumber.ToString()));
            data.Append(Encode("szDesiredStatus", "DELETE"));
            if (!string.IsNullOrEmpty(sjTransactionId)) data.Append(Encode("szTransactionId", sjTransactionId));
            //PREPARE THE DEBUG STRING
            if (this.UseDebugMode)
            {
                List<string> debugFind = new List<string>();
                List<string> debugReplace = new List<string>();
                debugFind.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.SerialNumber));
                debugReplace.Add("szSerialNumber=" + HttpUtility.UrlEncode(this.MakeReferenceNumber(this.SerialNumber)));
                debugFind.Add(Encode("szDeveloperSerialNumber", this.DeveloperSerialNumber));
                debugReplace.Add(Encode("szDeveloperSerialNumber", this.MakeReferenceNumber(this.DeveloperSerialNumber)));
                requestDebug = data.ToString();
                for (int i = 0; i < debugFind.Count; i++)
                {
                    requestDebug = requestDebug.Replace(debugFind[i], debugReplace[i]);
                }
            }
            else requestDebug = string.Empty;
            //RETURN THE VOID REQUEST
            return data.ToString(); ;
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

        private string SendRequestToGateway(string requestData, TransactionType transType)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            //DETERMINE URL FOR WEB REQUEST
            string liveURL, testURL; ;
            if (transType == TransactionType.Authorize ||
                transType == TransactionType.AuthorizeCapture)
            {
                liveURL = "https://www.skipjackic.com/scripts/evolvcc.dll?AuthorizeAPI";
                testURL = "https://developer.skipjackic.com/scripts/evolvcc.dll?AuthorizeAPI";
            }
            else if (transType == TransactionType.AuthorizeRecurring)
            {
                liveURL = "https://www.skipjackic.com/scripts/evolvcc.dll?SJAPI_RecurringPaymentAdd";
                testURL = "https://developer.skipjackic.com/scripts/evolvcc.dll?SJAPI_RecurringPaymentAdd";
            }
            else
            {
                liveURL = "https://www.skipjackic.com/scripts/evolvcc.dll?SJAPI_TransactionChangeStatusRequest";
                testURL = "https://developer.skipjackic.com/scripts/evolvcc.dll?SJAPI_TransactionChangeStatusRequest";
            }
            return SendRequestToGateway(requestData, (this.UseTestMode ? testURL : liveURL));
        }

        private string SendRequestToGateway(string requestData, string serverURL)
        {
            //CREATE WEB REQUEST
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(serverURL);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string referer = context.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrEmpty(referer)) httpWebRequest.Referer = referer;
            }
            //SEND REQUEST TO REMOTE SERVER
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            //READ RESPONSE FROM REMOTE SERVER
            string response;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
            {
                response = responseStream.ReadToEnd();
                responseStream.Close();
            }
            //RETURN RESPONSE TO CALLER
            return response;
        }

        private void VerifyGatewayConfig()
        {
            if (string.IsNullOrEmpty(this.SerialNumber))
            {
                throw new InvalidOperationException("Required Parameters missing : SerialNumber");
            }
        }

        // this is the method that will url encode the values before sending 
        private string Encode(string key, string value)
        {
            string encodedValue = string.Empty;
            try
            {
                encodedValue = HttpUtility.UrlEncode(value, System.Text.Encoding.UTF8);
            }
            catch
            {
                throw new Exception("Encoding failure: Key - " + key + " Value - " + value);
            }
            return "&" + key + "=" + encodedValue;
        }// end of the Encode method

        private Transaction ProcessAuthResponse(AuthorizeTransactionRequest authRequest, string responseData, TransactionType transactionType)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;
            transaction.Amount = authRequest.Amount;
            //PARSE THE HTTP ENCODED RESPONSE
            NameValueCollection response = ParseResponseString(responseData);

            bool success = false;
            string responseMessage="";
            int responseCode = 0;

            if (transactionType == TransactionType.Authorize ||
                transactionType == TransactionType.AuthorizeCapture)
            {
                responseCode = AlwaysConvert.ToInt(response.Get("szReturnCode"), -9999);
                if (responseCode == 1)
                {
                    int approveCode = AlwaysConvert.ToInt(response.Get("szIsApproved"), -1);
                    if (approveCode == 1)
                    {
                        success = true;
                    }
                    else
                    {
                        responseMessage = response.Get("szAuthorizationDeclinedMessage");
                    }
                }
                else
                {                    
                    responseMessage = GetMessage(responseCode);
                }
            }

            transaction.ProviderTransactionId = response.Get("szTransactionFileName");
            transaction.TransactionDate = DateTime.UtcNow;
            if (!success)
            {
                transaction.ResponseCode = responseCode.ToString();
                transaction.ResponseMessage = responseMessage;
            }
            transaction.AVSResultCode = response.Get("szAVSResponseCode");
            transaction.CVVResultCode = response.Get("szCVV2ResponseCode");
            if (!success)
            {
                //failed
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            else
            {
                //successful
                transaction.TransactionStatus = TransactionStatus.Successful; 
                transaction.AuthorizationCode = response.Get("szAuthorizationResponseCode");
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }

            return transaction;
        }

        private Transaction ProcessChangeStatusResponse(Payment payment, string responseData, TransactionType transactionType, LSDecimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;

            string headerLine="", dataLine="", msgLine="";
            int index = responseData.IndexOf("\r\n");
            if (index < 0)
            {
                headerLine = responseData;
            }
            else
            {
                headerLine = responseData.Substring(0, index);
                dataLine = responseData.Substring(index + 2);
                index = dataLine.IndexOf("\r\n");
                if (index >= 0)
                {
                    msgLine = dataLine.Substring(index + 2);
                    dataLine = dataLine.Substring(0, index);
                }
                else
                {
                    msgLine = dataLine.Trim();
                    dataLine = string.Empty;
                }
            }

            string[] header = ParseResponseRecord(headerLine);
            string[] response = ParseResponseRecord(dataLine);

            if (header == null || header.Length < 3)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = "E";
                transaction.ResponseMessage = "Invalid response. Header Too Short.";                
            }
            else
            {
                int responseCode = AlwaysConvert.ToInt(header[1],1);
                if (responseCode == 0)
                {
                    bool getAmountFromResponse = true;
                    if (response[3].ToUpper().Equals("SUCCESSFUL"))
                    {
                        //success
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        transaction.ProviderTransactionId = response[6];
                    }
                    else
                    {
                        //failure
                        transaction.TransactionStatus = TransactionStatus.Failed;
                        transaction.ProviderTransactionId = response[6];
                        transaction.ResponseCode = response[5];
                        transaction.ResponseMessage = response[4];
                        //IF THIS IS CAPTURE OR PARTIAL CAPTURE, DO NOT GET AMOUNT FROM RESPONSE
                        getAmountFromResponse = !(transaction.TransactionType == TransactionType.Capture || transaction.TransactionType == TransactionType.PartialCapture);
                    }
                    if (getAmountFromResponse)
                    {
                        //SOMETIMES THE AMOUNT DOES NOT INCLUDE A DECIMAL POINT
                        string tempAmount = response[1];
                        transaction.Amount = AlwaysConvert.ToDecimal(response[1], (decimal)transactionAmount);
                        if (!tempAmount.Contains(".") && (transaction.Amount != transactionAmount))
                        {
                            transaction.Amount = transaction.Amount / 100;
                        }
                    }
                    else transaction.Amount = transactionAmount;
                }
                else
                {
                    //failure
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.ResponseCode = responseCode.ToString();
                    if (string.IsNullOrEmpty(msgLine)) msgLine = GetResponseMessage(responseCode);
                    transaction.ResponseMessage = msgLine;
                }
            }

            return transaction;
        }

        private string GetResponseMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return "Success";
                case -1:
                    return "Invalid Command";
                case -2:
                    return "Parameter Missing";
                case -3:
                    return "Failed retrieving message";
                case -4:
                    return "Invalid Status";
                case -5:
                    return "Failed reading security flags";
                case -6:
                    return "Developer serial number not found";
                case -7:
                    return "Invalid serial number";
                case -11:
                    return "Failed Adding Recurring Payment";
                case -12:
                    return "Invalid Recurring Payment frequency";
                default:
                    return "Unknown Error";
            }
        }

        private string[] ParseResponseRecord(string responseData)
        {
            string recLine;
            int index = responseData.IndexOf("\r\n");
            if (index < 0)
            {
                //there is probably one record only.
                recLine = responseData;

            }
            else
            {
                //read the first line only
                recLine = responseData.Substring(0, index);
            }

            string[] separators = new string[] { "\",\"" };
            string[] returnArray = recLine.Split(separators, StringSplitOptions.None);
            if (returnArray!=null && returnArray.Length > 0)
            {
                returnArray[0] = TrimQuotes(returnArray[0]);
                returnArray[returnArray.Length - 1] = TrimQuotes(returnArray[returnArray.Length-1]);
            }

            return returnArray;
        }

        private NameValueCollection ParseResponseString(string responseData)
        {
            NameValueCollection resp = new NameValueCollection();

            int index = responseData.IndexOf("\r\n");
            if (index < 0)
            {
                throw new Exception("Failed to parse gateway response");
            }
            
            string paramsLine = responseData.Substring(0, index);
            string valuesLine = responseData.Substring(index + 2);

            int index2 = valuesLine.IndexOf("\r\n");
            if (index2 >= 0)
            {
                //there might be a third error details record. Ignore it.
                valuesLine = valuesLine.Substring(0, index2);
            }

            string[] separators = new string[] {"\",\""};

            string[] paramList = paramsLine.Split(separators, StringSplitOptions.None);
            string[] valueList = valuesLine.Split(separators, StringSplitOptions.None);

            if (paramList == null || valueList == null)
            {
                throw new Exception("Failed to parse gateway response");
            }

            if (paramList.Length != valueList.Length)
            {
                throw new Exception("Failed to parse response. Number of parameters did not match number of values");
            }

            for (int i = 0; i < paramList.Length; i++)
            {
                resp.Add(TrimQuotes(paramList[i]), TrimQuotes(valueList[i]));                
            }

            return resp;
        }

        private string TrimQuotes(string value)
        {
            if (value.StartsWith("\""))
            {
                value = value.Substring(1);
            }
            if (value.EndsWith("\""))
            {
                value = value.Substring(0, value.Length - 1);
            }
            return value;
        }
        
        private string GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 7) return "0";
                    if (authorizeRequest.PaymentFrequency == 14) return "1";
                    if (authorizeRequest.PaymentFrequency == 15) return "2";
                    if (authorizeRequest.PaymentFrequency == 28) return "4";
                    break;
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return "3";
                    if (authorizeRequest.PaymentFrequency == 2) return "5";
                    if (authorizeRequest.PaymentFrequency == 3) return "6";
                    if (authorizeRequest.PaymentFrequency == 6) return "7";
                    if (authorizeRequest.PaymentFrequency == 12) return "8";
                    break;
            }
            return string.Empty;
        }

        private DateTime GetNextPaymentDate(string payPeriod)
        {
            DateTime startDate;
            switch (payPeriod)
            {
                case "0":
                    startDate = LocaleHelper.LocalNow.AddDays(7);
                    break;
                case "1":
                    startDate = LocaleHelper.LocalNow.AddDays(14);
                    break;
                case "2":
                    startDate = LocaleHelper.LocalNow.AddDays(15);
                    break;
                case "3":
                    startDate = LocaleHelper.LocalNow.AddMonths(1);
                    break;
                case "4":
                    startDate = LocaleHelper.LocalNow.AddDays(28);
                    break;
                case "5":
                    startDate = LocaleHelper.LocalNow.AddMonths(2);
                    break;
                case "6":
                    startDate = LocaleHelper.LocalNow.AddMonths(3);
                    break;
                case "7":
                    startDate = LocaleHelper.LocalNow.AddMonths(6);
                    break;
                case "8":
                    startDate = LocaleHelper.LocalNow.AddYears(1);
                    break;
                default:
                    throw new ArgumentException("The specified payPeriod (" + payPeriod + ") is not valid for this processor.", "payPeriod");
            }
            //SKIPJACK CANNOT START RECURRING PAYMENTS ON THE 29, 30, OR 31 DAY OF MONTH
            //MUST ADVANCE IT TO THE FIRST OF NEXT MONTH
            if (startDate.Day > 28)
            {
                startDate = startDate.AddMonths(1).AddDays(-1 * (startDate.Day - 1));
            }
            return startDate;
        }

        private string GetMessage(int responseCode)
        {
            string responseMessage = "";

            if (responseCode == 0)
            {
                responseMessage = "Call failed.";
            }
            else if (responseCode == 1)
            {
                responseMessage = "Status complete.";
            }
            else if (responseCode == -1)
            {
                responseMessage = "Error in request.";
            }
            else if (responseCode == -34)
            {
                responseMessage = "Error failed authorization.";
            }
            else if (responseCode == -35)
            {
                responseMessage = "Error invalid credit card number.";
            }
            else if (responseCode == -37)
            {
                responseMessage = "Error failed dial.";
            }
            else if (responseCode == -39)
            {
                responseMessage = "Error length serial number.";
            }
            else if (responseCode == -51)
            {
                responseMessage = "Error length zip code.";
            }
            else if (responseCode == -52)
            {
                responseMessage = "Error length shipto zip code.";
            }
            else if (responseCode == -53)
            {
                responseMessage = "Error length expiration date.";
            }
            else if (responseCode == -54)
            {
                responseMessage = "Error length expiration year or month.";
            }
            else if (responseCode == -55)
            {
                responseMessage = "Error length street address.";
            }
            else if (responseCode == -56)
            {
                responseMessage = "Error length shipto street address.";
            }
            else if (responseCode == -57)
            {
                responseMessage = "Error length transaction amount. ";
            }
            else if (responseCode == -58)
            {
                responseMessage = "Error length name.";
            }
            else if (responseCode == -59)
            {
                responseMessage = "Error length location.";
            }
            else if (responseCode == -60)
            {
                responseMessage = "Error length state.";
            }
            else if (responseCode == -61)
            {
                responseMessage = "Error length shipto state.";
            }
            else if (responseCode == -62)
            {
                responseMessage = "Error length order string.";
            }
            else if (responseCode == -64)
            {
                responseMessage = "Error invalid phone number.";
            }
            else if (responseCode == -79)
            {
                responseMessage = "Error length customer name.";
            }
            else if (responseCode == -80)
            {
                responseMessage = "Error length shipto customer name.";
            }
            else if (responseCode == -81)
            {
                responseMessage = "Error length customer location.";
            }
            else if (responseCode == -82)
            {
                responseMessage = "Error length customer state.";
            }
            else if (responseCode == -83)
            {
                responseMessage = "Error length shipto phone.";
            }
            else if (responseCode == -65)
            {
                responseMessage = "Error empty name.";
            }
            else if (responseCode == -66)
            {
                responseMessage = "Error empty email.";
            }
            else if (responseCode == -67)
            {
                responseMessage = "Error empty street address.";
            }
            else if (responseCode == -68)
            {
                responseMessage = "Error empty city.";
            }
            else if (responseCode == -69)
            {
                responseMessage = "Error empty state.";
            }
            else if (responseCode == -70)
            {
                responseMessage = "Error empty zip code.";
            }
            else if (responseCode == -71)
            {
                responseMessage = "Error empty order number.";
            }
            else if (responseCode == -72)
            {
                responseMessage = "Error empty account (credit card) number.";
            }
            else if (responseCode == -73)
            {
                responseMessage = "Error empty credit card expiration month.";
            }
            else if (responseCode == -74)
            {
                responseMessage = "Error empty credit card expiration year.";
            }
            else if (responseCode == -75)
            {
                responseMessage = "Error empty serial number.";
            }
            else if (responseCode == -76)
            {
                responseMessage = "Error empty transaction amount.";
            }
            else if (responseCode == -77)
            {
                responseMessage = "Error empty order string.";
            }
            else if (responseCode == -78)
            {
                responseMessage = "Error empty phone number.";
            }
            else if (responseCode == -84)
            {
                responseMessage = "Transaction with a duplicate order number was submitted.";
            }
            else if (responseCode == -91)
            {
                responseMessage = "CVV2 Error.";
            }
            else if (responseCode == -93)
            {
                responseMessage = "Blind credits not allowed.";
            }
            else if (responseCode == -94)
            {
                responseMessage = "Blind credits failed.";
            }
            else if (responseCode == -95)
            {
                responseMessage = "Voice Authorization not allowed";
            }
            else if (responseCode == -96)
            {
                responseMessage = "Voice Authorization failed";
            }
            else
            {
                responseMessage = "Unknown Error Processing Payment (SkipJack).";
            }

            return responseMessage;
        }
    }
}
