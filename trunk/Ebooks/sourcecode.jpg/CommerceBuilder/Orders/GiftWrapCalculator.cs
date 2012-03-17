using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class for calculating gift wrap charges
    /// </summary>
    public static class GiftWrapCalculator
    {
        /// <summary>
        /// Calculates gift wrap charges for the given basket
        /// </summary>
        /// <param name="basket">The basket for which to perform calculations</param>
        public static void Calculate(Basket basket)
        {
            //DELETE ANY GIFTWRAP ITEMS CURRENTLY IN THE BASKET
            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand("DELETE FROM ac_BasketItems WHERE BasketId = @basketId AND OrderItemTypeId = @orderItemType");
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            database.AddInParameter(command, "@orderItemType", System.Data.DbType.Int16, OrderItemType.GiftWrap);
            database.ExecuteNonQuery(command);
            //LOOP BASKET ITEMS IN REVERSE, REMOVING ANY THAT MATCH GIFTWRAP TYPE
            for (int i = (basket.Items.Count - 1); i >= 0; i--)
            {
                if (basket.Items[i].OrderItemType == OrderItemType.GiftWrap) basket.Items.RemoveAt(i);
            }
            //LOOP BASKET ITEMS AND ADD GIFTWRAP ITEMS AS NEEDED
            List<BasketItem> wrapItemList = new List<BasketItem>();
            foreach (BasketItem parentItem in basket.Items)
            {
                //CHECK WHETHER THIS ITEM HAS GIFTWRAP
                if (parentItem.WrapStyle != null) {
                    //ADD A NEW ITEM FOR GIFTWRAP
                    BasketItem wrapItem = new BasketItem();
                    wrapItem.BasketId = basket.BasketId;
                    wrapItem.ParentItemId = parentItem.BasketItemId;
                    wrapItem.BasketShipmentId = parentItem.BasketShipmentId;
                    wrapItem.OrderItemType = OrderItemType.GiftWrap;
                    wrapItem.Name = parentItem.WrapStyle.Name;
                    wrapItem.Quantity = parentItem.Quantity;
                    wrapItem.Price = parentItem.WrapStyle.Price;
                    wrapItem.TaxCodeId = parentItem.WrapStyle.TaxCodeId;
                    wrapItem.Shippable = parentItem.Shippable;
                    wrapItem.Save();
                    wrapItemList.Add(wrapItem);
                }
            }
            //NOW ADD NEW ITEMS TO BASKET
            basket.Items.AddRange(wrapItemList);
        }
    }
}
