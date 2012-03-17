using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// DataSource class for KitComponent objects
    /// </summary>
    [DataObject(true)]
    public partial class KitComponentDataSource
    {
        /// <summary>
        /// Searches for kit components for given product name and component name
        /// </summary>
        /// <param name="productName">Name of the product to search for</param>
        /// <param name="componentName">Name of the component to search for</param>
        /// <returns>A collection of KitComponent objects for given product name and component name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static KitComponentCollection Search(string productName, string componentName)
        {
            return Search(productName, componentName, 0, 0, string.Empty);
        }

        /// <summary>
        /// Searches for kit components for given product name and component name
        /// </summary>
        /// <param name="productName">Name of the product to search for</param>
        /// <param name="componentName">Name of the component to search for</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of KitComponent objects for given product name and component name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static KitComponentCollection Search(string productName, string componentName, string sortExpression)
        {
            return Search(productName, componentName, 0, 0, sortExpression);
        }

        /// <summary>
        /// Searches for kit components for given product name and component name
        /// </summary>
        /// <param name="productName">Name of the product to search for</param>
        /// <param name="componentName">Name of the component to search for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <returns>A collection of KitComponent objects for given product name and component name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static KitComponentCollection Search(string productName, string componentName, int maximumRows, int startRowIndex)
        {
            return Search(productName, componentName, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Searches for kit components for given product name and component name
        /// </summary>
        /// <param name="productName">Name of the product to search for</param>
        /// <param name="componentName">Name of the component to search for</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>A collection of KitComponent objects for given product name and component name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static KitComponentCollection Search(string productName, string componentName, int maximumRows, int startRowIndex, string sortExpression)
        {
            KitComponentCollection componentList = new KitComponentCollection();
            //CREATE THE SQL STATEMENT
            productName = StringHelper.FixSearchPattern(productName);
            componentName = StringHelper.FixSearchPattern(componentName);
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + KitComponent.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_KitComponents WHERE KitComponentId IN (SELECT DISTINCT C.KitComponentId");
            selectQuery.Append(" FROM ((ac_KitComponents C INNER JOIN ac_ProductKitComponents PC ON C.KitComponentId = PC.KitComponentId)");
            selectQuery.Append(" INNER JOIN ac_Products P ON PC.ProductId = P.ProductId)");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(productName)) selectQuery.Append(" AND P.Name LIKE @productName");
            if (!string.IsNullOrEmpty(componentName)) selectQuery.Append(" AND C.Name LIKE @componentName");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            selectQuery.Append(")");
            //CREATE THE COMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(productName)) database.AddInParameter(selectCommand, "@productName", System.Data.DbType.String, productName);
            if (!string.IsNullOrEmpty(componentName)) database.AddInParameter(selectCommand, "@componentName", System.Data.DbType.String, componentName);
            //EXECUTE THE COMMAND
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        KitComponent component = new KitComponent();
                        KitComponent.LoadDataReader(component, dr);
                        componentList.Add(component);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return componentList;
        }

        /// <summary>
        /// Counts the number of kit components for given product name and component name
        /// </summary>
        /// <param name="productName">Name of the product to search for</param>
        /// <param name="componentName">Name of the component to search for</param>
        /// <returns>The number of kit components matching the search criteria</returns>
        public static int SearchCount(string productName, string componentName)
        {
            productName = StringHelper.FixSearchPattern(productName);
            componentName = StringHelper.FixSearchPattern(componentName);
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) AS TotalCount");
            selectQuery.Append(" FROM ac_KitComponents WHERE KitComponentId IN (SELECT DISTINCT C.KitComponentId");
            selectQuery.Append(" FROM ((ac_KitComponents C INNER JOIN ac_ProductKitComponents PC ON C.KitComponentId = PC.KitComponentId)");
            selectQuery.Append(" INNER JOIN ac_Products P ON PC.ProductId = P.ProductId)");
            selectQuery.Append(" WHERE P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(productName)) selectQuery.Append(" AND P.Name LIKE @productName");
            if (!string.IsNullOrEmpty(componentName)) selectQuery.Append(" AND C.Name LIKE @componentName");
            selectQuery.Append(")");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(productName)) database.AddInParameter(selectCommand, "@productName", System.Data.DbType.String, productName);
            if (!string.IsNullOrEmpty(componentName)) database.AddInParameter(selectCommand, "@componentName", System.Data.DbType.String, componentName);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of KitComponent objects for a given member product
        /// </summary>
        /// <param name="productId">Id of the member product for which to load the kit components</param>
        /// <returns>A collection of KitComponent objects for which the the given product is a member</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static KitComponentCollection LoadForMemberProduct(int productId)
        {
            KitComponentCollection kitComponents = new KitComponentCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + KitComponent.GetColumnNames("ac_KitComponents"));
            selectQuery.Append(" FROM ac_KitComponents, ac_KitProducts");
            selectQuery.Append(" WHERE ac_KitComponents.KitComponentId = ac_KitProducts.KitComponentId");
            selectQuery.Append(" AND ac_KitProducts.ProductId = @ProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    KitComponent kitKitComponent = new KitComponent();
                    KitComponent.LoadDataReader(kitKitComponent, dr);
                    kitComponents.Add(kitKitComponent);
                }
                dr.Close();
            }
            return kitComponents;
        }

        /// <summary>
        /// Counts the number of KitComponent objects for a given member product
        /// </summary>
        /// <param name="productId">Id of the member product for which to count the kit components</param>
        /// <returns>The number of KitComponent objects for which the the given product is a member</returns>
        public static int CountForMemberProduct(int productId)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT COUNT(*) As TotalCount");
            selectQuery.Append(" FROM ac_KitComponents, ac_KitProducts");
            selectQuery.Append(" WHERE ac_KitComponents.KitComponentId = ac_KitProducts.KitComponentId");
            selectQuery.Append(" AND ac_KitProducts.ProductId = @productId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
    }
}
