using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;


namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Base class for Catalogable items
    /// </summary>
    public abstract class CatalogableBase : ICatalogable
    {
        private string _ActiveTheme = string.Empty;
        
        /// <summary>
        /// Gets the active theme for this catalog item
        /// </summary>
        public string ActiveTheme
        {
            get
            {
                if (_ActiveTheme == string.Empty)
                {
                    if (this.Theme != string.Empty)
                    {
                        //A THEME IS SPECIFIED FOR THIS OBJECT
                        _ActiveTheme = this.Theme;
                    }
                    else
                    {
                        //RECURSE THE PATH AND FIND THE MOST SPECIFIC THEME
                        //FIRST WE MUST DETERMINE THE CATEGORY CONTEXT
                        int categoryId = 0;
                        //WE NEED TO KNOW IF THIS IS A CATEGORY, IN WHICH CASE WE WILL USE THE PARENT
                        Type myType = this.GetType();
                        bool isCategory = (myType == typeof(Category));
                        if (isCategory)
                        {
                            //USE THE PARENT OF THE CATEGORY
                            Category c = (Category)this;
                            categoryId = c.ParentId;
                        }
                        else
                        {
                            //IF WE ARE IN AN HTTP CONTEXT, CHECK FOR CATEGORY ID ON URL
                            HttpContext context = HttpContext.Current;
                            if ((context != null) && (!string.IsNullOrEmpty(context.Request.QueryString["CategoryId"])))
                            {
                                //TAKE CATEGORY ID FROM URL
                                categoryId = AlwaysConvert.ToInt(context.Request.QueryString["CategoryId"]);
                            }
                            else
                            {
                                //TAKE FIRST CATEGORY ASSOCIATED WITH OBJECT
                                if (myType == typeof(Product))
                                {
                                    Product p = (Product)this;
                                    if (p.Categories.Count > 0)
                                    {
                                        categoryId = p.Categories[0];
                                    }
                                }
                                else if (myType == typeof(Webpage))
                                {
                                    Webpage w = (Webpage)this;
                                    if (w.Categories.Count > 0)
                                    {
                                        categoryId = w.Categories[0];
                                    }
                                }
                                else if (myType == typeof(Link))
                                {
                                    Link l = (Link)this;
                                    if (l.Categories.Count > 0)
                                    {
                                        categoryId = l.Categories[0];
                                    }
                                }
                            }
                        }
                        //GET THE PATH TO THE CATEGORY CONTEXT
                        List<CatalogPathNode> pathNodes = CatalogDataSource.GetPath(categoryId, true);
                        //LOOP THE NODES IN REVERSE ORDER TO FIND THE MOST SPECIFIC THEME
                        for (int i = pathNodes.Count - 1; i >= 0; i--)
                        {
                            if (pathNodes[i].Theme != string.Empty)
                            {
                                //THEME FOUND, SET AND BREAK OUT OF LOOP
                                _ActiveTheme = pathNodes[i].Theme;
                                break;
                            }
                        }

                    }
                }
                return _ActiveTheme;
            }
        }

        #region ICatalogable Members

        /// <summary>
        /// The VisibilityId
        /// </summary>
        public abstract byte VisibilityId { get; set; }

        /// <summary>
        /// The catalog visibility
        /// </summary>
        public CatalogVisibility Visibility
        {
            get { return (CatalogVisibility)this.VisibilityId; }
            set { this.VisibilityId = (byte)value; }
        }

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; set; }
        /// <summary>
        /// Summary
        /// </summary>
        public abstract string Summary { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public abstract string Description { get; set; }
        /// <summary>
        /// ThumbnailUrl
        /// </summary>
        public abstract string ThumbnailUrl { get; set; }
        /// <summary>
        /// ThumbnailAltText
        /// </summary>
        public abstract string ThumbnailAltText { get; set; }
        /// <summary>
        /// NavigateUrl
        /// </summary>
        public abstract string NavigateUrl { get; }
        /// <summary>
        /// HtmlHead
        /// </summary>
        public abstract string HtmlHead { get; set; }
        /// <summary>
        /// Theme
        /// </summary>
        public abstract string Theme { get; set; }
        /// <summary>
        /// DisplayPage
        /// </summary>
        public abstract string DisplayPage { get; set; }

        #endregion
    }
}
