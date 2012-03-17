using System.ComponentModel;
using CommerceBuilder.Data;
using CommerceBuilder.Common;
using System.Text;
using System.Data.Common;
using System.Data;
using System;

namespace CommerceBuilder.Seo
{
    /// <summary>
    /// DataSource class for Redirect objects
    /// </summary>
    [DataObject(true)]
    public partial class RedirectDataSource
    {
        /// <summary>
        /// Counts the number of fixed Redirect objects for the current store.
        /// </summary>
        /// <returns>The Number of fixed Redirect objects in the current store.</returns>
        public static int CountFixedRedirects()
        {
            return CountRedirects(false);
        }

        /// <summary>
        /// Loads a collection of fixed Redirect objects for the current store from the database
        /// </summary>
        /// <returns>A collection of fixed Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadFixedRedirects()
        {
            return LoadRedirects(false, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of fixed Redirect objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of fixed Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadFixedRedirects(string sortExpression)
        {
            return LoadRedirects(false, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of fixed Redirect objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of fixed Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadFixedRedirects(int maximumRows, int startRowIndex)
        {
            return LoadRedirects(false, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of fixed Redirect objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of fixed Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadFixedRedirects(int maximumRows, int startRowIndex, string sortExpression)
        {
            return LoadRedirects(false, maximumRows, startRowIndex, sortExpression);
        }

        /// <summary>
        /// Counts the number of dynamic Redirect objects for the current store.
        /// </summary>
        /// <returns>The Number of dynamic Redirect objects in the current store.</returns>
        public static int CountDynamicRedirects()
        {
            return CountRedirects(true);
        }

        /// <summary>
        /// Counts the number of dynamic Redirect objects for the current store.
        /// </summary>
        /// <param name="dynamic">Indicates if the dynamic or fixed redirects need to be counted. 'true' for counting dynamic redirects while 'false' for counting fixed redirects. </param>
        /// <returns>The Number of dynamic Redirect objects in the current store.</returns>
        private static int CountRedirects(bool dynamic)
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Redirects WHERE StoreId = @storeId AND UseRegEx = @useRegEx");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            database.AddInParameter(selectCommand, "@useRegEx", System.Data.DbType.Boolean, dynamic);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of dynamic Redirect objects for the current store from the database
        /// </summary>
        /// <returns>A collection of dynamic Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadDynamicRedirects()
        {
            return LoadRedirects(true, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of dynamic Redirect objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of dynamic Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadDynamicRedirects(string sortExpression)
        {
            return LoadRedirects(true, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of dynamic Redirect objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of dynamic Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadDynamicRedirects(int maximumRows, int startRowIndex)
        {
            return LoadRedirects(true, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of dynamic Redirect objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of dynamic Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static RedirectCollection LoadDynamicRedirects(int maximumRows, int startRowIndex, string sortExpression)
        {
            return LoadRedirects(true, maximumRows, startRowIndex, sortExpression);
        }


        /// <summary>
        /// Loads a collection of dynamic Redirect objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="dynamic">Indicates if the dynamic or fixed redirects need to be loaded. 'true' for loading dynamic redirects while 'false' for loading fixed redirects. </param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of dynamic Redirect objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        private static RedirectCollection LoadRedirects(bool dynamic, int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Redirect.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Redirects");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND UseRegEx = @useRegEx");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            database.AddInParameter(selectCommand, "@useRegEx", System.Data.DbType.Boolean, dynamic);
            //EXECUTE THE COMMAND
            RedirectCollection results = new RedirectCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Redirect redirect = new Redirect();
                        Redirect.LoadDataReader(redirect, dr);
                        results.Add(redirect);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Load this Redirect object from the database for the given source URL.
        /// </summary>
        /// <param name="sourceUrl">Value of SourceUrl of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public static Redirect LoadForSourceUrl(String sourceUrl)
        {
            Redirect redirect = null;
            if (string.IsNullOrEmpty(sourceUrl)) throw new ArgumentException("sourceUrl");
            string loweredSourceUrl = sourceUrl.ToLowerInvariant();

            //CREATE THE SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + Redirect.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Redirects");
            selectQuery.Append(" WHERE LoweredSourceUrl = @loweredSourceUrl");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@loweredSourceUrl", System.Data.DbType.String, loweredSourceUrl);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    redirect = new Redirect();
                    Redirect.LoadDataReader(redirect, dr); ;
                }
                dr.Close();
            }
            return redirect;
        }
    }
}
