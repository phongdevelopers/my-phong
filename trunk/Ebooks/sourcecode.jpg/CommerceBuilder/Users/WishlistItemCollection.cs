namespace CommerceBuilder.Users
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Orders;
    using CommerceBuilder.Utility;

    public partial class WishlistItemCollection
    {
        /// <summary>
        /// Saves this collection of WishlistItem objects to database
        /// </summary>
        /// <returns><b>true</b> if all WishlistItem objects in this collection are saved successfuly, <b>false</b> otherwise</returns>
        public override bool Save()
        {
            this.CombineItems();
            bool allSaved = true;
            foreach (WishlistItem item in this)
            {
                allSaved = (allSaved && (item.Save() != SaveResult.Failed));
            }
            return allSaved;
        }

        private void CombineItems()
        {
            WebTrace.Write("Begin Combine");
            WishlistItem item;
            for (int i = this.Count - 1; i > 0; i--)
            {
                item = this[i];
                if (item.Desired > 0)
                {
                    WebTrace.Write("i: " + i.ToString() + ", WishlistItemId: " + item.WishlistItemId.ToString() + ", ProductId: " + item.ProductId);
                    WishlistItem match = this.Find(new System.Predicate<WishlistItem>(item.CanCombine));
                    WebTrace.Write("(match!=null): " + (match != null));
                    if ((match != null) && (item.WishlistItemId != match.WishlistItemId))
                    {
                        WebTrace.Write("match.WishlistItemId: " + match.WishlistItemId.ToString());
                        match.Desired += item.Desired;
                        this.DeleteAt(i);
                    }
                }
                else
                {
                    this.DeleteAt(i);
                }
            }
        }

        internal BasketItem GetBasketItem(int index, Basket basket)
        {
            WishlistItem item = this[index];
            BasketItem basketItem = BasketItemDataSource.CreateForProduct(item.ProductId, item.Desired, item.OptionList, item.KitList);
            basketItem.BasketId = basket.BasketId;
            if (item.Product.UseVariablePrice)
            {
                basketItem.Price = item.Price;
                if (basketItem.Price < item.Product.MinimumPrice) basketItem.Price = item.Product.MinimumPrice;
                if (basketItem.Price > item.Product.MaximumPrice) basketItem.Price = item.Product.MaximumPrice;
            }
            basketItem.WishlistItemId = item.WishlistItemId;
            basketItem.LineMessage = item.LineMessage;
            //COPY OVER ITEM INPUTS
            foreach (WishlistItemInput input in item.Inputs)
            {
                BasketItemInput cloneInput = new BasketItemInput();
                cloneInput.InputFieldId = input.InputFieldId;
                cloneInput.InputValue = input.InputValue;
                basketItem.Inputs.Add(cloneInput);
            }
            return basketItem;
        }

        /// <summary>
        /// Copies this collection of Wishlist items to the given basket
        /// </summary>
        /// <param name="index">Index of the basket item</param>
        /// <param name="basket">The basket to add to</param>
        public void CopyToBasket(int index, Basket basket)
        {
            BasketItem basketItem = GetBasketItem(index, basket);
            basket.Items.Add(basketItem);
            basket.Save();
        }

        /// <summary>
        /// Moves this collection of Wishlist items to the given basket
        /// </summary>
        /// <param name="index">Index of the basket item</param>
        /// <param name="basket">The basket to move to</param>
        public void MoveToBasket(int index, Basket basket)
        {
            BasketItem basketItem = GetBasketItem(index, basket);
            basket.Items.Add(basketItem);
            basket.Save();
            this.DeleteAt(index);
        }

        /// <summary>
        /// Adds a new basket item to this WishlistItem collection
        /// </summary>
        /// <param name="item">Basket item based on which to create a new WishlistItem</param>
        public void Add(BasketItem item)
        {
            WishlistItem wishlistItem = new WishlistItem();
            wishlistItem.ProductId = item.ProductId;
            wishlistItem.OptionList = item.OptionList;
            wishlistItem.KitList = item.KitList;
            wishlistItem.Desired = item.Quantity;
            //DEFAULT MEDIUM PRIORITY
            wishlistItem.Priority = 2;
            if (item.Product.UseVariablePrice) wishlistItem.Price = item.Price;
            wishlistItem.LineMessage = item.LineMessage;
            //COPY OVER ITEM INPUTS
            foreach (BasketItemInput input in item.Inputs)
            {
                WishlistItemInput cloneInput = new WishlistItemInput();
                cloneInput.InputFieldId = input.InputFieldId;
                cloneInput.InputValue = input.InputValue;
                wishlistItem.Inputs.Add(cloneInput);
            }
            this.Add(wishlistItem);
        }

    }
}
