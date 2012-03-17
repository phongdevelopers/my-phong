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
    /// DataSource class for Link objects
    /// </summary>
    [DataObject(true)]
    public partial class LinkDataSource
    {
        /// <summary>
        /// Loads a list of orphaned Link objects
        /// </summary>
        /// <returns>A list of orphaned Link objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Link> LoadOrphaned()
        {
            return LoadOrphaned(0, 0, String.Empty);
        }

        /// <summary>
        /// Loads a list of orphaned Link objects
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of orphaned Link objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Link> LoadOrphaned(string sortExpression)
        {
            return LoadOrphaned(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a list of orphaned Link objects
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A list of orphaned Link objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<Link> LoadOrphaned(int maximumRows, int startRowIndex, string sortExpression)
        {
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + Link.GetColumnNames("") + " FROM ");
            selectQuery.Append(" ac_Links");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (LinkId NOT IN");
            selectQuery.Append(" (SELECT DISTINCT CatalogNodeId FROM ac_CatalogNodes WHERE (CatalogNodeTypeId = @catalogNodeTypeId))");
            selectQuery.Append(" )");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);


            //BUILD THE DBCOMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "catalogNodeTypeId", DbType.Int16, CatalogNodeType.Link);

            //EXECUTE THE COMMAND
            List<Link> results = new List<Link>();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        Link link = new Link();
                        Link.LoadDataReader(link, dr);
                        results.Add(link);
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
