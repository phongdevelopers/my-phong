namespace CommerceBuilder.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Products;
    using CommerceBuilder.Shipping;
    using CommerceBuilder.Users;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Collection of BasketItem objects
    /// </summary>
    public partial class BasketItemCollection
    {
        //STORES A RECORD OF ITEMS REMOVED BY THE COMBINE METHOD WHEN SAVE PARAMETER IS FALSE
        private List<BasketItem> removedItems = new List<BasketItem>();

        /// <summary>
        /// Removes the element at the specified index and tracks the remove for persistence.
        /// </summary>
        /// <param name="index">The zero based index of the element to remove.</param>
        internal void TrackedRemoveAt(int index)
        {
            removedItems.Add(this[index]);
            this.RemoveAt(index);
        }

        /// <summary>
        /// Saves the collection.
        /// </summary>
        /// <returns>True if the save is successful, false otherwise.</returns>
        public override bool Save()
        {
            //IF ITEMS HAVE BEEN REMOVED FROM THE COLLECTION, DELETE THEM NOW
            if (removedItems.Count > 0)
            {
                foreach (BasketItem removedItem in removedItems)
                {
                    removedItem.Delete();
                }
                removedItems.Clear();
            }
            //INVOKE THE BASE SAVE
            return base.Save();
        }

        /// <summary>
        /// Move the basket item at the given index to the given wishlist
        /// </summary>
        /// <param name="index">Index of the item to move</param>
        /// <param name="wishlist">The wishlist to move to</param>
        public void MoveToWishlist(int index, Wishlist wishlist)
        {
            BasketItem item = this[index];
            wishlist.Items.Add(item);
            wishlist.Save();
            this.DeleteAt(index);
        }

        /// <summary>
        /// Gets the total price of all items in the shipment.
        /// </summary>
        /// <returns>The total price of all items in the shipment.</returns>
        public LSDecimal TotalPrice() { return this.TotalPrice(null); }

        /// <summary>
        /// Gets the total price of all items in the shipment that match the specified order item types.
        /// </summary>
        /// <param name="args">The order item types to include in the total.</param>
        /// <returns>The total price of all items in the shipment that match the specified order item types.</returns>
        /// <remarks>if args is null, the total of all items is returned</remarks>
        public LSDecimal TotalPrice(params OrderItemType[] args)
        {
            LSDecimal total = 0;
            foreach (BasketItem item in this)
            {
                if ((args == null) || (Array.IndexOf(args, item.OrderItemType) > -1)) total += item.ExtendedPrice;
            }
            return total;
        }

        /// <summary>
        /// Gets the total price of all products in the collection.
        /// </summary>
        /// <returns>The total price of all products in the collection.</returns>
        /// <remarks>The total includes modifications made to product price (i.e. discounts and coupons), but not items such as taxes or fees.</remarks>
        public LSDecimal TotalProductPrice()
        {
            return TotalProductPrice(null, FilterRule.All);
        }

        /// <summary>
        /// Gets the total price of all products in the collection.
        /// </summary>
        /// <param name="productIds">An array of integer that represents a list of product ids to filter on.</param>
        /// <param name="filter">The rule for filtering products.</param>
        /// <returns>The total price of all products in the collection, as modified by the filter..</returns>
        /// <remarks>The total includes modifications made to product price (i.e. discounts and coupons), but not items such as taxes or fees.</remarks>
        public LSDecimal TotalProductPrice(int[] productIds, FilterRule filter)
        {
            // INITIALIZE OUR RETURN VALUE
            LSDecimal total = 0;
            
            // DEAL WITH NULL/EMPTY ARRAYS
            if (productIds == null || productIds.Length == 0)
            {
                if (filter == FilterRule.IncludeSelected) return total;
                if (filter == FilterRule.ExcludeSelected) filter = FilterRule.All;
            }

            // Build a list of parent item ids for later reference
            List<int> parentList = new List<int>();

            // Loop and add all products
            foreach (BasketItem item in this)
            {
                if (item.OrderItemType == OrderItemType.Product)
                {
                    if (filter == FilterRule.All
                        || (filter == FilterRule.IncludeSelected && Array.IndexOf(productIds, item.ProductId) > -1)
                        || (filter == FilterRule.ExcludeSelected && Array.IndexOf(productIds, item.ProductId) == -1))
                    {
                        parentList.Add(item.BasketItemId);
                        total += item.ExtendedPrice;
                    }
                }
            }

            // Loop and add any discounts or coupons that apply to the selected products.
            foreach (BasketItem item in this)
            {
                if (item.OrderItemType == OrderItemType.Coupon || item.OrderItemType == OrderItemType.Discount)
                {
                    // SEE IF THE ITEM IS A PARENT ITEM
                    // OR A CHILD OF A PRODUCT ITEM THAT IS INCLUDED IN THE PRICE
                    if (!item.IsChildItem || parentList.Contains(item.ParentItemId))
                    {
                        total += item.ExtendedPrice;
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the total weight of all items in the shipment.
        /// </summary>
        /// <returns>The total weight of all items in the shipment.</returns>
        public LSDecimal TotalWeight() { return this.TotalWeight(null); }

        /// <summary>
        /// Gets the total weight of all items in the shipment that match the specified order item types.
        /// </summary>
        /// <param name="args">The order item types to include in the total.</param>
        /// <returns>The total weight of all items in the shipment that match the specified order item types.</returns>
        /// <remarks>if args is null, the total of all items is returned</remarks>
        public LSDecimal TotalWeight(params OrderItemType[] args)
        {
            LSDecimal total = 0;
            foreach (BasketItem item in this)
            {
                if ((args == null) || (Array.IndexOf(args, item.OrderItemType) > -1)) total += item.ExtendedWeight;
            }
            return total;
        }

        /// <summary>
        /// Gets the total quantity of all items in the collection
        /// </summary>
        /// <returns>The total quantity of all items in the collection.</returns>
        public int TotalQuantity() { return this.TotalQuantity(null); }

        /// <summary>
        /// Gets the total quantity of all items in the collection that match the specified order item types.
        /// </summary>
        /// <param name="args">The order item types to include in the total.</param>
        /// <returns>The total quantity of all items in the collection that match the specified order item types.</returns>
        /// <remarks>if args is null, the total quantity of all items is returned</remarks>
        public int TotalQuantity(params OrderItemType[] args)
        {
            int total = 0;
            foreach (BasketItem item in this)
            {
                if ((args == null) || (Array.IndexOf(args, item.OrderItemType) > -1)) total += item.Quantity;
            }
            return total;
        }

        /// <summary>
        /// Combines any items in the collection that have equivalent data.
        /// <param name="save">Flag indicating whether or not to persist changes.</param>
        /// </summary>
        public void Combine(bool save)
        {
            // WE NEED TO TRACK PARENT ITEMS THAT ARE COMBINED
            Dictionary<int, int> combineMapping = new Dictionary<int, int>();

            // LOOP THE BASKET ITEM COLLECTION AND COMBINE ROOT LEVEL PRODUCTS
            for (int thisIndex = 0; thisIndex < this.Count; thisIndex++)
            {
                BasketItem thisItem = this[thisIndex];
                // ONLY CHECK ROOT LEVEL PRODUCTS
                if (thisItem.OrderItemType == OrderItemType.Product && !thisItem.IsChildItem)
                {
                    // SEE IF WE HAVE ANY MATCHES TO COMBINE
                    int matchIndex = FindNextIndexForCombine(thisIndex);
                    while (matchIndex > -1)
                    {
                        BasketItem matchItem = this[matchIndex];
                        // TRACK NEW PARENT MAPPING FOR CHILD ITEMS
                        if (matchItem.BasketItemId > 0)
                            combineMapping[matchItem.BasketItemId] = thisItem.BasketItemId;
                        // COMBINE QUANTITIES, BE CAREFUL NOT TO EXCEED SHORT LIMIT
                        int newQuantity = thisItem.Quantity + matchItem.Quantity;
                        thisItem.Quantity = (newQuantity > short.MaxValue ? short.MaxValue : (short)newQuantity);
                        this.TrackedRemoveAt(matchIndex);
                        // SEE IF WE HAVE ANOTHER MATCH
                        matchIndex = FindNextIndexForCombine(thisIndex);
                    }
                }
            }

            // UPDATE PARENT IDS THAT HAVE CHANGED AS A RESULT OF THE COMBINE
            Dictionary<int, int> savedParentMappings = new Dictionary<int, int>();
            foreach (BasketItem thisItem in this)
            {
                if (thisItem.IsChildItem && combineMapping.ContainsKey(thisItem.ParentItemId))
                {
                    thisItem.ParentItemId = combineMapping[thisItem.ParentItemId];
                }
            }

            // LOOP THE BASKET ITEM COLLECTION AND TO OPERATE ON CHILD ITEMS
            OrderItemType[] combineTypes = new OrderItemType[] { OrderItemType.Product, OrderItemType.Coupon, OrderItemType.Discount };
            for (int thisIndex = 0; thisIndex < this.Count; thisIndex++)
            {
                BasketItem thisItem = this[thisIndex];
                // ONLY CHECK CHILD ITEMS OF THE SPECIFIED TYPES
                // AND WHO HAVE PARENTS THAT HAVE BEEN IDENTIFIED IN THE COLLECTION
                if (thisItem.IsChildItem)
                {
                    // COMBINE CHILD PRODUCTS, DISCOUNTS, AND COUPONS
                    if (Array.IndexOf(combineTypes, thisItem.OrderItemType) > -1)
                    {
                        // SEE IF WE HAVE ANY MATCHES TO COMBINE
                        int matchIndex = FindNextIndexForCombine(thisIndex);
                        while (matchIndex > -1)
                        {
                            BasketItem matchItem = this[matchIndex];
                            if (thisItem.OrderItemType == OrderItemType.Product)
                            {
                                // COMBINE QUANTITIES, BE CAREFUL NOT TO EXCEED SHORT LIMIT
                                int newQuantity = thisItem.Quantity + matchItem.Quantity;
                                thisItem.Quantity = (newQuantity > short.MaxValue ? short.MaxValue : (short)newQuantity);
                            }
                            else
                            {
                                // COUPON OR DISCOUNT MAINTAINS ONE QUANTITY, ALTER PRICE INSTEAD
                                thisItem.Price += matchItem.Price;
                            }
                            // REMOVE THE MATCH ITEM FROM THE COLLECTION
                            this.TrackedRemoveAt(matchIndex);
                            // SEE IF WE HAVE ANOTHER MATCH
                            matchIndex = FindNextIndexForCombine(thisIndex);
                        }
                    }
                }
            }

            // LOOP COLLECTION IN REVERSE AND REMOVE ZERO QUANTITY ITEMS
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (this[i].Quantity < 1) this.TrackedRemoveAt(i);
            }

            // IF INDICATED, PERSIST CHANGES TO THE DATABASE
            if (save) this.Save();
        }


        /// <summary>
        /// Returns the next available index for an item that can be combined with the current
        /// </summary>
        /// <param name="currentIndex">The index of the current item to match to.</param>
        /// <returns>Index of the next item that can combine with the current, or -1 if not found.</returns>
        private int FindNextIndexForCombine(int currentIndex)
        {
            // THE CURRENT INDEX MUST BE WITHIN THE BOUNDS OF THE COLLECTION
            if (currentIndex < 0 || currentIndex > this.Count) throw new ArgumentOutOfRangeException("currentIndex");

            // STARTING WITH THE CURRENT INDEX, MOVE FORWARD IN THE 
            // COLLECTION LOOKING FOR AN ITEM THAT COULD COMBINE WITH THIS ONE
            BasketItem currentItem = this[currentIndex];
            for (int nextIndex = currentIndex + 1; nextIndex < this.Count; nextIndex++)
            {
                BasketItem nextItem = this[nextIndex];
                if (currentItem.CanCombine(nextItem, true))
                {
                    return nextIndex;
                }
            }

            // NO MATCH FOUND
            return -1;
        }

        /// <summary>
        /// Get the child items of the given basket item.
        /// </summary>
        /// <param name="basketItem">BasketItem to get the child items for</param>
        /// <param name="orderItemTypes">Type of order item objects to include</param>
        /// <returns>A collection of child items of the given basket item</returns>
        public BasketItemCollection GetChildItems(BasketItem basketItem, params OrderItemType[] orderItemTypes)
        {
            BasketItemCollection childProducts = new BasketItemCollection();
            foreach (BasketItem item in this)
            {
                if (item.IsChildItem && item.ParentItemId == basketItem.BasketItemId &&
                    Array.IndexOf(orderItemTypes, item.OrderItemType) > -1)
                {
                    childProducts.Add(item);
                }
            }
            return childProducts;
        }
        
        /// <summary>
        /// Generates a hash of the basket items in this collection
        /// </summary>
        /// <returns>A hash of the basket items in this collection</returns>
        public string GenerateContentHash()
        {
            return GenerateContentHash(true);
        }

        /// <summary>
        /// Generates a hash of the basket items in this collection
        /// </summary>
        /// <param name="shippableOnly">If <b>true</b> only shippable items are included in hash calculation</param>
        /// <returns>A hash of the basket items in this collection</returns>
        public string GenerateContentHash(bool shippableOnly)
        {
            List<string> productList = new List<string>();
            foreach (BasketItem item in this)
            {
                if ((!shippableOnly) || (item.Shippable != CommerceBuilder.Shipping.Shippable.No))
                {
                    productList.Add(item.ProductId + "_" + item.Quantity);
                }
            }
            productList.Sort();
            return StringHelper.CalculateMD5Hash(string.Join("_", productList.ToArray()));
        }

        /// <summary>
        /// Creates a string representation of the items in this collection
        /// </summary>
        /// <returns>A string representation of the items in this collection</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("\r\n");
            foreach(BasketItem bitem in this) 
            {
                sb.Append("Item:" + bitem.Name + " Quantity:" + bitem.Quantity + " Price:" + bitem.Price + " Weight:" + bitem.Weight + "\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Determines if any id in the list of child items appears in the path
        /// </summary>
        /// <param name="childItemIds"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsChildItemInPath(List<int> childItemIds, List<int> path)
        {
            foreach (int childItemId in childItemIds)
            {
                if (path.Contains(childItemId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Finds an item in the list that has the same properties as the search item
        /// </summary>
        /// <param name="childProducts">List of items</param>
        /// <param name="find">Item to find</param>
        /// <remarks>If a matching item is found in the list, it is returned.  Otherwise the find item is returned.</remarks>
        private BasketItem FindChildProduct(List<BasketItem> childProducts, BasketItem find)
        {
            for (int i = 0; i < childProducts.Count; i++)
            {
                BasketItem bi = childProducts[i];
                if (bi.ParentItemId == find.ParentItemId
                    && bi.ProductId == find.ProductId
                    && bi.OptionList == find.OptionList)
                {
                    childProducts.RemoveAt(i);
                    return bi;
                }
            }
            return find;
        }

        /// <summary>
        /// Recalculates all items in the basket, for example price and kit member products
        /// </summary>
        internal void Recalculate()
        {
            // BUILD A LIST OF ANY CHILD (GENERATED) PRODUCTS
            List<int> childItemIds = new List<int>();
            List<BasketItem> childProducts = new List<BasketItem>();
            foreach (BasketItem item in this)
            {
                if (item.OrderItemType == OrderItemType.Product && item.IsChildItem)
                {
                    childItemIds.Add(item.BasketItemId);
                    childProducts.Add(item);
                }
            }

            // MOVE THROUGH THE COLLECTION AND REMOVE ANY ITEMS ASSOCIATED WITH A CHILD ITEM
            for (int i = this.Count - 1; i >= 0; i--)
            {
                BasketItem item = this[i];
                List<int> childPath = item.GetPath();
                if (IsChildItemInPath(childItemIds, childPath))
                {
                    if (item.OrderItemType == OrderItemType.Product) this.RemoveAt(i);
                    else this.DeleteAt(i);
                }
            }

            // LOOP EACH REMAINING ITEM AND RECALCULATE
            int currentCount = this.Count;
            for (int i = 0; i < currentCount; i++)
            {
                BasketItem basketItem = this[i];
                Basket basket = basketItem.Basket;
                int userId = (basket == null ? Token.Instance.UserId : basket.UserId);

                // WE ONLY NEED TO CHECK PRODUCTS, NON-PRODUCT ITEMS HAVE NO RECALCULATION TASKS
                if (basketItem.OrderItemType == OrderItemType.Product)
                {
                    // IF WE HAVE A KIT, WE MUST REFRESH ANY CONFIGURED KIT OPTIONS
                    bool isKit = (basketItem.Product.KitStatus == KitStatus.Master);
                    if (isKit)
                    {
                        basketItem.KitList = basketItem.Product.Kit.RefreshKitProducts(basketItem.KitList);
                    }

                    // RECALCULATE THE STARTING SKU/PRICE/WEIGHT FOR THIS ITEM
                    ProductCalculator pcalc = ProductCalculator.LoadForProduct(basketItem.ProductId, basketItem.Quantity, basketItem.OptionList, basketItem.KitList, userId);
                    basketItem.Sku = pcalc.Sku;
                    basketItem.Weight = pcalc.Weight;
                    if (isKit || !basketItem.Product.UseVariablePrice)
                    {
                        // KITS AND NONVARIABLE PRICED PRODUCTS MUST HAVE PRICE RECALCULATED
                        basketItem.Price = pcalc.Price;
                    }
                    basketItem.Save();
                    
                    // REGENERATE THE KIT ITEMS FOR THIS ITEM
                    if (basket != null && isKit)
                    {
                        // OBTAIN THE KIT PRODUCTS THAT ARE SELECTED RATHER THAN INCLUDED
                        int[] kitProductIds = AlwaysConvert.ToIntArray(basketItem.KitList);
                        if (kitProductIds != null && kitProductIds.Length > 0)
                        {
                            //keep track of the price/weight of the master line item
                            //decrement these values for each line item registered
                            LSDecimal masterPrice = basketItem.Price;
                            LSDecimal masterWeight = basketItem.Weight;
                            foreach (int kitProductId in kitProductIds)
                            {
                                KitProduct kitProduct = KitProductDataSource.Load(kitProductId);
                                if (kitProduct != null && kitProduct.KitComponent.InputType != KitInputType.IncludedHidden)
                                {
                                    // WE WANT TO GENERATE BASKET RECORDS FOR ALL ITEMS *EXCEPT* INCLUDED HIDDEN ITEMS
                                    // INCLUDED HIDDEN ITEMS ARE TREATED AS PART OF THE MAIN PRODUCT AND ARE NOT GENERATED
                                    // UNTIL THE ORDER IS FINALIZED
                                    Product product = kitProduct.Product;
                                    BasketItem searchItem = new BasketItem();
                                    searchItem.BasketId = basket.BasketId;
                                    searchItem.OrderItemType = OrderItemType.Product;
                                    searchItem.ParentItemId = basketItem.BasketItemId;
                                    searchItem.ProductId = product.ProductId;
                                    searchItem.OptionList = kitProduct.OptionList;
                                    searchItem.BasketShipmentId = basketItem.BasketShipmentId;

                                    // LOOK FOR ITEM
                                    BasketItem childItem = FindChildProduct(childProducts, searchItem);

                                    // UPDATE CALCULATED PROPERTIES
                                    childItem.Name = kitProduct.DisplayName;
                                    childItem.Quantity = (short)(kitProduct.Quantity * basketItem.Quantity);
                                    childItem.TaxCodeId = product.TaxCodeId;
                                    childItem.Shippable = product.Shippable;
                                    childItem.Price = kitProduct.CalculatedPrice / kitProduct.Quantity; ;
                                    childItem.Weight = kitProduct.CalculatedWeight / kitProduct.Quantity;
                                    // CALCULATE SKU
                                    ProductCalculator childCalc = ProductCalculator.LoadForProduct(childItem.ProductId, childItem.Quantity, childItem.OptionList, childItem.KitList, basket.UserId);
                                    childItem.Sku = childCalc.Sku;

                                    basket.Items.Add(childItem);
                                    childItem.Save();
                                    masterPrice -= kitProduct.CalculatedPrice;
                                    masterWeight -= kitProduct.CalculatedWeight;
                                }
                            }

                            // UPDATE MASTER PRICE, FACTORING IN CHILD ITEMS
                            basketItem.Price = masterPrice;
                            basketItem.Weight = masterWeight;
                            basketItem.Save();
                        }
                    }
                }
            }

            // DELETE ANY CHILD PRODUCTS THAT WERE NOT PRESERVED
            foreach (BasketItem bi in childProducts)
            {
                bi.Delete();
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the collection contains shippable products
        /// </summary>
        public bool HasShippableProducts
        {
            get
            {
                foreach (BasketItem item in this)
                {
                    if (item.OrderItemType == OrderItemType.Product && item.Shippable != Shippable.No)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Counts the total quantity of shippable products
        /// </summary>
        public int ShippableProductCount
        {
            get
            {
                int totalCount = 0;
                List<int> parentItemIds = new List<int>();
                foreach (BasketItem item in this)
                {
                    if (item.OrderItemType == OrderItemType.Product && item.Shippable != Shippable.No)
                    {
                        if (!item.IsChildItem && !parentItemIds.Contains(item.BasketItemId))
                        {
                            // PARENT ITEMS ARE SHIPPABLE
                            totalCount += item.Quantity;
                            parentItemIds.Add(item.BasketItemId);
                        }
                        else
                        {
                            BasketItem parentItem = item.GetParentItem(true);
                            // CHILD ITEMS SHOULD ONLY COUNT ONCE FOR THE PARENT ITEM
                            if (!parentItemIds.Contains(parentItem.BasketItemId))
                            {
                                totalCount += parentItem.Quantity;
                                parentItemIds.Add(parentItem.BasketItemId);
                            }
                        }
                    }
                }
                return totalCount;
            }
        }
    }
}
