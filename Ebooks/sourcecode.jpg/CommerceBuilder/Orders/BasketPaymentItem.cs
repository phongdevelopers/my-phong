using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class that holds payment data
    /// </summary>
    public class BasketPaymentItem
    {
        private string _Name = string.Empty;
        private string _AccountData = string.Empty;
        private LSDecimal _Amount = 0;
        private OrderItemType _OrderItemType;

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Details of payment account data
        /// </summary>
        public string AccountData
        {
            get { return _AccountData; }
            set { _AccountData = value; }
        }

        /// <summary>
        /// Amount
        /// </summary>
        public LSDecimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        /// <summary>
        /// Type of order Item
        /// </summary>
        public OrderItemType OrderItemType
        {
            get { return _OrderItemType; }
            set { _OrderItemType = value; }
        }

        /// <summary>
        /// The Payment object created from this BasketPaymentItem
        /// </summary>
        /// <returns></returns>
        public Payment GetPaymentObject()
        {
            Payment payment = new Payment();
            payment.AccountData = this.AccountData;
            payment.ReferenceNumber = StringHelper.MakeReferenceNumber(this.AccountData);
            payment.Amount = this.Amount;
            payment.PaymentStatus = PaymentStatus.Unprocessed;
            payment.PaymentDate = LocaleHelper.LocalNow;
            return payment;
        }
    }
}
