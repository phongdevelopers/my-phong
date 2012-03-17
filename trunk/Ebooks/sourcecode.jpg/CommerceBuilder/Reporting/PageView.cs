using System;
using System.Web;
using CommerceBuilder.Catalog;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class that represents a PageView object in the database
    /// </summary>
    public partial class PageView
    {
        /// <summary>
        /// The type of catalog node associated with this PageView object
        /// </summary>
        public CatalogNodeType CatalogNodeType
        {
            get
            {
                return (CatalogNodeType)this.CatalogNodeTypeId;
            }
            set
            {
                this.CatalogNodeTypeId = (byte)value;
            }
        }

        private ICatalogable _CatalogNode;

        /// <summary>
        /// The CagalogNode object associated with this PageView object
        /// </summary>
        public ICatalogable CatalogNode
        {
            get
            {
                if (_CatalogNode == null) _CatalogNode = CatalogDataSource.Load(this.CatalogNodeId, this.CatalogNodeType);
                return _CatalogNode;
            }
        }

        /// <summary>
        /// Registers the catalog node that is primarily associated with this request
        /// </summary>
        /// <param name="nodeId">ID of the catalog node</param>
        /// <param name="nodeType">Type of the catalog node</param>
        /// <remarks>This is intended for use with page tracking</remarks>
        public static void RegisterCatalogNode(int nodeId, CatalogNodeType nodeType)
        {
            HttpContext context = HttpContext.Current;
            context.Items["PageTracking_CatalogNodeId"] = nodeId;
            context.Items["PageTracking_CatalogNodeTypeId"] = (Int16)nodeType;
        }
    }
}
