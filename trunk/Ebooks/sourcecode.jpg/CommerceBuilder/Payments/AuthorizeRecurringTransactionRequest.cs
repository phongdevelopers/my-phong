using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Taxes;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Class representing a request to authorize a recurring payment
    /// </summary>
    public class AuthorizeRecurringTransactionRequest : BaseTransactionRequest
    {
        private TransactionOrigin _TransactionOrigin;
        private string _SubscriptionName;
        private LSDecimal _RecurringCharge;
        private bool _RecurringChargeSpecified;
        private int _NumberOfPayments;
        private int _PaymentFrequency;
        private PaymentFrequencyUnit _PaymentFrequencyUnit;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">The Payment object associated with this request</param>
        /// <param name="subscriptionPlan">The SubscriptionPlan associated witht this request</param>
        /// <param name="remoteIP">Remote IP of the user initiating the request</param>
        public AuthorizeRecurringTransactionRequest(Payment payment, SubscriptionPlan subscriptionPlan, string remoteIP) : base(payment, remoteIP)
        {
            this._TransactionOrigin = TransactionOrigin.Internet;
            if (subscriptionPlan != null)
            {
                this.SubscriptionName = subscriptionPlan.Name;
                this.Amount = payment.Amount;
                this.RecurringChargeSpecified = subscriptionPlan.RecurringChargeSpecified;
                // GET THE SUBSCRIPTION CHARGE WITH TAX
                Order order = payment.Order;
                int billToProvinceId = ProvinceDataSource.GetProvinceIdByName(order.BillToCountryCode, order.BillToProvince);
                TaxAddress billingAddress = new TaxAddress(order.BillToCountryCode, billToProvinceId, order.BillToPostalCode);
                this.RecurringCharge = TaxHelper.GetPriceWithTax(subscriptionPlan.RecurringCharge, subscriptionPlan.TaxCodeId, billingAddress, billingAddress);
                this.NumberOfPayments = subscriptionPlan.NumberOfPayments;
                this.PaymentFrequency = subscriptionPlan.PaymentFrequency;
                this.PaymentFrequencyUnit = subscriptionPlan.PaymentFrequencyUnit;
            }
        }

        /// <summary>
        /// Origin of the transaction
        /// </summary>
        public TransactionOrigin TransactionOrigin
        {
            get { return _TransactionOrigin; }
            set { _TransactionOrigin = value; }
        }

        /// <summary>
        /// Type of the transaction
        /// </summary>
        public override TransactionType TransactionType
        {
            get { return TransactionType.AuthorizeRecurring; }
        }

        /// <summary>
        /// Name of the subscription
        /// </summary>
        public string SubscriptionName
        {
            get { return _SubscriptionName; }
            set { _SubscriptionName = value; }
        }

        /// <summary>
        /// Number of payments in the subscription
        /// </summary>
        public int NumberOfPayments
        {
            get { return _NumberOfPayments; }
            set { _NumberOfPayments = value; }
        }

        /// <summary>
        /// Whether recurring charge is specified in subscription plan
        /// </summary>
        public bool RecurringChargeSpecified
        {
            get { return _RecurringChargeSpecified; }
            set { _RecurringChargeSpecified = value; }
        }

        /// <summary>
        /// The recurring charge in subscription plan
        /// </summary>
        public LSDecimal RecurringCharge
        {
            get { return _RecurringCharge; }
            set { _RecurringCharge = value; }
        }

        /// <summary>
        /// Frequency of payments for the subscription
        /// </summary>
        public int PaymentFrequency
        {
            get { return _PaymentFrequency; }
            set { _PaymentFrequency = value; }
        }

        /// <summary>
        /// The unit for measuring the payment frequency (Day or Month)
        /// </summary>
        public PaymentFrequencyUnit PaymentFrequencyUnit
        {
            get { return _PaymentFrequencyUnit; }
            set { _PaymentFrequencyUnit = value; }
        }

    }
}
