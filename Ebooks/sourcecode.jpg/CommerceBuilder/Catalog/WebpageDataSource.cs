using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Text;
using CommerceBuilder.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using System.Data;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// DataSource class for Webpage objects
    /// </summary>
    [DataObject(true)]
    public partial class WebpageDataSource
    {
        /// <summary>
        /// Loads orphaned Webpage objects
        /// </summary>
        /// <returns>List of orphaned Webpage objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Webpage> LoadOrphaned()
        {
            return LoadOrphaned(0, 0, String.Empty);
        }

        /// <summary>
        /// Loads orphaned Webpage objects
        /// </summary>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>List of orphaned Webpage objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Webpage> LoadOrphaned(string sortExpression)
        {
            return LoadOrphaned(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads orphaned Webpage objects
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving</param>
        /// <param name="sortExpression">Sort expression to use for sorting the loaded objects</param>
        /// <returns>List of orphaned Webpage objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Webpage> LoadOrphaned(int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Webpage.GetColumnNames("") + " FROM ");
            selectQuery.Append(" ac_Webpages");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (WebpageId NOT IN");
            selectQuery.Append(" (SELECT DISTINCT CatalogNodeId FROM ac_CatalogNodes WHERE (CatalogNodeTypeId = @catalogNodeTypeId))");
            selectQuery.Append(" )");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);


            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "catalogNodeTypeId", DbType.Int16, CatalogNodeType.Webpage);

            //EXECUTE THE COMMAND
            List<Webpage> results = new List<Webpage>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Webpage webpage = new Webpage();
                        Webpage.LoadDataReader(webpage, dr);
                        results.Add(webpage);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
    }
}
