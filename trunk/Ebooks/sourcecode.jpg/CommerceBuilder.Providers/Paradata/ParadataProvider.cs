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
using Paygateway;

namespace CommerceBuilder.Payments.Providers.Paradata
{
    public class PaymentProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        string _AccountToken = "";
        bool _UseAuthCapture = false;
        bool _UseTestMode = true;

        public override string Name
        {
            get { return "Paradata"; }
        }

        public override string Version
        {
            get { return "NET Client v3.0.0"; }
        }

        public override string Description
        {
            get { return "Paradata Description here"; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.Paradata.Logo.gif");
            return string.Empty;
        }

        public string AccountToken
        {
            get{return _AccountToken;}
            set { _AccountToken = value; }
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

        public override string ConfigReference
        {
            get { return "Account Token : " + _AccountToken; }
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
            gatewayLink.NavigateUrl = "http://www.paradata.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable Paradata, you must provide your Paradata Account Token.</p>"));
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
            currentCell.Controls.Add(new LiteralControl("Account Token:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your Paradata Account Token is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtAccountToken = new TextBox();
            txtAccountToken.ID = "Config_AccountToken";
            txtAccountToken.Columns = 70;
            txtAccountToken.MaxLength = 280;
            txtAccountToken.Text = this.AccountToken;
            currentCell.Controls.Add(txtAccountToken);
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and Paradata is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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
                return (SupportedTransactions.Authorize | SupportedTransactions.AuthorizeCapture 
                    | SupportedTransactions.Capture | SupportedTransactions.PartialCapture 
                    | SupportedTransactions.Refund  | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Void);
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("AccountToken")) AccountToken = ConfigurationData["AccountToken"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("UseTestMode")) UseTestMode = AlwaysConvert.ToBool(ConfigurationData["UseTestMode"],true);
        }

        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            VerifyStatus();
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");
            User user = order.User;
            if(user==null) throw new ArgumentNullException("request.Payment.Order.User");
            
            CreditCardRequest request = InitializeAuthRequest(authorizeRequest);

            CreditCardResponse response = null;

            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                string reqDebug = BuildRequestDebug(request);
                this.RecordCommunication(this.Name, CommunicationDirection.Send, reqDebug, null);
            }

            //TODO : Test mode is not supported.
            if (this.UseTestMode)
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }
            else
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }

            if (response != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    string respDebug = BuildResponseDebug(response);
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, respDebug, null);
                }

                if (this.UseAuthCapture)
                {
                    return ProcessResponse(payment, response, TransactionType.AuthorizeCapture, authorizeRequest.Amount); 
                }
                else
                {
                    return ProcessResponse(payment, response, TransactionType.Authorize, authorizeRequest.Amount);
                }
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
            
            CreditCardRequest request = InitializeCaptureRequest(payment, authorizeTransaction, captureRequest.Amount);

            CreditCardResponse response = null;

            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                string reqDebug = BuildRequestDebug(request);
                this.RecordCommunication(this.Name, CommunicationDirection.Send, reqDebug, null);
            }

            //TODO : Test mode is not supported.
            if (this.UseTestMode)
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }
            else
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }

            if (response != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    string respDebug = BuildResponseDebug(response);
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, respDebug, null);
                }                
                return ProcessResponse(payment, response, captureRequest.TransactionType, captureRequest.Amount);
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

            CreditCardRequest request = InitializeRefundRequest(payment, captureTransaction, creditRequest.Amount);

            CreditCardResponse response = null;

            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                string reqDebug = BuildRequestDebug(request);
                this.RecordCommunication(this.Name, CommunicationDirection.Send, reqDebug, null);
            }

            //TODO : Test mode is not supported.
            if (this.UseTestMode)
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }
            else
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }

            if (response != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    string respDebug = BuildResponseDebug(response);
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, respDebug, null);
                }
                return ProcessResponse(payment, response, creditRequest.TransactionType, creditRequest.Amount);
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

            CreditCardRequest request = InitializeVoidRequest(payment, authorizeTransaction);

            CreditCardResponse response = null;

            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                string reqDebug = BuildRequestDebug(request);
                this.RecordCommunication(this.Name, CommunicationDirection.Send, reqDebug, null);
            }

            //TODO : Test mode is not supported.
            if (this.UseTestMode)
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }
            else
            {
                response = (CreditCardResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }

            if (response != null)
            {
                //RECORD RESPONSE
                if (this.UseDebugMode)
                {
                    string respDebug = BuildResponseDebug(response);
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, respDebug, null);
                }

                return ProcessResponse(payment, response, TransactionType.Void, voidRequest.Amount);
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

            //VALIDATE THE PAYMENT PERIOD
            int payPeriod = GetPayPeriod(authorizeRequest);
            if (payPeriod == int.MinValue)
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

            RecurringRequest request = InitializeAuthRecurringRequest(authorizeRequest,payPeriod);
            RecurringResponse recResponse = null;

            //RECORD REQUEST : TODO
            /*if (this.UseDebugMode)
            {
                string reqDebug = BuildRequestDebug(request);
                this.RecordCommunication(this.Name, CommunicationDirection.Send, reqDebug);
            }*/

            //TODO : Test mode is not supported.
            if (this.UseTestMode)
            {
                recResponse = (RecurringResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }
            else
            {
                recResponse = (RecurringResponse)TransactionClient.doTransaction(request, this.AccountToken);
            }

            if (recResponse != null)
            {
                //RECORD RESPONSE 
                if (this.UseDebugMode)
                {
                    string respDebug = BuildRecurringResponseDebug(recResponse);
                    this.RecordCommunication(this.Name, CommunicationDirection.Receive, respDebug, null);
                }

                tr2 = ProcessRecurringResponse(authorizeRequest, recResponse);
                response.AddTransaction(tr2);
                response.Status = tr2.TransactionStatus;

                return response;
            }
            else
            {
                throw new Exception("Operation Failed, Response is null.");
            }
        }

        private CreditCardRequest InitializeAuthRequest(AuthorizeTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            Order order = payment.Order;
            User user = order.User;

            VerifyPaymentInstrument(payment);
            CreditCardRequest request = new CreditCardRequest();

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            //set user info
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                string[] names = accountData["AccountName"].Split(" ".ToCharArray());
                request.setBillFirstName(names[0]);
                if (names.Length > 1)
                {
                    request.setBillLastName(string.Join(" ", names, 1, names.Length - 1));
                }
                else
                {
                    //no last name. what to do? send empty string? TODO : check the API                    
                    request.setBillLastName(string.Empty);
                }
            }
            else
            {
                request.setBillFirstName(order.BillToFirstName);
                request.setBillLastName(order.BillToLastName);
            }

            request.setBillEmail(order.BillToEmail);
            request.setBillCompany(order.BillToCompany);
            request.setBillAddressOne(order.BillToAddress1);
            request.setBillAddressTwo(order.BillToAddress2);
            request.setBillCity(order.BillToCity);
            request.setBillStateOrProvince(order.BillToProvince);
            request.setBillPostalCode(order.BillToPostalCode);
            request.setBillCountryCode(order.BillToCountryCode);
            request.setBillPhone(order.BillToPhone);
            request.setBillFax(order.BillToFax);
            request.setOrderUserId(user.UserId.ToString());

            SetCreditCardData(request, payment, accountData);

            // set charge details
            if (this.UseAuthCapture)
            {
                request.setChargeType(CreditCardRequest.SALE);
            }
            else
            {
                request.setChargeType(CreditCardRequest.AUTH);
            }
            request.setChargeTotal((double)authorizeRequest.Amount);
            //TODO set tax and shipping 
            //request.setTaxAmount();
            //request.setShippingCharge();
            if (!string.IsNullOrEmpty(payment.CurrencyCode))
            {
                request.setCurrency(payment.CurrencyCode);
            }

            request.setOrderId(order.OrderId.ToString());
            request.setInvoiceNumber(order.OrderNumber.ToString());
            //TODO add description if present
            //if(!string.IsNullOrEmpty(order.Notes)) {
            //    request.setOrderDescription(order.Notes);
            //}

            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                request.setCustomerIpAddress(context.Request.ServerVariables["REMOTE_HOST"]);
            }
            
            return request;
        }

        private CreditCardRequest InitializeCaptureRequest(Payment payment, Transaction authorizeTransaction, LSDecimal amount)
        {
            VerifyPaymentInstrument(payment);
            CreditCardRequest request = new CreditCardRequest();
            request.setChargeType(CreditCardRequest.CAPTURE);
            request.setReferenceId(authorizeTransaction.ProviderTransactionId);
            request.setOrderId(payment.Order.OrderId.ToString());

            LSDecimal minAmount = 0.01M;

            if (amount > authorizeTransaction.Amount || amount < minAmount )
            {
                throw new InvalidOperationException("Capture Amount must be from 0.01 to the previously authorized amount");
            }

            request.setChargeTotal((double)amount);

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            SetCreditCardData(request, payment, accountData);

            if (!string.IsNullOrEmpty(payment.CurrencyCode))
            {
                request.setCurrency(payment.CurrencyCode);
            }

            return request;
        }

        private CreditCardRequest InitializeRefundRequest(Payment payment, Transaction captureTransaction, LSDecimal amount)
        {
            VerifyPaymentInstrument(payment);
            CreditCardRequest request = new CreditCardRequest();
            request.setChargeType(CreditCardRequest.CREDIT);
            request.setReferenceId(captureTransaction.ProviderTransactionId);
            request.setOrderId(payment.Order.OrderId.ToString());

            LSDecimal minAmount = 0.01M;

            if (amount > captureTransaction.Amount || amount < minAmount)
            {
                throw new InvalidOperationException("Refund Amount must be from 0.01 to the previously captured amount");
            }

            request.setChargeTotal((double)amount);

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            SetCreditCardData(request, payment, accountData);

            if (!string.IsNullOrEmpty(payment.CurrencyCode))
            {
                request.setCurrency(payment.CurrencyCode);
            }

            return request;
        }

        private CreditCardRequest InitializeVoidRequest(Payment payment, Transaction authorizeTransaction)
        {
            VerifyPaymentInstrument(payment);
            CreditCardRequest request = new CreditCardRequest();
            request.setChargeType(CreditCardRequest.VOID);
            request.setReferenceId(authorizeTransaction.ProviderTransactionId);
            request.setOrderId(payment.Order.OrderId.ToString());
            
            request.setChargeTotal((double)authorizeTransaction.Amount);

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            SetCreditCardData(request, payment, accountData);

            if (!string.IsNullOrEmpty(payment.CurrencyCode))
            {
                request.setCurrency(payment.CurrencyCode);
            }

            return request;
        }

        private RecurringRequest InitializeAuthRecurringRequest(AuthorizeRecurringTransactionRequest authRequest, int payPeriod)
        {
            VerifyPaymentInstrument(authRequest.Payment);
            RecurringRequest request = new RecurringRequest();

            Payment payment = authRequest.Payment;
            Order order = payment.Order;
            User user = payment.Order.User;
            //Address address = user.PrimaryAddress;

            //credit card data
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            string accountNumber = accountData.GetValue("AccountNumber");            
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
            string expirationYear = accountData.GetValue("ExpirationYear");

            request.setCommand(RecurringRequest.COMMAND_ADD_CUSTOMER_ACCOUNT_AND_RECURRENCE);                        
            request.setAccountType(RecurringRequest.ACCOUNT_TYPE_CREDIT_CARD);
            request.setCreditCardNumber(accountNumber);
            request.setExpireMonth(expirationMonth);
            request.setExpireYear(expirationYear);

            request.setBillingAddress(order.BillToAddress1 + ", " + order.BillToAddress2 + "," + order.BillToCity);
            request.setZipOrPostalCode(order.BillToPostalCode);
            request.setCountryCode(order.BillToCountryCode);

            LSDecimal amount = authRequest.RecurringChargeSpecified ? authRequest.RecurringCharge : authRequest.Amount;
            request.setChargeTotal((double)amount);

            request.setCustomerId(user.UserId.ToString());
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                request.setCustomerName(accountData["AccountName"]);
            }
            else
            {
                request.setCustomerName(order.BillToFirstName + " " + order.BillToLastName);
            }
            request.setDescription(authRequest.SubscriptionName);

            string strEmail = string.IsNullOrEmpty(order.BillToEmail) ? user.Email : order.BillToEmail; 
            request.setEmailAddress(user.Email);
            
            request.setNotifyCustomer(true);
            request.setNumberOfRetries(3);
            request.setRecurrenceId(authRequest.Payment.OrderId.ToString());
            request.setPeriod(payPeriod);

            DateTime startDt = LocaleHelper.LocalNow;
            if (authRequest.RecurringChargeSpecified)
            {
                startDt = GetNextPaymentDate(payPeriod);
            }
            request.setStartDay(startDt.Day);
            request.setStartMonth(startDt.Month);
            request.setStartYear(startDt.Year);

            DateTime endDt = GetEndDate(startDt, payPeriod, authRequest.NumberOfPayments);

            request.setEndDay(endDt.Day);
            request.setEndMonth(endDt.Month);
            request.setEndYear(endDt.Year);

            return request;
        }

        private int GetPayPeriod(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            switch (authorizeRequest.PaymentFrequencyUnit)
            {
                case CommerceBuilder.Products.PaymentFrequencyUnit.Day:
                    if (authorizeRequest.PaymentFrequency == 7) return RecurringRequest.PERIOD_WEEKLY;
                    if (authorizeRequest.PaymentFrequency == 14) return RecurringRequest.PERIOD_BIWEEKLY;
                    if (authorizeRequest.PaymentFrequency == 15) return RecurringRequest.PERIOD_SEMIMONTHLY;
                    break;                
                case CommerceBuilder.Products.PaymentFrequencyUnit.Month:
                    if (authorizeRequest.PaymentFrequency == 1) return RecurringRequest.PERIOD_MONTHLY;
                    if (authorizeRequest.PaymentFrequency == 3) return RecurringRequest.PERIOD_QUARTERLY;
                    if (authorizeRequest.PaymentFrequency == 12) return RecurringRequest.PERIOD_ANNUAL;                    
                    break;
            }
            return int.MinValue;
        }

        private DateTime GetNextPaymentDate(int payPeriod)
        {
            if (payPeriod == RecurringRequest.PERIOD_ANNUAL)
            {
                return LocaleHelper.LocalNow.AddYears(1);
            }
            else if (payPeriod == RecurringRequest.PERIOD_BIWEEKLY)
            {
                return LocaleHelper.LocalNow.AddDays(14);
            }
            else if (payPeriod == RecurringRequest.PERIOD_MONTHLY)
            {
                return LocaleHelper.LocalNow.AddMonths(1);
            }
            else if (payPeriod == RecurringRequest.PERIOD_QUARTERLY)
            {
                return LocaleHelper.LocalNow.AddMonths(3);
            }
            else if (payPeriod == RecurringRequest.PERIOD_SEMIMONTHLY)
            {
                return LocaleHelper.LocalNow.AddDays(15);
            }
            else if (payPeriod == RecurringRequest.PERIOD_WEEKLY)
            {
                return LocaleHelper.LocalNow.AddDays(7);
            }
            else
            {
                return LocaleHelper.LocalNow;
            }
        }

        private DateTime GetEndDate(DateTime startDt, int payPeriod, int numPayments)
        {
            if (payPeriod == RecurringRequest.PERIOD_ANNUAL)
            {
                return startDt.AddYears(numPayments);
            }
            else if(payPeriod == RecurringRequest.PERIOD_BIWEEKLY)
            {
                return startDt.AddDays(14 * numPayments);
            }
            else if(payPeriod == RecurringRequest.PERIOD_MONTHLY)
            {
                return startDt.AddMonths(1 * numPayments);
            }
            else if(payPeriod == RecurringRequest.PERIOD_QUARTERLY)
            {
                return startDt.AddMonths(3 * numPayments);
            }
            else if(payPeriod == RecurringRequest.PERIOD_SEMIMONTHLY)
            {
                return startDt.AddDays(15 * numPayments);
            }
            else if(payPeriod == RecurringRequest.PERIOD_WEEKLY)
            {
                return startDt.AddDays(7 * numPayments);
            }            
            else 
            {
                return startDt;
            }
        }

        private void VerifyStatus()
        {
            if (this.AccountToken == null || this.AccountToken.Length == 0)
            {
                throw new InvalidOperationException("Account Token missing");
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

        private void SetCreditCardData(CreditCardRequest request, Payment payment, AccountDataDictionary accountData)
        {
            //set credit card data
            string accountNumber = accountData.GetValue("AccountNumber");            
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            if (expirationMonth.Length == 1) expirationMonth.Insert(0, "0");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (string.IsNullOrEmpty(accountNumber))
            {
                //credit card data missing. can't do much
                return;
            }
            request.setCreditCardNumber(accountNumber);
            request.setExpireMonth(expirationMonth);
            request.setExpireYear(expirationYear);
 
            string securityCode = accountData.GetValue("SecurityCode");
            if (!string.IsNullOrEmpty(securityCode))
            {
                request.setCreditCardVerificationNumber(securityCode);
            }
        }

        private Transaction ProcessResponse(Payment payment, CreditCardResponse response, TransactionType transactionType, LSDecimal transAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = transactionType;
            transaction.Amount = transAmount;

            if (response.getResponseCode() != 1)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = response.getResponseCode().ToString();
                transaction.ResponseMessage = response.getResponseCodeText();
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = response.getReferenceId(); // getBankTransactionId();
                transaction.TransactionDate = response.getTimeStamp();// DateTime.UtcNow;                
                transaction.ResponseCode = response.getResponseCode().ToString();
                transaction.ResponseMessage = response.getResponseCodeText();
                transaction.AuthorizationCode = response.getBankApprovalCode();
                transaction.AVSResultCode = response.getAvsCode();
                //if (transaction.AVSResultCode.Equals("P") || transaction.AVSResultCode.Equals("B")) transaction.AVSResultCode = "U";
                transaction.CVVResultCode = response.getCreditCardVerificationResponse();
                //payment.ReferenceNumber = response.getReferenceId();

                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }

            return transaction;
        }

        private Transaction ProcessRecurringResponse(AuthorizeRecurringTransactionRequest authRequest, RecurringResponse response)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.AuthorizeRecurring;
            LSDecimal transAmount = authRequest.RecurringChargeSpecified ? authRequest.RecurringCharge : authRequest.Amount;
            transaction.Amount = transAmount;

            if (response.getResponseCode() != 1)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = response.getResponseCode().ToString();
                transaction.ResponseMessage = response.getResponseCodeText();
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
                //transaction.ProviderTransactionId = response.getReferenceId(); 
                transaction.TransactionDate = response.getTimeStamp();
                transaction.ResponseCode = response.getResponseCode().ToString();
                transaction.ResponseMessage = response.getResponseCodeText();

                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                }
            }

            return transaction;
        }

        private string BuildRequestDebug(CreditCardRequest request)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\nParadata PayGateway.CreditCardRequest Object\r\n");
            sb.Append("BankApprovalCode=" + request.getBankApprovalCode() + "\r\n");
            sb.Append("BillAddressOne=" + request.getBillAddressOne() + "\r\n");
            sb.Append("BillAddressTwo=" + request.getBillAddressTwo() + "\r\n");
            sb.Append("BillCity=" + request.getBillCity() + "\r\n");
            sb.Append("BillCompany=" + request.getBillCompany() + "\r\n");
            sb.Append("BillCountryCode=" + request.getBillCountryCode() + "\r\n");
            sb.Append("BillCustomerTitle=" + request.getBillCustomerTitle() + "\r\n");
            sb.Append("BillEmail=" + request.getBillEmail() + "\r\n");
            sb.Append("BillFax=" + request.getBillFax() + "\r\n");
            sb.Append("BillFirstName=" + request.getBillFirstName() + "\r\n");
            sb.Append("BillLastName=" + request.getBillLastName() + "\r\n");
            sb.Append("BillMiddleName=" + request.getBillMiddleName() + "\r\n");
            sb.Append("BillNote=" + request.getBillNote() + "\r\n");
            sb.Append("BillPhone=" + request.getBillPhone() + "\r\n");
            sb.Append("BillPostalCode=" + request.getBillPostalCode() + "\r\n");
            sb.Append("BillStateOrProvince=" + request.getBillStateOrProvince() + "\r\n");
            sb.Append("BuyerCode=" + request.getBuyerCode() + "\r\n");
            sb.Append("CardBrand=" + request.getCardBrand() + "\r\n");
            sb.Append("ChargeTotal=" + request.getChargeTotal() + "\r\n");
            sb.Append("ChargeType=" + request.getChargeType() + "\r\n");
            sb.Append("Currency=" + request.getCurrency() + "\r\n");
            sb.Append("CaptureReferenceId=" + request.getCaptureReferenceId() + "\r\n");
            sb.Append("CustomerIpAddress=" + request.getCustomerIpAddress() + "\r\n");
            sb.Append("Credit Card Number=" + MakeReferenceNumber(request.getCreditCardNumber()) + "\r\n");
            sb.Append("ExpireMonth=" + request.getExpireMonth() + "\r\n");
            sb.Append("ExpireYear=" + request.getExpireYear() + "\r\n");
            sb.Append("InvoiceNumber=" + request.getInvoiceNumber() + "\r\n");
            sb.Append("OrderCustomerId=" + request.getOrderCustomerId() + "\r\n");
            sb.Append("OrderDescription=" + request.getOrderDescription() + "\r\n");
            sb.Append("OrderId=" + request.getOrderId() + "\r\n");
            sb.Append("OrderUserId=" + request.getOrderUserId() + "\r\n");
            sb.Append("ShipAddressOne=" + request.getShipAddressOne() + "\r\n");
            sb.Append("ShipAddressTwo=" + request.getShipAddressTwo() + "\r\n");
            sb.Append("ShipCity=" + request.getShipCity() + "\r\n");
            sb.Append("ShipCompany=" + request.getShipCompany() + "\r\n");
            sb.Append("ShipCountryCode=" + request.getShipCountryCode() + "\r\n");
            sb.Append("ShipCustomerTitle=" + request.getShipCustomerTitle() + "\r\n");
            sb.Append("ShipEmail=" + request.getShipEmail() + "\r\n");
            sb.Append("ShipFax=" + request.getShipFax() + "\r\n");
            sb.Append("ShipFirstName=" + request.getShipFirstName() + "\r\n");
            sb.Append("ShipLastName=" + request.getShipLastName() + "\r\n");
            sb.Append("ShipMiddleName=" + request.getShipMiddleName() + "\r\n");
            sb.Append("ShipNote=" + request.getShipNote() + "\r\n");
            sb.Append("ShipPhone=" + request.getShipPhone() + "\r\n");
            sb.Append("ShippingCharge=" + request.getShippingCharge() + "\r\n");
            sb.Append("ShipPostalCode=" + request.getShipPostalCode() + "\r\n");
            sb.Append("ShipStateOrProvince=" + request.getShipStateOrProvince() + "\r\n");            
            return sb.ToString();
        }

        private string BuildResponseDebug(CreditCardResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\nParadata PayGateway.CreditCardResponse Object\r\n");
            sb.Append("AuthorizedAmount=" + response.getAuthorizedAmount() + "\r\n");
            sb.Append("AvsCode=" + response.getAvsCode() + "\r\n");
            sb.Append("BankApprovalCode=" + response.getBankApprovalCode() + "\r\n");
            sb.Append("BankTransactionId=" + response.getBankTransactionId() + "\r\n");
            sb.Append("BatchId=" + response.getBatchId() + "\r\n");
            sb.Append("CapturedAmount=" + response.getCapturedAmount() + "\r\n");
            sb.Append("CaptureReferenceId=" + response.getCaptureReferenceId() + "\r\n");
            sb.Append("CreditCardVerificationResponse=" + response.getCreditCardVerificationResponse() + "\r\n");
            sb.Append("CreditedAmount=" + response.getCreditedAmount() + "\r\n");
            sb.Append("OrderId=" + response.getOrderId() + "\r\n");
            sb.Append("OriginalAuthorizedAmount=" + response.getOriginalAuthorizedAmount() + "\r\n");
            sb.Append("PayerAuthenticationResponse=" + response.getPayerAuthenticationResponse() + "\r\n");
            sb.Append("ReferenceId=" + response.getReferenceId() + "\r\n");
            sb.Append("ResponseCode=" + response.getResponseCode() + "\r\n");
            sb.Append("ResponseCodeText=" + response.getResponseCodeText() + "\r\n");
            sb.Append("RetryRecommended=" + response.getRetryRecommended() + "\r\n");
            sb.Append("SecondaryResponseCode=" + response.getSecondaryResponseCode() + "\r\n");
            sb.Append("State=" + response.getState() + "\r\n");
            sb.Append("TimeStamp=" + response.getTimeStamp() + "\r\n");

            return sb.ToString();
        }

        private string BuildRecurringResponseDebug(RecurringResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\nParadata PayGateway.RecurringResponse Object\r\n");
            sb.Append("ResponseCode=" + response.getResponseCode() + "\r\n");
            sb.Append("ResponseCodeText=" + response.getResponseCodeText() + "\r\n");
            sb.Append("SecondaryResponseCode=" + response.getSecondaryResponseCode() + "\r\n");
            sb.Append("TimeStamp=" + response.getTimeStamp() + "\r\n");
            //sb.Append("responseString=" + response.responseString + "\r\n");

            return sb.ToString();
        }
    }
}
