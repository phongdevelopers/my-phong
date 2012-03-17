using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// CmsPathNode
    /// </summary>
    public class CmsPathNode
    {
        private int _NodeId;
        private CatalogNodeType _NodeType;
        private string _Title;
        private string _Description;
        private string _Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeId">Id of the node</param>
        /// <param name="nodeType">node type</param>
        /// <param name="url">Url of the node</param>
        /// <param name="title">Node title</param>
        /// <param name="description">Description of the node</param>
        public CmsPathNode(int nodeId, CatalogNodeType nodeType, string url, string title, string description)
        {
            this._NodeId = nodeId;
            this._NodeType = nodeType;
            this._Url = url;
            this._Title = title;
            this._Description = description;
        }

        /// <summary>
        /// Id of the node
        /// </summary>
        public int NodeId
        {
            get { return this._NodeId; }
            set { this._NodeId = value; }
        }

        /// <summary>
        /// Type of the node
        /// </summary>
        public CatalogNodeType NodeType
        {
            get { return this._NodeType; }
            set { this._NodeType = value; }
        }

        /// <summary>
        /// Url of the Node
        /// </summary>
        public String Url
        {
            get { return this._Url; }
            set { this._Url = value; }
        }

        /// <summary>
        /// Title of the node
        /// </summary>
        public String Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        /// <summary>
        /// Description of the node
        /// </summary>
        public String Description
        {
            get { return this._Description; }
            set { this._Description = value; }
        }

    }
}
