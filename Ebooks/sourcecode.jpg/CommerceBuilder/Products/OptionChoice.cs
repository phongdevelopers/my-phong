using System.Collections.Generic;
using CommerceBuilder.Common;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a OptionChoice object in the database.
    /// </summary>
    public partial class OptionChoice
    {
        /// <summary>
        /// Deletes this OptionChoice object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            //STORE DATA NECESSARY TO SCRUB VARIANT GRID AFTER DELETE
            int deletedChoiceId = this.OptionChoiceId;
            List<int> associatedProducts = new List<int>();
            foreach (ProductOption po in this.Option.ProductOptions)
            {
                associatedProducts.Add(po.ProductId);
            }
            //NOW CALL THE DELETE
            if (BaseDelete())
            {
                //DELETE SUCCEEDED, SCRUB THE VARIANT GRID
                foreach (int productId in associatedProducts)
                {
                    ProductVariantManager.ScrubVariantGrid(productId, deletedChoiceId);
                }
                //RETURN SUCCESS
                return true;
            }
            //BASE DELETE FAILED
            return false;
        }

        /// <summary>
        /// Saves this OptionChoice object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public SaveResult Save()
        {
            return BaseSave();
        }
    }
}
