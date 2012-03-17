using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Indicates the period for referrals for an affiliate
    /// </summary>
    public enum AffiliateReferralPeriod
    {
        /// <summary>
        /// Referrals are good for the number of days specified by Affiliate.ReferralDays
        /// </summary>
        NumberOfDays,
        /// <summary>
        /// Referrals are good only for the first order placed by the customer
        /// </summary>
        FirstOrder,
        /// <summary>
        /// Referrals are good for the first order placed by a customer if placed within the number of days specified by Affiliate.ReferralDays.
        /// </summary>
        FirstOrderWithinNumberOfDays,
        /// <summary>
        /// Referrals are good for all orders placed by the customer
        /// </summary>
        Persistent
    }
}
