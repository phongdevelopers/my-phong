using System;
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// This class represents a Catalog Node object
    /// </summary>
    public partial class CatalogNode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryId">Category Id of this catalog node</param>
        /// <param name="catalogNodeId">Catalog node Id of this catalog node</param>
        /// <param name="catalogNodeType">Type of this catalog node</param>
        /// <param name="orderBy">The value of OrderBy field</param>
        /// <param name="isDirty">Does this catalog node need to be saved to database or not</param>
        public CatalogNode(int categoryId, int catalogNodeId, CatalogNodeType catalogNodeType, Int16 orderBy, bool isDirty)
            : base()
        {
            this.CategoryId = categoryId;
            this.CatalogNodeId = catalogNodeId;
            this.CatalogNodeType = catalogNodeType;
            this.OrderBy = orderBy;
            this.IsDirty = isDirty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryId">Category Id of this catalog node</param>
        /// <param name="catalogNodeId">Catalog node Id of this catalog node</param>
        /// <param name="catalogNodeType">Type of this catalog node</param>
        /// <param name="orderBy">The value of OrderBy field</param>
        /// <param name="isDirty">Does this catalog node need to be saved to database or not</param>
        /// <param name="visibility">The visibility of this catalog node</param>
        /// <param name="name">The name of this catalog node</param>
        /// <param name="summary">The summary field of this catalog node</param>
        /// <param name="thumbnailUrl">The thumbnail url of this catalog node</param>
        /// <param name="thumbnailAltText">The alt text for the thumbnail of this catalog node</param>
        public CatalogNode(int categoryId, int catalogNodeId, CatalogNodeType catalogNodeType, Int16 orderBy, bool isDirty, CatalogVisibility visibility, string name, string summary, string thumbnailUrl, string thumbnailAltText)
            : this(categoryId, catalogNodeId, catalogNodeType, orderBy, isDirty)
        {
            this.Visibility = visibility;
            this.Name = name;
            this.Summary = summary;
            this.ThumbnailUrl = thumbnailUrl;
            this.ThumbnailAltText = thumbnailAltText;
            this.m_ICatalogableInitialized = true;
        }
        
        /// <summary>
        /// The type of this Catalog Node
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

        /// <summary>
        /// enclosed child object
        /// </summary>
        protected ICatalogable m_ChildObject;

        /// <summary>
        /// The Catalogable object represented by this catalog node
        /// </summary>
        public ICatalogable ChildObject
        {
            get
            {
                if (m_ChildObject == null)
                {
                    switch (CatalogNodeType)
                    {
                        case CatalogNodeType.Category:
                            m_ChildObject = CategoryDataSource.Load(this.CatalogNodeId);
                            break;
                        case CatalogNodeType.Product:
                            m_ChildObject = ProductDataSource.Load(this.CatalogNodeId);
                            break;
                        case CatalogNodeType.Webpage:
                            m_ChildObject = WebpageDataSource.Load(this.CatalogNodeId);
                            break;
                        case CatalogNodeType.Link:
                            m_ChildObject = LinkDataSource.Load(this.CatalogNodeId);
                            break;
                    }
                }
                return m_ChildObject;
            }
        }
        
#region ICatalogable Members

        /// <summary>
        /// Enclosed m_Visibility field
        /// </summary>
        protected CatalogVisibility m_Visibility;
        /// <summary>
        /// Enclosed m_Name field
        /// </summary>
        protected string m_Name;
        /// <summary>
        /// Enclosed m_Summary field
        /// </summary>
        protected string m_Summary;
        /// <summary>
        /// Enclosed m_ThumbnailUrl field
        /// </summary>
        protected string m_ThumbnailUrl;
        /// <summary>
        /// Enclosed m_ThumbnailAltText field
        /// </summary>
        protected string m_ThumbnailAltText;

        /// <summary>
        /// Visibility
        /// </summary>
        public CatalogVisibility Visibility
        {
            get
            {
                if (!this.m_ICatalogableInitialized) InitializeICatalogable();
                return this.m_Visibility;
            }
            set
            {
                this.m_Visibility = value;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                if (!this.m_ICatalogableInitialized) InitializeICatalogable();
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary
        {
            get
            {
                if (!this.m_ICatalogableInitialized) InitializeICatalogable();
                return this.m_Summary;
            }
            set
            {
                this.m_Summary = value;
            }
        }

        /// <summary>
        /// ThumbnailUrl
        /// </summary>
        public string ThumbnailUrl
        {
            get
            {
                if (!this.m_ICatalogableInitialized) InitializeICatalogable();
                return this.m_ThumbnailUrl;
            }
            set
            {
                this.m_ThumbnailUrl = value;
            }
        }

        /// <summary>
        /// ThumbnailAltText
        /// </summary>
        public string ThumbnailAltText
        {
            get
            {
                if (!this.m_ICatalogableInitialized) InitializeICatalogable();
                return this.m_ThumbnailAltText;
            }
            set
            {
                this.m_ThumbnailAltText = value;
            }
        }

        private bool m_ICatalogableInitialized = false;
        private void InitializeICatalogable()
        {
            if (this.ChildObject != null)
            {
                switch (this.CatalogNodeType)
                {
                    case CatalogNodeType.Category:
                        Category category = (Category)this.ChildObject;
                        this.Visibility = category.Visibility;
                        this.Name = category.Name;
                        this.Summary = category.Summary;
                        this.ThumbnailUrl = category.ThumbnailUrl;
                        this.ThumbnailAltText = category.ThumbnailAltText;
                        break;
                    case CatalogNodeType.Product:
                        Product product = (Product)this.ChildObject;
                        this.Visibility = product.Visibility;
                        this.Name = product.Name;
                        this.Summary = product.Summary;
                        this.ThumbnailUrl = product.ThumbnailUrl;
                        this.ThumbnailAltText = product.ThumbnailAltText;
                        break;
                    case CatalogNodeType.Webpage:
                        Webpage webpage = (Webpage)this.ChildObject;
                        this.Visibility = webpage.Visibility;
                        this.Name = webpage.Name;
                        this.Summary = webpage.Summary;
                        this.ThumbnailUrl = webpage.ThumbnailUrl;
                        this.ThumbnailAltText = webpage.ThumbnailAltText;
                        break;
                    default:
                        Link link = (Link)this.ChildObject;
                        this.Visibility = link.Visibility;
                        this.Name = link.Name;
                        this.Summary = link.Summary;
                        this.ThumbnailUrl = link.ThumbnailUrl;
                        this.ThumbnailAltText = link.ThumbnailAltText;
                        break;
                }
                this.m_ICatalogableInitialized = true;
            }
        }

#endregion

        /// <summary>
        /// enclosed m_NavigateUrl field
        /// </summary>
        protected string m_NavigateUrl = string.Empty;

        /// <summary>
        /// NavigateUrl
        /// </summary>
        public string NavigateUrl
        {
            get
            {
                if (string.IsNullOrEmpty(m_NavigateUrl) && (!string.IsNullOrEmpty(this.Name)))
                {
                    m_NavigateUrl = UrlGenerator.GetBrowseUrl(this.CategoryId, this.CatalogNodeId, this.CatalogNodeType, this.Name);
                }
                return m_NavigateUrl;
            }
        }

        /// <summary>
        /// Save this CatalogNode object to the database
        /// </summary>
        /// <param name="saveChild">if <b>true</b> the enclosed child object is also saved.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of save operation.</returns>
        public SaveResult Save(bool saveChild)
        {
            SaveResult result = this.Save();
            if (saveChild && (result != SaveResult.Failed))
            {
                switch (this.CatalogNodeType)
                {
                    case CatalogNodeType.Category:
                        Category category = (Category)this.ChildObject;
                        category.Save();
                        break;
                    case CatalogNodeType.Product:
                        Product product = (Product)this.ChildObject;
                        product.Save();
                        break;
                    case CatalogNodeType.Webpage:
                        Webpage webpage = (Webpage)this.ChildObject;
                        webpage.Save();
                        break;
                    default:
                        Link link = (Link)this.ChildObject;
                        link.Save();
                        break;
                }
            }
            return result;
        }

    }
}
