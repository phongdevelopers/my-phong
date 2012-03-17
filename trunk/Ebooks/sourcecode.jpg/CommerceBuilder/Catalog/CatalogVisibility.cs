using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Enumeration that represents the visibility of a CatalogNode
    /// </summary>
    public enum CatalogVisibility : byte
    {
        /// <summary>
        /// CatalogNode is publicly visible. 
        /// </summary>
        Public, 
        
        /// <summary>
        /// CatalogNode is hidden. Can be accessed via direct link.
        /// </summary>
        Hidden, 
        
        /// <summary>
        /// CatalogNode is private. Can not be accessed.
        /// </summary>
        Private
    }

}
