using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class ChargebackAmountHandler
    {
        private ChargebackAmountNotification N1;
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

        public ChargebackAmountHandler(ChargebackAmountNotification n1)
        {
            this.N1 = n1;
        }

        public void Process()
        {
            //This state has no proper representation in AC.
            //It is roughly equivalant to RefundPending            
            string googleOrderNum = N1.googleordernumber;
            Order order = OrderDataSource.LoadForGoogleOrderNumber(googleOrderNum);
            if (order == null)
            {
                Logger.Warn("Unknown Google Order Number Chargeback Initialed. GoogleOrderNumber=" + googleOrderNum +
                    ". Amount=" + N1.latestchargebackamount.Value);
            }
            else
            {
                Payment payment = AcHelper.GetGCPayment(order, GatewayInstance, true);
                Transaction trans = new Transaction();
                trans.TransactionStatus = TransactionStatus.Pending;
                //trans.TransactionDate = N1.timestamp;
                trans.Amount = N1.latestchargebackamount.Value;                
                trans.TransactionType = TransactionType.PartialRefund;
                trans.TransactionDate = LocaleHelper.LocalNow;
                trans.PaymentGatewayId = GatewayInstance.PaymentGatewayId;
                trans.ResponseMessage = "Customer Initiated Chargeback";
                trans.ProviderTransactionId = N1.googleordernumber;
                
                payment.PaymentStatus = PaymentStatusHelper.GetStatusAfterTransaction(payment, trans);

                payment.Transactions.Add(trans);
                payment.Save();

            }
        }
    }
}
