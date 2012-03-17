//Generated by DataSourceBaseGenerator_Assn

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
namespace CommerceBuilder.Shipping
{

    /// <summary>
    /// DataSource class for ShipZoneCountry objects
    /// </summary>
    public partial class ShipZoneCountryDataSource
    {
        /// <summary>
        /// Deletes a ShipZoneCountry object from the database
        /// </summary>
        /// <param name="shipZoneCountry">The ShipZoneCountry object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(ShipZoneCountry shipZoneCountry)
        {
            return shipZoneCountry.Delete();
        }

        /// <summary>
        /// Deletes a ShipZoneCountry object with given id from the database
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to delete.</param>
        /// <param name="countryCode">Value of CountryCode of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 shipZoneId, String countryCode)
        {
            ShipZoneCountry shipZoneCountry = new ShipZoneCountry();
            if (shipZoneCountry.Load(shipZoneId, countryCode)) return shipZoneCountry.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a ShipZoneCountry object to the database.
        /// </summary>
        /// <param name="shipZoneCountry">The ShipZoneCountry object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(ShipZoneCountry shipZoneCountry) { return shipZoneCountry.Save(); }

        /// <summary>
        /// Load a ShipZoneCountry object for the given primary key from the database.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <param name="countryCode">Value of CountryCode of the object to load.</param>
        /// <returns>The loaded ShipZoneCountry object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipZoneCountry Load(Int32 shipZoneId, String countryCode)
        {
            ShipZoneCountry shipZoneCountry = new ShipZoneCountry();
            shipZoneCountry.ShipZoneId = shipZoneId;
            shipZoneCountry.CountryCode = countryCode;
            shipZoneCountry.IsDirty = false;
            return shipZoneCountry;
        }

        /// <summary>
        /// Loads a collection of ShipZoneCountry objects for the given criteria for Country from the database.
        /// </summary>
        /// <param name="countryCode">Value of CountryCode of the object to load.</param>
        /// <returns>A collection of ShipZoneCountry objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipZoneCountryCollection LoadForCountry(String countryCode)
        {
            ShipZoneCountryCollection ShipZoneCountries = new ShipZoneCountryCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ShipZoneId");
            selectQuery.Append(" FROM ac_ShipZoneCountries");
            selectQuery.Append(" WHERE CountryCode = @countryCode");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipZoneCountry shipZoneCountry = new ShipZoneCountry();
                    shipZoneCountry.CountryCode = countryCode;
                    shipZoneCountry.ShipZoneId = dr.GetInt32(0);
                    ShipZoneCountries.Add(shipZoneCountry);
                }
                dr.Close();
            }
            return ShipZoneCountries;
        }

        /// <summary>
        /// Loads a collection of ShipZoneCountry objects for the given criteria for ShipZone from the database.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <returns>A collection of ShipZoneCountry objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipZoneCountryCollection LoadForShipZone(Int32 shipZoneId)
        {
            ShipZoneCountryCollection ShipZoneCountries = new ShipZoneCountryCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT CountryCode");
            selectQuery.Append(" FROM ac_ShipZoneCountries");
            selectQuery.Append(" WHERE ShipZoneId = @shipZoneId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipZoneCountry shipZoneCountry = new ShipZoneCountry();
                    shipZoneCountry.ShipZoneId = shipZoneId;
                    shipZoneCountry.CountryCode = dr.GetString(0);
                    ShipZoneCountries.Add(shipZoneCountry);
                }
                dr.Close();
            }
            return ShipZoneCountries;
        }

        /// <summary>
        /// Updates/Saves the given ShipZoneCountry object to the database.
        /// </summary>
        /// <param name="shipZoneCountry">The ShipZoneCountry object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(ShipZoneCountry shipZoneCountry) { return shipZoneCountry.Save(); }

    }
}
