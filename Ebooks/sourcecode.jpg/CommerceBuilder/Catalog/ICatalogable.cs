using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Objects that can be cataloged implement this interface. 
    /// For example Categories, Products, Webpages and Links
    /// </summary>
    public interface ICatalogable
    {
        /// <summary>
        /// The visibility status of the catalog node object
        /// </summary>
        byte VisibilityId { get; set; }

        /// <summary>
        /// The visibility status of the catalog node object
        /// </summary>
        CatalogVisibility Visibility { get; set; }

        /// <summary>
        /// Name of the catalog node object
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Summary description of the catalog node object
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// Detailed description of the catalog node object
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Thumbnail URL of the catalog node object
        /// </summary>
        string ThumbnailUrl { get; set; }

        /// <summary>
        /// Alt Text of the Thumbnail URL of the catalog node object
        /// </summary>
        string ThumbnailAltText { get; set; }

        /// <summary>
        /// Navigation URL of the catalog node object
        /// </summary>
        string NavigateUrl { get; }

        /// <summary>
        /// HTML Head data for the catalog node object
        /// </summary>
        string HtmlHead { get; set; }

        /// <summary>
        /// Theme set for the catalog node object
        /// </summary>
        string Theme { get; set; }

        /// <summary>
        /// Display Page of the catalog node object
        /// </summary>
        string DisplayPage { get; set; }
    }
}
