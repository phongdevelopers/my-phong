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
    /// DataSource class for Province objects
    /// </summary>
    public partial class ProvinceDataSource
    {
        /// <summary>
        /// Deletes a Province object from the database
        /// </summary>
        /// <param name="province">The Province object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(Province province)
        {
            return province.Delete();
        }

        /// <summary>
        /// Deletes a Province object with given id from the database
        /// </summary>
        /// <param name="provinceId">Value of ProvinceId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 provinceId)
        {
            Province province = new Province();
            if (province.Load(provinceId)) return province.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a Province object to the database.
        /// </summary>
        /// <param name="province">The Province object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(Province province) { return province.Save(); }

        /// <summary>
        /// Loads a Province object for given Id from the database.
        /// </summary>
        /// <param name="provinceId">Value of ProvinceId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded Province object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Province Load(Int32 provinceId)
        {
            return ProvinceDataSource.Load(provinceId, true);
        }

        /// <summary>
        /// Loads a Province object for given Id from the database.
        /// </summary>
        /// <param name="provinceId">Value of ProvinceId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded Province object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Province Load(Int32 provinceId, bool useCache)
        {
            if (provinceId == 0) return null;
            Province province = null;
            string key = "Province_" + provinceId.ToString();
            if (useCache)
            {
                province = ContextCache.GetObject(key) as Province;
                if (province != null) return province;
            }
            province = new Province();
            if (province.Load(provinceId))
            {
                if (useCache) ContextCache.SetObject(key, province);
                return province;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of Province objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the Province objects that should be loaded.</param>
        /// <returns>The number of Province objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Provinces" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Province objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Province objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Province objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Province objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Province.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Provinces");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            ProvinceCollection results = new ProvinceCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Province province = new Province();
                        Province.LoadDataReader(province, dr);
                        results.Add(province);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Province objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <returns>The number of Province objects associated with with the given ShipZoneId</returns>
        public static int CountForShipZone(Int32 shipZoneId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ShipZoneProvinces WHERE ShipZoneId = @shipZoneId");
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the Province objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <returns>A collection of Province objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForShipZone(Int32 shipZoneId)
        {
            return ProvinceDataSource.LoadForShipZone(shipZoneId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the Province objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForShipZone(Int32 shipZoneId, string sortExpression)
        {
            return ProvinceDataSource.LoadForShipZone(shipZoneId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the Province objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Province objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForShipZone(Int32 shipZoneId, int maximumRows, int startRowIndex)
        {
            return ProvinceDataSource.LoadForShipZone(shipZoneId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the Province objects associated with the given ShipZoneId
        /// </summary>
        /// <param name="shipZoneId">The given ShipZoneId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects associated with with the given ShipZoneId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForShipZone(Int32 shipZoneId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Province.GetColumnNames("ac_Provinces"));
            selectQuery.Append(" FROM ac_Provinces, ac_ShipZoneProvinces");
            selectQuery.Append(" WHERE ac_Provinces.ProvinceId = ac_ShipZoneProvinces.ProvinceId");
            selectQuery.Append(" AND ac_ShipZoneProvinces.ShipZoneId = @shipZoneId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@shipZoneId", System.Data.DbType.Int32, shipZoneId);
            //EXECUTE THE COMMAND
            ProvinceCollection results = new ProvinceCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Province province = new Province();
                        Province.LoadDataReader(province, dr);
                        results.Add(province);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of Province objects for the given CountryCode in the database.
        /// <param name="countryCode">The given CountryCode</param>
        /// </summary>
        /// <returns>The Number of Province objects for the given CountryCode in the database.</returns>
        public static int CountForCountry(String countryCode)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Provinces WHERE CountryCode = @countryCode");
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of Province objects for the given CountryCode from the database
        /// </summary>
        /// <param name="countryCode">The given CountryCode</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCountry(String countryCode)
        {
            return LoadForCountry(countryCode, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Province objects for the given CountryCode from the database
        /// </summary>
        /// <param name="countryCode">The given CountryCode</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCountry(String countryCode, string sortExpression)
        {
            return LoadForCountry(countryCode, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Province objects for the given CountryCode from the database
        /// </summary>
        /// <param name="countryCode">The given CountryCode</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCountry(String countryCode, int maximumRows, int startRowIndex)
        {
            return LoadForCountry(countryCode, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Province objects for the given CountryCode from the database
        /// </summary>
        /// <param name="countryCode">The given CountryCode</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Province objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProvinceCollection LoadForCountry(String countryCode, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Province.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Provinces");
            selectQuery.Append(" WHERE CountryCode = @countryCode");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            //EXECUTE THE COMMAND
            ProvinceCollection results = new ProvinceCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Province province = new Province();
                        Province.LoadDataReader(province, dr);
                        results.Add(province);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given Province object to the database.
        /// </summary>
        /// <param name="province">The Province object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(Province province) { return province.Save(); }

    }
}
