using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class that holds sale summary data
    /// </summary>
    public class SalesSummary
    {
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _OrderCount;
        private LSDecimal _ProductTotal;
        private LSDecimal _ShippingTotal;
        private LSDecimal _TaxTotal;
        private LSDecimal _CouponTotal;
        private LSDecimal _DiscountTotal;
        private LSDecimal _CostOfGoodTotal;
        private LSDecimal _GiftWrapTotal;        
        private int _ProductCount;        
        private int _UserCount;        
       
        private LSDecimal _OtherTotal;
        private LSDecimal _GrandTotal;

        /// <summary>
        /// Start date for this sales summary data
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        /// <summary>
        /// End date for this sales summary data
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        /// <summary>
        /// Total number of orders
        /// </summary>
        public int OrderCount
        {
            get { return _OrderCount; }
            set { _OrderCount = value; }
        }

        /// <summary>
        /// Total value of products
        /// </summary>
        public LSDecimal ProductTotal
        {
            get { return _ProductTotal; }
            set { _ProductTotal = value; }
        }

        /// <summary>
        /// Total value of shipping charges
        /// </summary>
        public LSDecimal ShippingTotal
        {
            get { return _ShippingTotal; }
            set { _ShippingTotal = value; }
        }

        /// <summary>
        /// Total value of taxes
        /// </summary>
        public LSDecimal TaxTotal
        {
            get { return _TaxTotal; }
            set { _TaxTotal = value; }
        }

        /// <summary>
        /// Total value of coupons
        /// </summary>
        public LSDecimal CouponTotal
        {
            get { return _CouponTotal; }
            set { _CouponTotal = value; }
        }

        /// <summary>
        /// Total value of discounts
        /// </summary>
        public LSDecimal DiscountTotal
        {
            get { return _DiscountTotal; }
            set { _DiscountTotal = value; }
        }

        /// <summary>
        /// Total value of other items
        /// </summary>
        public LSDecimal OtherTotal
        {
            get { return _OtherTotal; }
            set { _OtherTotal = value; }
        }

        /// <summary>
        /// Grand total
        /// </summary>
        public LSDecimal GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; }
        }

        /// <summary>
        /// Total cost of goods
        /// </summary>
        public LSDecimal CostOfGoodTotal
        {
            get { return _CostOfGoodTotal; }
            set { _CostOfGoodTotal = value; }
        }

        /// <summary>
        /// Total Profit. Profit = Total Price of Products - Discounts and Coupons - Total Cost of Goods
        /// </summary>
        public LSDecimal ProfitTotal
        {
            //To calculate profit, we must only calculate in values that relate to products.  
            //Profit = Total Price of Products - Discounts and Coupons - Total Cost of Goods
            //We'll need to be careful not to take away coupon amounts that apply to   shipping.
            // AS COUPONS TOTAL WILL BE A NEGITIVE VALUE SO, it is added instead subtraction.
            get { return (ProductTotal - (-DiscountTotal) - (-CouponTotal) - CostOfGoodTotal); }
        }

        /// <summary>
        /// Total receivable amount. Receivables = ProductTotal - (-DiscountTotal) - (-CouponTotal) + GiftWrapTotal + TaxTotal + Shipping
        /// </summary>
        public LSDecimal TotalReceivables
        {
            get { return (ProductTotal - (-DiscountTotal) - (-CouponTotal) + GiftWrapTotal + TaxTotal + ShippingTotal); }
        }

        /// <summary>
        /// Total number of users
        /// </summary>
        public int UserCount
        {
            get { return _UserCount; }
            set { _UserCount = value; }
        }

        /// <summary>
        /// Products total
        /// </summary>
        public int ProductCount
        {
            get { return _ProductCount; }
            set { _ProductCount = value; }
        }

        /// <summary>
        /// Total price for gift wrap items.
        /// </summary>
        public LSDecimal GiftWrapTotal
        {
            get { return _GiftWrapTotal; }
            set { _GiftWrapTotal = value; }
        }

    }
}
