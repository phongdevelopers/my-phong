using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// DataSource class for Kit objects
    /// </summary>
    [DataObject(true)]
    public partial class KitDataSource
    {
        #region LoadForStore
        /// <summary>
        /// Counts the number of kit products in the database for the current store
        /// </summary>
        /// <returns>The number of kit products in the database for the current store</returns>
        public static int CountForStore()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Products WHERE StoreId = @storeId AND ProductId IN (SELECT DISTINCT ProductId FROM ac_ProductKitComponents)");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of kit products for the current store
        /// </summary>
        /// <returns>A collection of kit products for the current store</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForStore()
        {
            return KitDataSource.LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of kit products for the current store
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of kit products for the current store</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForStore(string sortExpression)
        {
            return KitDataSource.LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of kit products for the current store
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of kit products for the current store</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return KitDataSource.LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of kit products for the current store
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of kit products for the current store</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Product.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND ProductId IN (SELECT DISTINCT ProductId FROM ac_ProductKitComponents)");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            ProductCollection results = new ProductCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Product product = new Product();
                        Product.LoadDataReader(product, dr);
                        results.Add(product);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
        #endregion

        #region FindByName

        /// <summary>
        /// Counts the number of kit products in the database matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <returns>The number of kit products in the database matching the given name</returns>
        public static int FindByNameCount(string nameToMatch)
        {
            if (string.IsNullOrEmpty(nameToMatch)) return CountForStore();
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Products WHERE StoreId = @storeId AND ProductId IN (SELECT DISTINCT ProductId FROM ac_ProductKitComponents) AND (Name LIKE @nameToMatch)");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of kit products from the database matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <returns>A collection of kit products from the database matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection FindByName(string nameToMatch)
        {
            return KitDataSource.FindByName(nameToMatch, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of kit products from the database matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of kit products from the database matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection FindByName(string nameToMatch, string sortExpression)
        {
            return KitDataSource.FindByName(nameToMatch, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of kit products from the database matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of kit products from the database matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex)
        {
            return KitDataSource.FindByName(nameToMatch, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of kit products from the database matching the given name
        /// </summary>
        /// <param name="nameToMatch">The name to match</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of kit products from the database matching the given name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductCollection FindByName(string nameToMatch, int maximumRows, int startRowIndex, string sortExpression)
        {
            if (string.IsNullOrEmpty(nameToMatch)) return LoadForStore(maximumRows, startRowIndex, sortExpression);
            nameToMatch = StringHelper.FixSearchPattern(nameToMatch);
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Product.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND ProductId IN (SELECT DISTINCT ProductId FROM ac_ProductKitComponents)");
            selectQuery.Append(" AND (Name LIKE @nameToMatch)");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@nameToMatch", System.Data.DbType.String, nameToMatch);
            //EXECUTE THE COMMAND
            ProductCollection results = new ProductCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Product product = new Product();
                        Product.LoadDataReader(product, dr);
                        results.Add(product);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
        #endregion

    }
}
