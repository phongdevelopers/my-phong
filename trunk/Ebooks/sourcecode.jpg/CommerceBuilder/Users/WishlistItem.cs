using System;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Class representing objects in WishlistItems table
    /// </summary>
    public partial class WishlistItem
    {
        private ProductVariant _ProductVariant = null;
        /// <summary>
        /// Product variant for this basket item.
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if (!string.IsNullOrEmpty(this.OptionList) && _ProductVariant == null)
                {
                    _ProductVariant = ProductVariantDataSource.LoadForOptionList(this.ProductId, this.OptionList);
                }
                return _ProductVariant;
            }
        }

        /// <summary>
        /// Saves this wishlist item to database
        /// </summary>
        /// <returns></returns>
        public virtual SaveResult Save()
        {
            if (this.Desired < 1)
            {
                this.Delete();
                return CommerceBuilder.Common.SaveResult.RecordDeleted;
            }
            this.LastModifiedDate = LocaleHelper.LocalNow;
            return this.BaseSave();
        }

        /// <summary>
        /// Can this WishlistItem combine with the given WishlistItem?
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if this item can combine with the given WishlistItem, false otherwise</returns>
        public bool CanCombine(WishlistItem other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (this.ProductId != other.ProductId) return false;
            if (this.OptionList != other.OptionList) return false;
            if (this.KitList != other.KitList) return false;
            if (this.LineMessage != other.LineMessage) return false;
            if (this.Inputs.Count > 0)
            {
                //compare all of the input values to see if they match
                if (this.Inputs.Count != other.Inputs.Count) return false;
                foreach (WishlistItemInput input in this.Inputs)
                {
                    WishlistItemInput otherInput = WishlistItem.GetInput(other.Inputs, input.InputFieldId);
                    if (otherInput == null) return false;
                    if (!input.InputValue.Equals(otherInput.InputValue)) return false;
                }
            }
            if ((this.Product.UseVariablePrice) && (this.Price != other.Price)) return false;
            return true;
        }

        private static WishlistItemInput GetInput(WishlistItemInputCollection collection, int productInputId)
        {
            foreach (WishlistItemInput input in collection)
            {
                if (input.InputFieldId.Equals(productInputId)) return input;
            }
            return null;
        }

        /// <summary>
        /// Recalculates the item price for the user the wishlist is associated to
        /// </summary>
        public void Recalculate()
        {
            if (this.Product.UseVariablePrice) return;
            //CALCULATE THE PRICE OF THE PRODUCT
            ProductCalculator pcalc = ProductCalculator.LoadForProduct(this.ProductId, this.Desired, this.OptionList, this.KitList, this.Wishlist.UserId);
            if (this.Price != pcalc.Price)
            {
                this.Price = pcalc.Price;
                this.Save();
            }
        }

        /// <summary>
        /// Gets a list of KitProduct objects associated with this WishlistItem
        /// </summary>
        /// <returns>A list of KitProduct objects</returns>
        /// <remarks>includes hidden items</remarks>
        public List<KitProduct> GetKitProducts()
        {
            return GetKitProducts(true);
        }

        /// <summary>
        /// Gets a list of KitProduct objects associated with this WishlistItem
        /// </summary>
        /// <param name="includeHidden">If true, items in hidden components are included.  If false, only items in visible component are included.</param>
        /// <returns>A list of KitProduct objects</returns>
        public List<KitProduct> GetKitProducts(bool includeHidden)
        {
            // BUILD LIST OF KIT PRODUCTS
            List<KitProduct> kitProductList = new List<KitProduct>();
            int[] kitProductIds = AlwaysConvert.ToIntArray(this.KitList);
            if (kitProductIds != null && kitProductIds.Length > 0)
            {
                for (int i = 0; i < kitProductIds.Length; i++)
                {
                    KitProduct kp = KitProductDataSource.Load(kitProductIds[i]);
                    if (kp != null)
                    {
                        bool addToList = (includeHidden || (kp.KitComponent.InputType != KitInputType.IncludedHidden));
                        if (addToList) kitProductList.Add(kp);
                    }
                }
            }
            return kitProductList;
        }
    }
}
