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
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments.Providers.Protx
{
    public class Protx : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        private static string VPSVersion = "2.23";
        private const string service_register = "register";
        private const string service_release = "release";
        private const string service_abort = "abort";
        private const string service_refund = "refund";

        string _VendorName;
        string _TransactionKey;
        bool _UseAuthCapture = false;
        GatewayModeOption _GatewayMode = GatewayModeOption.TestMode;
        AccountTypeOption _AccountType = AccountTypeOption.E;
        bool _IsSecureSource;

        public string VendorName
        {
            get { return _VendorName; }
            set { _VendorName = value; }
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

        public AccountTypeOption AccountType
        {
            get { return _AccountType; }
            set { _AccountType = value; }
        }

        public bool IsSecureSource
        {
            get { return _IsSecureSource; }
            set { _IsSecureSource = value; }
        }

        public override string Name
        {
            get { return "SagePay (formerly Protx)"; }
        }

        public override string Description
        {
            get { return "SagePay provides secure online credit and debit card payment solutions for thousands of online and mail order businesses across the UK. For more information about SagePay, see the <a href=\"http://www.sagepay.com/aboutus.asp\" target=\"_blank\">About Us</a> page."; }
        }

        public override string GetLogoUrl(ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.Protx.Logo.jpg");
            return string.Empty;
        }

        public override string Version
        {
            get { return "SagePay VSP Direct " + VPSVersion; }
        }

        public override string ConfigReference
        {
            get { return "Vendor Login: " + this.VendorName; }
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Refund
                    | SupportedTransactions.Void);
            }
        }

        private string GetActiveUrl(string service)
        {
            switch (GatewayMode)
            {
                case GatewayModeOption.LiveMode:
                    switch (service)
                    {
                        case service_register:
                            return "https://live.sagepay.com/gateway/service/vspdirect-register.vsp";
                        case service_release:
                            return "https://live.sagepay.com/gateway/service/release.vsp";
                        case service_abort:
                            return "https://live.sagepay.com/gateway/service/abort.vsp";
                        case service_refund:
                            return "https://live.sagepay.com/gateway/service/refund.vsp";
                        default:
                            throw new PaymentGatewayProviderException("Service " + service + " not supported.");
                    }
                case GatewayModeOption.TestMode:
                    switch (service)
                    {
                        case service_register:
                            return "https://test.sagepay.com/gateway/service/vspdirect-register.vsp";
                        case service_release:
                            return "https://test.sagepay.com/gateway/service/release.vsp";
                        case service_abort:
                            return "https://test.sagepay.com/gateway/service/abort.vsp";
                        case service_refund:
                            return "https://test.sagepay.com/gateway/service/refund.vsp";
                        default:
                            throw new PaymentGatewayProviderException("Service " + service + " not supported.");
                    }
                case GatewayModeOption.SimulatedMode:
                    switch (service)
                    {
                        case service_register:
                            return "https://test.sagepay.com/Simulator/VSPDirectGateway.asp";
                        case service_release:
                            return "https://test.sagepay.com/Simulator/VSPServerGateway.asp?Service=VendorReleaseTx";
                        case service_abort:
                            return "https://test.sagepay.com/Simulator/VSPServerGateway.asp?Service=VendorAbortTx";
                        case service_refund:
                            return "https://test.sagepay.com/Simulator/VSPServerGateway.asp?Service=VendorRefundTx";
                        default:
                            throw new PaymentGatewayProviderException("Service " + service + " not supported.");
                    }
                default:
                    throw new PaymentGatewayProviderException("Unrecognized gateway mode: " + GatewayMode.ToString());
            }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("VendorName")) VendorName = ConfigurationData["VendorName"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("GatewayMode")) GatewayMode = (GatewayModeOption)AlwaysConvert.ToInt(ConfigurationData["GatewayMode"]);
            if (ConfigurationData.ContainsKey("AccountType")) AccountType = (AccountTypeOption)AlwaysConvert.ToInt(ConfigurationData["AccountType"]);
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
            gatewayLink.NavigateUrl = "http://www.sagepay.com";
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">To enable SagePay VSP Direct, you must provide your Vendor Name.</p>"));
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

            //GET THE Vendor Name 
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Vendor Name:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Your SagePay VSP Direct Vendor Name is required.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtVendorName = new TextBox();
            txtVendorName.ID = "Config_VendorName";
            txtVendorName.Columns = 20;
            txtVendorName.MaxLength = 50;
            txtVendorName.Text = this.VendorName;
            currentCell.Controls.Add(txtVendorName);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE Account Type
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Account Type:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">The type of merchant account in use.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblAccountType = new RadioButtonList();
            rblAccountType.ID = "Config_AccountType";
            rblAccountType.Items.Add(new ListItem("E : The e-commerce merchant account (default)", ((int)AccountTypeOption.E).ToString()));
            rblAccountType.Items.Add(new ListItem("C : The continuous authority merchant account", ((int)AccountTypeOption.C).ToString()));
            rblAccountType.Items.Add(new ListItem("M : The mail order, telephone order account", ((int)AccountTypeOption.M).ToString()));
            rblAccountType.Items[(int)this.AccountType].Selected = true;
            currentCell.Controls.Add(rblAccountType);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);


            //GET THE AUTHORIZATION MODE
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("Transaction Mode:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Use \"Authorize (DEFERRED)\" to reserve the funds without actually capturing them at the time of purchase. You can capture authorized transactions through the order admin interface later. Use \"Authorize & Capture (PAYMENT)\" to capture funds immediately at the time of purchase.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblTransactionType = new RadioButtonList();
            rblTransactionType.ID = "Config_UseAuthCapture";
            rblTransactionType.Items.Add(new ListItem("Authorize / DEFERRED (recommended)", "false"));
            rblTransactionType.Items.Add(new ListItem("Authorize & Capture / PAYMENT", "true"));
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">You can configure to run in simulated mode, test mode or live mode. Use simulated or test mode during testing and development.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblGatewayMode = new RadioButtonList();
            rblGatewayMode.ID = "Config_GatewayMode";
            rblGatewayMode.Items.Add(new ListItem("Live Mode", ((int)GatewayModeOption.LiveMode).ToString()));
            rblGatewayMode.Items.Add(new ListItem("Test Mode", ((int)GatewayModeOption.TestMode).ToString()));
            rblGatewayMode.Items.Add(new ListItem("Simulated Mode", ((int)GatewayModeOption.SimulatedMode).ToString()));
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, the communication between AbleCommerce and SagePay is recorded in the store \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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

        #region Do Action methods

        public override Transaction DoAuthorize(AuthorizeTransactionRequest transactionRequest)
        {
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            string uniqueTransId = MakeUniqueTransId(transactionRequest.Payment, service_register);
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Authorize(transactionRequest, uniqueTransId, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("Vendor={0}", this.VendorName);
                string debugCredentials = "Vendor=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest, service_register);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Authorize(transactionRequest, uniqueTransId, response);
        }

        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            //VALIDATE CAPTURE AMOUNT MEETS SAGEPAY RESTRICTIONS
            if (this.GatewayMode == GatewayModeOption.SimulatedMode)
            {
                if (captureRequest.Amount != captureRequest.AuthorizeTransaction.Amount)
                {
                    throw new PaymentGatewayProviderException("When using simulator mode, the capture amount must match the authorization amount of " + captureRequest.AuthorizeTransaction.Amount.ToString("lc") + ".");
                }
            }
            else
            {
                if (captureRequest.Amount > captureRequest.AuthorizeTransaction.Amount)
                {
                    throw new PaymentGatewayProviderException("The capture amount cannot exceed the authorization amount of " + captureRequest.AuthorizeTransaction.Amount.ToString("lc") + ".");
                }
            }
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Capture(captureRequest);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("Vendor={0}", this.VendorName);
                string debugCredentials = "Vendor=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest, service_release);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Capture(captureRequest, response);
        }

        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Void(voidRequest);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("Vendor={0}", this.VendorName);
                string debugCredentials = "Vendor=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest, service_abort);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Void(voidRequest.Payment, response, voidRequest);
        }

        public override Transaction DoRefund(RefundTransactionRequest refundRequest)
        {
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            string uniqueTransId = MakeUniqueTransId(refundRequest.Payment, service_refund);
            //BUILD THE REQUEST
            string gatewayRequest = BuildGatewayRequest_Refund(refundRequest, uniqueTransId, sensitiveData);
            //RECORD REQUEST
            if (this.UseDebugMode)
            {
                //ALWAYS MASK THE CREDENTIALS
                string credentials = String.Format("Vendor={0}", this.VendorName);
                string debugCredentials = "Vendor=xxxxxxxx";
                sensitiveData[credentials] = debugCredentials;
                this.RecordCommunication(this.Name, CommunicationDirection.Send, gatewayRequest, sensitiveData);
            }
            //SEND REQUEST
            string response = this.SendRequestToGateway(gatewayRequest, service_refund);
            //RECORD RESPONSE
            if (this.UseDebugMode) this.RecordCommunication(this.Name, CommunicationDirection.Receive, response, null);
            //PROCESS RESPONSE AND RETURN RESULT
            return this.ProcessGatewayResponse_Refund(refundRequest, refundRequest.Payment, uniqueTransId, response);
        }

        #endregion

        #region Process Response methods

        private Transaction ProcessGatewayResponse_Authorize(AuthorizeTransactionRequest request, string uniqueTransId, string authorizeResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = this.UseAuthCapture ? TransactionType.AuthorizeCapture : request.TransactionType;
            transaction.Amount = request.Amount;
            transaction.RemoteIP = request.RemoteIP;
            //PARSE THE RESPONSE FROM SAGEPAY            
            Dictionary<string, string> responseData = ParseResponse(authorizeResponse);
            transaction.ResponseCode = TryGetValue(responseData, "Status"); ;
            transaction.ResponseMessage = TryGetValue(responseData, "StatusDetail");
            transaction.ProviderTransactionId = TryGetValue(responseData, "VPSTxId");
            transaction.AuthorizationCode = TryGetValue(responseData, "TxAuthNo");
            Dictionary<string, string> additionalData = transaction.AdditionalDataDictionary;
            if (additionalData == null) additionalData = new Dictionary<string, string>();
            additionalData["UniqueTransactionId"] = uniqueTransId;
            string securityKey = TryGetValue(responseData, "SecurityKey");
            if (!string.IsNullOrEmpty(securityKey)) additionalData["SecurityKey"] = securityKey;
            transaction.UpdateAdditionalData(additionalData);
            transaction.AVSResultCode = GetAvsCode(TryGetValue(responseData, "AddressResult"), TryGetValue(responseData, "PostCodeResult"));
            transaction.CVVResultCode = GetCvvCode(TryGetValue(responseData, "CV2Result"));
            if (transaction.ResponseCode == "OK")
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                if (transaction.ResponseCode == "3DAUTH")
                    transaction.ResponseMessage = "3D-Secure is not supported by this integration of SagePay VSP Direct";
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Capture(CaptureTransactionRequest request, string captureResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Capture;
            transaction.Amount = request.Amount;
            transaction.RemoteIP = request.RemoteIP;
            //PARSE THE RESPONSE FROM SAGEPAY            
            Dictionary<string, string> responseData = ParseResponse(captureResponse);
            transaction.ResponseCode = TryGetValue(responseData, "Status"); ;
            transaction.ResponseMessage = TryGetValue(responseData, "StatusDetail");
            if (transaction.ResponseCode == "OK")
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = request.AuthorizeTransaction.ProviderTransactionId;
                transaction.AuthorizationCode = request.AuthorizeTransaction.AuthorizationCode;
                transaction.AdditionalData = request.AuthorizeTransaction.AdditionalData;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Void(Payment payment, string voidResponse, VoidTransactionRequest request)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            transaction.Amount = request.Amount;
            transaction.RemoteIP = request.RemoteIP;
            //PARSE THE RESPONSE FROM SAGEPAY            
            Dictionary<string, string> responseData = ParseResponse(voidResponse);
            transaction.ResponseCode = TryGetValue(responseData, "Status"); ;
            transaction.ResponseMessage = TryGetValue(responseData, "StatusDetail");
            if (transaction.ResponseCode == "OK")
            {
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = request.AuthorizeTransaction.ProviderTransactionId;
                transaction.AuthorizationCode = request.AuthorizeTransaction.AuthorizationCode;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
            }
            return transaction;
        }

        private Transaction ProcessGatewayResponse_Refund(RefundTransactionRequest request, Payment payment, string uniqueTransId, string refundResponse)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Refund;
            transaction.Amount = request.Amount;
            transaction.RemoteIP = request.RemoteIP;
            //PARSE THE RESPONSE FROM SAGEPAY            
            Dictionary<string, string> responseData = ParseResponse(refundResponse);
            transaction.ResponseCode = TryGetValue(responseData, "Status"); ;
            transaction.ResponseMessage = TryGetValue(responseData, "StatusDetail");
            transaction.ProviderTransactionId = TryGetValue(responseData, "VPSTxId");
            transaction.AuthorizationCode = TryGetValue(responseData, "TxAuthNo");
            Dictionary<string, string> additionalData = transaction.AdditionalDataDictionary;
            if (additionalData == null) additionalData = new Dictionary<string, string>();
            additionalData["UniqueTransactionId"] = uniqueTransId;
            transaction.UpdateAdditionalData(additionalData);
            if (transaction.ResponseCode == "OK")
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

        #region Build request methods

        private string BuildGatewayRequestPart_VendorAccountInformation()
        {
            return String.Format("VPSProtocol={0}&Vendor={1}&AccountType={2}", VPSVersion, VendorName, AccountType.ToString());
        }

        private string BuildGatewayRequestPart_CustomerDetails(Order order, AccountDataDictionary accountData)
        {
            StringBuilder customerDetails = new StringBuilder();
            //ADD CUSTOMER INFORMATION
            if (accountData.ContainsKey("AccountName") && !string.IsNullOrEmpty(accountData["AccountName"]))
            {
                customerDetails.Append("&CardHolder=" + HttpUtility.UrlEncode(accountData["AccountName"]));
            }
            else
            {
                customerDetails.Append("&CardHolder=" + HttpUtility.UrlEncode(order.BillToFirstName + " " + order.BillToLastName));
            }
            customerDetails.Append("&BillingSurname=" + HttpUtility.UrlEncode(order.BillToLastName));
            customerDetails.Append("&BillingFirstnames=" + HttpUtility.UrlEncode(order.BillToFirstName));
            customerDetails.Append("&BillingAddress1=" + HttpUtility.UrlEncode(order.BillToAddress1));
            if (!string.IsNullOrEmpty(order.BillToAddress2))
                customerDetails.Append("&BillingAddress2=" + HttpUtility.UrlEncode(order.BillToAddress2));
            customerDetails.Append("&BillingCity=" + HttpUtility.UrlEncode(order.BillToCity));
            customerDetails.Append("&BillingPostCode=" + HttpUtility.UrlEncode(order.BillToPostalCode));
            customerDetails.Append("&BillingCountry=" + HttpUtility.UrlEncode(order.BillToCountryCode));
            if (order.BillToCountryCode == "US")
                customerDetails.Append("&BillingState=" + HttpUtility.UrlEncode(order.BillToProvince));
            if (!string.IsNullOrEmpty(order.BillToPhone))
                customerDetails.Append("&BillingPhone=" + HttpUtility.UrlEncode(order.BillToPhone));
            customerDetails.Append("&CustomerEMail=" + HttpUtility.UrlEncode(order.BillToEmail));
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                string remoteIp = context.Request.ServerVariables["REMOTE_HOST"];
                customerDetails.Append("&ClientIPAddress=" + AlwaysConvert.ToIPV4(remoteIp));
            }

            return customerDetails.ToString();
        }

        private string BuildGatewayRequestPart_CustomerShippingAddress(OrderShipment shipment)
        {
            StringBuilder shippingAddress = new StringBuilder();
            shippingAddress.Append("&DeliverySurname=" + HttpUtility.UrlEncode(shipment.ShipToLastName));
            shippingAddress.Append("&DeliveryFirstnames=" + HttpUtility.UrlEncode(shipment.ShipToFirstName));
            shippingAddress.Append("&DeliveryAddress1=" + HttpUtility.UrlEncode(shipment.ShipToAddress1));
            if (!string.IsNullOrEmpty(shipment.ShipToAddress2))
                shippingAddress.Append("&DeliveryAddress2=" + HttpUtility.UrlEncode(shipment.ShipToAddress2));
            shippingAddress.Append("&DeliveryCity=" + HttpUtility.UrlEncode(shipment.ShipToCity));
            shippingAddress.Append("&DeliveryPostCode=" + HttpUtility.UrlEncode(shipment.ShipToPostalCode));
            shippingAddress.Append("&DeliveryCountry=" + HttpUtility.UrlEncode(shipment.ShipToCountryCode));
            if (shipment.ShipToCountryCode == "US")
                shippingAddress.Append("&DeliveryState=" + HttpUtility.UrlEncode(shipment.ShipToProvince));
            if (!string.IsNullOrEmpty(shipment.ShipToPhone ))
                shippingAddress.Append("&DeliveryPhone=" + HttpUtility.UrlEncode(shipment.ShipToPhone));
            return shippingAddress.ToString();
        }

        private string BuildGatewayRequestPart_AuthorizeTransactionData(AuthorizeTransactionRequest authorizeRequest, AccountDataDictionary accountData, string uniqueTransId, Dictionary<string, string> sensitiveData)
        {
            StringBuilder transactionData = new StringBuilder();
            bool capture = (this.UseAuthCapture || authorizeRequest.Capture);
            transactionData.Append(capture ? "&TxType=PAYMENT" : "&TxType=DEFERRED");
            //APPEND AMOUNT
            transactionData.Append(String.Format("&Amount={0:F2}", authorizeRequest.Amount));
            transactionData.Append("&Currency=" + GetCurrencyCode(authorizeRequest.CurrencyCode, authorizeRequest.Payment.CurrencyCode));
            transactionData.Append("&VendorTxCode=" + uniqueTransId);
            transactionData.Append("&Description=" + HttpUtility.UrlEncode(GetDescription(authorizeRequest.Payment, service_register)));

            //DETERMINE METHOD AND TYPE
            Payment payment = authorizeRequest.Payment;
            PaymentMethod method = payment.PaymentMethod;
            PaymentInstrument instrument = method.PaymentInstrument;
            switch (instrument)
            {
                case PaymentInstrument.AmericanExpress:
                    transactionData.Append("&CardType=AMEX"); break;
                case PaymentInstrument.MasterCard:
                    transactionData.Append("&CardType=MC"); break;
                case PaymentInstrument.Visa:
                    transactionData.Append("&CardType=VISA"); break;
                case PaymentInstrument.DinersClub:
                    transactionData.Append("&CardType=DC"); break;
                case PaymentInstrument.JCB:
                    transactionData.Append("&CardType=JCB"); break;
                case PaymentInstrument.Maestro:
                    transactionData.Append("&CardType=MAESTRO"); break;
                case PaymentInstrument.SwitchSolo:
                    transactionData.Append("&CardType=SOLO"); break;
                case PaymentInstrument.VisaDebit:
                    string methodName = method.Name.ToUpperInvariant();
                    string cardType = methodName.Contains("ELECTRON") ? "UKE" : "DELTA";
                    transactionData.Append("&CardType=" + cardType); break;
                default:
                    throw new ArgumentException("This gateway does not support the requested payment instrument: " + instrument.ToString());
            }
            //APPEND PAYMENT INSTRUMENT DETAILS
            string accountNumber = accountData.GetValue("AccountNumber");
            transactionData.Append("&CardNumber=" + accountNumber);
            if (this.UseDebugMode) sensitiveData[accountNumber] = MakeReferenceNumber(accountNumber);
            string expirationMonth = accountData.GetValue("ExpirationMonth");
            if (expirationMonth.Length == 1) expirationMonth = expirationMonth.Insert(0, "0");
            string expirationYear = accountData.GetValue("ExpirationYear");
            if (expirationYear.Length == 1) expirationYear = expirationYear.Insert(0, "0");
            if (expirationYear.Length > 2) expirationYear = StringHelper.Right(expirationYear, 2);
            transactionData.Append("&ExpiryDate=" + HttpUtility.UrlEncode(expirationMonth + expirationYear));
            //PROCESS CREDIT CARD ACCOUNT DATA
            string securityCode = accountData.GetValue("SecurityCode");
            if (!string.IsNullOrEmpty(securityCode))
            {
                transactionData.Append("&CV2=" + securityCode);
                if (this.UseDebugMode) sensitiveData["CV2=" + securityCode] = "CV2=" + (new string('x', securityCode.Length));
            }
            //ADD ISSUENUMBER AND STARTDATE (SOME MAESTRO / SOLO ONLY)
            string issueNumber = accountData.GetValue("IssueNumber");
            if (!string.IsNullOrEmpty(issueNumber))
            {
                transactionData.Append("&IssueNumber=" + issueNumber);
                if (this.UseDebugMode) sensitiveData["IssueNumber=" + issueNumber] = "IssueNumber=" + (new string('x', issueNumber.Length));
            }
            string startDateMonth = accountData.GetValue("StartDateMonth");
            string startDateYear = accountData.GetValue("StartDateYear");
            if (!string.IsNullOrEmpty(startDateMonth) && !string.IsNullOrEmpty(startDateYear))
            {
                if (startDateMonth.Length == 1) startDateMonth.Insert(0, "0");
                if (startDateYear.Length > 2) startDateYear = StringHelper.Right(startDateYear, 2);
                transactionData.Append("&StartDate=" + startDateMonth + startDateYear);
                if (this.UseDebugMode) sensitiveData["StartDate=" + startDateMonth + startDateYear] = "StartDate=xxxx";
            }
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_CaptureTransactionData(CaptureTransactionRequest captureRequest)
        {
            Dictionary<string, string> additionalData = captureRequest.AuthorizeTransaction.AdditionalDataDictionary;
            StringBuilder transactionData = new StringBuilder();
            transactionData.Append("&TxType=RELEASE");
            transactionData.Append("&VendorTxCode=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "UniqueTransactionId")));
            transactionData.Append("&VPSTxId=" + HttpUtility.UrlEncode(captureRequest.AuthorizeTransaction.ProviderTransactionId));
            transactionData.Append("&SecurityKey=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "SecurityKey")));
            transactionData.Append("&TxAuthNo=" + HttpUtility.UrlEncode(captureRequest.AuthorizeTransaction.AuthorizationCode));
            transactionData.Append("&ReleaseAmount=" + String.Format("{0:F2}", captureRequest.Amount));
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_VoidTransactionData(VoidTransactionRequest voidRequest)
        {
            Dictionary<string, string> additionalData = voidRequest.AuthorizeTransaction.AdditionalDataDictionary;
            StringBuilder transactionData = new StringBuilder();
            transactionData.Append("&TxType=ABORT");
            transactionData.Append("&VendorTxCode=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "UniqueTransactionId")));
            transactionData.Append("&VPSTxId=" + HttpUtility.UrlEncode(voidRequest.AuthorizeTransaction.ProviderTransactionId));
            transactionData.Append("&SecurityKey=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "SecurityKey")));
            transactionData.Append("&TxAuthNo=" + HttpUtility.UrlEncode(voidRequest.AuthorizeTransaction.AuthorizationCode));
            return transactionData.ToString();
        }

        private string BuildGatewayRequestPart_RefundTransactionData(RefundTransactionRequest refundRequest, string uniqueTransId, Dictionary<string, string> sensitiveData)
        {
            Dictionary<string, string> additionalData = refundRequest.CaptureTransaction.AdditionalDataDictionary;
            StringBuilder transactionData = new StringBuilder();
            transactionData.Append("&TxType=REFUND");
            transactionData.Append("&VendorTxCode=" + uniqueTransId);
            transactionData.Append(String.Format("&Amount={0:F2}", refundRequest.Amount));
            transactionData.Append("&Currency=" + GetCurrencyCode(refundRequest.CurrencyCode, refundRequest.Payment.CurrencyCode));
            transactionData.Append("&Description=" + HttpUtility.UrlEncode(GetDescription(refundRequest.Payment, service_refund)));
            transactionData.Append("&RelatedVPSTxId=" + HttpUtility.UrlEncode(refundRequest.CaptureTransaction.ProviderTransactionId));
            transactionData.Append("&RelatedVendorTxCode=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "UniqueTransactionId")));
            transactionData.Append("&RelatedSecurityKey=" + HttpUtility.UrlEncode(TryGetValue(additionalData, "SecurityKey")));
            transactionData.Append("&RelatedTxAuthNo=" + HttpUtility.UrlEncode(refundRequest.CaptureTransaction.AuthorizationCode));
            return transactionData.ToString();
        }

        private string BuildGatewayRequest_Authorize(AuthorizeTransactionRequest request, string uniqueTransId, Dictionary<string, string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = request.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Order order = payment.Order;
            if (order == null) throw new ArgumentNullException("request.Payment.Order");

            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);

            //GENERATE REQUEST
            StringBuilder sb = new StringBuilder();
            sb.Append(BuildGatewayRequestPart_VendorAccountInformation());
            sb.Append(BuildGatewayRequestPart_CustomerDetails(order, accountData));
            //IF ONLY ONE SHIPMENT IN ORDER, APPEND CUSTOMER SHIPPING ADDRESS
            if (order.Shipments.Count == 1)
            {
                sb.Append(BuildGatewayRequestPart_CustomerShippingAddress(order.Shipments[0]));
            }
            sb.Append(BuildGatewayRequestPart_AuthorizeTransactionData(request, accountData, uniqueTransId, sensitiveData));

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
            sb.Append(BuildGatewayRequestPart_VendorAccountInformation());
            sb.Append(BuildGatewayRequestPart_CaptureTransactionData(request));

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
            request.Append(BuildGatewayRequestPart_VendorAccountInformation());
            request.Append(BuildGatewayRequestPart_VoidTransactionData(voidRequest));
            return request.ToString();
        }

        private string BuildGatewayRequest_Refund(RefundTransactionRequest refundRequest, string uniqueTransId, Dictionary<string, string> sensitiveData)
        {
            //ACCESS REQUIRED DATA FOR BUILDING REQUEST
            Payment payment = refundRequest.Payment;
            if (payment == null) throw new ArgumentNullException("refundRequest.Payment");
            Transaction captureTransaction = refundRequest.CaptureTransaction;
            if (captureTransaction == null) throw new ArgumentNullException("refundRequest.AuthorizeTransaction");
            AccountDataDictionary accountData = new AccountDataDictionary(payment.AccountData);
            //GENERATE REQUEST
            StringBuilder request = new StringBuilder();
            request.Append(BuildGatewayRequestPart_VendorAccountInformation());
            Order order = payment.Order;
            request.Append(BuildGatewayRequestPart_CustomerDetails(order, accountData));
            request.Append(BuildGatewayRequestPart_RefundTransactionData(refundRequest, uniqueTransId, sensitiveData));
            return request.ToString();
        }

        private string GetCurrencyCode(string txCode, string paymentCode)
        {
            if (!string.IsNullOrEmpty(txCode) && txCode.Length == 3) return txCode;
            if (!string.IsNullOrEmpty(paymentCode) && paymentCode.Length == 3) return paymentCode;
            return Token.Instance.Store.BaseCurrency.ISOCode;
        }

        private string GetDescription(Payment payment, string service)
        {
            string serviceSuffix = string.Empty;
            switch (service)
            {
                case service_abort:
                    serviceSuffix = " void";
                    break;
                case service_refund:
                    serviceSuffix = " refund";
                    break;
                case service_register:
                    break;
                case service_release:
                    serviceSuffix = " capture";
                    break;
            }
            string orderNumber = string.Empty;
            if (payment != null)
            {
                Order o = payment.Order;
                if (o != null)
                {
                    orderNumber = " Order #" + o.OrderNumber;
                }
            }
            return StringHelper.RemoveSpecialChars(Token.Instance.Store.Name) + orderNumber + serviceSuffix;
        }

        private string MakeUniqueTransId(Payment payment, string service)
        {
            string serviceSuffix = string.Empty;
            if (service == service_refund) serviceSuffix = "-R";
            string orderNumber = "0";
            if (payment != null)
            {
                Order o = payment.Order;
                if (o != null)
                {
                    orderNumber = o.OrderNumber.ToString();
                }
            }
            return string.Format("{0}-{1}", orderNumber, LocaleHelper.LocalNow.ToString("HHmmssfff")) + serviceSuffix;
        }

        #endregion

        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            throw new NotImplementedException("Recurring transactions are not supported.");
        }

        private string SendRequestToGateway(string requestData, string service)
        {
            //EXECUTE WEB REQUEST, SET RESPONSE
            string requestUrl = this.GetActiveUrl(service);
            string response;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = false;
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

        private Dictionary<string, string> ParseResponse(string responseText)
        {
            Dictionary<string, string> responseData = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(responseText)) return responseData;
            string[] respSplit = responseText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (respSplit == null || respSplit.Length == 0) return responseData;
            string[] namevalue;
            string name, value;
            foreach (string respItem in respSplit)
            {
                namevalue = respItem.Split("=".ToCharArray(), StringSplitOptions.None);
                if (namevalue == null || namevalue.Length == 0) continue;
                name = namevalue[0].Trim();
                if (namevalue.Length > 1) value = namevalue[1];
                else value = string.Empty;
                responseData.Add(name, value);
            }
            return responseData;
        }

        private string TryGetValue(Dictionary<string, string> dictionary, string key)
        {
            string retVal = string.Empty;
            if (dictionary.ContainsKey(key))
            {
                retVal = dictionary[key];
                if (retVal == null) retVal = string.Empty;
            }
            return retVal;
        }

        private string GetAvsCode(string avsResult, string postalResult)
        {
            if (avsResult == null) avsResult = string.Empty;
            if (postalResult == null) postalResult = string.Empty;

            bool addressMatch = false;
            bool postalMatch = false;
            bool addrNotAvailable = false;
            bool postalNotAvailable = false;

            switch (avsResult)
            {
                case "MATCHED":
                    addressMatch = true; break;
                case "NOTMATCHED":
                    addressMatch = false; break;
                default:
                    addrNotAvailable = true; break;
            }

            switch (postalResult)
            {
                case "MATCHED":
                    postalMatch = true; break;
                case "NOTMATCHED":
                    postalMatch = false; break;
                default:
                    postalNotAvailable = true; break;
            }

            if (postalNotAvailable && addrNotAvailable) return "U";
            if (postalNotAvailable && !addrNotAvailable)
            {
                //addr available, postal not
                if (addressMatch) return "A";
                return "N";
            }
            else if (addrNotAvailable && !postalNotAvailable)
            {
                //postal available, addr not
                if (postalMatch) return "Z";
                return "N";
            }
            else
            {
                //both available
                if (addressMatch && postalMatch) return "Y";
                if (!addressMatch && postalMatch) return "Z";
                if (addressMatch && !postalMatch) return "A";
                return "N";
            }
        }

        private string GetCvvCode(string cvvResult)
        {
            switch (cvvResult)
            {
                case "NOTPROVIDED":
                    return "P";
                case "NOTCHECKED":
                    return "P";
                case "MATCHED":
                    return "M";
                case "NOTMATCHED":
                    return "N";
            }
            return "X";
        }

        public enum GatewayModeOption : int
        {
            LiveMode = 0, TestMode, SimulatedMode
        }

        public enum AccountTypeOption : int
        {
            E = 0, C, M
        }
    }
}