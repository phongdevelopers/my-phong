using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Payments.Providers.GoogleCheckout.OrderProcessing;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class AcNotifier
    {
        public static Transaction RefundOrder(GoogleCheckout instance, RefundTransactionRequest refundRequest)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;
            string orderNum = refundRequest.Payment.Order.GoogleOrderNumber;
            string currency = refundRequest.Payment.CurrencyCode;
            if (currency == null || currency.Length == 0)
            {
                currency = "USD";
            }            
            LSDecimal amount = refundRequest.Amount;
            LSDecimal totalCharged = refundRequest.Payment.Transactions.GetTotalCaptured();

            RefundOrderRequest request = new RefundOrderRequest(merchantId, merchantKey, env, orderNum, "Refund Requested",currency,amount,"");
            Util.GCheckoutResponse response = request.Send();

            Transaction transaction = new Transaction();
            transaction.Amount = refundRequest.Amount;
            transaction.ProviderTransactionId = orderNum;
            transaction.PaymentGatewayId = instance.PaymentGatewayId;

            if (totalCharged > amount)
            {
                transaction.TransactionType = TransactionType.PartialRefund;
            }
            else
            {
                transaction.TransactionType = TransactionType.Refund;
            }

            if (response.IsGood)
            {                
                transaction.TransactionStatus = TransactionStatus.Pending;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = response.ErrorMessage;
            }

            return transaction;
        }

        public static Transaction ChargeOrder(GoogleCheckout instance, CaptureTransactionRequest captureRequest)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;
            string orderNum = captureRequest.Payment.Order.GoogleOrderNumber;
            string currency = captureRequest.Payment.CurrencyCode;
            if (currency == null || currency.Length == 0)
            {
                currency = "USD";
            }
            LSDecimal amount = captureRequest.Amount;
            LSDecimal remainingCapture = captureRequest.Payment.Transactions.GetTotalAuthorized() 
                                        - captureRequest.Payment.Transactions.GetTotalCaptured();

            ChargeOrderRequest request = new ChargeOrderRequest(merchantId, merchantKey, env, orderNum,currency,amount);
            Util.GCheckoutResponse response = request.Send();

            Transaction transaction = new Transaction();
            transaction.Amount = captureRequest.Amount;
            transaction.ProviderTransactionId = orderNum;
            transaction.PaymentGatewayId = instance.PaymentGatewayId;
            if (remainingCapture > amount)
            {
                transaction.TransactionType = TransactionType.PartialCapture;
            }
            else
            {
                transaction.TransactionType = TransactionType.Capture;
            }

            if (response.IsGood)
            {                
                transaction.TransactionStatus = TransactionStatus.Pending;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseMessage = response.ErrorMessage;                
            }
            
            return transaction;
        }

        public static void CancelOrder(GoogleCheckout instance, string googleOrderNumber, string reason)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;
            //LSDecimal totalAuthorized 

            CancelOrderRequest request = new CancelOrderRequest(merchantId, merchantKey, env, googleOrderNumber, reason);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Cancel Order Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Cancel Order Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }

        public static void AddMerchantOrderNumber(GoogleCheckout instance, string googleOrderNumber, string acOrderNumber)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            AddMerchantOrderNumberRequest onReq =
               new AddMerchantOrderNumberRequest(merchantId, merchantKey, env, googleOrderNumber, acOrderNumber);

            Util.GCheckoutResponse resp = onReq.Send();

            if (resp.IsGood)
            {
                Utility.Logger.Debug("Add Merchant Order Number Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber
                    + " AC OrderNumber=" + acOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Add Merchant Order Number Request could not be initiated. ErrorMessage=" + resp.ErrorMessage);
            }
        }

        public static void SendBuyerMessage(GoogleCheckout instance, string googleOrderNumber, string message)
        {
            SendBuyerMessage(instance, googleOrderNumber, message, false);
        }

        public static void SendBuyerMessage(GoogleCheckout instance, string googleOrderNumber, string message, bool sendEmail)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            SendBuyerMessageRequest request = new SendBuyerMessageRequest(merchantId, merchantKey, env, googleOrderNumber, message, sendEmail);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Send Buyer Message Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Send Buyer Message Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }

        public static void ArchiveOrder(GoogleCheckout instance, string googleOrderNumber)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            ArchiveOrderRequest request = new ArchiveOrderRequest(merchantId, merchantKey, env, googleOrderNumber);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Archive Order Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Archive Order Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }

        public static void UnarchiveOrder(GoogleCheckout instance, string googleOrderNumber)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            UnarchiveOrderRequest request = new UnarchiveOrderRequest(merchantId, merchantKey, env, googleOrderNumber);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Unarchive Order Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Unarchive Order Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }

        public static void ProcessOrder(GoogleCheckout instance, string googleOrderNumber)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            ProcessOrderRequest request = new ProcessOrderRequest(merchantId, merchantKey, env, googleOrderNumber);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Process Order Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Process Order Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }


        public static void DeliverOrder(GoogleCheckout instance, string googleOrderNumber)
        {
            DeliverOrder(instance, googleOrderNumber, null, null, true);
        }

        public static void DeliverOrder(GoogleCheckout instance, string googleOrderNumber, bool sendEmail)
        {
            DeliverOrder(instance, googleOrderNumber, null, null, sendEmail);
        }

        public static void DeliverOrder(GoogleCheckout instance, string googleOrderNumber, string carrier, string trackingNo)
        {
            DeliverOrder(instance, googleOrderNumber, carrier, trackingNo, true);
        }

        public static void DeliverOrder(GoogleCheckout instance, string googleOrderNumber, string carrier, string trackingNo, bool sendEmail)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            DeliverOrderRequest request = new DeliverOrderRequest(merchantId, merchantKey, env, googleOrderNumber, carrier, trackingNo, sendEmail);
            Util.GCheckoutResponse response = request.Send();

            if (response.IsGood)
            {
                Utility.Logger.Debug("Deliver Order Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);
            }
            else
            {
                Utility.Logger.Debug("Deliver Order Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }

        public static void AddTrackingData(GoogleCheckout instance, string googleOrderNumber, string carrier, string trackingNo)
        {
            string env = instance.UseTestMode ? "Sandbox" : "Production";
            string merchantId = instance.MerchantID;
            string merchantKey = instance.MerchantKey;

            AddTrackingDataRequest request = new AddTrackingDataRequest(merchantId, merchantKey, env, googleOrderNumber, carrier, trackingNo);
            Util.GCheckoutResponse response = request.Send();            

            if (response.IsGood)
            {
                Utility.Logger.Debug("Add Tracking Data Request initiated successfuly. GoogleOrderNumber=" + googleOrderNumber);                
            }
            else
            {
                Utility.Logger.Warn("Add Tracking Data Request could not be initiated. ErrorMessage=" + response.ErrorMessage);
            }
        }
        
    }
}
