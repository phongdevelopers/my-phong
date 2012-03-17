using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Marketing;
using CommerceBuilder.Users;
using CommerceBuilder.Orders;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class that holds affiliate sales summary data
    /// </summary>
    public class AffiliateSalesSummary
    {
        private int _AffiliateId;
        private string _AffiliateName;
        private Affiliate _Affiliate;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _ReferralCount = -1;
        private int _OrderCount = -1;
        private LSDecimal _ProductSubtotal = -1;
        private LSDecimal _OrderTotal = -1;
        private LSDecimal _ConversionRate = -1;
        private LSDecimal _Commission = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="affiliateId">Affiliate Id</param>
        /// <param name="startDate">Start date for this summary data</param>
        /// <param name="endDate">End date for this summary data</param>
        /// <param name="orderCount">Total number of orders</param>
        /// <param name="productSubtotal">Subtotal of products</param>
        /// <param name="orderTotal">Total value of orders</param>
        public AffiliateSalesSummary(int affiliateId, DateTime startDate, DateTime endDate, int orderCount, LSDecimal productSubtotal, LSDecimal orderTotal)
        {
            _AffiliateId = affiliateId;
            _StartDate = startDate;
            _EndDate = endDate;
            _OrderCount = orderCount;
            _ProductSubtotal = productSubtotal;
            _OrderTotal = orderTotal;
        }

        /// <summary>
        /// Id of the affiliate
        /// </summary>
        public int AffiliateId
        {
            get { return _AffiliateId; }
        }

        /// <summary>
        /// The affiliate object
        /// </summary>
        public Affiliate Affiliate
        {
            get
            {
                if (_Affiliate == null)
                {
                    _Affiliate = AffiliateDataSource.Load(_AffiliateId);
                }
                return _Affiliate;
            }
            set { _Affiliate = value; }
        }

        /// <summary>
        /// Name of the affiliate
        /// </summary>
        public string AffiliateName
        {
            get
            {
                if (string.IsNullOrEmpty(_AffiliateName))
                {
                    _AffiliateName = this.Affiliate.Name;
                }
                return _AffiliateName;
            }
            set { _AffiliateName = value; }
        }

        /// <summary>
        /// Start date for this summary data
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
        }

        /// <summary>
        /// End date for this summary data
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
        }

        /// <summary>
        /// Total number of referrals
        /// </summary>
        public int ReferralCount
        {
            get
            {
                if (_ReferralCount < 0)
                {
                    _ReferralCount = AffiliateDataSource.GetReferralCount(_AffiliateId, _StartDate, _EndDate);
                }
                return _ReferralCount;
            }
            set { _ReferralCount = value; }
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
        /// Sub total of the products
        /// </summary>
        public LSDecimal ProductSubtotal
        {
            get { return _ProductSubtotal; }
            set { _ProductSubtotal = value; }
        }

        /// <summary>
        /// Total value of orders
        /// </summary>
        public LSDecimal OrderTotal
        {
            get { return _OrderTotal; }
            set { _OrderTotal = value; }
        }

        /// <summary>
        /// Conversion rate
        /// </summary>
        public LSDecimal ConversionRate
        {
            get
            {
                if (_ConversionRate < 0)
                {
                    _ConversionRate = AffiliateDataSource.GetConversionRate(_AffiliateId, _StartDate, _EndDate, this.ReferralCount);
                }
                return _ConversionRate;
            }
            set { _ConversionRate = value; }
        }

        /// <summary>
        /// Commission calculated
        /// </summary>
        public LSDecimal Commission
        {
            get
            {
                if (_Commission < 0) _Commission = Affiliate.CalculateCommission(_OrderCount, _ProductSubtotal, _OrderTotal);
                return _Commission;
            }
        }
    }
}
