//Generated by DataSourceBaseGenerator_AssnWithColumns

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
namespace CommerceBuilder.Products
{

    /// <summary>
    /// DataSource class for ProductOption objects
    /// </summary>
    public partial class ProductOptionDataSource
    {
        /// <summary>
        /// Deletes a ProductOption object from the database
        /// </summary>
        /// <param name="productOption">The ProductOption object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(ProductOption productOption)
        {
            return productOption.Delete();
        }

        /// <summary>
        /// Deletes a ProductOption object with given id from the database
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to delete.</param>
        /// <param name="optionId">Value of OptionId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 productId, Int32 optionId)
        {
            ProductOption productOption = new ProductOption();
            if (productOption.Load(productId, optionId)) return productOption.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a ProductOption object to the database.
        /// </summary>
        /// <param name="productOption">The ProductOption object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(ProductOption productOption) { return productOption.Save(); }

        /// <summary>
        /// Load a ProductOption object for the given primary key from the database.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <param name="optionId">Value of OptionId of the object to load.</param>
        /// <returns>The loaded ProductOption object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductOption Load(Int32 productId, Int32 optionId)
        {
            ProductOption productOption = null;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + ProductOption.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductOptions");
            selectQuery.Append(" WHERE ProductId = @productId AND OptionId = @optionId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@optionId", System.Data.DbType.Int32, optionId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    productOption = new ProductOption();
                    ProductOption.LoadDataReader(productOption, dr);
                }
                dr.Close();
            }
            return productOption;
        }

        /// <summary>
        /// Loads a collection of ProductOption objects for the given criteria for Option from the database.
        /// </summary>
        /// <param name="optionId">Value of OptionId of the object to load.</param>
        /// <returns>A collection of ProductOption objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductOptionCollection LoadForOption(Int32 optionId)
        {
            ProductOptionCollection ProductOptions = new ProductOptionCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + ProductOption.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductOptions");
            selectQuery.Append(" WHERE OptionId = @optionId");
            selectQuery.Append(" ORDER BY OrderBy");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@optionId", System.Data.DbType.Int32, optionId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ProductOption productOption = new ProductOption();
                    ProductOption.LoadDataReader(productOption, dr);
                    ProductOptions.Add(productOption);
                }
                dr.Close();
            }
            return ProductOptions;
        }

        /// <summary>
        /// Loads a collection of ProductOption objects for the given criteria for Product from the database.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <returns>A collection of ProductOption objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ProductOptionCollection LoadForProduct(Int32 productId)
        {
            ProductOptionCollection ProductOptions = new ProductOptionCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT!
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + ProductOption.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductOptions");
            selectQuery.Append(" WHERE ProductId = @productId");
            selectQuery.Append(" ORDER BY OrderBy");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ProductOption productOption = new ProductOption();
                    ProductOption.LoadDataReader(productOption, dr);
                    ProductOptions.Add(productOption);
                }
                dr.Close();
            }
            return ProductOptions;
        }

        /// <summary>
        /// Updates/Saves the given ProductOption object to the database.
        /// </summary>
        /// <param name="productOption">The ProductOption object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(ProductOption productOption) { return productOption.Save(); }

        /// <summary>
        /// Gets the next value of the OrderBy field for ProductOption objects.
        /// </summary>
        /// <param name="productId">The ProductId for which to get the next OrderBy value</param>
        /// <returns>The next value of the OrderBy field for ProductOption objects</returns>
        public static short GetNextOrderBy(Int32 productId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT (Max(OrderBy) + 1) AS NextOrderBy FROM ac_ProductOptions WHERE ProductId = @productId"))
            {
                database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
                object result = database.ExecuteScalar(selectCommand);
                if (result.Equals(DBNull.Value)) return 1;
                return (short)(int)result;
            }
        }

    }
}
