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
    /// DataSource class for ProductReview objects
    /// </summary>
    public partial class ProductReviewDataSource
    {
        /// <summary>
        /// Deletes a ProductReview object from the database
        /// </summary>
        /// <param name="productReview">The ProductReview object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(ProductReview productReview)
        {
            return productReview.Delete();
        }

        /// <summary>
        /// Deletes a ProductReview object with given id from the database
        /// </summary>
        /// <param name="productReviewId">Value of ProductReviewId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 productReviewId)
        {
            ProductReview productReview = new ProductReview();
            if (productReview.Load(productReviewId)) return productReview.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a ProductReview object to the database.
        /// </summary>
        /// <param name="productReview">The ProductReview object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(ProductReview productReview) { return productReview.Save(); }

        /// <summary>
        /// Loads a ProductReview object for given Id from the database.
        /// </summary>
        /// <param name="productReviewId">Value of ProductReviewId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded ProductReview object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReview Load(Int32 productReviewId)
        {
            return ProductReviewDataSource.Load(productReviewId, true);
        }

        /// <summary>
        /// Loads a ProductReview object for given Id from the database.
        /// </summary>
        /// <param name="productReviewId">Value of ProductReviewId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded ProductReview object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReview Load(Int32 productReviewId, bool useCache)
        {
            if (productReviewId == 0) return null;
            ProductReview productReview = null;
            string key = "ProductReview_" + productReviewId.ToString();
            if (useCache)
            {
                productReview = ContextCache.GetObject(key) as ProductReview;
                if (productReview != null) return productReview;
            }
            productReview = new ProductReview();
            if (productReview.Load(productReviewId))
            {
                if (useCache) ContextCache.SetObject(key, productReview);
                return productReview;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of ProductReview objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the ProductReview objects that should be loaded.</param>
        /// <returns>The number of ProductReview objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ProductReviews" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of ProductReview objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            ProductReviewCollection results = new ProductReviewCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductReview productReview = new ProductReview();
                        ProductReview.LoadDataReader(productReview, dr);
                        results.Add(productReview);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of ProductReview objects for the given ProductId in the database.
        /// <param name="productId">The given ProductId</param>
        /// </summary>
        /// <returns>The Number of ProductReview objects for the given ProductId in the database.</returns>
        public static int CountForProduct(Int32 productId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ProductReviews WHERE ProductId = @productId");
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForProduct(Int32 productId)
        {
            return LoadForProduct(productId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForProduct(Int32 productId, string sortExpression)
        {
            return LoadForProduct(productId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex)
        {
            return LoadForProduct(productId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ProductId from the database
        /// </summary>
        /// <param name="productId">The given ProductId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForProduct(Int32 productId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" WHERE ProductId = @productId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            ProductReviewCollection results = new ProductReviewCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductReview productReview = new ProductReview();
                        ProductReview.LoadDataReader(productReview, dr);
                        results.Add(productReview);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of ProductReview objects for the given ReviewerProfileId in the database.
        /// <param name="reviewerProfileId">The given ReviewerProfileId</param>
        /// </summary>
        /// <returns>The Number of ProductReview objects for the given ReviewerProfileId in the database.</returns>
        public static int CountForReviewerProfile(Int32 reviewerProfileId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ProductReviews WHERE ReviewerProfileId = @reviewerProfileId");
            database.AddInParameter(selectCommand, "@reviewerProfileId", System.Data.DbType.Int32, reviewerProfileId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ReviewerProfileId from the database
        /// </summary>
        /// <param name="reviewerProfileId">The given ReviewerProfileId</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForReviewerProfile(Int32 reviewerProfileId)
        {
            return LoadForReviewerProfile(reviewerProfileId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ReviewerProfileId from the database
        /// </summary>
        /// <param name="reviewerProfileId">The given ReviewerProfileId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForReviewerProfile(Int32 reviewerProfileId, string sortExpression)
        {
            return LoadForReviewerProfile(reviewerProfileId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ReviewerProfileId from the database
        /// </summary>
        /// <param name="reviewerProfileId">The given ReviewerProfileId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForReviewerProfile(Int32 reviewerProfileId, int maximumRows, int startRowIndex)
        {
            return LoadForReviewerProfile(reviewerProfileId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of ProductReview objects for the given ReviewerProfileId from the database
        /// </summary>
        /// <param name="reviewerProfileId">The given ReviewerProfileId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of ProductReview objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductReviewCollection LoadForReviewerProfile(Int32 reviewerProfileId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + ProductReview.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductReviews");
            selectQuery.Append(" WHERE ReviewerProfileId = @reviewerProfileId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@reviewerProfileId", System.Data.DbType.Int32, reviewerProfileId);
            //EXECUTE THE COMMAND
            ProductReviewCollection results = new ProductReviewCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        ProductReview productReview = new ProductReview();
                        ProductReview.LoadDataReader(productReview, dr);
                        results.Add(productReview);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given ProductReview object to the database.
        /// </summary>
        /// <param name="productReview">The ProductReview object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(ProductReview productReview) { return productReview.Save(); }

    }
}