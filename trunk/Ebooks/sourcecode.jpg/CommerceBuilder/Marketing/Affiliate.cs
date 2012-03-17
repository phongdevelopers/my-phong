using System;
using CommerceBuilder.Common;
using CommerceBuilder.Data;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// This class represents a Affiliate object in the database.
    /// </summary>
    public partial class Affiliate
    {
        /// <summary>
        /// The period used for referrals by this affiliate
        /// </summary>
        public AffiliateReferralPeriod ReferralPeriod
        {
            get
            {
                return (AffiliateReferralPeriod)this.ReferralPeriodId;
            }
            set
            {
                this.ReferralPeriodId = (byte)value;
            }
        }

        /// <summary>
        /// Calculates commission for this affiliate
        /// </summary>
        /// <param name="orderCount">Number of orders</param>
        /// <param name="productSubtotal">Sub total of the products</param>
        /// <param name="orderTotal">Total value of orders</param>
        /// <returns>The calculated commission</returns>
        public LSDecimal CalculateCommission(int orderCount, LSDecimal productSubtotal, LSDecimal orderTotal)
        {
            if (this.CommissionIsPercent)
            {
                if (this.CommissionOnTotal) return Math.Round((((Decimal)orderTotal * (Decimal)this.CommissionRate) / 100), 2);
                return Math.Round((((Decimal)productSubtotal * (Decimal)this.CommissionRate) / 100), 2);
            }
            else
            {
                return (orderCount * this.CommissionRate);
            }
        }

        /// <summary>
        /// Deletes this Affiliate object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("UPDATE ac_Orders SET AffiliateId = NULL WHERE AffiliateId = @affiliateId"))
            {
                database.AddInParameter(deleteCommand, "@affiliateId", System.Data.DbType.Int32, this.AffiliateId);
                database.ExecuteNonQuery(deleteCommand);
            }
            return this.BaseDelete();
        }

    }
}
