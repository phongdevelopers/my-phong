using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Payments;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Stores;
using CommerceBuilder.Marketing;
using CommerceBuilder.Taxes;
using System.ComponentModel;
using System.Transactions;
using System.Web;

namespace CommerceBuilder.Users
{
    public partial class Wishlist
    {
        /// <summary>
        /// Moves the source wishlist to the target wishlist if the source wishlist is not empty.
        /// </summary>
        /// <param name="sourceId">The ID of the source wishlist.</param>
        /// <param name="targetId">The ID of the target wishlist.</param>
        /// <remarks>Any existing contents of the target wishlist are removed prior to transfer.</remarks>
        public static void Transfer(int sourceId, int targetId)
        {
            Transfer(sourceId, targetId, false);
        }

        /// <summary>
        /// Moves the source wishlist to the target wishlist.
        /// </summary>
        /// <param name="sourceId">The ID of the user with the source wishlist.</param>
        /// <param name="targetId">The ID of the user to transfer the wishlist to.</param>
        /// <param name="transferEmptyWishlist">If false, the wishlist is not transferred when the source wishlist is empty.  If true, the wishlist is always transferred.</param>
        /// <remarks>Any existing contents of the target wishlist are removed prior to transfer.</remarks>
        public static void Transfer(int sourceId, int targetId, bool transferEmptyWishlist)
        {
            if (sourceId != targetId)
            {
                //GET THE DEFAULT BASKET FOR THE SOURCE USER
                WishlistCollection sourceWishlists = WishlistDataSource.LoadForUser(sourceId);
                if (sourceWishlists.Count == 0) return;
                Wishlist sourceWishlist = sourceWishlists[0];
                if (!transferEmptyWishlist)
                {
                    //WE SHOULD NOT TRANSFER EMPTY BASKETS, COUNT THE SOURCE ITEMS
                    int sourceCount = WishlistItemDataSource.CountForWishlist(sourceWishlist.WishlistId);
                    if (sourceCount == 0) return;
                }
                //DETERMINE WHETHER USER HAS A WISHLIST ALREADY
                Database database = Token.Instance.Database;
                DbCommand command = database.GetSqlStringCommand("SELECT WishlistId FROM ac_Wishlists WHERE UserId = @targetId");
                database.AddInParameter(command, "@targetId", System.Data.DbType.Int32, targetId);
                int targetWishlistId = AlwaysConvert.ToInt(database.ExecuteScalar(command));
                if (targetWishlistId == 0)
                {
                    //USER HAS NO WISHLIST, SO MOVE THE SOURCE WISHLIST TO NEW USER
                    sourceWishlist.UserId = targetId;
                    sourceWishlist.Save();
                }
                else
                {
                    //USER HAS A WISHLIST, JUST MOVE ITEMS FROM SOURCE TO TARGET
                    command = database.GetSqlStringCommand("UPDATE ac_WishlistItems SET WishlistId = @targetWishlistId WHERE WishlistId = @sourceWishlistId");
                    database.AddInParameter(command, "@targetWishlistId", System.Data.DbType.Int32, targetWishlistId);
                    database.AddInParameter(command, "@sourceWishlistId", System.Data.DbType.Int32, sourceWishlist.WishlistId);
                    database.ExecuteNonQuery(command);
                }
            }
        }

        /// <summary>
        /// Validates the contents of this wishlist
        /// </summary>
        /// <param name="warningMessages">contains any validation errors or warning messages</param>
        /// <returns><b>true</b> if there are no validation errors, <b>false</b> otherwise</returns>
        public bool Validate(out List<string> warningMessages)
        {
            warningMessages = new List<string>();
            //CHECK FOR ANY PRODUCTS WITH VARIANTS THAT ARE NOT VALID
            List<int> invalidItems = new List<int>();
            foreach (WishlistItem item in this.Items)
            {
                if (item.Product == null || item.Product.Visibility == CommerceBuilder.Catalog.CatalogVisibility.Private)
                {
                    warningMessages.Add(string.Format(Properties.Resources.WishlistItemUnavailable, item.Product.Name));
                    invalidItems.Add(item.WishlistItemId);
                }
                else
                {
                    if (item.Product.ProductOptions.Count > 0)
                    {
                        //CHECK WHETHER A VALID VARIANT IS ASSOCIATED WITH THIS BASKET ITEM
                        ProductVariant pv = item.ProductVariant;
                        if (pv == null)
                        {
                            //VARIANT COULD NOT BE DETERMINED
                            warningMessages.Add(string.Format(Properties.Resources.WishlistItemUnavailable, item.Product.Name));
                            invalidItems.Add(item.WishlistItemId);
                        }
                        else if (!pv.Available)
                        {
                            //VARIANT IS MARKED NOT AVAILABLE
                            warningMessages.Add(string.Format(Properties.Resources.WishlistItemUnavailable, item.Product.Name + " (" + pv.VariantName + ")"));
                            invalidItems.Add(item.WishlistItemId);
                        }
                    }
                    else
                    {
                        //THE ITEM SHOULD NOT HAVE AN OPTION LIST ASSOCIATED
                        item.OptionList = string.Empty;
                        item.Save();
                    }

                    // VALIDATE KIT
                    if (item.Product.KitStatus == KitStatus.Master)
                    {
                        String formattedMessage = String.Empty;
                        Kit kit = item.Product.Kit;
                        string originalList = item.KitList;
                        item.KitList = kit.RefreshKitProducts(item.KitList);
                        bool validKit = item.Product.Kit.ValidateChoices(item.KitList);
                        if (!validKit)
                        {
                            formattedMessage = string.Format(Properties.Resources.WishlistItemUnavailable, item.Product.Name);
                            if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                            invalidItems.Add(item.WishlistItemId);
                        }
                        else if (originalList != item.KitList)
                        {
                            formattedMessage = string.Format(Properties.Resources.BasketItemModified, item.Product.Name);
                            if (!warningMessages.Contains(formattedMessage)) warningMessages.Add(formattedMessage);
                            item.Save();
                        }
                    }
                }
            }
            
            foreach (int invalidBasketItemId in invalidItems)
            {
                int index = this.Items.IndexOf(invalidBasketItemId);
                if (index > -1) this.Items.DeleteAt(index);
            }
            return (warningMessages.Count == 0);
        }

        /// <summary>
        /// Save this wishlist for the given user
        /// </summary>
        /// <param name="userID">Id of the user for which to save this wishlist</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of save operation</returns>
        public SaveResult SaveForUser(int userID)
        {
            User user = UserDataSource.Load(userID);
            //MAKE SURE A USER ACCOUNT EXISTS
            if (user != null)
            {
                this.UserId = user.UserId;
                return this.BaseSave();
            }
            else
            {
                //associate with an anonymous user
                return this.Save();
            }            
        }

        /// <summary>
        /// Save this wishlist object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of save operation</returns>
        public SaveResult Save()
        {
            User user = Token.Instance.User;
            //MAKE SURE A USER ACCOUNT EXISTS
            if (user.UserId == 0)
            {
                user.Save();
                this.UserId = user.UserId;
            }
            return this.BaseSave();
        }

        /// <summary>
        /// Recalculates the prices of wishlist items based on any updates by the merchant
        /// </summary>
        public void Recalculate()
        {
            foreach (WishlistItem item in Items)
            {
                item.Recalculate();
            }
        }
    }
}
