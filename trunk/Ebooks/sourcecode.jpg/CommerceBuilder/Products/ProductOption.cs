using CommerceBuilder.Common;

namespace CommerceBuilder.Products
{
    
    /// <summary>
    /// This class represents a ProductOption object in the database.
    /// </summary>
    public partial class ProductOption
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="optionId">Id of the option</param>
        /// <param name="orderBy">value of orderby field</param>
        public ProductOption(int productId, int optionId, short orderBy)
        {
            this.ProductId = productId;
            this.OptionId = optionId;
            this.OrderBy = orderBy;
        }

        /// <summary>
        /// Deletes this ProductOption object from the database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            bool result = this.BaseDelete();
            OptionDataSource.DeleteOrphanedOptions();
            //REBUILD VARIANT GRID FOR THIS PRODUCT
            ProductVariantManager.ResetVariantGrid(this.ProductId);
            return result;
        }

        /// <summary>
        /// Saves this ProductOption object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            SaveResult baseResult = this.BaseSave();
            if (baseResult == SaveResult.RecordInserted)
            {
                //REBUILD VARIANT GRID FOR THIS PRODUCT
                ProductVariantManager.ResetVariantGrid(this.ProductId);
            }
            return baseResult;
        }
    }
}
