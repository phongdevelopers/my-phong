using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class ChargeAmountHandler
    {
        private ChargeAmountNotification N1;
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

        public ChargeAmountHandler(ChargeAmountNotification n1)
        {
            this.N1 = n1;
        }

        public void Process()
        {
            string googleOrderNum = N1.googleordernumber;
            Order order = OrderDataSource.LoadForGoogleOrderNumber(googleOrderNum);
            if (order == null)
            {
                Logger.Warn("Unknown Google Order Number Charged. GoogleOrderNumber=" + googleOrderNum +
                    ". Amount=" + N1.latestchargeamount.Value);
            }
            else
            {
                Payment payment = AcHelper.GetGCPayment(order, GatewayInstance, true);
                Transaction trans = payment.Transactions.LastCapturePending;
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
                //trans.TransactionDate = N1.timestamp;
                trans.Amount = N1.latestchargeamount.Value;
                trans.PaymentGatewayId = GatewayInstance.PaymentGatewayId;
                trans.ProviderTransactionId = googleOrderNum;

                LSDecimal totalAuth = payment.Transactions.GetTotalAuthorized();
                LSDecimal totalCapt = payment.Transactions.GetTotalCaptured();
                LSDecimal remainCapt = totalAuth - totalCapt;

                trans.TransactionType = TransactionType.PartialCapture;
                if (remainCapt > trans.Amount)
                {
                    trans.TransactionType = TransactionType.PartialCapture;
                }
                else
                {
                    trans.TransactionType = TransactionType.Capture;
                }
                
                if (payment.PaymentStatus == PaymentStatus.CapturePending)
                {
                    PaymentEngine.ProcessCapturePending(payment, trans);
                }
                else
                {
                    PaymentEngine.ForceTransaction(payment, trans);
                }

            }

        }

    }
}
