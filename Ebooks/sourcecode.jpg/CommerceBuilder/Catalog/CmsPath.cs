using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Utility class for getting cms path
    /// </summary>
    public static class CmsPath
    {
        /// <summary>
        /// Gets cms path for the given node. Only supported for Category type catalog node objects
        /// </summary>
        /// <param name="categoryId">Id of the category</param>
        /// <param name="currentNodeId">Id of the node</param>
        /// <param name="currentNodeType">Type of node</param>
        /// <returns>List of cms path nodes</returns>
        public static List<CmsPathNode> GetCmsPath(int categoryId, int currentNodeId, CatalogNodeType currentNodeType)
        {
            List<CmsPathNode> path = new List<CmsPathNode>();
            Database database = Token.Instance.Database;
            StringBuilder selectQuery = new StringBuilder();

            switch (currentNodeType)
            {
                case CatalogNodeType.Category :
                    selectQuery.Append("SELECT CP.ParentId AS NodeId, 0 As NodeType, C.Name, C.Summary, CP.ParentLevel, CP.ParentNumber");
                    selectQuery.Append(" FROM ac_CategoryParents AS CP INNER JOIN");
                    selectQuery.Append(" ac_Categories C ON CP.ParentId = C.CategoryId");
                    selectQuery.Append(" WHERE CP.CategoryId = @currentNodeId");
                    selectQuery.Append(" ORDER BY CP.ParentLevel");
                    break;
                default:
                    throw new Exception("Not supported.");
            }
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@currentNodeId", DbType.Int32, currentNodeId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                int nodeId;
                CatalogNodeType nodeType;
                String url;
                String title;
                String description;
                while (dr.Read())
                {
                    nodeId = dr.GetInt32(0);
                    nodeType = (CatalogNodeType)dr.GetInt32(1);
                    switch (nodeType) {
                        case CatalogNodeType.Category:
                            url = "~/Admin/Catalog/Browse.aspx?CategoryId=" + nodeId.ToString();
                            break;
                        default:
                            url = string.Empty;
                            break;
                    }
                    title = dr.GetString(2);
                    description = NullableData.GetString(dr, 3);
                    path.Add(new CmsPathNode(nodeId, nodeType, url, title, description));
                }
                dr.Close();
            }
            if (path.Count == 0) return null;
            return path;
        }

    }
}
