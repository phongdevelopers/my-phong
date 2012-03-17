using System;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// This class represents a Catalog Node of type product
    /// </summary>
    public class CatalogProductNode : CatalogNode
    {
        private LSDecimal _Price;
        /// <summary>
        /// Price of the product represented by this node
        /// </summary>
        public LSDecimal Price
        {
            get { return _Price; }
            set { _Price = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryId">The category Id of this node</param>
        /// <param name="catalogNodeId">The catalog node id of this node</param>
        /// <param name="catalogNodeType">The type of this catalog node</param>
        /// <param name="orderBy">The value of OrderBy field</param>
        /// <param name="isDirty">Does this node need to be saved to the database?</param>
        /// <param name="visibility">The visibility setting of this node</param>
        /// <param name="name">The name of this node</param>
        /// <param name="summary">The summary field of this node</param>
        /// <param name="thumbnailUrl">The thumbnail url of this node</param>
        /// <param name="thumbnailAltText">The alt text for the thumbnail of this node</param>
        /// <param name="price">The Price of the associated product</param>
        public CatalogProductNode(int categoryId, int catalogNodeId, CatalogNodeType catalogNodeType, Int16 orderBy, bool isDirty, CatalogVisibility visibility, string name, string summary, string thumbnailUrl, string thumbnailAltText, LSDecimal price)
            : base(categoryId, catalogNodeId, catalogNodeType, orderBy, isDirty, visibility, name, summary, thumbnailUrl, thumbnailAltText)
        {
            this.Price = price;
        }

    }
}
