using System;
using System.Collections;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// IComparer implementation of OrderItem objects
    /// </summary>
    public class OrderItemComparer : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Less than zero if x is less than y. <br/>
        /// Zero of x equals y.<br/>
        /// Greater than zero if x is greater than y.<br/>
        /// </returns> 
        public int Compare(object x, object y)
        {
            OrderItem item1 = x as OrderItem;
            OrderItem item2 = y as OrderItem;
            // COMPARING WITH YOUR OWN CHILD
            if (item1.IsChildItem && item1.ParentItemId == item2.OrderItemId) return 1;
            // COMPARING WITH YOUR OWN PARENT
            if (item2.IsChildItem && item2.ParentItemId == item1.OrderItemId) return -1;
            // USE SORT PRIORITY, TREAT CHILD ITEMS AS PRODUCTS
            int item1Priority = GetSortPriority(item1, true);
            int item2Priority = GetSortPriority(item2, true);
            int result = item1Priority.CompareTo(item2Priority);
            if (result != 0) return result;
            // SORT BY ROOT PARENT ID TO GROUP ALL RELATED PRODUCTS
            int item1CompareId = item1.GetParentItem(true).OrderItemId;
            int item2CompareId = item2.GetParentItem(true).OrderItemId;
            result = item1CompareId.CompareTo(item2CompareId);
            if (result != 0) return result;
            // SORT BY BY ITEM ID, BUT USE PARENT IDS FOR TAXES
            if (item1.IsChildItem && item1.OrderItemType == OrderItemType.Tax)
            {
                item1CompareId = item1.ParentItemId;
            }
            else item1CompareId = item1.OrderItemId;
            if (item2.IsChildItem && item2.OrderItemType == OrderItemType.Tax)
            {
                item2CompareId = item2.ParentItemId;
            }
            else item2CompareId = item2.OrderItemId;
            result = item1CompareId.CompareTo(item2CompareId);
            if (result != 0) return result;
            // USE SORT PRIORITY AND DO NOT GIVE SPECIAL HANDLING FOR CHILD ITEMS
            item1Priority = GetSortPriority(item1, false);
            item2Priority = GetSortPriority(item2, false);
            result = item1Priority.CompareTo(item2Priority);
            if (result != 0) return result;
            // FINALLY RESORT TO ORDER BY VALUES
            result = item1.OrderBy.CompareTo(item2.OrderBy);
            return result;
        }

        /// <summary>
        /// Determines the sort priority based on item type.
        /// </summary>
        /// <param name="item">The item to get sort priority for.</param>
        /// <param name="treatChildAsProduct">If true, child items are treated as products.  If false, the sort priority is based on the type of the child item.</param>
        /// <returns>An integer indicating the sort priority, lower values should appear before higher values.</returns>
        public int GetSortPriority(OrderItem item, bool treatChildAsProduct)
        {
            OrderItemType checkType = item.OrderItemType;
            if ((treatChildAsProduct) && item.IsChildItem)
            {
                OrderItem parent = item.GetParentItem(false);
                if (parent != null) checkType = parent.OrderItemType;
            }
            switch (checkType)
            {
                case OrderItemType.Product:
                    return 0;
                case OrderItemType.Discount:
                    return 1;
                case OrderItemType.GiftWrap:
                    return 2;
                case OrderItemType.Shipping:
                    return 3;
                case OrderItemType.Handling:
                    return 4;
                case OrderItemType.Coupon:
                    return 5;
                case OrderItemType.Tax:
                    return 6;
                case OrderItemType.Charge:
                    return 7;
                case OrderItemType.Credit:
                    return 8;
                default:
                    return 9;
            }
        }

        #endregion
    }
}