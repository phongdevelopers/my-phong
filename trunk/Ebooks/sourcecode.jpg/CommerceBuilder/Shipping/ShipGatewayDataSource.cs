using System;
using System.ComponentModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// DataSource class for ShipGateway objects
    /// </summary>
    [DataObject(true)]
    public partial class ShipGatewayDataSource
    {
        /// <summary>
        /// Given the class Id of a ship gateway implementation, finds out the object Id of the corresponding record in database.
        /// </summary>
        /// <param name="classId">class Id of a ship gateway implementation</param>
        /// <returns>Object Id of the corresponding record in database</returns>
        public static int GetShipGatewayIdByClassId(string classId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT ShipGatewayId FROM ac_ShipGateways WHERE StoreId=@storeId AND ClassId=@classId");
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@classId", DbType.String, classId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of ShipGateway objects for given class Id
        /// </summary>
        /// <param name="classId">class Id of the ship gateway implementation</param>
        /// <returns>A collection of ShipGateway objects for given class Id</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipGatewayCollection LoadForClassId(string classId)
        {
            ShipGatewayCollection ShipGateways = new ShipGatewayCollection();
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT " + ShipGateway.GetColumnNames(string.Empty) + "  FROM ac_ShipGateways WHERE StoreId=@storeId AND ClassId=@classId");
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@classId", DbType.String, classId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipGateway shipGateway = new ShipGateway();
                    ShipGateway.LoadDataReader(shipGateway, dr);
                    ShipGateways.Add(shipGateway);
                }
                dr.Close();
            }
            return ShipGateways;
        }
    }
}
