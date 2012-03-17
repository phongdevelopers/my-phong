using System;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using CommerceBuilder.Exceptions;
using System.Data.Common;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents a BasketItem object in the database.
    /// </summary>
    public partial class BasketItem
    {
        /// <summary>
        /// Order Item type
        /// </summary>
        public OrderItemType OrderItemType
        {
            get
            {
                return (OrderItemType)this.OrderItemTypeId;
            }
            set
            {
                this.OrderItemTypeId = (short)value;
            }
        }

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

        private BasketShipment _BasketShipment;
        /// <summary>
        /// Basket shipment
        /// </summary>
        public BasketShipment BasketShipment
        {
            get
            {
                if ((this._BasketShipment == null) && (!this.BasketShipmentId.Equals(0)))
                {
                    foreach (BasketShipment shipment in this.Basket.Shipments)
                    {
                        if (shipment.BasketShipmentId.Equals(this.BasketShipmentId))
                        {
                            _BasketShipment = shipment;
                            break;
                        }
                    }
                    //TODO: UNSURE WHETHER WE SHOULD LOAD FROM DATABASE AT THIS POINT
                    //WHY WOULD THE SHIPMENT NOT BE AVAILABLE THROUGH THE BASKET.SHIPMENTS COLLECTION?
                    //WHAT ARE THE IMPLICATIONS IF WE LOAD MANUALLY?
                    _BasketShipment = BasketShipmentDataSource.Load(this._BasketShipmentId);
                }
                return _BasketShipment;
            }
        }

        /// <summary>
        /// Is this a shippable item?
        /// </summary>
        public Shippable Shippable
        {
            get { return (Shippable)this.ShippableId; }
            set { this.ShippableId = (byte)value; }
        }

        /// <summary>
        /// Extended price of this item
        /// </summary>
        public LSDecimal ExtendedPrice
        {
            get { return Math.Round(this.Quantity * (Decimal)this.Price, 2); }
        }

        /// <summary>
        /// Extended weight of this item
        /// </summary>
        public LSDecimal ExtendedWeight
        {
            get { return Math.Round(this.Quantity * (Decimal)this.Weight, 2); }
        }

        /// <summary>
        /// Saves this BasketItem object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.Quantity < 1)
            {
                this.Delete();
                return CommerceBuilder.Common.SaveResult.RecordDeleted;
            }
            this.LastModifiedDate = LocaleHelper.LocalNow;
            //EXECUTE THE GENERATED SAVE METHOD AND RETURN RESULT
            return this.BaseSave();
        }

        /// <summary>
        /// Can this BasketItem combine with the given BasketItem?
        /// </summary>
        /// <param name="other">The BasketItem to check for combine</param>
        /// <returns><b>true</b> if it can combine, <b>false</b> otherwise</returns>
        public bool CanCombine(BasketItem other)
        {
            return CanCombine(other, false);
        }

        /// <summary>
        /// Can this BasketItem combine with the given BasketItem?
        /// </summary>
        /// <param name="other">The BasketItem to check for combine</param>
        /// <returns><b>true</b> if it can combine, <b>false</b> otherwise</returns>
        internal bool CanCombine(BasketItem other, bool combineKitProducts)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (this.ProductId != other.ProductId) return false;
            if (this.IsChildItem)
            {
                // IF THE OTHER ITEM IS NOT A CHILD, THIS IS NOT A MATCH
                if (!other.IsChildItem) return false;
                // IS THIS IS A CHILD PRODUCT OF A KIT?
                if (this.OrderItemType == OrderItemType.Product)
                {
                    // DO NOT COMBINE CHILD PRODUCTS UNLESS INDICATED
                    if (!combineKitProducts) return false;
                    // DO NOT COMBINE IF NAME,PRICE,WEIGHT MISMATCH
                    if (this.Name != other.Name
                        || this.Price != other.Price
                        || this.Weight != other.Weight) return false;
                }
                // IF THE OTHER ITEM HAS A DIFFERENT PARENT IT IS NOT A MATCH
                if (this.ParentItemId != other.ParentItemId) return false;
            }
            else
            {
                //THIS ITEM IS NOT A CHILD, SO THE OTHER IS NOT A MATCH IF IT IS A CHILD
                if (other.IsChildItem) return false;
            }
            if (this.OptionList != other.OptionList) return false;
            if (this.LineMessage != other.LineMessage) return false;
            if (this.WishlistItemId != other.WishlistItemId) return false;
            if (this.BasketShipmentId != other.BasketShipmentId) return false;
            if (this.GiftMessage != other.GiftMessage) return false;
            if (this.WrapStyleId != other.WrapStyleId) return false;
            if (this.Inputs.Count > 0)
            {
                //compare all of the input values to see if they match
                if (this.Inputs.Count != other.Inputs.Count) return false;
                foreach (BasketItemInput input in this.Inputs)
                {
                    BasketItemInput otherInput = BasketItem.GetInput(other.Inputs, input.InputFieldId);
                    if (otherInput == null) return false;
                    if (!input.InputValue.Equals(otherInput.InputValue)) return false;
                }
            }
            if (this.KitList != other.KitList) return false;
            if (this.OrderItemType != OrderItemType.Product)
            {
                if (this.Name != other.Name) return false;
                if (this.Sku != other.Sku) return false;
                if (this.Price != other.Price) return false;
            }
            else
            {
                //NEED TO CHECK WHETHER THIS IS A VARIABLE PRICE PRODUCT
                if (this.Product != null && this.Product.UseVariablePrice)
                {
                    //IS MY PRICE DIFFERENT FROM THE OTHER PRICE?
                    if (this.Price != other.Price) return false;
                }
            }
            return true;
        }

        private static BasketItemInput GetInput(BasketItemInputCollection collection, int productInputId)
        {
            foreach (BasketItemInput input in collection)
            {
                if (input.InputFieldId.Equals(productInputId)) return input;
            }
            return null;
        }

        /// <summary>
        /// Creates a clone of this BasketItem object
        /// </summary>
        /// <returns>A clone of this BasketItem object</returns>
        public BasketItem Clone()
        {
            BasketItem cloneItem = new BasketItem();
            cloneItem.BasketId = this.BasketId;
            cloneItem.BasketItemId = 0;
            cloneItem.BasketShipmentId = this.BasketShipmentId;
            cloneItem.CustomFields.Parse(this.CustomFields.ToString());
            cloneItem.GiftMessage = this.GiftMessage;
            cloneItem.KitList = this.KitList;
            cloneItem.LineMessage = this.LineMessage;
            cloneItem.Name = this.Name;
            cloneItem.OptionList = this.OptionList;
            cloneItem.OrderBy = this.OrderBy;
            cloneItem.OrderItemType = this.OrderItemType;
            cloneItem.ParentItemId = this.IsChildItem ? this.ParentItemId : 0;
            cloneItem.Price = this.Price;
            cloneItem.ProductId = this.ProductId;
            cloneItem.Quantity = this.Quantity;
            cloneItem.ShippableId = this.ShippableId;
            cloneItem.Sku = this.Sku;
            cloneItem.TaxAmount = this.TaxAmount;
            cloneItem.TaxCodeId = this.TaxCodeId;
            cloneItem.TaxRate = this.TaxRate;
            cloneItem.Weight = this.Weight;
            cloneItem.WishlistItemId = this.WishlistItemId;
            cloneItem.WrapStyleId = this.WrapStyleId;
            return cloneItem;
        }

        /// <summary>
        /// Gets a list of KitProduct objects associated with this BasketItem
        /// </summary>
        /// <returns>A list of KitProduct objects</returns>
        /// <remarks>includes hidden items</remarks>
        public List<KitProduct> GetKitProducts()
        {
            return GetKitProducts(true);
        }

        /// <summary>
        /// Gets a list of KitProduct objects associated with this BasketItem
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

        /// <summary>
        /// Indicates whether the item is a child of another item.
        /// </summary>
        public bool IsChildItem
        {
            get
            {
                return ((this.BasketItemId != this.ParentItemId) && (this.ParentItemId != 0));
            }
        }

        /// <summary>
        /// Gets the immediate parent item for this basket item.
        /// </summary>
        /// <returns>Returns the BasketItem instance for the parent item.  If the current item has no parent, the current item is returned.</returns>
        [Obsolete("Use GetParentItem(bool recurse) to specify how nested children are handled.")]
        public BasketItem GetParentItem()
        {
            return GetParentItem(false);
        }

        /// <summary>
        /// Gets the parent item for this basket item.
        /// </summary>
        /// <param name="recurse">If true, this method gets the top level parent for this item.  If false, only the immediate parent is returned.</param>
        /// <returns>Returns the BasketItem instance for the parent item.  If the current item has no parent, the current item is returned.</returns>
        public BasketItem GetParentItem(bool recurse)
        {
            List<int> itemPath = new List<int>();
            return InternalGetParentItem(this, recurse, itemPath);
        }

        /// <summary>
        /// Gets the path for this basket item
        /// </summary>
        /// <returns>A list of basket item IDs from top level item to this item.</returns>
        public List<int> GetPath()
        {
            List<int> path = new List<int>();
            BasketItem bi = InternalGetParentItem(this, true, path);
            return path;
        }

        /// <summary>
        /// Gets the parent item for the given basket item.
        /// </summary>
        /// <param name="item">Item for which to find the parent.</param>
        /// <param name="recurse">If true, this method gets the top level parent for this item.  If false, only the immediate parent is returned.</param>
        /// <param name="itemPath">List to track the current item path, to prevent recursive loops.</param>
        /// <returns>Returns the BasketItem instance for the parent item.  If the current item has no parent, the current item is returned.</returns>
        private static BasketItem InternalGetParentItem(BasketItem item, bool recurse, List<int> itemPath)
        {
            // IF PARENT ITEM INDICATED, LOOK FOR IT IN THE BASKET
            int parentItemId = item.ParentItemId;
            int basketItemId = item.BasketItemId;
            itemPath.Insert(0, basketItemId);
            if (item.IsChildItem)
            {
                Basket basket = item.Basket;
                if (basket != null)
                {
                    foreach (BasketItem otherItem in basket.Items)
                    {
                        int otherItemId = otherItem.BasketItemId;
                        if (otherItemId == parentItemId)
                        {
                            // CHECK TO MAKE SURE WE ARE NOT IN A RECURSIVE LOOP
                            if (itemPath.Contains(otherItemId))
                            {
                                Logger.Error("Circular parent reference in basket.  Path: " + AlwaysConvert.ToList(",", itemPath.ToArray()));
                                throw new CircularParentReference("Circular parent reference in basket.  Path: " + AlwaysConvert.ToList(",", itemPath.ToArray()));
                            }

                            if (recurse) return InternalGetParentItem(otherItem, recurse, itemPath);
                            // NON-RECURSIVE, ADD THIS ITEM TO THE PATH AND RETURN
                            itemPath.Insert(0, otherItem.BasketItemId);
                            return otherItem;
                        }
                    }
                }
            }

            // NO PARENT OR NO PARENT FOUND, ENSURE PARENT ID IS CORRECTLY SET
            item.ParentItemId = basketItemId;
            return item;
        }

        /// <summary>
        /// This method no longer has any effect.
        /// </summary>
        [Obsolete("This method is no longer used.  Only basket objects can be recalculated, not individual items.")]
        public void Recalculate() { }

        /// <summary>
        /// Deletes this BasketItem object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            // REMOVE ALL CHILD ITEMS
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_BasketItems WHERE ParentItemId = @parentItemId"))
            {
                database.AddInParameter(deleteCommand, "@parentItemId", System.Data.DbType.Int32, this.BasketItemId);
                database.ExecuteNonQuery(deleteCommand);
            }

            return this.BaseDelete();
        }

        /// <summary>
        /// Gets the net extended price (this item less any child coupons or discounts in the associated basket)
        /// </summary>
        public LSDecimal NetExtendedPrice
        {
            get
            {
                // WE CANNOT DETERMINE THE TOTAL PRICE WITHOUT THE BASKET
                // WE ALSO CANNOT DETERMINE THE TOTAL PRICE IF THIS ITEM DOES NOT YET HAVE A UNIQUE ID
                if (this.BasketItemId == 0 || this.Basket == null) return this.ExtendedPrice;

                // LOOP ALL THE ITEMS AND ADD ANY CHILD DISCOUNTS / COUPONS
                LSDecimal priceAdjustments = 0;
                foreach (BasketItem otherItems in this.Basket.Items)
                {
                    // CHECK CHILDREN, IGNORING SELF
                    if (otherItems.ParentItemId == this.BasketItemId
                        && otherItems.BasketItemId != this.BasketItemId)
                    {
                        // INCLUDE COUPONS / DISCOUNTS
                        if (otherItems.OrderItemType == OrderItemType.Coupon
                            || otherItems.OrderItemType == OrderItemType.Discount)
                        {
                            priceAdjustments += otherItems.ExtendedPrice;
                        }
                    }
                }

                // RETURN TOTAL OF EXTENDED PRICE + ADJUSTMENTS
                return this.ExtendedPrice + priceAdjustments;
            }
        }

        /// <summary>
        /// Gets the  price of the item factoring in child products for kit items.  For bundled kits, this calculates the price of the entire kit.  For itemized kits, this calculates the price of the master product plus any hidden kit items.
        /// </summary>
        public LSDecimal KitPrice
        {
            get
            {
                LSDecimal kitPrice = this.Price;
                bool itemizedKit = (this.Product != null && this.Product.Kit.ItemizeDisplay);
                if (itemizedKit) return kitPrice;
                Basket basket = this.Basket;
                foreach (BasketItem bi in basket.Items)
                {
                    if (bi.IsChildItem
                        && bi.ParentItemId == this.BasketItemId
                        && bi.OrderItemType == OrderItemType.Product)
                    {
                        kitPrice += bi.Price;
                    }
                }
                return kitPrice;
            }
        }
        
        /// <summary>
        /// Gets the extended price of the item factoring in child products for kit items.  For bundled kits, this calculates the price of the entire kit.  For itemized kits, this calculates the price of the master product plus any hidden kit items.
        /// </summary>
        public LSDecimal KitExtendedPrice
        {
            get
            {
                LSDecimal extendedKitPrice = this.ExtendedPrice;
                bool itemizedKit = (this.Product != null && this.Product.Kit.ItemizeDisplay);
                if (itemizedKit) return extendedKitPrice;
                Basket basket = this.Basket;
                foreach (BasketItem bi in basket.Items)
                {
                    if (bi.IsChildItem
                        && bi.ParentItemId == this.BasketItemId
                        && bi.OrderItemType == OrderItemType.Product)
                    {
                        extendedKitPrice += bi.ExtendedPrice;
                    }
                }
                return extendedKitPrice;
            }
        }
    }
}