namespace CommerceBuilder.Catalog
{
    using System;
    using CommerceBuilder.Common;
    using CommerceBuilder.Seo;

    /// <summary>
    /// This class represents a CustomUrl object in the database.
    /// </summary>
    public partial class CustomUrl
    {
        public CatalogNodeType CatalogNodeType
        {
            get { return (CatalogNodeType)CatalogNodeTypeId; }
            set { CatalogNodeTypeId = (byte)value; }
        }

        public virtual bool Delete()
        {
            bool result = this.BaseDelete();
            IUrlRewriter rewriter = RewriteServiceLocator.GetInstance(Token.Instance.UrlRewriterSettings.Provider);
            if (rewriter != null) rewriter.ReloadCache();
            return result;
        }

        public virtual SaveResult Save()
        {
            CustomUrl checkExists = CustomUrlDataSource.LoadCustomUrl(this.Url);
            if (checkExists == null || checkExists.CustomUrlId == this.CustomUrlId)
            {
                SaveResult result = this.BaseSave();
                IUrlRewriter rewriter = RewriteServiceLocator.GetInstance(Token.Instance.UrlRewriterSettings.Provider);
                if (rewriter != null) rewriter.ReloadCache();
                return result;
            }
            else
            {
                throw new InvalidOperationException("The value of Url is already defined in the database.  Url must be unique.");
            }
        }
    }
}
