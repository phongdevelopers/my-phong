using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments.Providers.GoogleCheckout.AutoGen;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.AC
{
    public class RiskInformationHandler
    {
        RiskInformationNotification N1;
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

        public RiskInformationHandler(RiskInformationNotification n1)
        {
            N1 = n1;
        }

        public void Process()
        {
            string googleOrderNum = N1.googleordernumber;
            Order order = OrderDataSource.LoadForGoogleOrderNumber(googleOrderNum);
            if (order == null)
            {
                Logger.Warn("Unknown Google Order Number Risk Information. GoogleOrderNumber=" + googleOrderNum);
            }
            else
            {
                Payment payment = AcHelper.GetGCPayment(order, GatewayInstance, true);
                //IF THE PAYMENT IS A GIFT CERTIFICATE WE SHOULD ADD THIS TX FOR INFO ONLY
                int gcPayMethodId = PaymentEngine.GetGiftCertificatePaymentMethod().PaymentMethodId;
                bool isGiftCert = (payment.PaymentMethodId == gcPayMethodId);

                Transaction trans = GetGCAuthorizationTransaction(payment.Transactions, N1.googleordernumber);

                trans.AVSResultCode = N1.riskinformation.avsresponse;
                trans.RemoteIP = N1.riskinformation.ipaddress;
                if (!isGiftCert)
                {
                    trans.Amount = payment.Amount;
                    trans.TransactionType = TransactionType.Authorize;
                }
                else
                {
                    trans.Amount = 0;
                    trans.TransactionType = TransactionType.AuthorizeCapture;
                }

                trans.TransactionDate = N1.timestamp;                
                trans.PaymentGatewayId = GatewayInstance.PaymentGatewayId;
                trans.ProviderTransactionId = googleOrderNum;                
                bool protection = N1.riskinformation.eligibleforprotection;
                string cvn = N1.riskinformation.cvnresponse;
                int age = N1.riskinformation.buyeraccountage;
                trans.CVVResultCode = cvn;
                StringBuilder responseMessage = new StringBuilder();
                if (protection) responseMessage.Append("Eligible for protection.");
                else responseMessage.Append("NOT eligible for protection.");
                responseMessage.Append("  Buyer account is " + age + " days old.");
                trans.ResponseMessage = responseMessage.ToString();
                trans.AuthorizationCode = googleOrderNum;
                //trans.AuthorizationCode = (protection ? 1 : 0).ToString() + "|" + cvn + "|" + age;
                payment.Transactions.Add(trans);
                //do not update payment status. 
                //It will be updated when order state change notification is received
                payment.Save();
            }
        }

        private Transaction GetGCAuthorizationTransaction(TransactionCollection transactions, string googleOrderNumber)
        {
            int index = transactions.Count - 1;
            for (index = transactions.Count - 1; index >= 0; index--)
            {
                if ((transactions[index].TransactionType == TransactionType.Authorize
                    || transactions[index].TransactionType == TransactionType.AuthorizeCapture)
                    && transactions[index].ProviderTransactionId.Equals(googleOrderNumber))
                {
                    return transactions[index];
                }
            }

            Transaction trans = new Transaction();
            trans.ProviderTransactionId = googleOrderNumber;
            trans.TransactionType = TransactionType.Authorize;
            return trans;
        }
    }
}
