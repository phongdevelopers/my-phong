using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.ComponentModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// DataSource class for Coupon objects
    /// </summary>
    [DataObject(true)]
    public partial class CouponDataSource
    {
        /// <summary>
        /// Counts the number of users using the given coupon code
        /// </summary>
        /// <param name="couponCode">The coupon code for which to count the users</param>
        /// <returns>Number of users using the given coupon code</returns>
        public static int CountUses(string couponCode)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT Count(*)");
            selectQuery.Append(" FROM ac_OrderCoupons, ac_Orders");
            selectQuery.Append(" WHERE ac_OrderCoupons.OrderId = ac_Orders.OrderId");
            selectQuery.Append(" AND ac_OrderCoupons.CouponCode = @couponCode");
            selectQuery.Append(" AND ac_Orders.StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return (int)database.ExecuteScalar(selectCommand);
        }

        /// <summary>
        /// Counts the numer of times the given coupon has been used by a particular user
        /// </summary>
        /// <param name="couponCode">The coupon to check for usage</param>
        /// <param name="userId">Id of the user for which to check the coupon usage</param>
        /// <returns>Number of times the given coupon has been used by the given user</returns>
        public static int CountUsesForUser(string couponCode, int userId)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT Count(*)");
            selectQuery.Append(" FROM ac_OrderCoupons, ac_Orders");
            selectQuery.Append(" WHERE ac_OrderCoupons.OrderId = ac_Orders.OrderId");
            selectQuery.Append(" AND ac_OrderCoupons.CouponCode = @couponCode");
            selectQuery.Append(" AND ac_Orders.UserId = @userId");
            selectQuery.Append(" AND ac_Orders.StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return (int)database.ExecuteScalar(selectCommand);
        }

        /// <summary>
        /// Loads a Coupon object for the given coupon code
        /// </summary>
        /// <param name="couponCode">Coupon code for which to load the coupon</param>
        /// <returns>The Coupon object loaded</returns>
        public static Coupon LoadForCouponCode(string couponCode)
        {
            couponCode = couponCode.ToUpperInvariant();
            string key = "Coupon_" + couponCode;
            Coupon coupon = ContextCache.GetObject(key) as Coupon;
            if (coupon != null) return coupon;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT CouponId");
            selectQuery.Append(" FROM ac_Coupons");
            selectQuery.Append(" WHERE CouponCode = @couponCode");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@couponCode", System.Data.DbType.String, couponCode);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            int couponId = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            if (couponId != 0)
            {
                coupon = CouponDataSource.Load(couponId);
                ContextCache.SetObject(key, coupon);
                return coupon;
            }
            return null;
        }
    }
}
