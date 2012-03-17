using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// DataSource class for ProductInventory objects
    /// </summary>
    [DataObject(true)]
    public static class ProductInventoryDataSource
    {
        /// <summary>
        /// Get all inventory items that are at or below the reorder point.
        /// </summary>
        /// <returns>A list of InventoryDataItem of all items at or below the reorder point.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetLowProductInventory()
        {
            return GetLowProductInventory(0, 0, string.Empty);
        }

        /// <summary>
        /// Get all inventory items that are at or below the reorder point.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of InventoryDataItem of all items at or below the reorder point.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetLowProductInventory(string sortExpression)
        {
            return GetLowProductInventory(0, 0, sortExpression);
        }
        
        /// <summary>
        /// Get all inventory items that are at or below the reorder point.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of InventoryDataItem of all items at or below the reorder point.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetLowProductInventory(int maximumRows, int startRowIndex, string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name, VariantName";
            if (sortExpression.ToUpperInvariant().Equals("NAME"))
            {
                sortExpression = "Name, VariantName";
            }
            else if (sortExpression.ToUpperInvariant().Equals("NAME DESC"))
            {
                sortExpression = "Name DESC, VariantName DESC";
            }
            List<ProductInventoryDetail> results = new List<ProductInventoryDetail>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ProductId, Name, NULL AS ProductVariantId, NULL As VariantName, InStock, InStockWarningLevel");
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE InStock <= InStockWarningLevel");
            selectQuery.Append(" AND InventoryModeId = 1");
            selectQuery.Append(" UNION");
            selectQuery.Append(" SELECT V.ProductId, P.Name, V.ProductVariantId, V.VariantName, V.InStock, V.InStockWarningLevel ");
            selectQuery.Append(" FROM ac_ProductVariants V, ac_Products P");
            selectQuery.Append(" WHERE V.ProductId = P.ProductId");
            selectQuery.Append(" AND P.InventoryModeId = 2");
            selectQuery.Append(" AND V.InStock <= V.InStockWarningLevel");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        results.Add(new ProductInventoryDetail(dr.GetInt32(0), dr.GetString(1), NullableData.GetInt32(dr, 2), NullableData.GetString(dr, 3), dr.GetInt32(4), dr.GetInt32(5)));
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the number of items that are at or below the reorder point.
        /// </summary>
        /// <returns>The number of items that are at or below the reorder point.</returns>
        public static int GetLowProductInventoryCount()
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS SubTotal");
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE InStock <= InStockWarningLevel");
            selectQuery.Append(" AND InventoryModeId = 1");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            int count1 = CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS SubTotal");
            selectQuery.Append(" FROM ac_ProductVariants V, ac_Products P");
            selectQuery.Append(" WHERE V.ProductId = P.ProductId");
            selectQuery.Append(" AND P.InventoryModeId = 2");
            selectQuery.Append(" AND V.InStock <= V.InStockWarningLevel");
            selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            int count2 = CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            return (count1 + count2);
        }

        /// <summary>
        /// Get all inventory items that are at or below the given inventory level.
        /// </summary>
        /// <param name="maxInStock">The given inventory level</param>
        /// <returns>A list of all inventory items that are at or below the given inventory level.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetProductInventory(int maxInStock)
        {
            return GetProductInventory(maxInStock, 0, 0, string.Empty);
        }

        /// <summary>
        /// Get all inventory items that are at or below the given inventory level.
        /// </summary>
        /// <param name="maxInStock">The given inventory level</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of all inventory items that are at or below the given inventory level.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetProductInventory(int maxInStock, string sortExpression)
        {
            return GetProductInventory(maxInStock, 0, 0, sortExpression);
        }

        /// <summary>
        /// Get all inventory items that are at or below the given inventory level.
        /// </summary>
        /// <param name="maxInStock">The given inventory level</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of all inventory items that are at or below the given inventory level.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<ProductInventoryDetail> GetProductInventory(int maxInStock, int maximumRows, int startRowIndex, string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "Name, VariantName";
            if (sortExpression.ToUpperInvariant().Equals("NAME"))
            {
                sortExpression = "Name, VariantName";
            }
            else if (sortExpression.ToUpperInvariant().Equals("NAME DESC"))
            {
                sortExpression = "Name DESC, VariantName DESC";
            }
            List<ProductInventoryDetail> results = new List<ProductInventoryDetail>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ProductId, Name, NULL AS ProductVariantId, NULL As VariantName, InStock, InStockWarningLevel");
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE InStock <= @maxInStock");
            selectQuery.Append(" AND InventoryModeId = 1");
            selectQuery.Append(" UNION");
            selectQuery.Append(" SELECT V.ProductId, P.Name, V.ProductVariantId, V.VariantName, V.InStock, V.InStockWarningLevel ");
            selectQuery.Append(" FROM ac_ProductVariants V, ac_Products P");
            selectQuery.Append(" WHERE V.ProductId = P.ProductId");
            selectQuery.Append(" AND P.InventoryModeId = 2");
            selectQuery.Append(" AND V.InStock <= @maxInStock");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "maxInStock", DbType.Int32, maxInStock);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        results.Add(new ProductInventoryDetail(dr.GetInt32(0), dr.GetString(1), NullableData.GetInt32(dr, 2), NullableData.GetString(dr, 3), dr.GetInt32(4), dr.GetInt32(5)));
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of inventory items that are at or below the given inventory level.
        /// </summary>
        /// <param name="maxInStock">The given inventory level</param>
        /// <returns></returns>
        public static int GetProductInventoryCount(int maxInStock)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT SUM(SubTotal) AS Total FROM (SELECT COUNT(*) AS SubTotal");
            selectQuery.Append(" FROM ac_Products");
            selectQuery.Append(" WHERE InStock <= @maxInStock");
            selectQuery.Append(" AND InventoryModeId = 1");
            selectQuery.Append(" UNION");
            selectQuery.Append(" SELECT COUNT(*) AS SubTotal");
            selectQuery.Append(" FROM ac_ProductVariants V, ac_Products P");
            selectQuery.Append(" WHERE V.ProductId = P.ProductId");
            selectQuery.Append(" AND P.InventoryModeId = 2");
            selectQuery.Append(" AND V.InStock <= @maxInStock) AS SubTotals");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "maxInStock", DbType.Int32, maxInStock);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

    }
}
