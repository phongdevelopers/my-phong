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
using PayPalWCF.PayPalApi;
using System.Xml;
namespace CommerceBuilder.Payments.Providers.PayPal
{
    /// <summary>
    /// Express Checkout Implementation
    /// </summary>
    public partial class PayPalProvider
    {
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
            CurrencyCodeType baseCurrency = PayPalProvider.GetPayPalCurrencyType(baseCurrencyCode);
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
            if (billingAddress.Country != null) address.Country = PayPalProvider.GetPayPalCountry(billingAddress.CountryCode);
            else address.Country = CountryCodeType.US;
            address.CountrySpecified = true;
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
            CurrencyCodeType baseCurrencyCode = PayPalProvider.GetPayPalCurrencyType(storeCurrencyCode);

            //CREATE THE EXPRESS CHECKOUT
            DoExpressCheckoutPaymentRequestType expressCheckoutRequest = new DoExpressCheckoutPaymentRequestType();
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.Token = paypalSession.Token;
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentAction = this.UseAuthCapture ? PaymentActionCodeType.Sale : PaymentActionCodeType.Authorization;
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentActionSpecified = true;
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
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails = new PaymentDetailsType[1];
            expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails[0] = new PaymentDetailsType();
            PaymentDetailsType paymentDetails = expressCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails[0];
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
                    PaymentInfoType paymentInfo = expressCheckoutResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0];
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
