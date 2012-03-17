using System;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Catalog;
using CommerceBuilder.Users;

namespace CommerceBuilder.Marketing
{
    public partial class VolumeDiscount
    {
        /// <summary>
        /// Is this volume discount valid for given user?
        /// </summary>
        /// <param name="user">user to check validation for</param>
        /// <returns><b>true</b> if this volume discount is valid for given user, <b>false</b> otherwise</returns>
        public bool IsValidForUser(User user)
        {
            if (user == null) return false;
            if (this.VolumeDiscountGroups.Count == 0) return true;
            foreach (VolumeDiscountGroup vdg in this.VolumeDiscountGroups)
            {
                if (user.IsInGroup(vdg.GroupId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the appropriate discount based on the defined levels.
        /// </summary>
        /// <param name="lineItemQuantity">Quantity of line item to be discounted.</param>
        /// <param name="totalQuantity">Total quantity of all line items used to determine eligibility for discount.</param>
        /// <param name="lineItemValue">Extended Price of product (or line item) to be discounted.</param>
        /// <param name="totalValue">If grouping discount mode is enabled, this is the total Extended Price of all products from the same category; if line item discount mode is enabled, this should be the same value as lineItemValue.</param>
        /// <returns>The calculated discount</returns>
        public LSDecimal CalculateDiscount(int lineItemQuantity, int totalQuantity, LSDecimal lineItemValue, LSDecimal totalValue)
        {
            if (lineItemQuantity < 1) return 0;
            LSDecimal appliedDiscount = 0;
            LSDecimal matchCriteria = this.IsValueBased ? totalValue : totalQuantity;
            decimal discountRatio = (decimal)lineItemQuantity / (decimal)totalQuantity;
            foreach (VolumeDiscountLevel level in this.Levels)
            {
                if ((level.MinValue <= matchCriteria) && ((level.MaxValue == 0) || (level.MaxValue >= matchCriteria)))
                {
                    LSDecimal tempDiscount;
                    if (level.IsPercent)
                    {
                        tempDiscount = Math.Round(((Decimal)level.DiscountAmount * (Decimal)lineItemValue) / 100, 2);
                    }
                    else
                    {
                        tempDiscount = (level.DiscountAmount * totalQuantity);
                        if (discountRatio != 1)
                        {
                            tempDiscount = (decimal)tempDiscount * discountRatio;
                            tempDiscount = Math.Round((decimal)tempDiscount, 2);
                        }
                    }
                    if (tempDiscount > appliedDiscount) appliedDiscount = tempDiscount;
                }
            }
            return appliedDiscount;
        }

        /// <summary>
        /// Saves this volume discount object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            //IF THE DISCOUNT IS GLOBAL, IT CANNOT HAVE ASSOCIATED PRODUCTS OR CATEGORIES
            if (this.IsGlobal)
            {
                this.ProductVolumeDiscounts.DeleteAll();
                this.CategoryVolumeDiscounts.DeleteAll();
            }
            return this.BaseSave();
        }

        /// <summary>
        /// Creates a copy of a volume discount
        /// </summary>
        /// <param name="volumeDiscountId">Id of the volume discount of which to create a copy</param>
        /// <param name="deepCopy">If <b>true</b> all child references are also copied</param>
        /// <returns>Copy of the given volume discount</returns>
        public static VolumeDiscount Copy(int volumeDiscountId, bool deepCopy)
        {
            //LOAD COPY (NO CACHE)
            VolumeDiscount source = new VolumeDiscount();
            if (source.Load(volumeDiscountId))
            {
                //IF DEEP COPY, RESET ALL CHILD REFERENCES
                if (deepCopy)
                {
                    foreach (VolumeDiscountLevel child in source.Levels)
                    {
                        child.VolumeDiscountLevelId = 0;
                        child.VolumeDiscountId = 0;
                    }
                    foreach (VolumeDiscountGroup child in source.VolumeDiscountGroups)
                    {
                        child.VolumeDiscountId = 0;
                    }
                    foreach (CategoryVolumeDiscount child in source.CategoryVolumeDiscounts)
                    {
                        child.VolumeDiscountId = 0;
                    }
                    foreach (ProductVolumeDiscount child in source.ProductVolumeDiscounts)
                    {
                        child.VolumeDiscountId = 0;
                    }
                }
                source.VolumeDiscountId = 0;
                source.Save();
                return source;
            }
            return null;
        }
    }
}
