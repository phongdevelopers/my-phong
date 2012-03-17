//Generated by DataSourceBaseGenerator_Assn

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
namespace CommerceBuilder.Taxes
{

    /// <summary>
    /// DataSource class for TaxRuleShipZone objects
    /// </summary>
    public partial class TaxRuleShipZoneDataSource
    {
        /// <summary>
        /// Deletes a TaxRuleShipZone object from the database
        /// </summary>
        /// <param name="taxRuleShipZone">The TaxRuleShipZone object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(TaxRuleShipZone taxRuleShipZone)
        {
            return taxRuleShipZone.Delete();
        }

        /// <summary>
        /// Deletes a TaxRuleShipZone object with given id from the database
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the object to delete.</param>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 taxRuleId, Int32 shipZoneId)
        {
            TaxRuleShipZone taxRuleShipZone = new TaxRuleShipZone();
            if (taxRuleShipZone.Load(taxRuleId, shipZoneId)) return taxRuleShipZone.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a TaxRuleShipZone object to the database.
        /// </summary>
        /// <param name="taxRuleShipZone">The TaxRuleShipZone object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(TaxRuleShipZone taxRuleShipZone) { return taxRuleShipZone.Save(); }

        /// <summary>
        /// Load a TaxRuleShipZone object for the given primary key from the database.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the object to load.</param>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <returns>The loaded TaxRuleShipZone object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static TaxRuleShipZone Load(Int32 taxRuleId, Int32 shipZoneId)
        {
            TaxRuleShipZone taxRuleShipZone = new TaxRuleShipZone();
            taxRuleShipZone.TaxRuleId = taxRuleId;
            taxRuleShipZone.ShipZoneId = shipZoneId;
            taxRuleShipZone.IsDirty = false;
            return taxRuleShipZone;
        }

        /// <summary>
        /// Loads a collection of TaxRuleShipZone objects for the given criteria for ShipZone from the database.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <returns>A collection of TaxRuleShipZone objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static TaxRuleShipZoneCollection LoadForShipZone(Int32 shipZoneId)
        {
            TaxRuleShipZoneCollection TaxRuleShipZones = new TaxRuleShipZoneCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT TaxRuleId");
            selectQuery.Append(" FROM ac_TaxRuleShipZones");
            selectQuery.Append(" WHERE ShipZoneId = @shipZoneId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    TaxRuleShipZone taxRuleShipZone = new TaxRuleShipZone();
                    taxRuleShipZone.ShipZoneId = shipZoneId;
                    taxRuleShipZone.TaxRuleId = dr.GetInt32(0);
                    TaxRuleShipZones.Add(taxRuleShipZone);
                }
                dr.Close();
            }
            return TaxRuleShipZones;
        }

        /// <summary>
        /// Loads a collection of TaxRuleShipZone objects for the given criteria for TaxRule from the database.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the object to load.</param>
        /// <returns>A collection of TaxRuleShipZone objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static TaxRuleShipZoneCollection LoadForTaxRule(Int32 taxRuleId)
        {
            TaxRuleShipZoneCollection TaxRuleShipZones = new TaxRuleShipZoneCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ShipZoneId");
            selectQuery.Append(" FROM ac_TaxRuleShipZones");
            selectQuery.Append(" WHERE TaxRuleId = @taxRuleId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@taxRuleId", System.Data.DbType.Int32, taxRuleId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    TaxRuleShipZone taxRuleShipZone = new TaxRuleShipZone();
                    taxRuleShipZone.TaxRuleId = taxRuleId;
                    taxRuleShipZone.ShipZoneId = dr.GetInt32(0);
                    TaxRuleShipZones.Add(taxRuleShipZone);
                }
                dr.Close();
            }
            return TaxRuleShipZones;
        }

        /// <summary>
        /// Updates/Saves the given TaxRuleShipZone object to the database.
        /// </summary>
        /// <param name="taxRuleShipZone">The TaxRuleShipZone object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(TaxRuleShipZone taxRuleShipZone) { return taxRuleShipZone.Save(); }

    }
}
