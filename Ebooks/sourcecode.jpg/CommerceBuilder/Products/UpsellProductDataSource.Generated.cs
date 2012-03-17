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

namespace CommerceBuilder.Products
{
    /// <summary>
    /// DataSource class for UpsellProduct objects
    /// </summary>
    public partial class UpsellProductDataSource
    {
        /// <summary>
        /// Deletes a UpsellProduct object from the database
        /// </summary>
        /// <param name="upsellProduct">The UpsellProduct object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(UpsellProduct upsellProduct)
        {
            return upsellProduct.Delete();
        }

        /// <summary>
        /// Deletes a UpsellProduct object with given id from the database
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to delete.</param>
        /// <param name="childProductId">Value of ChildProductId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 productId, Int32 childProductId)
        {
            UpsellProduct upsellProduct = new UpsellProduct();
            if (upsellProduct.Load(productId, childProductId)) return upsellProduct.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a UpsellProduct object to the database.
        /// </summary>
        /// <param name="upsellProduct">The UpsellProduct object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(UpsellProduct upsellProduct) { return upsellProduct.Save(); }

        /// <summary>
        /// Loads a UpsellProduct object for given key from the database.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <param name="childProductId">Value of ChildProductId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded UpsellProduct object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProduct Load(Int32 productId, Int32 childProductId)
        {
            UpsellProduct upsellProduct = new UpsellProduct();
            if (upsellProduct.Load(productId, childProductId)) return upsellProduct;
            return null;
        }

        /// <summary>
        /// Counts the number of UpsellProduct objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the UpsellProduct objects that should be loaded.</param>
        /// <returns>The number of UpsellProduct objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_UpsellProducts" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + UpsellProduct.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UpsellProducts");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            UpsellProductCollection results = new UpsellProductCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        UpsellProduct upsellProduct = new UpsellProduct();
                        UpsellProduct.LoadDataReader(upsellProduct, dr);
                        results.Add(upsellProduct);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of UpsellProduct objects for the given ProductId in the database.
        /// <param name="productId">The given ProductId</param>
        /// </summary>
        /// <returns>The Number of UpsellProduct objects for the given ProductId in the database.</returns>
        public static int CountForProduct(Int32 productId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_UpsellProducts WHERE ProductId = @productId");
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForProduct(Int32 productId)
        {
            return LoadForProduct(productId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForProduct(Int32 productId, string sortExpression)
        {
            return LoadForProduct(productId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex)
        {
            return LoadForProduct(productId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of UpsellProduct objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of UpsellProduct objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static UpsellProductCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + UpsellProduct.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UpsellProducts");
            selectQuery.Append(" WHERE ProductId = @productId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            UpsellProductCollection results = new UpsellProductCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        UpsellProduct upsellProduct = new UpsellProduct();
                        UpsellProduct.LoadDataReader(upsellProduct, dr);
                        results.Add(upsellProduct);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the next value of the OrderBy field for UpsellProduct objects.
        /// </summary>
        /// <param name="productId">The ProductId for which to get the next OrderBy value</param>
        /// <returns>The next value of the OrderBy field for UpsellProduct objects</returns>
        public static short GetNextOrderBy(Int32 productId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT (Max(OrderBy) + 1) AS NextOrderBy FROM ac_UpsellProducts WHERE ProductId = @productId"))
            {
                database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                object result = database.ExecuteScalar(selectCommand);
                if (result.Equals(DBNull.Value)) return 1;
                return (short)(int)result;
            }
        }

        /// <summary>
        /// Updates/Saves the given UpsellProduct object to the database.
        /// </summary>
        /// <param name="upsellProduct">The UpsellProduct object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(UpsellProduct upsellProduct) { return upsellProduct.Save(); }

    }
}