using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DigitalDelivery
{
    [DataObject(true)]
    public partial class LicenseAgreementDataSource
    {
        /// <summary>
        /// Changes the agreement for a group of digital goods.
        /// </summary>
        /// <param name="oldAgreementId">The agreement that digital goods are to be moved from.</param>
        /// <param name="newAgreementId">The agreement that digital goods are to be moved to.</param>
        public static void MoveDigitalGoods(int oldAgreementId, int newAgreementId)
        {
            if (oldAgreementId != newAgreementId)
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand("UPDATE ac_DigitalGoods SET LicenseAgreementId = @newAgreementId WHERE LicenseAgreementId = @oldAgreementId");
                database.AddInParameter(selectCommand, "@newAgreementId", System.Data.DbType.Int32, NullableData.DbNullify(newAgreementId));
                database.AddInParameter(selectCommand, "@oldAgreementId", System.Data.DbType.Int32, oldAgreementId);
                database.ExecuteNonQuery(selectCommand);
            }
        }
    }
}
