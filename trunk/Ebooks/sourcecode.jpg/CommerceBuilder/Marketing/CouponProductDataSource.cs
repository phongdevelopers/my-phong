using System.ComponentModel;
using System;
using System.Text;
using CommerceBuilder.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using System.Data;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// DataSource class for CouponProduct objects
    /// </summary>
    [DataObject(true)]
    public partial class CouponProductDataSource
    {

        /// <summary>
        /// Loads a collection of CouponProduct objects for the given couponId
        /// </summary>
        /// <param name="couponId">Id of the coupon for which to load</param>
        /// <param name="excludeGiftCerts">If <b>true</b> gift certificate products are excluded</param>
        /// <returns>A collection of CouponProduct objects for the given couponId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CouponProductCollection LoadForCoupon(Int32 couponId, bool excludeGiftCerts)
        {            
            if (excludeGiftCerts)
            {
                CouponProductCollection CouponProducts = new CouponProductCollection();
                //CREATE THE DYNAMIC SQL TO LOAD OBJECT
                StringBuilder selectQuery = new StringBuilder();
                selectQuery.Append("SELECT CP.ProductId");
                selectQuery.Append(" FROM ac_CouponProducts CP INNER JOIN ac_Products P ON CP.ProductId=P.ProductId");
                selectQuery.Append(" WHERE CP.CouponId = @couponId");
                selectQuery.Append(" AND P.IsGiftCertificate = 0");
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
                database.AddInParameter(selectCommand, "@couponId", System.Data.DbType.Int32, couponId);
                //EXECUTE THE COMMAND
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        CouponProduct couponProduct = new CouponProduct();
                        couponProduct.CouponId = couponId;
                        couponProduct.ProductId = dr.GetInt32(0);
                        CouponProducts.Add(couponProduct);
                    }
                    dr.Close();
                }
                return CouponProducts;

            }
            else
            {
                return LoadForCoupon(couponId);
            }
        }

    }
}
