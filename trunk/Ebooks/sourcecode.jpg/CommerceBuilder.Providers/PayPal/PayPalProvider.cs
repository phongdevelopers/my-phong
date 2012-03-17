//IMPORT SYSTEM NAMESPACES
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

//IMPORT COMMERCEBUILDER NAMESPACES
using CommerceBuilder.Common;
using CommerceBuilder.Marketing;
using CommerceBuilder.Payments;
using CommerceBuilder.Orders;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

//IMPORT PAYPAL NAMESPACES
using com.paypal.soap.api;
using com.paypal.sdk.profiles;
using com.paypal.sdk.services;
using com.paypal.sdk.util;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class PayPalProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        string _PayPalAccount;
        string _ApiUsername;
        string _ApiPassword;
        string _ApiSignature;
        bool _UseAuthCapture;
        bool _UseSandbox;

        public string PayPalAccount
        {
            get { return _PayPalAccount; }
            set { _PayPalAccount = value; }
        }

        public string ApiUsername
        {
            get { return _ApiUsername; }
            set { _ApiUsername = value; }
        }

        public string ApiPassword
        {
            get { return _ApiPassword; }
            set { _ApiPassword = value; }
        }

        public string ApiSignature
        {
            get { return _ApiSignature; }
            set { _ApiSignature = value; }
        }

        public bool UseAuthCapture
        {
            get { return _UseAuthCapture; }
            set { _UseAuthCapture = value; }
        }

        public bool UseSandbox
        {
            get { return _UseSandbox; }
            set { _UseSandbox = value; }
        }

        public bool ApiEnabled
        {
            get { return (!(string.IsNullOrEmpty(ApiUsername) || string.IsNullOrEmpty(ApiPassword) || string.IsNullOrEmpty(ApiSignature))); }
        }

        public static string SignupUrl
        {
            get { return "https://www.paypal.com/us/mrb/pal=393Q5LFLKJUCU&mrb=R-4SH04028LF295822X"; }
        }

        public override void Initialize(int PaymentGatewayId, Dictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("PayPalAccount")) PayPalAccount = ConfigurationData["PayPalAccount"];
            if (ConfigurationData.ContainsKey("ApiUsername")) ApiUsername = ConfigurationData["ApiUsername"];
            if (ConfigurationData.ContainsKey("ApiPassword")) ApiPassword = ConfigurationData["ApiPassword"];
            if (ConfigurationData.ContainsKey("ApiSignature")) ApiSignature = ConfigurationData["ApiSignature"];
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
            if (ConfigurationData.ContainsKey("UseSandbox")) UseSandbox = ConfigurationData["UseSandbox"].ToLowerInvariant().Equals("on");
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
            currentCell.Controls.Add(new LiteralControl("PayPal&reg;"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD INSTRUCTION TEXT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">With more than 56 million accounts in 45 countries, PayPal offers a fast, affordable and convenient online payment service for businesses of all sizes.  Don't have an account?  Click the signup link below.</p>"));
            currentCell.Controls.Add(new LiteralControl("<p style=\"text-align:center;margin-bottom:8px;\">"));
            HyperLink gatewayLink = new HyperLink();
            gatewayLink.Text = "Get A Paypal Merchant Account Here!";
            gatewayLink.NavigateUrl = PayPalProvider.SignupUrl;
            gatewayLink.Target = "_blank";
            currentCell.Controls.Add(gatewayLink);
            currentCell.Controls.Add(new LiteralControl("</p>"));
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

            //GET THE PAYPAL ACCOUNT
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("PayPal Account:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Enter your PayPal email address used by your customers to make payments.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtPayPalAccount = new TextBox();
            txtPayPalAccount.ID = "Config_PayPalAccount";
            txtPayPalAccount.Width = new Unit(200D, UnitType.Pixel);
            txtPayPalAccount.MaxLength = 100;
            txtPayPalAccount.Text = this.PayPalAccount;
            currentCell.Controls.Add(txtPayPalAccount);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD IPN SECTION HEADER
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "subSectionHeader");
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("Instant Payment Notification (IPN)"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD IPN INSTRUCTION
            string baseUrl = Token.Instance.Store.StoreUrl;
            if (Store.GetCachedSettings().SSLEnabled) baseUrl = baseUrl.Replace("http:", "https:");
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            string processUrl = baseUrl + "ProcessPayPal.ashx";
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">Be sure to configure your PayPal account to support IPN.  Find this under Profile -> Selling Preferences -> Instant Payment Notification Preferences.  Turn this feature on and provide the URL printed below.  You must have SSL enabled for IPN to work with a LIVE PayPal account:</p>"));
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\" style=\"text-align:center;margin-bottom:8px;\">" + processUrl + "</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD API SECTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "subSectionHeader");
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("Express Checkout and Direct Payment"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD API INSTRUCTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">For PayPal Express Checkout and Direct Payment, you must have an API username, password, and signature issued by PayPal.</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE API Username
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("API Username:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Enter your PayPal API Username.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtApiUsername = new TextBox();
            txtApiUsername.ID = "Config_ApiUsername";
            txtApiUsername.Columns = 40;
            txtApiUsername.Text = this.ApiUsername;
            currentCell.Controls.Add(txtApiUsername);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE API Password
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("API Password:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Enter your PayPal API Password.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtApiPassword = new TextBox();
            txtApiPassword.ID = "Config_ApiPassword";
            txtApiPassword.Columns = 40;
            txtApiPassword.Text = this.ApiPassword;
            currentCell.Controls.Add(txtApiPassword);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE API Signature
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "rowHeader");
            currentCell.Attributes.Add("style", "white-space:normal;");
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            currentCell.Controls.Add(new LiteralControl("API Signature:"));
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Enter the PayPal generated API Signature.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            TextBox txtApiSignature = new TextBox();
            txtApiSignature.ID = "Config_ApiSignature";
            txtApiSignature.Columns = 40;
            txtApiSignature.Text = this.ApiSignature;
            currentCell.Controls.Add(txtApiSignature);
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">Use \"Authorize\" to request authorization without capturing funds at the time of purchase. You can capture authorized transactions through the order admin interface. Use \"Sale\" to capture funds immediately at the time of purchase.</span>"));
            currentRow.Cells.Add(currentCell);
            currentCell = new HtmlTableCell();
            currentCell.VAlign = "Top";
            currentCell.Width = "50%";
            RadioButtonList rblTransactionType = new RadioButtonList();
            rblTransactionType.ID = "Config_UseAuthCapture";
            rblTransactionType.Items.Add(new ListItem("Authorize (recommended)", "false"));
            rblTransactionType.Items.Add(new ListItem("Sale", "true"));
            rblTransactionType.Items[UseAuthCapture ? 1 : 0].Selected = true;
            currentCell.Controls.Add(rblTransactionType);
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
            currentCell.Controls.Add(new LiteralControl("<br /><span class=\"helpText\">When debug mode is enabled, communication between the store and gateway is recorded in the \"logs\" folder. Sensitive information is stripped from the log entries.</span>"));
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

            //ADD SANDBOX SECTION HEADER
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell("th");
            currentCell.Attributes.Add("class", "subSectionHeader");
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("Testing PayPal"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //ADD SANDBOX INSTRUCTION
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Controls.Add(new LiteralControl("<p class=\"InstructionText\">Paypal provides a developer \"sandbox\" where you can test your site with PayPal.  To use the sandbox you must have created a developer account and used that account in the configuration sections above.</p>"));
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //GET THE SANDBOX SETTING
            currentRow = new HtmlTableRow();
            currentCell = new HtmlTableCell();
            currentCell.ColSpan = 2;
            currentCell.Align = "center";
            CheckBox chkUseSandbox = new CheckBox();
            chkUseSandbox.ID = "Config_UseSandbox";
            chkUseSandbox.Checked = this.UseSandbox;
            chkUseSandbox.Text = "<b>Use Sandbox</b>";
            currentCell.Controls.Add(chkUseSandbox);
            currentRow.Cells.Add(currentCell);
            configTable.Rows.Add(currentRow);

            //CREATE LITERAL CONTROL WITH HTML CONTENT
            parentControl.Controls.Add(configTable);
        }

        public override string ConfigReference
        {
            get { return "PayPal Account: " + this.PayPalAccount; }
        }

        public CreditCardTypeType GetCardType(string accountNumber)
        {
            //*CARD TYPES            *PREFIX           *WIDTH
            //American Express       34, 37            15
            //Discover               6011              16
            //Master Card            51 to 55          16
            //Visa                   4                 13, 16

            //Remove all spaces and dashes from the passed string
            accountNumber = accountNumber.Replace(" ", string.Empty).Replace("=", string.Empty);
            //Check the first two digits first
            int firstTwo = AlwaysConvert.ToInt(accountNumber.Substring(0, 2));
            switch (firstTwo)
            {
                case 34:
                case 37:
                    return CreditCardTypeType.Amex;
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                    return CreditCardTypeType.MasterCard;
                case 60:
                    return CreditCardTypeType.Discover;
            }
            //RETURN VISA IF NO OTHER MATCH
            return CreditCardTypeType.Visa;
        }

        /// <summary>
        /// Gets a shipment that contains the destination (ship to) for an order
        /// </summary>
        /// <param name="order">The order to get destination for</param>
        /// <returns>Teh shipment that contains the destination, or null if not available</returns>
        /// <remarks>If there is only one shipment, or all shipments have the same 
        /// destination, the return value is the first shipment.  If there is no shipment 
        /// or multiple destinations, null is returned.</remarks>
        private OrderShipment GetDestination(Order order)
        {
            //UNSHIPPABLE ORDER?
            if (order.Shipments.Count == 0) return null;
            //IF ONLY ONE SHIPMENT NO CHECK REQUIRED
            if (order.Shipments.Count == 1) return order.Shipments[0];
            //SEE IF ANY SHIPMENTS HAVE A DIFFERENT ADDRESS
            OrderShipment shipment = order.Shipments[0];
            string firstAddress = GetShipmentAddressString(shipment);
            for (int i = 1; i < order.Shipments.Count - 1; i++)
            {
                //IF THE ADDRESS CONTENT DOES NOT MATCH, RETURN NULL (MULTIPLE DESTINATIONS)
                if (firstAddress != GetShipmentAddressString(order.Shipments[i])) return null;
            }
            return shipment;
        }

        /// <summary>
        /// Gets a string concatenation of all ship to values
        /// </summary>
        /// <param name="shipment">Shipment to compile shipto address for</param>
        /// <returns>A string of all ship to address information</returns>
        /// <remarks>Used to determine if two order shipments contain equal ship to addresses</remarks>
        private string GetShipmentAddressString(OrderShipment shipment)
        {
            StringBuilder address = new StringBuilder();
            address.Append(shipment.ShipToFullName);
            address.Append(shipment.ShipToAddress1);
            address.Append(shipment.ShipToAddress2);
            address.Append(shipment.ShipToCity);
            address.Append(shipment.ShipToProvince);
            address.Append(shipment.ShipToPostalCode);
            address.Append(shipment.ShipToCountryCode);
            address.Append(shipment.ShipToPhone);
            return address.ToString();
        }

        public override Transaction DoAuthorize(CommerceBuilder.Payments.AuthorizeTransactionRequest authorizeRequest)
        {
            //IF THE PAYMENT INSTRUMENT IS PAYPAL, AUTHORIZATION TAKES PLACE VIA IPN
            if (authorizeRequest.Payment.PaymentMethod.PaymentInstrument == PaymentInstrument.PayPal)
            {
                //CREATE THE TRANSACTION RETURN OBJECT
                Transaction transaction = new Transaction();
                //SET VALUES COMMON TO ALL TRANSACTIONS
                transaction.PaymentGatewayId = this.PaymentGatewayId;
                transaction.TransactionType = TransactionType.Authorize;
                transaction.RemoteIP = authorizeRequest.RemoteIP;
                transaction.ResponseCode = "PENDING";
                transaction.ResponseMessage = "Waiting for IPN";
                transaction.TransactionStatus = TransactionStatus.Pending;
                transaction.TransactionDate = DateTime.UtcNow;
                return transaction;
            }
            //THIS IS REFACTORED TO A PRIVATE METHOD BECAUSE IT SEEMS TO RESOLVE
            //THE ISSUE OF ALLOWING PARTIALLY TRUSTED CALLERS TO ACCESS THIS METHOD
            //WHEN LEGACY PAYPAL PAY NOW BUTTONS ARE EMPLOYED (SEE BUG 6647)
            return DoAuthorize_Internal(authorizeRequest);
        }

        private Transaction DoAuthorize_Internal(CommerceBuilder.Payments.AuthorizeTransactionRequest authorizeRequest)
        {
            //BUILD REQUEST FOR PAYPAL
            Dictionary<string, string> sensitiveData = new Dictionary<string, string>();
            DoDirectPaymentRequestType oReq = new DoDirectPaymentRequestType();
            oReq.Version = "1.00";
            oReq.DoDirectPaymentRequestDetails = new DoDirectPaymentRequestDetailsType();
            bool useAuthCapture = (this.UseAuthCapture || authorizeRequest.Capture);
            oReq.DoDirectPaymentRequestDetails.PaymentAction = useAuthCapture ? PaymentActionCodeType.Sale : PaymentActionCodeType.Authorization;
            oReq.DoDirectPaymentRequestDetails.PaymentDetails = new PaymentDetailsType();
            PaymentDetailsType oPaymentDetails = oReq.DoDirectPaymentRequestDetails.PaymentDetails;
            oReq.DoDirectPaymentRequestDetails.CreditCard = new CreditCardDetailsType();
            CreditCardDetailsType oCreditCard = oReq.DoDirectPaymentRequestDetails.CreditCard;

            //SET THE CREDIT CARD INFO
            AccountDataDictionary paymentData = new AccountDataDictionary(authorizeRequest.Payment.AccountData);
            string creditCardNumber = paymentData.GetValue("AccountNumber");
            sensitiveData[creditCardNumber] = MakeReferenceNumber(creditCardNumber);
            int expMonth = AlwaysConvert.ToInt(paymentData.GetValue("ExpirationMonth"));
            int expYear = AlwaysConvert.ToInt(paymentData.GetValue("ExpirationYear"));
            string securityCode = paymentData.GetValue("SecurityCode");
            oCreditCard.CreditCardType = GetCardType(creditCardNumber);
            oCreditCard.CreditCardNumber = creditCardNumber;
            oCreditCard.ExpMonth = expMonth;
            oCreditCard.ExpYear = expYear;
            oCreditCard.CVV2 = securityCode;
            sensitiveData["CVV2>" + securityCode] = "CVV2>" + (new string('x', securityCode.Length));

            //SET THE CREDIT CARD OWNER INFO
            Payment payment = authorizeRequest.Payment;
            Order order = payment.Order;
            User user = order.User;            
            oCreditCard.CardOwner = new PayerInfoType();
            oCreditCard.CardOwner.PayerCountry = GetPayPalCountry(order.BillToCountryCode);
            oCreditCard.CardOwner.PayerName = new PersonNameType();

            if (paymentData.ContainsKey("AccountName") && !string.IsNullOrEmpty(paymentData["AccountName"]))
            {
                string[] names = paymentData["AccountName"].Split(" ".ToCharArray());
                oCreditCard.CardOwner.PayerName.FirstName = names[0];                
                if (names.Length > 1)
                    oCreditCard.CardOwner.PayerName.LastName = string.Join(" ", names, 1, names.Length - 1);                    
                else
                    //no last name. set to empty string? 
                    oCreditCard.CardOwner.PayerName.LastName = string.Empty;                
            }
            else
            {
                oCreditCard.CardOwner.PayerName.FirstName = order.BillToFirstName;
                oCreditCard.CardOwner.PayerName.LastName = order.BillToLastName;
            }
            
            oCreditCard.CardOwner.Address = new AddressType();
            oCreditCard.CardOwner.Address.Street1 = order.BillToAddress1;
            oCreditCard.CardOwner.Address.Street2 = order.BillToAddress2;
            oCreditCard.CardOwner.Address.CityName = order.BillToCity;
            oCreditCard.CardOwner.Address.StateOrProvince = order.BillToProvince;
            oCreditCard.CardOwner.Address.PostalCode = order.BillToPostalCode;
            oCreditCard.CardOwner.Address.Country = oCreditCard.CardOwner.PayerCountry;
            oCreditCard.CardOwner.Address.CountrySpecified = true;
            oCreditCard.CardOwner.Payer = order.BillToEmail;

            //GET THE CURRENCY FOR THE TRANSACTION
            string currencyCode = Token.Instance.Store.BaseCurrency.ISOCode;
            CurrencyCodeType baseCurrencyCode = GetPayPalCurrencyType(currencyCode);

            //SET THE ORDER TOTAL AMOUNTS
            LSDecimal curOrderTotal = order.Items.TotalPrice();
            if (curOrderTotal == authorizeRequest.Amount)
            {
                LSDecimal curShippingTotal = order.Items.TotalPrice(OrderItemType.Shipping) + GetShippingCouponTotal(order.Items);
                LSDecimal curHandlingTotal = order.Items.TotalPrice(OrderItemType.Handling);
                LSDecimal curTaxTotal = order.Items.TotalPrice(OrderItemType.Tax);
                LSDecimal curItemTotal = curOrderTotal - (curShippingTotal + curHandlingTotal + curTaxTotal);
                //MAKE SURE OUR BREAKDOWN IS VALID
                if ((curShippingTotal < 0) || (curHandlingTotal < 0) || (curTaxTotal < 0) || (curItemTotal < 0))
                {
                    //THE BREAKDOWN IS INVALID, DO NOT INCLUDE IT IN THE REQUEST
                    curShippingTotal = 0;
                    curHandlingTotal = 0;
                    curTaxTotal = 0;
                    curItemTotal = curOrderTotal;
                }

                //SET THE PAYMENT AMOUNT
                oPaymentDetails.OrderTotal = new BasicAmountType();
                oPaymentDetails.OrderTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.OrderTotal.Value = string.Format("{0:##,##0.00}", curOrderTotal);

                oPaymentDetails.ItemTotal = new BasicAmountType();
                oPaymentDetails.ItemTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.ItemTotal.Value = string.Format("{0:##,##0.00}", curItemTotal);

                oPaymentDetails.ShippingTotal = new BasicAmountType();
                oPaymentDetails.ShippingTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.ShippingTotal.Value = string.Format("{0:##,##0.00}", curShippingTotal);

                oPaymentDetails.HandlingTotal = new BasicAmountType();
                oPaymentDetails.HandlingTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.HandlingTotal.Value = string.Format("{0:##,##0.00}", curHandlingTotal);

                oPaymentDetails.TaxTotal = new BasicAmountType();
                oPaymentDetails.TaxTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.TaxTotal.Value = string.Format("{0:##,##0.00}", curTaxTotal);
            }
            else
            {
                oPaymentDetails.OrderTotal = new BasicAmountType();
                oPaymentDetails.OrderTotal.currencyID = baseCurrencyCode;
                oPaymentDetails.OrderTotal.Value = string.Format("{0:##,##0.00}", authorizeRequest.Amount);
            }

            //SET THE SHIPPING ADDRESS
            OrderShipment shipment = GetDestination(order);
            if (shipment != null)
            {
                oPaymentDetails.ShipToAddress = new AddressType();
                AddressType oShippingAddress = oPaymentDetails.ShipToAddress;
                oShippingAddress.Name = shipment.ShipToFullName;
                oShippingAddress.Street1 = shipment.ShipToAddress1;
                oShippingAddress.Street2 = shipment.ShipToAddress2;
                oShippingAddress.CityName = shipment.ShipToCity;
                oShippingAddress.StateOrProvince = shipment.ShipToProvince;
                oShippingAddress.PostalCode = shipment.ShipToPostalCode;
                oShippingAddress.Phone = shipment.ShipToPhone;
                oShippingAddress.Country = GetPayPalCountry(shipment.ShipToCountryCode);
                oShippingAddress.CountryName = shipment.ShipToCountry.Name;
                oShippingAddress.CountrySpecified = true;
            }

            //TODO: INCLUDE ORDER ITEMS IF SPECIFIED

            //SET THE ORDER DESCRIPTION
            oPaymentDetails.OrderDescription = "Order #: " + order.OrderNumber.ToString();

            //SET THE CUSTOM VALUE TO THE AC ORDER ID
            oPaymentDetails.Custom = order.OrderId.ToString();

            //SET THE IP ADDRESS
            oReq.DoDirectPaymentRequestDetails.IPAddress = authorizeRequest.RemoteIP;

            //SET THE BUTTON SOURCE
            oReq.DoDirectPaymentRequestDetails.PaymentDetails.ButtonSource = "ablecommerce-DP";

            //SET THE NOTIFY URL
            oReq.DoDirectPaymentRequestDetails.PaymentDetails.NotifyURL = GetStoreUrl() + "/ProcessPayPal.ashx";

            //RECORD SEND
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oReq), sensitiveData);
            }

            //EXECUTE REQUEST
            DoDirectPaymentResponseType oResp;
            oResp = (DoDirectPaymentResponseType)this.SoapCall("DoDirectPayment", oReq);

            //RECORD RECEIVE
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oResp), null);
            }

            //CREATE THE TRANSACTION RETURN OBJECT
            Transaction transaction = new Transaction();
            //SET VALUES COMMON TO ALL TRANSACTIONS
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = useAuthCapture ? TransactionType.AuthorizeCapture : TransactionType.Authorize;
            transaction.RemoteIP = authorizeRequest.RemoteIP;
            //CHECK PAYPAL RESPONSE
            if (oResp != null)
            {
                //TRANSACTIONID IS THE IDENTIFIER USED FOR CAPTURE, VOID, ETC.
                transaction.ProviderTransactionId = oResp.TransactionID;
                transaction.AuthorizationCode = oResp.TransactionID;
                //IF THERE WERE ERRORS, RECORD THEM IN THE RESPONSE MESSAGE
                if (oResp.Errors != null)
                {
                    List<string> errorList = new List<string>();
                    foreach (ErrorType error in oResp.Errors) errorList.Add(error.LongMessage + " (" + error.ErrorCode + ")");
                    transaction.ResponseMessage = String.Join(";", errorList.ToArray());
                }
                bool successful = ((oResp.Ack == AckCodeType.Success) || (oResp.Ack == AckCodeType.SuccessWithWarning) || (oResp.Ack == AckCodeType.Warning));
                if (successful)
                {
                    transaction.ResponseCode = "PENDING";
                    transaction.TransactionStatus = TransactionStatus.Successful;
                }
                else
                {
                    transaction.ResponseCode = oResp.Ack.ToString();
                    transaction.TransactionStatus = TransactionStatus.Failed;
                }
                transaction.TransactionDate = oResp.Timestamp.ToUniversalTime();
                if (oResp.Amount != null) transaction.Amount = AlwaysConvert.ToDecimal(oResp.Amount.Value, (Decimal)authorizeRequest.Amount);
                else transaction.Amount = authorizeRequest.Amount;
                transaction.AVSResultCode = oResp.AVSCode;
                transaction.CVVResultCode = oResp.CVV2Code;
            }
            else
            {
                //NULL RESPONSE, SOMETHING FAILED IN SOAP CALL
                transaction.ResponseCode = AckCodeType.Failure.ToString();
                transaction.ResponseMessage = "Error occurred in PayPal communication.";
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = curOrderTotal;
            }
            return transaction;
        }

        public override CommerceBuilder.Payments.Transaction DoCapture(CommerceBuilder.Payments.CaptureTransactionRequest captureRequest)
        {
            //BUILD REQUEST FOR PAYPAL
            DoCaptureRequestType oReq = new DoCaptureRequestType();
            oReq.Version = "1.00";
            oReq.AuthorizationID = captureRequest.AuthorizeTransaction.AuthorizationCode;
            oReq.Amount = new BasicAmountType();
            oReq.Amount.currencyID = GetPayPalCurrencyType(Token.Instance.Store.BaseCurrency.ISOCode);
            oReq.Amount.Value = string.Format("{0:##,##0.00}", captureRequest.Amount);
            oReq.CompleteType = captureRequest.IsFinal ? CompleteCodeType.Complete : CompleteCodeType.NotComplete;

            //RECORD SEND
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oReq), null);
            }

            //EXECUTE REQUEST
            DoCaptureResponseType oResp;
            oResp = (DoCaptureResponseType)this.SoapCall("DoCapture", oReq);

            //RECORD RECEIVE
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Receive, XMLSerializer.ToXML(oResp), null);
            }

            //CREATE THE TRANSACTION RETURN OBJECT
            Payment payment = captureRequest.Payment;
            Transaction transaction = new Transaction();
            //SET VALUES COMMON TO ALL TRANSACTIONS
            transaction.PaymentId = payment.PaymentId;
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = captureRequest.TransactionType;
            transaction.RemoteIP = captureRequest.RemoteIP;
            //CHECK PAYPAL RESPONSE
            if (oResp != null)
            {
                //TRANSACTIONID IS THE IDENTIFIER USED FOR CAPTURE, VOID, ETC.
                transaction.ProviderTransactionId = oResp.DoCaptureResponseDetails.PaymentInfo.TransactionID;
                transaction.AuthorizationCode = oResp.DoCaptureResponseDetails.AuthorizationID;
                //IF THERE WERE ERRORS, RECORD THEM IN THE RESPONSE MESSAGE
                if (oResp.Errors != null)
                {
                    List<string> errorList = new List<string>();
                    foreach (ErrorType error in oResp.Errors) errorList.Add(error.LongMessage + " (" + error.ErrorCode + ")");
                    transaction.ResponseMessage = String.Join(";", errorList.ToArray());
                }
                bool successful = ((oResp.Ack == AckCodeType.Success) || (oResp.Ack == AckCodeType.SuccessWithWarning) || (oResp.Ack == AckCodeType.Warning));
                if (successful)
                {
                    transaction.ResponseCode = string.Empty;
                    transaction.TransactionStatus = TransactionStatus.Successful;
                }
                else
                {
                    transaction.ResponseCode = oResp.Ack.ToString();
                    transaction.TransactionStatus = TransactionStatus.Failed;
                }
                
                transaction.TransactionDate = oResp.Timestamp.ToUniversalTime();
                transaction.Amount = captureRequest.Amount;
                //WE HAVE TO MAKE SURE WE DO NOT GET DUPLICATE ENTRIES FROM IPN
                transaction.Save();
                TransactionCollection matchingTransactions = TransactionDataSource.LoadForProviderTransaction(this.PaymentGatewayId, transaction.ProviderTransactionId);
                if (matchingTransactions.Count > 1)
                {
                    //DELETE ANY TRANSACTIONS THAT ARE NOT THIS ONE (ADDED BY IPN)
                    int i = matchingTransactions.Count - 1;
                    while (i >= 0)
                    {
                        if (matchingTransactions[i].TransactionId != transaction.TransactionId)
                            matchingTransactions[i].Delete();
                        i--;
                    }
                }
            }
            else
            {
                //NULL RESPONSE, SOMETHING FAILED IN SOAP CALL
                transaction.ResponseCode = AckCodeType.Failure.ToString();
                transaction.ResponseMessage = "Error occurred in PayPal communication.";
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = captureRequest.Amount;
            }
            return transaction;
        }

        private bool HasRefundTransactions(TransactionCollection transactions)
        {
            foreach (Transaction tx in transactions)
            {
                if ((tx.TransactionType == TransactionType.PartialRefund || tx.TransactionType == TransactionType.Refund) &&
                    tx.TransactionStatus != TransactionStatus.Failed) return true;
            }
            return false;
        }

        public override Transaction DoRefund(RefundTransactionRequest refundRequest)
        {
            //BUILD REQUEST FOR PAYPAL
            Payment originalPayment = refundRequest.Payment;
            RefundTransactionRequestType oReq = new RefundTransactionRequestType();
            oReq.Version = "1.00";
            oReq.TransactionID = refundRequest.CaptureTransaction.ProviderTransactionId;
            LSDecimal totalCaptured = originalPayment.Transactions.GetTotalCaptured();
            LSDecimal refundAmount = refundRequest.Amount;
            if (refundAmount > totalCaptured) refundAmount = totalCaptured;
            if ((refundAmount == totalCaptured) && (!HasRefundTransactions(originalPayment.Transactions)))
            {
                oReq.Amount = new BasicAmountType();
                oReq.Amount.currencyID = GetPayPalCurrencyType(refundRequest.CurrencyCode);
                oReq.Amount.Value = string.Format("{0:##,##0.00}", refundAmount);
                oReq.RefundType = RefundPurposeTypeCodeType.Full;
            }
            else
            {
                oReq.Amount = new BasicAmountType();
                oReq.Amount.currencyID = GetPayPalCurrencyType(refundRequest.CurrencyCode);
                oReq.Amount.Value = string.Format("{0:##,##0.00}", refundAmount);
                oReq.RefundType = RefundPurposeTypeCodeType.Partial;
            }
            oReq.RefundTypeSpecified = true;

            //RECORD SEND
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oReq), null);
            }

            //EXECUTE REQUEST
            RefundTransactionResponseType oResp = (RefundTransactionResponseType)this.SoapCall("RefundTransaction", oReq);

            //RECORD RECEIVE
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oResp), null);
            }

            //CREATE THE TRANSACTION RETURN OBJECT
            Transaction transaction = new Transaction();
            //SET VALUES COMMON TO ALL TRANSACTIONS
            transaction.PaymentId = originalPayment.PaymentId;
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.RemoteIP = refundRequest.RemoteIP;
            //CHECK PAYPAL RESPONSE
            if (oResp != null)
            {
                //TRY TO GET THE ACTUAL AMOUNT REFUNDED
                LSDecimal actualRefund;
                if (oResp.GrossRefundAmount != null)
                {
                    WebTrace.Write("PP Refund Amount: " + oResp.GrossRefundAmount.Value);
                    actualRefund = AlwaysConvert.ToDecimal(oResp.GrossRefundAmount.Value);
                    WebTrace.Write("actualRefund Value: " + actualRefund.ToString());
                }
                else actualRefund = refundAmount;
                transaction.TransactionType = (actualRefund == totalCaptured) ? TransactionType.Refund : TransactionType.PartialRefund;

                //TRANSACTIONID IS THE IDENTIFIER USED FOR CAPTURE, VOID, ETC.
                transaction.ProviderTransactionId = oResp.RefundTransactionID;
                transaction.AuthorizationCode = refundRequest.CaptureTransaction.AuthorizationCode;
                //IF THERE WERE ERRORS, RECORD THEM IN THE RESPONSE MESSAGE
                if (oResp.Errors != null)
                {
                    List<string> errorList = new List<string>();
                    foreach (ErrorType error in oResp.Errors) errorList.Add(error.LongMessage + " (" + error.ErrorCode + ")");
                    transaction.ResponseMessage = String.Join(";", errorList.ToArray());
                    transaction.ResponseCode = oResp.Ack.ToString();
                }
                bool successful = ((oResp.Ack == AckCodeType.Success) || (oResp.Ack == AckCodeType.SuccessWithWarning) || (oResp.Ack == AckCodeType.Warning));
                transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
                transaction.TransactionDate = oResp.Timestamp.ToUniversalTime();
                transaction.Amount = actualRefund;
                //WE HAVE TO MAKE SURE WE DO NOT GET DUPLICATE ENTRIES FROM IPN
                transaction.Save();
                TransactionCollection matchingTransactions = TransactionDataSource.LoadForProviderTransaction(this.PaymentGatewayId, transaction.ProviderTransactionId);
                if (matchingTransactions.Count > 1)
                {
                    //DELETE ANY TRANSACTIONS THAT ARE NOT THIS ONE (ADDED BY IPN)
                    int i = matchingTransactions.Count - 1;
                    while (i >= 0)
                    {
                        if (matchingTransactions[i].TransactionId != transaction.TransactionId)
                            matchingTransactions[i].Delete();
                        i--;
                    }
                }
            }
            else
            {
                //NULL RESPONSE, SOMETHING FAILED IN SOAP CALL
                transaction.TransactionType = (refundAmount == totalCaptured) ? TransactionType.Refund : TransactionType.PartialRefund;
                transaction.ResponseCode = AckCodeType.Failure.ToString();
                transaction.ResponseMessage = "Error occurred in PayPal communication.";
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = refundRequest.Amount;
            }
            return transaction;
        }

        public override CommerceBuilder.Payments.Transaction DoVoid(CommerceBuilder.Payments.VoidTransactionRequest voidRequest)
        {
            //BUILD REQUEST FOR PAYPAL
            DoVoidRequestType oReq = new DoVoidRequestType();
            oReq.Version = "1.00";
            oReq.AuthorizationID = voidRequest.AuthorizeTransaction.AuthorizationCode;

            //RECORD SEND
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oReq), null);
            }

            //EXECUTE REQUEST
            DoVoidResponseType oResp;
            oResp = (DoVoidResponseType)this.SoapCall("DoVoid", oReq);

            //RECORD RECEIVE
            if (this.UseDebugMode)
            {
                this.RecordCommunication(this.Name, CommunicationDirection.Send, XMLSerializer.ToXML(oResp), null);
            }

            //CREATE THE TRANSACTION RETURN OBJECT
            Payment payment = voidRequest.Payment;
            Transaction transaction = new Transaction();
            //SET VALUES COMMON TO ALL TRANSACTIONS
            transaction.PaymentId = payment.PaymentId;
            transaction.PaymentGatewayId = this.PaymentGatewayId;
            transaction.TransactionType = TransactionType.Void;
            transaction.TransactionType = voidRequest.TransactionType;
            transaction.RemoteIP = voidRequest.RemoteIP;
            //CHECK PAYPAL RESPONSE
            if (oResp != null)
            {
                //TRANSACTIONID IS THE IDENTIFIER USED FOR CAPTURE, VOID, ETC.
                transaction.ProviderTransactionId = oResp.AuthorizationID;
                transaction.AuthorizationCode = oResp.AuthorizationID;
                transaction.ResponseCode = oResp.Ack.ToString();
                //IF THERE WERE ERRORS, RECORD THEM IN THE RESPONSE MESSAGE
                if (oResp.Errors != null)
                {
                    List<string> errorList = new List<string>();
                    foreach (ErrorType error in oResp.Errors) errorList.Add(error.LongMessage + " (" + error.ErrorCode + ")");
                    transaction.ResponseMessage = String.Join(";", errorList.ToArray());
                }
                bool successful = ((oResp.Ack == AckCodeType.Success) || (oResp.Ack == AckCodeType.SuccessWithWarning) || (oResp.Ack == AckCodeType.Warning));
                transaction.TransactionStatus = successful ? TransactionStatus.Successful : TransactionStatus.Failed;
                transaction.TransactionDate = oResp.Timestamp.ToUniversalTime();
                transaction.Amount = voidRequest.Amount;
                //ADD AN ARTIFICIAL DELAY SO IPN IS SURE TO OCCUR FIRST
                System.Threading.Thread.Sleep(6000);
                //WE HAVE TO MAKE SURE WE DO NOT GET DUPLICATE ENTRIES FROM IPN
                transaction.Save();
                TransactionCollection matchingTransactions = TransactionDataSource.LoadForProviderTransaction(this.PaymentGatewayId, transaction.ProviderTransactionId);
                if (matchingTransactions.Count > 1)
                {
                    //DELETE ANY TRANSACTIONS THAT ARE NOT THIS ONE (ADDED BY IPN)
                    //MAKE SURE THE TRANSACTION IS A VOID AND NOT AN AUTH AS THEY WILL HAVE THE SAME TXID
                    int i = matchingTransactions.Count - 1;
                    while (i >= 0)
                    {
                        if ((matchingTransactions[i].TransactionType == TransactionType.Void)
                            && (matchingTransactions[i].TransactionId != transaction.TransactionId))
                            matchingTransactions[i].Delete();
                        i--;
                    }
                }
            }
            else
            {
                //NULL RESPONSE, SOMETHING FAILED IN SOAP CALL
                transaction.ResponseCode = AckCodeType.Failure.ToString();
                transaction.ResponseMessage = "Error occurred in PayPal communication.";
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.Amount = voidRequest.Amount;
            }
            return transaction;
        }

        public override string Name
        {
            get { return "PayPal"; }
        }

        public override string Description
        {
            get { return "PayPal processor description!"; }
        }

        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize | SupportedTransactions.AuthorizeCapture | SupportedTransactions.PartialCapture | SupportedTransactions.Capture | SupportedTransactions.PartialRefund | SupportedTransactions.Refund | SupportedTransactions.Void);
            }
        }

        public override string Version
        {
            get { return "PayPal SDK v4.2.1.0"; }
        }

        public ExpressCheckoutResult SetExpressCheckout()
        {
            HttpContext context = HttpContext.Current;
            User user = Token.Instance.User;
            Basket basket = user.Basket;

            //MAKE SURE BASKET IS PROPERLY PACKAGED FOR CHECKOUT
            basket.Package();

            //GET EXISTING SESSION IF IT IS PRESENT
            ExpressCheckoutSession existingSession = ExpressCheckoutSession.Current;
            if (existingSession != null) WebTrace.Write("Existing session token: " + existingSession.Token);

            //CREATE THE EXPRESS CHECKOUT REQUEST OBJECT
            SetExpressCheckoutRequestType expressCheckoutRequest = new SetExpressCheckoutRequestType();
            expressCheckoutRequest.SetExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType();
            if (existingSession != null) expressCheckoutRequest.SetExpressCheckoutRequestDetails.Token = existingSession.Token;
            expressCheckoutRequest.Version = "1.0";

            //GET THE CURRENCY FOR THE TRANSACTION
            string baseCurrencyCode = Token.Instance.Store.BaseCurrency.ISOCode;
            CurrencyCodeType baseCurrency = GetPayPalCurrencyType(baseCurrencyCode);

            //BUILD THE REQUEST DETAILS
            SetExpressCheckoutRequestDetailsType expressCheckoutDetails = expressCheckoutRequest.SetExpressCheckoutRequestDetails;
            LSDecimal basketTotal = basket.Items.TotalPrice();
            WebTrace.Write("Basket Total: " + basketTotal.ToString());
            expressCheckoutDetails.OrderTotal = new BasicAmountType();
            expressCheckoutDetails.OrderTotal.currencyID = baseCurrency;
            expressCheckoutDetails.OrderTotal.Value = string.Format("{0:##,##0.00}", basketTotal);
            expressCheckoutDetails.MaxAmount = new BasicAmountType();
            expressCheckoutDetails.MaxAmount.currencyID = baseCurrency;
            expressCheckoutDetails.MaxAmount.Value = string.Format("{0:##,##0.00}", basketTotal + 50);

            //SET THE URLS
            string storeUrl = GetStoreUrl();
            expressCheckoutDetails.ReturnURL = storeUrl + "/PayPalExpressCheckout.aspx?Action=GET";
            expressCheckoutDetails.CancelURL = storeUrl + "/PayPalExpressCheckout.aspx?Action=CANCEL";

            //SET THE CUSTOM VALUE TO THE USER ID FOR MATCHING DURING GET
            expressCheckoutDetails.Custom = "UID" + basket.UserId.ToString();

            //SET THE CUSTOMER ADDRESS
            Address billingAddress = user.PrimaryAddress;
            AddressType address = new AddressType();
            address.Name = billingAddress.FirstName + " " + billingAddress.LastName;
            address.Street1 = billingAddress.Address1;
            address.Street2 = billingAddress.Address2;
            address.CityName = billingAddress.City;
            address.PostalCode = billingAddress.PostalCode;
            if (billingAddress.Country != null) address.Country = GetPayPalCountry(billingAddress.CountryCode);
            else address.Country = CountryCodeType.US;
            expressCheckoutDetails.BuyerEmail = billingAddress.Email;
            expressCheckoutDetails.Address = address;

            //SET THE PAYMENT ACTION
            expressCheckoutDetails.PaymentAction = this.UseAuthCapture ? PaymentActionCodeType.Sale : PaymentActionCodeType.Authorization;
            expressCheckoutDetails.PaymentActionSpecified = true;

            //EXECUTE REQUEST
            SetExpressCheckoutResponseType expressCheckoutResponse;
            context.Trace.Write("DO SOAP CALL");
            expressCheckoutResponse = (SetExpressCheckoutResponseType)SoapCall("SetExpressCheckout", expressCheckoutRequest);
            context.Trace.Write("CHECK SOAP RESULT");
            if (expressCheckoutResponse == null)
            {
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "NORESP";
                customError.ShortMessage = "No Response From Server";
                customError.LongMessage = "The PayPal service is unavailable at this time.";
                customErrorList[0] = customError;
                return new ExpressCheckoutResult(0, string.Empty, customErrorList);
            }

            //IF ERRORS ARE IN RESPONSE, RETURN THEM AND EXIT PROCESS
            if (expressCheckoutResponse.Errors != null) return new ExpressCheckoutResult(0, string.Empty, expressCheckoutResponse.Errors);

            //NO ERRORS FOUND, PUT PAYPAL DETAILS INTO SESSION
            context.Trace.Write("Store PayPal Token In Session");
            ExpressCheckoutSession newSession = new ExpressCheckoutSession();
            newSession.Token = expressCheckoutResponse.Token;
            newSession.TokenExpiration = DateTime.UtcNow.AddHours(3);
            newSession.Save();

            context.Trace.Write("Saved PayPal Token:" + newSession.Token);
            context.Trace.Write("Token Expiration:" + newSession.TokenExpiration.ToLongDateString());

            //RETURN TO CALLER INCLUDING REDIRECTION URL
            string redirectUrl = "https://www" + (this.UseSandbox ? ".sandbox" : string.Empty) + ".paypal.com/webscr?cmd=_express-checkout&token=" + expressCheckoutResponse.Token;
            return new ExpressCheckoutResult(0, redirectUrl, null);
        }

        public GetExpressCheckoutResult GetExpressCheckout()
        {
            HttpContext context = HttpContext.Current;
            ExpressCheckoutSession existingSession = ExpressCheckoutSession.Current;
            if (existingSession == null)
            {
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "SESSION";
                customError.ShortMessage = "Missing Token";
                customError.LongMessage = "The PayPal session token was expired or unavailable.  Please try again.";
                customErrorList[0] = customError;
                return new GetExpressCheckoutResult(null, customErrorList);
            }
            context.Trace.Write("Detected PayPal Token:" + existingSession.Token);
            context.Trace.Write("Token Expiration:" + existingSession.TokenExpiration.ToLongDateString());

            GetExpressCheckoutDetailsRequestType expressCheckoutRequest = new GetExpressCheckoutDetailsRequestType();
            expressCheckoutRequest.Token = existingSession.Token;
            expressCheckoutRequest.Version = "1.0";

            //EXECUTE REQUEST
            GetExpressCheckoutDetailsResponseType expressCheckoutResponse;
            expressCheckoutResponse = (GetExpressCheckoutDetailsResponseType)SoapCall("GetExpressCheckoutDetails", expressCheckoutRequest);
            if (expressCheckoutResponse == null)
            {
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "NORESP";
                customError.ShortMessage = "No Response From Server";
                customError.LongMessage = "The PayPal service is unavailable at this time.";
                customErrorList[0] = customError;
                return new GetExpressCheckoutResult(null, customErrorList);
            }

            //IF ERRORS ARE IN RESPONSE, RETURN THEM AND EXIT PROCESS
            if (expressCheckoutResponse.Errors != null) return new GetExpressCheckoutResult(null, expressCheckoutResponse.Errors);

            //GET THE DETAILS OF THE REQUEST
            GetExpressCheckoutDetailsResponseDetailsType expressCheckoutDetails;
            expressCheckoutDetails = expressCheckoutResponse.GetExpressCheckoutDetailsResponseDetails;

            //MAKE SURE CUSTOMER IDS MATCH
            User currentUser = Token.Instance.User;
            if (expressCheckoutDetails.Custom != ("UID" + currentUser.UserId.ToString()))
            {
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "USER";
                customError.ShortMessage = "User Mismatch";
                customError.LongMessage = "The PayPal basket did not have the expected user context.";
                customErrorList[0] = customError;
                Logger.Warn("Error in PayPal GetExpressCheckout.  User ID detected in PayPal response: " + expressCheckoutDetails.Custom + ", Customer User ID: " + currentUser.UserId.ToString());
                return new GetExpressCheckoutResult(null, customErrorList);
            }

            //CHECK WHETHER AN EXISTING USER IS ASSOCIATED WITH THE RETURNED PAYPAL ID
            //IF THE CURRENT USER DOES NOT MATCH, LOG IN THE PAYPAL USER ACCOUNT
            string paypalEmail = expressCheckoutDetails.PayerInfo.Payer;
            string paypalPayerID = expressCheckoutDetails.PayerInfo.PayerID;
            //PAYER ID IS SUPPOSED TO BE UNIQUE REGARDLESS OF EMAIL ADDRESS, LOOK FOR ASSOCIATED ACCT
            User paypalUser = UserDataSource.LoadForPayPalId(paypalPayerID);
            //IF NOT FOUND, SEE IF AN ACCOUNT EXISTS WITH THAT EMAIL AS USERNAME
            if (paypalUser == null) paypalUser = UserDataSource.LoadForUserName(paypalEmail);
            if (paypalUser != null)
            {
                //WE FOUND AN ACCOUNT FOR THIS PAYPAL USER
                context.Trace.Write(this.GetType().ToString(), "PAYPAL USER FOUND IN DATABASE");
                if (currentUser.UserId != paypalUser.UserId)
                {
                    //THE PAYPAL USER IS NOT THE CURRENT USER CONTEXT, SO TRANSFER THE BASKET
                    context.Trace.Write(this.GetType().ToString(), "MOVE BASKET TO " + paypalUser.UserName);
                    Basket.Transfer(currentUser.UserId, paypalUser.UserId, true);
                    //REMOVE PAYPAL EXPRESS SESSION FROM OLD USER SESSION
                    ExpressCheckoutSession.Delete(currentUser);
                }
            }
            else
            {
                //WE DID NOT FIND AN ACCOUNT
                context.Trace.Write(this.GetType().ToString(), "PAYPAL USER NOT FOUND IN DATABASE");
                if (currentUser.IsAnonymous)
                {
                    //CURRENT USER IS ANON, REGISTER A NEW USER ACCOUNT
                    context.Trace.Write(this.GetType().ToString(), "REGISTERING " + paypalEmail);
                    MembershipCreateStatus status;
                    paypalUser = UserDataSource.CreateUser(paypalEmail, paypalEmail, StringHelper.RandomString(8), string.Empty, string.Empty, true, 0, out status);
                    paypalUser.PayPalId = paypalPayerID;
                    paypalUser.Save();
                    Basket.Transfer(currentUser.UserId, paypalUser.UserId, true);
                    //REMOVE PAYPAL EXPRESS SESSION FROM OLD USER SESSION
                    ExpressCheckoutSession.Delete(currentUser);
                }
                else
                {
                    //UPDATE THE PAYPAL ID OF THE CURRENTLY AUTHENTICATED USER
                    context.Trace.Write(this.GetType().ToString(), "ASSIGNING CURRENT USER TO " + paypalEmail);
                    paypalUser = currentUser;
                    paypalUser.PayPalId = paypalPayerID;
                    paypalUser.Save();
                }
            }

            //PAYPAL HAS AUTHENTICATED THE USER
            FormsAuthentication.SetAuthCookie(paypalUser.UserName, false);
            //UPDATE THE PRIMARY ADDRESS INFORMATION FOR THE USER
            Address billingAddress = paypalUser.PrimaryAddress;
            billingAddress.FirstName = expressCheckoutDetails.PayerInfo.PayerName.FirstName;
            billingAddress.LastName = expressCheckoutDetails.PayerInfo.PayerName.LastName;
            billingAddress.Company = expressCheckoutDetails.PayerInfo.PayerBusiness;
            billingAddress.Address1 = expressCheckoutDetails.PayerInfo.Address.Street1;
            billingAddress.Address2 = expressCheckoutDetails.PayerInfo.Address.Street2;
            billingAddress.City = expressCheckoutDetails.PayerInfo.Address.CityName;
            billingAddress.Province = expressCheckoutDetails.PayerInfo.Address.StateOrProvince;
            billingAddress.PostalCode = expressCheckoutDetails.PayerInfo.Address.PostalCode;
            billingAddress.CountryCode = expressCheckoutDetails.PayerInfo.Address.Country.ToString();
            if (!string.IsNullOrEmpty(expressCheckoutDetails.ContactPhone)) billingAddress.Phone = expressCheckoutDetails.ContactPhone;
            billingAddress.Email = expressCheckoutDetails.PayerInfo.Payer;
            billingAddress.Residence = (!string.IsNullOrEmpty(billingAddress.Company));
            paypalUser.Save();

            //UPDATE THE SHIPPING ADDRESS IN THE BASKET
            Basket basket = paypalUser.Basket;
            basket.Package();
            foreach (BasketShipment shipment in basket.Shipments) shipment.AddressId = billingAddress.AddressId;
            basket.Save();

            //PUT PAYPAL DETAILS INTO SESSION
            context.Trace.Write(this.GetType().ToString(), "Saving ExpressCheckoutSession");
            existingSession.Token = expressCheckoutDetails.Token;
            existingSession.TokenExpiration = DateTime.UtcNow.AddHours(3);
            existingSession.PayerID = paypalPayerID;
            existingSession.Payer = expressCheckoutDetails.PayerInfo.Payer;
            existingSession.Save(paypalUser);
            context.Trace.Write("Saved PayPal Token:" + existingSession.Token);
            context.Trace.Write("Token Expiration:" + existingSession.TokenExpiration.ToLongDateString());
            return new GetExpressCheckoutResult(paypalUser, null);
        }

        public ExpressCheckoutResult DoExpressCheckout()
        {
            HttpContext context = HttpContext.Current;
            TraceContext trace = context.Trace;
            string traceCategory = this.GetType().ToString();
            ExpressCheckoutSession paypalSession = ExpressCheckoutSession.Current;
            if (paypalSession == null)
            {
                //EXIT WITH EXCEPTION
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "SESSION";
                customError.ShortMessage = "Missing Token";
                customError.LongMessage = "The PayPal session token was expired or unavailable.  Please try again.";
                customErrorList[0] = customError;
                return new ExpressCheckoutResult(0, string.Empty, customErrorList);
            }
            trace.Write(traceCategory, "Detected PayPal Token:" + paypalSession.Token);
            trace.Write(traceCategory, "Token Expiration:" + paypalSession.TokenExpiration.ToLongDateString());

            if (string.IsNullOrEmpty(paypalSession.PayerID))
            {
                //EXIT WITH EXCEPTION
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "SESSION";
                customError.ShortMessage = "Missing Payer ID";
                customError.LongMessage = "The PayPal Payer ID is not present.";
                customErrorList[0] = customError;
                return new ExpressCheckoutResult(0, string.Empty, customErrorList);
            }
            trace.Write(traceCategory, "Detected PayPal Payer ID:" + paypalSession.PayerID);

            //GET THE CURRENCY FOR THE TRANSACTION
            string storeCurrencyCode = Token.Instance.Store.BaseCurrency.ISOCode;
            CurrencyCodeType baseCurrencyCode = GetPayPalCurrencyType(storeCurrencyCode);

            //CREATE THE EXPRESS CHECKOUT
            DoExpressCheckoutPaymentRequestType expressCheckoutRequest = new DoExpressCheckoutPaymentRequestType();
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.Token = paypalSession.Token;
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentAction = this.UseAuthCapture ? PaymentActionCodeType.Sale : PaymentActionCodeType.Authorization;
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PayerID = paypalSession.PayerID;
            expressCheckoutRequest.Version = "1.0";

            //SET THE ORDER TOTAL AMOUNTS
            Basket basket = Token.Instance.User.Basket;
            trace.Write(traceCategory, "Set Order Totals");
            LSDecimal curOrderTotal = basket.Items.TotalPrice();
            LSDecimal curShippingTotal = basket.Items.TotalPrice(OrderItemType.Shipping) + GetShippingCouponTotal(basket.Items);
            LSDecimal curHandlingTotal = basket.Items.TotalPrice(OrderItemType.Handling);
            LSDecimal curTaxTotal = basket.Items.TotalPrice(OrderItemType.Tax);
            LSDecimal curItemTotal = curOrderTotal - (curShippingTotal + curHandlingTotal + curTaxTotal);
            //MAKE SURE OUR BREAKDOWN IS VALID
            if ((curShippingTotal < 0) || (curHandlingTotal < 0) || (curTaxTotal < 0) || (curItemTotal < 0))
            {
                //THE BREAKDOWN IS INVALID, DO NOT INCLUDE IT IN THE REQUEST
                curShippingTotal = 0;
                curHandlingTotal = 0;
                curTaxTotal = 0;
                curItemTotal = curOrderTotal;
            }

            //SET THE PAYMENT DETAILS
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails = new PaymentDetailsType();
            PaymentDetailsType paymentDetails = expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails;
            paymentDetails.OrderTotal = new BasicAmountType();
            paymentDetails.OrderTotal.currencyID = baseCurrencyCode;
            paymentDetails.OrderTotal.Value = string.Format("{0:##,##0.00}", curOrderTotal);

            paymentDetails.ItemTotal = new BasicAmountType();
            paymentDetails.ItemTotal.currencyID = baseCurrencyCode;
            paymentDetails.ItemTotal.Value = string.Format("{0:##,##0.00}", curItemTotal);

            paymentDetails.ShippingTotal = new BasicAmountType();
            paymentDetails.ShippingTotal.currencyID = baseCurrencyCode;
            paymentDetails.ShippingTotal.Value = string.Format("{0:##,##0.00}", curShippingTotal);

            paymentDetails.HandlingTotal = new BasicAmountType();
            paymentDetails.HandlingTotal.currencyID = baseCurrencyCode;
            paymentDetails.HandlingTotal.Value = string.Format("{0:##,##0.00}", curHandlingTotal);

            paymentDetails.TaxTotal = new BasicAmountType();
            paymentDetails.TaxTotal.currencyID = baseCurrencyCode;
            paymentDetails.TaxTotal.Value = string.Format("{0:##,##0.00}", curTaxTotal);

            trace.Write(traceCategory, "Order Total: " + curOrderTotal);
            trace.Write(traceCategory, "Item Total: " + curItemTotal);
            trace.Write(traceCategory, "Shipping Total: " + curShippingTotal);
            trace.Write(traceCategory, "Handling Total: " + curHandlingTotal);
            trace.Write(traceCategory, "Tax Total: " + curTaxTotal);

            //SET THE BUTTON SOURCE
            trace.Write(traceCategory, "Set Button Source");
            paymentDetails.ButtonSource = "ablecommerce-EC";
            
            //SET THE NOTIFY URL
            string notifyUrl = GetStoreUrl() + "/ProcessPayPal.ashx";
            trace.Write(traceCategory, "IPN Callback URL: " + notifyUrl);
            paymentDetails.NotifyURL = notifyUrl;

            //WE HAVE ALL NECESSARY INFORMATION TO DO EXPRESS CHECKOUT
            //COMMIT THE ORDER BEFORE SUBMITTING THE PAYPAL TRANSACTION

            //CREATE THE ABLECOMMERCE PAYMENT ITEM
            Payment checkoutPayment = new Payment();
            checkoutPayment.PaymentMethodId = GetPayPalPaymentMethodId(false);
            checkoutPayment.Amount = curOrderTotal;
            checkoutPayment.CurrencyCode = baseCurrencyCode.ToString();

            //AT THIS POINT, EXECUTE THE CHECKOUT TO SUBMIT THE ORDER
            CheckoutRequest checkoutRequest = new CheckoutRequest(checkoutPayment);
            CheckoutResponse checkoutResponse = basket.Checkout(checkoutRequest);
            int orderId = checkoutResponse.OrderId;

            //LOAD THE ORDER AND RE-OBTAIN THE PAYMENT RECORD TO AVOID DATA INCONSISTENCIES
            Order order = OrderDataSource.Load(orderId);
            if (order == null)
            {
                //EXIT WITH EXCEPTION
                ErrorType[] customErrorList = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "ORDER";
                customError.ShortMessage = "Your order could not be completed at this time.";
                customError.LongMessage = "Your order could not be completed at this time and payment was not processed. " + string.Join(" ", checkoutResponse.WarningMessages.ToArray());
                customErrorList[0] = customError;
                return new ExpressCheckoutResult(0, string.Empty, customErrorList);
            }

            int findPaymentId = checkoutPayment.PaymentId;
            foreach (Payment payment in order.Payments)
            {
                if (payment.PaymentId == findPaymentId)
                    checkoutPayment = payment;
            }

            //SET THE DESCRIPTION
            paymentDetails.OrderDescription = "Order #" + order.OrderNumber.ToString();
            paymentDetails.Custom = orderId.ToString();

            //EXECUTE PAYPAL REQUEST
            trace.Write(traceCategory, "Do Request");
            DoExpressCheckoutPaymentResponseType expressCheckoutResponse = (DoExpressCheckoutPaymentResponseType)SoapCall("DoExpressCheckoutPayment", expressCheckoutRequest);
            ErrorType[] responseErrors = null;
            PaymentStatus finalPaymentStatus = PaymentStatus.Unprocessed;
            bool isPendingeCheck = false;
            if (expressCheckoutResponse != null)
            {
                if (expressCheckoutResponse.Errors == null)
                {
                    //CREATE THE PAYPAL TRANSACTION RECORD
                    Transaction checkoutTransaction = new Transaction();
                    PaymentInfoType paymentInfo = expressCheckoutResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo;
                    isPendingeCheck = (paymentInfo.PaymentStatus == PaymentStatusCodeType.Pending && paymentInfo.PendingReason == PendingStatusCodeType.echeck);
                    PaymentStatusCodeType paymentStatus = paymentInfo.PaymentStatus;
                    switch (paymentStatus)
                    {
                        case PaymentStatusCodeType.Completed:
                        case PaymentStatusCodeType.Processed:
                        case PaymentStatusCodeType.Pending:
                            if (isPendingeCheck)
                            {
                                finalPaymentStatus = PaymentStatus.CapturePending;
                                checkoutTransaction.ResponseCode = "PENDING";
                                checkoutTransaction.ResponseMessage = "echeck";
                            }
                            else finalPaymentStatus = (paymentStatus != PaymentStatusCodeType.Pending) ? PaymentStatus.Captured : PaymentStatus.Authorized;
                            checkoutTransaction.TransactionStatus = TransactionStatus.Successful;
                            break;
                        default:
                            finalPaymentStatus = PaymentStatus.Unprocessed;
                            checkoutTransaction.TransactionStatus = TransactionStatus.Failed;
                            checkoutTransaction.ResponseCode = expressCheckoutResponse.Ack.ToString();
                            checkoutTransaction.ResponseMessage = paymentStatus.ToString().ToUpperInvariant();
                            break;
                    }
                    checkoutTransaction.TransactionType = this.UseAuthCapture ? TransactionType.Capture : TransactionType.Authorize;
                    checkoutTransaction.Amount = AlwaysConvert.ToDecimal(paymentInfo.GrossAmount.Value, (Decimal)curOrderTotal);
                    checkoutTransaction.AuthorizationCode = paymentInfo.TransactionID;
                    checkoutTransaction.AVSResultCode = "U";
                    checkoutTransaction.ProviderTransactionId = paymentInfo.TransactionID;
                    checkoutTransaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                    checkoutTransaction.PaymentGatewayId = this.PaymentGatewayId;
                    checkoutTransaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                    checkoutPayment.Transactions.Add(checkoutTransaction);

                    //FIND THE WAITING FOR IPN TRANSACTION AND REMOVE
                    int i = checkoutPayment.Transactions.Count - 1;
                    while (i >= 0)
                    {
                        if (string.IsNullOrEmpty(checkoutPayment.Transactions[i].AuthorizationCode))
                            checkoutPayment.Transactions.DeleteAt(i);
                        i--;
                    }
                }
                else
                {
                    //SOME SORT OF ERROR ATTEMPTING CHECKOUT
                    responseErrors = expressCheckoutResponse.Errors;
                }
            }
            else
            {
                //NO RESPONSE, GENERATE CUSTOM ERROR
                responseErrors = new ErrorType[1];
                ErrorType customError = new ErrorType();
                customError.ErrorCode = "NORESP";
                customError.ShortMessage = "No Response From Server";
                customError.LongMessage = "The PayPal service is unavailable at this time.";
                responseErrors[0] = customError;
            }
            trace.Write(traceCategory, "Do Request Done");

            //ERRORS IN RESPONSE?
            if ((responseErrors != null) && (responseErrors.Length > 0))
            {
                //CREATE THE PAYPAL TRANSACTION RECORD FOR ERROR
                Transaction checkoutTransaction = new Transaction();
                finalPaymentStatus = PaymentStatus.Unprocessed;
                checkoutTransaction.TransactionStatus = TransactionStatus.Failed;
                checkoutTransaction.Amount = curOrderTotal;
                checkoutTransaction.AuthorizationCode = string.Empty;
                checkoutTransaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
                checkoutTransaction.PaymentGatewayId = this.PaymentGatewayId;
                checkoutTransaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                checkoutTransaction.ResponseCode = responseErrors[0].ShortMessage;
                checkoutTransaction.ResponseMessage = responseErrors[0].LongMessage;
                checkoutPayment.Transactions.Add(checkoutTransaction);
            }

            //MAKE SURE PAYMENT STATUS IS CORRECT
            checkoutPayment.ReferenceNumber = paypalSession.Payer;
            checkoutPayment.PaymentStatus = finalPaymentStatus;
            if (isPendingeCheck) checkoutPayment.PaymentStatusReason = "echeck";
            
            //RECALCULATE THE ORDER STATUS (BUG 6384) AND TRIGGER PAYMENT EVENTS (BUG 8650)
            order.Save(true, true);

            //CLEAR THE TOKENS SET IN SESSION
            paypalSession.Delete();
            return new ExpressCheckoutResult(orderId, string.Empty, responseErrors);
        }

        public string GetStoreUrl()
        {
            HttpContext context = HttpContext.Current;
            if (context == null) return string.Empty;
            Uri currentUrl = context.Request.Url;
            string scheme = currentUrl.Scheme + "://";
            string port = (currentUrl.Port == 80 || currentUrl.Port == 443) ? string.Empty : ":" + currentUrl.Port.ToString();
            string host = currentUrl.Host;
            return scheme + host + port + context.Request.ApplicationPath;
        }

        public static CurrencyCodeType GetPayPalCurrencyType(string currencyCode)
        {
            CurrencyCodeType currency;
            try
            {
                currency = (CurrencyCodeType)System.Enum.Parse(typeof(CurrencyCodeType), currencyCode);
            }
            catch (ArgumentException)
            {
                currency = CurrencyCodeType.USD;
            }
            return currency;
        }

        public static CountryCodeType GetPayPalCountry(string countryCode)
        {
            CountryCodeType country;
            try
            {
                country = (CountryCodeType)System.Enum.Parse(typeof(CountryCodeType), countryCode);
            }
            catch
            {
                country = CountryCodeType.US;
            }
            return country;
        }

        public Control GetPayNowButton(Order order, int paymentId)
        {
            return new PayNowButton(order, paymentId, _PayPalAccount, _UseAuthCapture, _UseSandbox);
        }

        private static bool IsPayPalMethod(PaymentMethod method)
        {
            return (method.PaymentInstrument == PaymentInstrument.PayPal);
        }

        public bool ValidateNotification(string formValues)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.UseSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            string request = formValues + "&cmd=_notify-validate";
            httpWebRequest.ContentLength = request.Length;
            using (StreamWriter requestStream = new StreamWriter(httpWebRequest.GetRequestStream(), Encoding.ASCII))
            {
                requestStream.Write(request);
                requestStream.Close();
            }
            string response;
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                response = responseStream.ReadToEnd();
                responseStream.Close();
            }
            return (response.Equals("VERIFIED"));
        }

        public static int GetPayPalPaymentMethodId(bool checkCache)
        {
            //TODO: LOAD FOR PAYMENTINSTRUMENTID TO FILTER WITHOUT NAME SEARCH
            //LOAD THE PAYMENT GATEWAY
            int paymentMethodId = 0;
            HttpContext context = HttpContext.Current;
            if (checkCache && (context != null)) paymentMethodId = AlwaysConvert.ToInt(context.Cache["PayPal_PaymentMethodId"]);
            if (paymentMethodId == 0)
            {
                PaymentMethod paymentMethod = Token.Instance.Store.PaymentMethods.Find(PayPalProvider.IsPayPalMethod);
                if (paymentMethod == null)
                {
                    //NEED TO CREATE PAYPAL METHOD
                    paymentMethod = new PaymentMethod();
                    paymentMethod.Name = "PayPal";
                    paymentMethod.PaymentInstrument = PaymentInstrument.PayPal;
                    paymentMethod.OrderBy = -1;
                    paymentMethod.Save();
                }
                paymentMethodId = paymentMethod.PaymentMethodId;
                if (context != null) context.Cache.Add("PayPal_PaymentMethodId", paymentMethodId, null, DateTime.MaxValue, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return paymentMethodId;
        }

        public static PaymentMethod GetPayPalPaymentMethod(bool checkCache)
        {
            int paymentMethodId = PayPalProvider.GetPayPalPaymentMethodId(checkCache);
            if (paymentMethodId != 0)
            {
                PaymentMethod paypalMethod = new PaymentMethod();
                if (paypalMethod.Load(paymentMethodId)) return paypalMethod;
            }
            return null;
        }

        public static int GetPayPalPaymentGatewayId(bool checkCache)
        {
            int paymentGatewayId = 0;
            HttpContext context = HttpContext.Current;
            if (checkCache && (context != null)) paymentGatewayId = AlwaysConvert.ToInt(context.Cache["PayPal_PaymentGatewayId"]);
            if (paymentGatewayId == 0)
            {
                paymentGatewayId = PaymentGatewayDataSource.GetPaymentGatewayIdByClassId(Utility.Misc.GetClassId(typeof(PayPalProvider)));
                if (context != null) context.Cache.Add("PayPal_PaymentGatewayId", paymentGatewayId, null, DateTime.MaxValue, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return paymentGatewayId;
        }

        public static PaymentGateway GetPayPalPaymentGateway(bool checkCache)
        {
            int paymentGatewayId = PayPalProvider.GetPayPalPaymentGatewayId(checkCache);
            if (paymentGatewayId != 0)
            {
                PaymentGateway paypalGateway = new PaymentGateway();
                if (paypalGateway.Load(paymentGatewayId)) return paypalGateway;
            }
            return null;
        }

        public override string GetLogoUrl(System.Web.UI.ClientScriptManager cs)
        {
            if (cs != null)
                return cs.GetWebResourceUrl(this.GetType(), "CommerceBuilder.Payments.Providers.PayPal.Resources.Logo.gif");
            return string.Empty;
        }

        private static LSDecimal GetShippingCouponTotal(OrderItemCollection orderItems)
        {
            LSDecimal shipCouponTotal = 0;
            foreach (OrderItem oi in orderItems)
            {
                if (oi.OrderItemType == OrderItemType.Coupon)
                {
                    Coupon c = CouponDataSource.LoadForCouponCode(oi.Sku);
                    if ((c != null) && (c.CouponType == CouponType.Shipping))
                        shipCouponTotal += oi.ExtendedPrice;
                }
            }
            return shipCouponTotal;
        }

        private static LSDecimal GetShippingCouponTotal(BasketItemCollection basketItems)
        {
            LSDecimal shipCouponTotal = 0;
            foreach (BasketItem bi in basketItems)
            {
                if (bi.OrderItemType == OrderItemType.Coupon)
                {
                    Coupon c = CouponDataSource.LoadForCouponCode(bi.Sku);
                    if ((c != null) && (c.CouponType == CouponType.Shipping))
                        shipCouponTotal += bi.ExtendedPrice;
                }
            }
            return shipCouponTotal;
        }

        private AbstractResponseType SoapCall(string methodName, AbstractRequestType request)
        {
            //CREATE CLIENT PROFILE
            IAPIProfile clientProfile = ProfileFactory.CreateAPIProfile();
            clientProfile.APIUsername = this.ApiUsername;
            clientProfile.APIPassword = this.ApiPassword;
            clientProfile.APISignature = this.ApiSignature;
            clientProfile.Environment = this.UseSandbox ? "sandbox" : "live";
            WebTrace.Write("ApiUsername: " + clientProfile.APIUsername);
            WebTrace.Write("ApiPassword: " + clientProfile.APIPassword);
            WebTrace.Write("ApiSignature: " + clientProfile.APISignature);
            WebTrace.Write("Environment: " + clientProfile.Environment);
            WebTrace.Write("request: " + request.ToString());
            //USE SDK API SERVICES
            CallerServices apiClient = new CallerServices();
            apiClient.APIProfile = clientProfile;
            //MAKE THE SOAP CALL AND RETURN RESULT
            return apiClient.Call(methodName, request);
        }

        public class ExpressCheckoutResult
        {
            private int _OrderId;
            private string _RedirectUrl;
            private ErrorType[] _Errors;

            public string RedirectUrl
            {
                get { return _RedirectUrl; }
            }

            public ErrorType[] Errors
            {
                get
                {
                    return _Errors;
                }
            }

            public int OrderId
            {
                get
                {
                    return _OrderId;
                }
            }

            internal ExpressCheckoutResult(int orderId, string redirectUrl, ErrorType[] errors)
            {
                this._OrderId = orderId;
                this._RedirectUrl = redirectUrl;
                this._Errors = errors;
            }
        }

    }
}
