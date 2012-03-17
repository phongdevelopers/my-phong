using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;
using System.Collections.Specialized;

//IMPORT COMMERCEBUILDER NAMESPACES
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.PayPal
{
    public class IpnProcessor : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        private Payment FindPaypalPayment(int paypalMethodId, PaymentCollection orderPayments, PaymentStatus[] validStatuses)
        {
            //FIND THE MOST RECENT PAYMENT THAT MATCHES ONE OF THE GIVEN STATUS
            int i = (orderPayments.Count - 1);
            while (i >= 0)
            {
                Payment payment = orderPayments[i];
                if ((payment.PaymentMethodId == paypalMethodId) && (Array.IndexOf(validStatuses, payment.PaymentStatus) > -1))
                    return payment;
                i--;
            }
            return null;
        }

        private Payment FindPayPalPayment(int paypalGatewayId, PaymentCollection orderPayments, string transactionId)
        {
            //FIND THE MOST RECENT PAYMENT THAT MATCHES ONE OF THE GIVEN TRANSACTIONID
            if (paypalGatewayId > 0)
            {
                int i = (orderPayments.Count - 1);
                while (i >= 0)
                {
                    Payment payment = orderPayments[i];
                    if ((payment.PaymentMethod != null) && (payment.PaymentMethod.PaymentGatewayId == paypalGatewayId))
                    {
                        //THIS IS A PAYPAL PAYMENT, CHECK THE TRANSACTIONS
                        foreach (Transaction transaction in payment.Transactions)
                        {
                            if (transaction.ProviderTransactionId == transactionId) return payment;
                        }
                    }
                    i--;
                }
            }
            return null;
        }

        private int FindPayPalOrderId(int paypalGatewayId, string transactionId)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                TransactionCollection parentTransactions = TransactionDataSource.LoadForProviderTransaction(paypalGatewayId, transactionId);
                if ((parentTransactions != null) && (parentTransactions.Count > 0))
                {
                    return parentTransactions[0].Payment.OrderId;
                }
            }
            return 0;
        }

        public void ProcessRequest(HttpContext context)
        {
            //GET REFERENCE TO REQUEST
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            TraceContext trace = context.Trace;
            //RECORD FORM VALUES TO TRACE OUTPUT
            foreach (string key in request.Form) context.Trace.Write(key + ":" + request.Form[key]);
            //SETUP DEFAULT REDIRECT URL
            string redirectUrl = "~/Default.aspx";
            //INITIALIZE THE PAYPAL PROVIDER
            PaymentGateway paypalGateway = PayPalProvider.GetPayPalPaymentGateway(true);
            if (paypalGateway == null) response.Redirect(redirectUrl);
            //LOOK FOR ORDER ID
            int orderId;
            string customValue = request.Form["custom"];
            if (!String.IsNullOrEmpty(customValue))
            {
                int index = customValue.IndexOf(":");
                if (index > 0) orderId = AlwaysConvert.ToInt(customValue.Substring(0, index));
                else orderId = AlwaysConvert.ToInt(customValue);
            }
            else
            {
                // TRY TO LOCATE ORDER ID AS QUERY STRING PARAMETER
                orderId = AlwaysConvert.ToInt(request.QueryString["OrderId"]);
            }

            //IF ORDER ID WAS NOT IN CUSTOM, SEE IF WE CAN FIND THE ORDER VIA TRANSACTION ID
            if (orderId == 0)
            {
                trace.Write("OrderId not found in custom field; lookup via transaction ID");
                string parentTransactionId = IpnProcessor.GetFormValue(request.Form, "parent_txn_id");
                if (!string.IsNullOrEmpty(parentTransactionId) && (paypalGateway != null))
                {
                    trace.Write("Query for parent transaction " + parentTransactionId);
                    orderId = FindPayPalOrderId(paypalGateway.PaymentGatewayId, parentTransactionId);
                    if (orderId != 0)
                    {
                        trace.Write("Order ID Found: " + orderId.ToString());
                    }
                }
            }
            //TRY TO LOAD ORDER
            Order order = OrderDataSource.Load(orderId);
            //IF ORDER LOAD FAILS, STOP PROCESSING AND REDIRECT
            if (order == null) response.Redirect(redirectUrl);
            //ORDER LOAD SUCCESSFUL, UPDATE DEFAULT REDIRECT URL
            redirectUrl = "~/Members/MyOrder.aspx?OrderId=" + orderId.ToString();
            //IF GATEWAY NOT FOUND, STOP PROCESSING AND REDIRECT
            PayPalProvider provider = (PayPalProvider)paypalGateway.GetInstance();
            //GET TRANSACTION AMOUNT
            LSDecimal curSignedPayment = AlwaysConvert.ToDecimal(request.Form["mc_gross"]);
            LSDecimal curPayment = Math.Abs((Decimal)curSignedPayment);
            context.Trace.Write("Transaction Amount is " + curPayment.ToString());
            if (curPayment != 0)
            {
                //VERIFY PAYMENT NOTIFICATION WITH PAYPAL
                bool valid = provider.ValidateNotification(request.Form.ToString());
                if (!valid) response.Redirect(redirectUrl);
                //VERIFY THE RECEIVER EMAIL
                string lowerReceiverEmail = AlwaysConvert.ToString(request.Form["receiver_email"]).ToLowerInvariant();
                string lowerProviderAccount = provider.PayPalAccount.ToLowerInvariant();
                if (lowerReceiverEmail != lowerProviderAccount)
                {
                    context.Trace.Write("Receiver Email (" + lowerReceiverEmail + ") does not match Primary Account (" + lowerProviderAccount + ")");
                    response.Redirect(redirectUrl);
                }
                //CHECK WHETHER TRANSACTION IS ALREADY PRESENT IN DATABASE
                string paypalTransactionId = IpnProcessor.GetFormValue(request.Form, "txn_id");
                string authTransactionId = IpnProcessor.GetFormValue(request.Form, "auth_id");
                string paymentStatus = IpnProcessor.GetFormValue(request.Form, "payment_status").ToUpperInvariant();
                string authStatus = IpnProcessor.GetFormValue(request.Form, "auth_status").ToUpperInvariant();
                context.Trace.Write("Transaction ID Is " + paypalTransactionId);
                context.Trace.Write("Payment Status Is " + paymentStatus);
                context.Trace.Write("Auth Status Is " + authStatus);

                //CHECK FOR THIS PAYPAL TRANSACTION (MATCHING PROVIDER, PAYPAL TRANSACTION ID, AND PAYMENT STATUS)
                Payment payment = null;
                Transaction pendingTransaction = null;
                if (!string.IsNullOrEmpty(paypalTransactionId))
                {
                    TransactionCollection matchingTransactions = TransactionDataSource.LoadForProviderTransaction(paypalGateway.PaymentGatewayId, paypalTransactionId);
                    foreach (Transaction tx in matchingTransactions)
                    {
                        //WHEN PAYMENT IS BY ECHECK, IPN ISSUES A PENDING TRANSACTION
                        //SECOND IPN COMES FOR COMPLETED STATUS USING SAME TRANSACTION ID
                        if ((tx.ResponseCode == "PENDING") && (paymentStatus != "PENDING"))
                        {
                            //WE HAVE TO GET THE TRANSACTION VIA THE PAYMENT OBJECT
                            //OTHERWISE WE WILL HAVE PROBLEMS WITH DATA CONSISTENCY LATER
                            payment = tx.Payment;
                            foreach (Transaction ptx in payment.Transactions)
                            {
                                if (ptx.TransactionId == tx.TransactionId)
                                    pendingTransaction = ptx;
                            }
                        }
                        else if ((tx.TransactionType != TransactionType.Void) && (paymentStatus == "VOIDED"))
                        {
                            //IF WE VOID AN AUTHORIZATION, THE VOID HAS THE SAME TXID
                            //THE AUTHORIZATION WILL HAVE A BLANK RESPONSE CODE
                            //THE VOID SHOULD HAVE A 'VOIDED' RESPONSE CODE
                            //(THIS TRANSACTION IS NOT A MATCH AND SHOULD BE IGNORED)
                        }
                        else
                        {
                            //NO FURTHER PROCESSING, REDIR TO ORDER SCREEN
                            context.Trace.Write("Transaction ID " + paypalTransactionId + " Already Exists in Database");
                            response.Redirect(redirectUrl);
                        }
                    }
                }

                Transaction transaction = null;
                PaymentMethod paypalMethod = PayPalProvider.GetPayPalPaymentMethod(true);
                PaymentCollection orderPayments = order.Payments;
                Transaction authTransaction = null;
                PaymentStatus[] validAuthStatuses = { PaymentStatus.Unprocessed, PaymentStatus.AuthorizationPending };
                context.Trace.Write("Processing Payment Status: " + paymentStatus);
                switch (paymentStatus)
                {
                    case "PENDING":
                        //THIS IS A PENDING TRANSACTION, GET PENDING REASON AND FIND OUT IF IT IS AN ECHECK WAITING TO CLEAR
                        string pendingReason = IpnProcessor.GetFormValue(request.Form, "pending_reason").ToLowerInvariant();
                        bool isPendingeCheck = (pendingReason == "echeck");
                        bool isPendingAuthorization = (pendingReason == "authorization");
                        context.Trace.Write("Pending Reason: " + pendingReason);
                        context.Trace.Write("Is Pending eCheck: " + isPendingeCheck.ToString());
                        context.Trace.Write("Is Pending Authorization: " + isPendingAuthorization.ToString());
                        //FIND THE PAYPAL PAYMENT THAT IS UNPROCESSED OR PENDING AUTHORIZATION
                        payment = FindPaypalPayment(paypalMethod.PaymentMethodId, orderPayments, validAuthStatuses);
                        if (payment != null)
                        {
                            //SEE IF WE CAN FIND A PENDING PAYPAL TRANSACTION WITHOUT A TXID
                            foreach (Transaction tx in payment.Transactions)
                            {
                                if ((tx.ResponseCode == "PENDING") && string.IsNullOrEmpty(tx.ProviderTransactionId))
                                    transaction = tx;
                            }
                        }
                        //IF WE DID NOT FIND AN EXISTING TRANSACTION, CREATE A NEW ONE
                        if (transaction == null) transaction = new Transaction();
                        //UPDATE THE TRANSACTION VALUES
                        transaction.TransactionType = (isPendingeCheck ? TransactionType.Capture : TransactionType.Authorize);
                        transaction.PaymentGatewayId = paypalGateway.PaymentGatewayId;
                        transaction.ProviderTransactionId = IpnProcessor.GetFormValue(request.Form, "txn_id");
                        transaction.TransactionDate = AlwaysConvert.ToDateTime(request.Form["payment_date"], DateTime.UtcNow).ToUniversalTime();
                        transaction.Amount = AlwaysConvert.ToDecimal(IpnProcessor.GetFormValue(request.Form, "mc_gross"));
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        if (isPendingAuthorization)
                        {
                            //THIS IS AN EXPECTED RESPONSE, NO NEED TO SAVE THE REASON CODES
                            transaction.ResponseMessage = string.Empty;
                            transaction.ResponseCode = string.Empty;
                        }
                        else
                        {
                            transaction.ResponseMessage = pendingReason;
                            transaction.ResponseCode = "PENDING";
                        }
                        transaction.AuthorizationCode = IpnProcessor.GetFormValue(request.Form, "auth_id");
                        transaction.RemoteIP = IpnProcessor.GetFormValue(request.ServerVariables, "REMOTE_ADDR");
                        transaction.Referrer = IpnProcessor.GetFormValue(request.ServerVariables, "HTTP_REFERER");
                        //CREATE A PAYMENT IF AN EXISTING ONE WAS NOT FOUND
                        if (payment == null)
                        {
                            payment = new Payment();
                            payment.OrderId = orderId;
                            payment.PaymentMethodId = paypalMethod.PaymentMethodId;
                            payment.PaymentMethodName = paypalMethod.Name;
                            order.Payments.Add(payment);
                        }
                        //UPDATE PAYMENT DETAILS
                        payment.ReferenceNumber = IpnProcessor.GetFormValue(request.Form, "payer_email");
                        payment.Amount = transaction.Amount;
                        payment.PaymentDate = transaction.TransactionDate;
                        if (isPendingAuthorization)
                        {
                            payment.PaymentStatus = PaymentStatus.Authorized;
                            payment.PaymentStatusReason = string.Empty;
                        }
                        else
                        {
                            payment.PaymentStatus = (isPendingeCheck ? PaymentStatus.CapturePending : PaymentStatus.AuthorizationPending);
                            payment.PaymentStatusReason = transaction.ResponseMessage;
                        }
                        //ADD IN TRANSACTION
                        payment.Transactions.Add(transaction);
                        break;

                    case "COMPLETED":
                        //IF THIS IS A CAPTURE FROM AN AUTHORIZATION, FIND THE AUTHORIZATION TRANSACTION
                        //AND UPDATE THE STATUS ACCORDINGLY, DEPENDING ON WHETHER ADDITIONAL SETTLEMENT TRANSACTIONS REMAIN (INTREMSETTLE > 0)
                        authTransaction = null;
                        authTransactionId = IpnProcessor.GetFormValue(request.Form, "auth_id");
                        if (!string.IsNullOrEmpty(authTransactionId))
                        {
                            TransactionCollection matchingTransactions = TransactionDataSource.LoadForProviderTransaction(paypalGateway.PaymentGatewayId, authTransactionId);
                            //SHOULD ONLY BE ONE
                            if (matchingTransactions.Count > 0) authTransaction = matchingTransactions[0];
                        }
                        //IF PAYPAL IS RUNNING IN CAPTURE MODE, WE MAY HAVE A COMPLETED PAYMENT
                        //WITH A PENDING OR UNPROCESSED PAYMENT ALREADY ASSOCIATED TO ORDER
                        if (pendingTransaction == null)
                        {
                            //FIND THE PAYPAL PAYMENT THAT IS UNPROCESSED OR PENDING
                            payment = FindPaypalPayment(paypalMethod.PaymentMethodId, orderPayments, validAuthStatuses);
                            if (payment != null)
                            {
                                //SEE IF WE CAN FIND A PENDING PAYPAL TRANSACTION WITHOUT A TXID
                                foreach (Transaction tx in payment.Transactions)
                                {
                                    if ((tx.ResponseCode == "PENDING") && string.IsNullOrEmpty(tx.ProviderTransactionId))
                                        pendingTransaction = tx;
                                }
                            }
                        }
                        //SEE IF THIS TRANSACTION WAS PENDING (SUCH AS A CHECK WAITING TO CLEAR)
                        if (pendingTransaction != null)
                        {
                            //GET THE PENDING TRANSACTION AND PAYMENT
                            payment = order.Payments[order.Payments.IndexOf(pendingTransaction.PaymentId)];
                            transaction = payment.Transactions[payment.Transactions.IndexOf(pendingTransaction.TransactionId)];
                        }
                        else
                        {
                            //THIS IS NOT A PENDING TRANSACTION
                            //LOCATE THE APPROPRIATE PAYMENT
                            if (authTransaction != null)
                            {
                                payment = order.Payments[order.Payments.IndexOf(authTransaction.PaymentId)];
                            }
                            else
                            {
                                //FIND THE PAYPAL PAYMENT THAT CAN BE CAPTURED
                                PaymentStatus[] validCaptureStatuses = { PaymentStatus.Unprocessed, PaymentStatus.AuthorizationPending, PaymentStatus.Authorized, PaymentStatus.CaptureFailed, PaymentStatus.CapturePending };
                                payment = FindPaypalPayment(paypalMethod.PaymentMethodId, orderPayments, validCaptureStatuses);
                                //CREATE A PAYMENT IF AN EXISTING ONE WAS NOT FOUND
                                if (payment == null)
                                {
                                    payment = new Payment();
                                    payment.OrderId = orderId;
                                    payment.PaymentMethodId = paypalMethod.PaymentMethodId;
                                    payment.PaymentMethodName = paypalMethod.Name;
                                    order.Payments.Add(payment);
                                }
                            }
                            //CREATE A NEW TRANSACTION RECORD
                            transaction = new Transaction();
                            transaction.PaymentId = payment.PaymentId;
                        }
                        //UPDATE THE TRANSACTION DETAILS
                        transaction.TransactionType = TransactionType.Capture;
                        transaction.PaymentGatewayId = paypalGateway.PaymentGatewayId;
                        transaction.TransactionDate = AlwaysConvert.ToDateTime(request.Form["payment_date"], DateTime.UtcNow).ToUniversalTime();
                        transaction.RemoteIP = IpnProcessor.GetFormValue(request.ServerVariables, "REMOTE_ADDR");
                        transaction.Referrer = IpnProcessor.GetFormValue(request.ServerVariables, "HTTP_REFERER");
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        transaction.ProviderTransactionId = IpnProcessor.GetFormValue(request.Form, "txn_id");
                        transaction.AuthorizationCode = IpnProcessor.GetFormValue(request.Form, "auth_id");
                        transaction.Amount = AlwaysConvert.ToDecimal(IpnProcessor.GetFormValue(request.Form, "mc_gross"));
                        transaction.ResponseCode = paymentStatus;
                        transaction.ResponseMessage = string.Empty;

                        //HANDLE PARTIAL / FINAL CAPTURES
                        int remainingSettle = AlwaysConvert.ToInt(IpnProcessor.GetFormValue(request.Form, "remaining_settle"));
                        if (remainingSettle == 0)
                        {
                            //THIS IS A FINAL CAPTURE
                            transaction.TransactionType = TransactionType.Capture;
                            //SET PAYMENT AMOUNT TO SUM OF ALL CAPTURES
                            LSDecimal totalCaptures = 0;
                            foreach (Transaction tx in payment.Transactions)
                            {
                                if ((transaction.TransactionId != tx.TransactionId) &&
                                    (tx.TransactionType == TransactionType.PartialCapture || tx.TransactionType == TransactionType.Capture))
                                    totalCaptures += tx.Amount;
                            }
                            totalCaptures += transaction.Amount;
                            payment.Amount = totalCaptures;
                        }
                        else
                        {
                            //THIS IS A PARTIAL CAPTURE
                            transaction.TransactionType = TransactionType.PartialCapture;
                            //LEAVE PAYMENT AMOUNT ALONE (AMOUNT OF AUTHORIZATION)
                        }

                        //UPDATE PAYMENT DETAILS
                        payment.PaymentDate = transaction.TransactionDate;
                        payment.PaymentStatus = (remainingSettle == 0) ? PaymentStatus.Captured : PaymentStatus.Authorized;
                        payment.PaymentStatusReason = string.Empty;

                        //ADD IN TRANSACTION IF NEEDED
                        if (transaction.TransactionId == 0)
                        {
                            payment.Transactions.Add(transaction);
                        }
                        break;

                    case "REFUNDED":
                    case "REVERSED":
                        //GET THE REFUND AMOUNT
                        LSDecimal refundAmount = Math.Abs(AlwaysConvert.ToDecimal(IpnProcessor.GetFormValue(request.Form, "mc_gross")));
                        //TRY TO LOCATE THE CORRECT PAYMENT BASED ON CAPTURE TRANSACITON ID
                        payment = FindPayPalPayment(paypalGateway.PaymentGatewayId, orderPayments, IpnProcessor.GetFormValue(request.Form, "parent_txn_id"));
                        if (payment == null)
                        {
                            //SEE IF WE CAN FIND THE PAYMENT VIA AUTH TRANSACTION ID
                            payment = FindPayPalPayment(paypalGateway.PaymentGatewayId, orderPayments, IpnProcessor.GetFormValue(request.Form, "auth_id"));
                        }
                        //CREATE A REFUND TRANSACTION
                        transaction = new Transaction();
                        //CREATE A PAYMENT IF AN EXISTING ONE WAS NOT FOUND
                        if (payment == null)
                        {
                            payment = new Payment();
                            payment.OrderId = orderId;
                            payment.PaymentMethodId = paypalMethod.PaymentMethodId;
                            payment.PaymentMethodName = paypalMethod.Name;
                            payment.Amount = -1 * refundAmount;
                            transaction.TransactionType = TransactionType.Refund;
                            order.Payments.Add(payment);
                        }
                        else
                        {
                            if (payment.Amount == refundAmount)
                            {
                                //FULL REFUND
                                transaction.TransactionType = TransactionType.Refund;
                                payment.PaymentStatus = PaymentStatus.Refunded;
                            }
                            else
                            {
                                //PARTIAL REFUND
                                transaction.TransactionType = TransactionType.PartialRefund;
                                payment.Amount -= refundAmount;
                                payment.PaymentStatus = PaymentStatus.Captured;
                            }
                        }
                        transaction.PaymentGatewayId = paypalGateway.PaymentGatewayId;
                        transaction.ProviderTransactionId = IpnProcessor.GetFormValue(request.Form, "txn_id");
                        transaction.TransactionDate = AlwaysConvert.ToDateTime(request.Form["payment_date"], DateTime.UtcNow).ToUniversalTime();
                        transaction.TransactionStatus = TransactionStatus.Successful;
                        transaction.AuthorizationCode = IpnProcessor.GetFormValue(request.Form, "auth_id");
                        transaction.RemoteIP = IpnProcessor.GetFormValue(request.ServerVariables, "REMOTE_ADDR");
                        transaction.Referrer = IpnProcessor.GetFormValue(request.ServerVariables, "HTTP_REFERER");
                        transaction.Amount = refundAmount;
                        string responseMessage = IpnProcessor.GetFormValue(request.Form, "reason_code");
                        if (responseMessage != "refund")
                        {
                            transaction.ResponseCode = paymentStatus;
                            transaction.ResponseMessage = responseMessage;
                        }
                        //UPDATE PAYMENT DETAILS
                        payment.PaymentDate = transaction.TransactionDate;
                        payment.PaymentStatusReason = string.Empty;
                        //ADD IN TRANSACTION
                        payment.Transactions.Add(transaction);
                        break;

                    case "VOIDED":
                        //SEE IF WE CAN FIND THE PAYMENT VIA AUTH TRANSACTION ID
                        payment = FindPayPalPayment(paypalGateway.PaymentGatewayId, orderPayments, IpnProcessor.GetFormValue(request.Form, "auth_id"));
                        //WE ONLY NEED TO CONTINUE IF A PAYMENT TO VOID WAS FOUND
                        if (payment != null)
                        {
                            //PAYPAL DOES NOT SEND THE AMOUNT OF THE VOID
                            //SO IF THIS PAYMENT WAS PARTIALLY CAPTURED, WE NEED TO KNOW HOW MUCH TO VOID
                            LSDecimal remainingAuthorization = payment.Transactions.GetRemainingAuthorized();
                            if (remainingAuthorization > 0)
                            {
                                //CREATE A VOID TRANSACTION
                                transaction = new Transaction();
                                transaction.TransactionType = TransactionType.Void;
                                transaction.Amount = remainingAuthorization;
                                transaction.PaymentGatewayId = paypalGateway.PaymentGatewayId;
                                transaction.ProviderTransactionId = IpnProcessor.GetFormValue(request.Form, "txn_id");
                                transaction.TransactionDate = AlwaysConvert.ToDateTime(request.Form["payment_date"], DateTime.UtcNow).ToUniversalTime();
                                transaction.TransactionStatus = TransactionStatus.Successful;
                                transaction.AuthorizationCode = IpnProcessor.GetFormValue(request.Form, "auth_id");
                                transaction.RemoteIP = IpnProcessor.GetFormValue(request.ServerVariables, "REMOTE_ADDR");
                                transaction.Referrer = IpnProcessor.GetFormValue(request.ServerVariables, "HTTP_REFERER");
                                //UPDATE PAYMENT DETAILS
                                payment.PaymentDate = transaction.TransactionDate;
                                payment.PaymentStatusReason = string.Empty;
                                if (payment.Amount == remainingAuthorization)
                                {
                                    //FULL VOID, CHANGE PAYMENT STATUS TO VOID
                                    payment.PaymentStatus = PaymentStatus.Void;
                                }
                                else
                                {
                                    //PARTIAL VOID, REDUCE PAYMENT AMOUNT BY VOID
                                    payment.Amount -= remainingAuthorization;
                                    //PAYMENT HAS NO REMAINING AUTHORIZATION AND SO IT IS CAPTURED
                                    payment.PaymentStatus = PaymentStatus.Captured;
                                }
                                //ADD IN TRANSACTION
                                payment.Transactions.Add(transaction);
                            }
                        }
                        break;

                    case "FAILED":
                        //THIS IS A FAILED E-CHECK
                        //PENDINGTRANSACTION SHOULD HAVE BEEN OBTAINED ABOVE
                        if (payment != null && pendingTransaction != null)
                        {
                            pendingTransaction.TransactionStatus = TransactionStatus.Failed;
                            //MAKE SURE TO CLEAR OUT PENDING RESPONSECODE
                            pendingTransaction.ResponseCode = string.Empty;
                            //GET THE CURRENT TRANSACTION DATE
                            pendingTransaction.TransactionDate = AlwaysConvert.ToDateTime(request.Form["payment_date"], DateTime.UtcNow).ToUniversalTime();
                            //UPDATE PAYMENT DETAILS
                            payment.PaymentDate = pendingTransaction.TransactionDate;
                            payment.PaymentStatus = (IsVoidableFailure(payment) ? PaymentStatus.Void : PaymentStatus.CaptureFailed);
                            payment.PaymentStatusReason = string.Empty;
                            //SAVE PAYMENT (AND CHILD TRANSACTIONS)
                            payment.Save();
                        }
                        break;
                    default:
                        Logger.Warn("PayPal IPN transaction " + paypalTransactionId + " with a \"" + paymentStatus + "\" status was unhandled.");
                        break;
                }

                //IF PAYMENT IS SET, SAVE UPDATES
                if (payment != null) payment.Save();
            }
            response.Redirect(redirectUrl);
        }

        private bool IsVoidableFailure(Payment payment)
        {
            foreach (Transaction tx in payment.Transactions)
            {
                if (tx.TransactionStatus != TransactionStatus.Failed) return false;
            }
            return true;
        }

        #endregion

        private static string GetFormValue(NameValueCollection form, string key)
        {
            object value = form[key];
            if (value == null) return string.Empty;
            return (string)value;
        }
    }
}