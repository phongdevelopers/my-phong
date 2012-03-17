using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Catalog;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Represents a CatalogPathNode
    /// </summary>
    public class CatalogPathNode : CatalogNode
    {
        private string _Theme = string.Empty;
        /// <summary>
        /// Theme applied to this node
        /// </summary>
        public string Theme
        {
            get { return _Theme; }
            set { _Theme = value; }
        }


        new private bool IsDirty { get { throw new CommerceBuilder.Exceptions.CommerceBuilderException("unsupported"); } }
        new private int OrderBy { get { throw new CommerceBuilder.Exceptions.CommerceBuilderException("unsupported"); } }

        new private CommerceBuilder.Common.SaveResult Save()
        {
            throw new CommerceBuilder.Exceptions.CommerceBuilderException("CatalogPathNode is a read-only object.");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryId">Category Id for this path node</param>
        /// <param name="catalogNodeId">CatalogNode Id for this path node</param>
        /// <param name="catalogNodeType">CatalogNode type for this path node</param>
        /// <param name="visibility">Visibility setting for this path node</param>
        /// <param name="name">Name of this path node</param>
        /// <param name="summary">Summary of this path node</param>
        /// <param name="thumbnailUrl">Thumbnail URL for this path node</param>
        /// <param name="thumbnailAltText">Alternate text for the thumbnail for this path node</param>
        /// <param name="theme">The theme applied to this path node</param>
        public CatalogPathNode(int categoryId, int catalogNodeId, CatalogNodeType catalogNodeType, CatalogVisibility visibility, string name, string summary, string thumbnailUrl, string thumbnailAltText, string theme) 
        {
            this.CategoryId = categoryId;
            this.CatalogNodeId = catalogNodeId;
            this.CatalogNodeType = catalogNodeType;
            this.Visibility = visibility;
            this.Name = name;
            this.Summary = summary;
            this.ThumbnailUrl = thumbnailUrl;
            this.ThumbnailAltText = thumbnailAltText;
            this.Theme = theme;
        }
    }
}
