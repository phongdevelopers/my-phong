using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    [DataObject(true)]
    public partial class ManufacturerDataSource
    {
        /// <summary>
        /// Loads a manufacturer given the name.
        /// </summary>
        /// <param name="name">The name of the manufacturer to load.</param>
        /// <param name="create">Specifies whether or not to create the manufacturer if it does not exist.</param>
        /// <returns>If found or created, returns the manufacturer instance.  Otherwise returns null.</returns>
        /// <remarks>Case sensitivity of the name matche is determined by the database.  If more than one matching manufacturer is found, only the first match is returned.</remarks>
        public static Manufacturer LoadForName(string name, bool create)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(name);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + Manufacturer.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Manufacturers");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND Name = @name");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@name", System.Data.DbType.String, name);
            //EXECUTE THE COMMAND
            Manufacturer manufacturer = null;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    manufacturer = new Manufacturer();
                    Manufacturer.LoadDataReader(manufacturer, dr);
                }
                dr.Close();
            }
            if ((manufacturer == null) && create)
            {
                manufacturer = new Manufacturer();
                manufacturer.Name = name;
                manufacturer.Save();
            }
            return manufacturer;
        }
        
        /// <summary>
        /// Moves all products from one manufacturer to another.
        /// </summary>
        /// <param name="oldManufacturerId">The manufacturer that products are to be moved from.</param>
        /// <param name="newManufacturerId">The manufacturer that products are to be moved to.</param>
        public static void MoveProducts(int oldManufacturerId, int newManufacturerId)
        {            
            if (oldManufacturerId != newManufacturerId)
            {
                int storeId = Token.Instance.StoreId;
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand("UPDATE ac_Products SET ManufacturerId = @newManufacturerId WHERE StoreId=@storeId AND ManufacturerId=@oldManufacturerId");
                if (newManufacturerId > 0)
                {
                    database.AddInParameter(selectCommand, "@newManufacturerId", System.Data.DbType.Int32, newManufacturerId);
                }
                else
                {
                    database.AddInParameter(selectCommand, "@newManufacturerId", System.Data.DbType.Int32, null);
                }
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
                database.AddInParameter(selectCommand, "@oldManufacturerId", System.Data.DbType.Int32, oldManufacturerId);
                database.ExecuteNonQuery(selectCommand);
            }
        }

        /// <summary>
        /// Finds manufacturers with names matching the given string pattern.
        /// </summary>
        /// <param name="searchPattern">String pattern to search in manufacturer names</param>
        /// <returns>A collection of manufacturers matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ManufacturerCollection FindManufacturersByName(string searchPattern)
        {
            return FindManufacturersByName(searchPattern, 0, 0, string.Empty);
        }

        /// <summary>
        /// Finds manufacturers with names matching the given string pattern. 
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of manufacturers matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ManufacturerCollection FindManufacturersByName(string searchPattern, string sortExpression)
        {
            return FindManufacturersByName(searchPattern, 0, 0, sortExpression);
        }

        /// <summary>
        /// Finds manufacturers with names matching the given string pattern. 
        /// </summary>
        /// <param name="searchPattern">String pattern to search in manufacturer names</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of manufacturers matching the search criteria</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ManufacturerCollection FindManufacturersByName(string searchPattern, int maximumRows, int startRowIndex, string sortExpression)
        {
            ManufacturerCollection manufacturerCollection = new ManufacturerCollection();
            searchPattern = StringHelper.FixSearchPattern(searchPattern, false).ToLowerInvariant();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT            
            string whereClause;
            if (!string.IsNullOrEmpty(searchPattern))
            {
                whereClause = " WHERE LOWER(M.Name) LIKE @searchPattern ";
            }
            else
            {
                whereClause = string.Empty;
            }
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" M.ManufacturerId AS ManufacturerId");
            selectQuery.Append(" FROM ac_Manufacturers M ");

            if (!String.IsNullOrEmpty(sortExpression) && sortExpression.Contains("COUNT(P.ProductId)"))
            {
                selectQuery.Append(" LEFT OUTER JOIN ac_Products P ");
                selectQuery.Append(" ON M.ManufacturerId = P.ManufacturerId ");
                selectQuery.Append(whereClause);
                selectQuery.Append(" GROUP BY M.ManufacturerId ");
                selectQuery.Append(" ORDER BY " + sortExpression);
            }
            else
                if (!String.IsNullOrEmpty(sortExpression))
                {
                    selectQuery.Append(whereClause);
                    selectQuery.Append(" ORDER BY " + sortExpression);
                }
                else
                    selectQuery.Append(whereClause);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            if (!string.IsNullOrEmpty(searchPattern))
            {
                database.AddInParameter(selectCommand, "searchPattern", DbType.String, searchPattern);
            }
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Manufacturer manufacturer = null;
                        manufacturer = ManufacturerDataSource.Load(AlwaysConvert.ToInt(dr["ManufacturerId"]));
                        if (manufacturer != null)
                            manufacturerCollection.Add(manufacturer);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return manufacturerCollection;
        }
    }
}
