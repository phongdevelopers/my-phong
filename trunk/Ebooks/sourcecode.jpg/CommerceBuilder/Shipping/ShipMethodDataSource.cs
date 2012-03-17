using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// DataSource class for ShipMethod objects
    /// </summary>
    [DataObject(true)]
    public partial class ShipMethodDataSource
    {
        /// <summary>
        /// Loads a collection of ShipMethod objects for given BasketShipment
        /// </summary>
        /// <param name="shipment">BasketShipment to load the ship methods for</param>
        /// <returns>A collection of ShipMethod objects for given BasketShipment</returns>
        public static ShipMethodCollection LoadForShipment(IShipment shipment)
        {
            //GET ALL SHIP METHODS THAT CAN APPLY TO THIS SHIPMENT
            ShipMethodCollection shipMethodCollection = new ShipMethodCollection();

            //FIRST DETERMINE THE SHIP ZONE(S) FOR THE SHIPMENT
            ShipZoneCollection shipmentZoneCollection = shipment.ShipZones;
            //BUILD A LIST OF ZONEIDS FOR QUERY CRITERIA

            //NOW QUERY ANY SHIP METHODS THAT CAN SHIP FROM THIS WAREHOUSE TO ANY ZONE OR ONE OF THE ASSOCIATED ZONES
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT " + ShipMethod.GetColumnNames("ac_ShipMethods"));
            selectQuery.Append(" FROM ((ac_ShipMethods LEFT JOIN ac_ShipMethodWarehouses ON ac_ShipMethods.ShipMethodId = ac_ShipMethodWarehouses.ShipMethodId)");
            selectQuery.Append(" LEFT JOIN ac_ShipMethodShipZones ON ac_ShipMethods.ShipMethodId = ac_ShipMethodShipZones.ShipMethodId)");
            selectQuery.Append(" LEFT JOIN ac_ShipMethodGroups ON ac_ShipMethods.ShipMethodId = ac_ShipMethodGroups.ShipMethodId");
            selectQuery.Append(" WHERE StoreId = @storeId");
            //PROCESS MINPURCHASE EXCLUSION
            selectQuery.Append(" AND MinPurchase <= @shipmentValue");
            //PROCESS WAREHOUSE EXCLUSION
            selectQuery.Append(" AND (ac_ShipMethodWarehouses.WarehouseId IS NULL OR ac_ShipMethodWarehouses.WarehouseId = @warehouseId)");
            //PROCESS SHIPZONE EXCLUSION
            selectQuery.Append(" AND (ac_ShipMethodShipZones.ShipZoneId IS NULL");
            for (int i = 0; i < shipmentZoneCollection.Count; i++)
            {
                selectQuery.Append(" OR ac_ShipMethodShipZones.ShipZoneId = @shipZoneId" + i.ToString());
            }
            selectQuery.Append(")");

            //PROCESS GROUP EXCLUSION
            selectQuery.Append(" AND (ac_ShipMethodGroups.GroupId IS NULL");
            User user = UserDataSource.LoadForShipment(shipment);
            if (user != null)
            {
                for (int i = 0; i < user.UserGroups.Count; i++)
                {
                    selectQuery.Append(" OR ac_ShipMethodGroups.GroupId = @groupId" + i.ToString());
                }
            }
            selectQuery.Append(")");
            //ORDER ACCORDING TO MERCHANT RULES
            //selectQuery.Append(" ORDER BY ac_ShipMethods.OrderBy");


            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //ADD IN MINPURCHASE PARAMETER
            database.AddInParameter(selectCommand, "@shipmentValue", System.Data.DbType.Decimal, shipment.GetItems().TotalProductPrice());
            //ADD IN WAREHOUSE PARAMETER
            database.AddInParameter(selectCommand, "@warehouseId", System.Data.DbType.Int32, shipment.WarehouseId);
            //ADD IN NUMBERED ZONE PARAMETERS
            for (int i = 0; i < shipmentZoneCollection.Count; i++)
            {
                database.AddInParameter(selectCommand, "@shipZoneId" + i.ToString(), System.Data.DbType.Int32, shipmentZoneCollection[i].ShipZoneId);
            }
            //ADD IN NUMBERED GROUP PARAMETERS
            for (int i = 0; i < user.UserGroups.Count; i++)
            {
                database.AddInParameter(selectCommand, "@groupId" + i.ToString(), System.Data.DbType.Int32, user.UserGroups[i].GroupId);
            }
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipMethod shipMethod = new ShipMethod();
                    ShipMethod.LoadDataReader(shipMethod, dr);
                    shipMethodCollection.Add(shipMethod);
                }
                dr.Close();
            }
            //SORT THE ITEMS (NOT DONE IN QUERY BECAUSE OF DISTINCT CLAUSE)
            shipMethodCollection.Sort("OrderBy");
            return shipMethodCollection;
        }

        /// <summary>
        /// Loads a ship method for given ship method name
        /// </summary>
        /// <param name="shipMethodName">Name of the ship method to load</param>
        /// <returns>The loaded ship method or null if no ship method with given name could be found</returns>
        public static ShipMethod LoadForShipMethodName(string shipMethodName)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append(ShipMethod.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ShipMethods");
            selectQuery.Append(" WHERE ac_ShipMethods.Name = @shipMethodName");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipMethodName", System.Data.DbType.String, shipMethodName);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    ShipMethod shipMeth = new ShipMethod();
                    ShipMethod.LoadDataReader(shipMeth, dr);
                    return shipMeth;
                }
                dr.Close();
            }
            return null;
        }

    }
}