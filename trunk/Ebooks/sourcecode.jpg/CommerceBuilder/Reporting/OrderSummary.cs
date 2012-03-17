using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Order Summary
    /// </summary>
    public class OrderSummary
    {
        private int _OrderId;
        private int _OrderNumber;
        private LSDecimal _ProductTotal;
        private LSDecimal _ShippingTotal;
        private LSDecimal _TaxTotal;
        private LSDecimal _CouponTotal;
        private LSDecimal _DiscountTotal;
        private LSDecimal _OtherTotal;
        private LSDecimal _GrandTotal;
        private LSDecimal _CostOfGoodTotal;        

        /// <summary>
        /// OrderId
        /// </summary>
        public int OrderId
        {
            get { return _OrderId; }
            set { _OrderId = value; }
        }

        /// <summary>
        /// OrderId
        /// </summary>
        public int OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }

        /// <summary>
        /// ProductTotal
        /// </summary>
        public LSDecimal ProductTotal
        {
            get { return _ProductTotal; }
            set { _ProductTotal = value; }
        }

        /// <summary>
        /// ShippingTotal
        /// </summary>
        public LSDecimal ShippingTotal
        {
            get { return _ShippingTotal; }
            set { _ShippingTotal = value; }
        }

        /// <summary>
        /// TaxTotal
        /// </summary>
        public LSDecimal TaxTotal
        {
            get { return _TaxTotal; }
            set { _TaxTotal = value; }
        }

        /// <summary>
        /// CouponTotal
        /// </summary>
        public LSDecimal CouponTotal
        {
            get { return _CouponTotal; }
            set { _CouponTotal = value; }
        }

        /// <summary>
        /// DiscountTotal
        /// </summary>
        public LSDecimal DiscountTotal
        {
            get { return _DiscountTotal; }
            set { _DiscountTotal = value; }
        }

        /// <summary>
        /// OtherTotal
        /// </summary>
        public LSDecimal OtherTotal
        {
            get { return _OtherTotal; }
            set { _OtherTotal = value; }
        }

        /// <summary>
        /// GrandTotal
        /// </summary>
        public LSDecimal GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; }
        }

        /// <summary>
        /// CostOfGoodTotal
        /// </summary>
        public LSDecimal CostOfGoodTotal
        {
            get { return _CostOfGoodTotal; }
            set { _CostOfGoodTotal = value; }
        }

        /// <summary>
        /// ProfitTotal
        /// </summary>
        public LSDecimal ProfitTotal
        {         
            //To calculate profit, we must only calculate in values that relate to products.  
            //Profit = Total Price of Products - Discounts and Coupons - Total Cost of Goods
            //We'll need to be careful not to take away coupon amounts that apply to   shipping.
            // AS COUPONS TOTAL WILL BE A NEGITIVE VALUE SO, it is added instead subtraction.
            get { return (ProductTotal - (-DiscountTotal) - (-CouponTotal) - CostOfGoodTotal); }
        }        

    }
}
