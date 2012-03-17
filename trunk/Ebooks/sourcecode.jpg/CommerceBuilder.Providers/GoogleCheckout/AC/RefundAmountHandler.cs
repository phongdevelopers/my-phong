using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class RefundAmountHandler
    {
        private RefundAmountNotification N1;
        private GoogleCheckout _GatewayInstance = null;
        private GoogleCheckout GatewayInstance
        {
            get
            {
                if (_GatewayInstance != null)
                {
                    return _GatewayInstance;
                }
                else
                {
                    _GatewayInstance = GoogleCheckout.GetInstance();
                    return _GatewayInstance;
                }
            }
        }

        public RefundAmountHandler(RefundAmountNotification n1)
        {
            this.N1 = n1;
        }

        public void Process()
        {
            // Google has successfully refunded the customer's credit card.
            string googleOrderNum = N1.googleordernumber;
            Order order = OrderDataSource.LoadForGoogleOrderNumber(googleOrderNum);
            if (order == null)
            {
                Logger.Warn("Unknown Google Order Number Refunded. GoogleOrderNumber=" + googleOrderNum +
                    ". Amount=" + N1.latestrefundamount.Value);
            }
            else
            {
                Payment payment = AcHelper.GetGCPayment(order, GatewayInstance, true);
                Transaction trans = payment.Transactions.LastRefundPending;
                if (trans == null)
                {
                    trans = new Transaction();
                }
                else
                {
                    //remove the transaction from collection for correct calculations
                    payment.Transactions.Remove(trans);
                }
                trans.TransactionStatus = TransactionStatus.Successful;
                trans.PaymentGatewayId = GatewayInstance.PaymentGatewayId;

                //LSDecimal totalRefunded = payment.Transactions.GetTotalRefunded();
                //trans.Amount = N1.totalrefundamount.Value - totalRefunded;
                trans.Amount = N1.latestrefundamount.Value;
                trans.ProviderTransactionId = N1.googleordernumber;

                LSDecimal totalCharged = payment.Transactions.GetTotalCaptured();
                if (totalCharged > trans.Amount)
                {
                    trans.TransactionType = TransactionType.PartialRefund;
                }
                else
                {
                    trans.TransactionType = TransactionType.Refund;
                }

                if (payment.PaymentStatus == PaymentStatus.RefundPending)
                {
                    PaymentEngine.ProcessRefundPending(payment, trans);
                }
                else
                {
                    PaymentEngine.ForceTransaction(payment, trans);
                }
            }
        }
    }
}
