//Generated by DataSourceBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// DataSource class for Country objects
    /// </summary>
    public partial class CountryDataSource
    {
        /// <summary>
        /// Deletes a Country object from the database
        /// </summary>
        /// <param name="country">The Country object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(Country country)
        {
            return country.Delete();
        }

        /// <summary>
        /// Deletes a Country object with given id from the database
        /// </summary>
        /// <param name="countryCode">Value of CountryCode of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(String countryCode)
        {
            Country country = new Country();
            if (country.Load(countryCode)) return country.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a Country object to the database.
        /// </summary>
        /// <param name="country">The Country object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(Country country) { return country.Save(); }

        /// <summary>
        /// Loads a Country object for given key from the database.
        /// </summary>
        /// <param name="countryCode">Value of CountryCode of the object to load.</param>
        /// <returns>If the load is successful the newly loaded Country object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Country Load(String countryCode)
        {
            Country country = new Country();
            if (country.Load(countryCode)) return country;
            return null;
        }

        /// <summary>
        /// Counts the number of Country objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the Country objects that should be loaded.</param>
        /// <returns>The number of Country objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Countries" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Country objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Country objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Country objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Country objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Country.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Countries");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            CountryCollection results = new CountryCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Country country = new Country();
                        Country.LoadDataReader(country, dr);
                        results.Add(country);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Country objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <returns>The number of Country objects associated with with the given ShipZoneId</returns>
        public static int CountForShipZone(Int32 shipZoneId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ShipZoneCountries WHERE ShipZoneId = @shipZoneId");
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the Country objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <returns>A collection of Country objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForShipZone(Int32 shipZoneId)
        {
            return CountryDataSource.LoadForShipZone(shipZoneId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the Country objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForShipZone(Int32 shipZoneId, string sortExpression)
        {
            return CountryDataSource.LoadForShipZone(shipZoneId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the Country objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Country objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForShipZone(Int32 shipZoneId, int maximumRows, int startRowIndex)
        {
            return CountryDataSource.LoadForShipZone(shipZoneId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the Country objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForShipZone(Int32 shipZoneId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Country.GetColumnNames("ac_Countries"));
            selectQuery.Append(" FROM ac_Countries, ac_ShipZoneCountries");
            selectQuery.Append(" WHERE ac_Countries.CountryCode = ac_ShipZoneCountries.CountryCode");
            selectQuery.Append(" AND ac_ShipZoneCountries.ShipZoneId = @shipZoneId");
            selectQuery.Append(" AND StoreId = @storeId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            CountryCollection results = new CountryCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Country country = new Country();
                        Country.LoadDataReader(country, dr);
                        results.Add(country);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Country objects for the current store.
        /// </summary>
        /// <returns>The Number of Country objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Countries WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Country objects for the current store from the database
        /// </summary>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Country objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Country objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Country objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Country objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static CountryCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Country.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Countries");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            CountryCollection results = new CountryCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Country country = new Country();
                        Country.LoadDataReader(country, dr);
                        results.Add(country);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given Country object to the database.
        /// </summary>
        /// <param name="country">The Country object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(Country country) { return country.Save(); }

    }
}
