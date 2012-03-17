using System.ComponentModel;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a UpsellProduct object in the database.
    /// </summary>
    public partial class UpsellProduct
    {
        private Product _ChildProduct;

        /// <summary>
        /// Child product
        /// </summary>
        public Product ChildProduct
        {
            get
            {
                if (!this.ChildProductLoaded)
                {
                    this._ChildProduct = ProductDataSource.Load(this.ChildProductId);
                }
                return this._ChildProduct;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ChildProductLoaded { get { return ((this._ChildProduct != null) && (this._ChildProduct.ProductId == this.ChildProductId)); } }
    }    
}
