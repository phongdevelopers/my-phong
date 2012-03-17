using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Products;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class that holds inventory details of a basket item
    /// </summary>
    public class BasketItemInventory
    {
        private int _BasketItemId;
        private InventoryMode _InventoryMode;
        private int _InStock;
        private bool _AllowBackorder;

        /// <summary>
        /// Id of the basket item
        /// </summary>
        public int BasketItemId
        {
            get { return _BasketItemId; }
            set { _BasketItemId = value; }
        }

        /// <summary>
        /// Inventory mode
        /// </summary>
        public InventoryMode InventoryMode
        {
            get { return _InventoryMode; }
            set { _InventoryMode = value; }
        }

        /// <summary>
        /// Number of items in stock
        /// </summary>
        public int InStock
        {
            get { return _InStock; }
            set { _InStock = value; }
        }

        /// <summary>
        /// Is backorder allowed for this item
        /// </summary>
        public bool AllowBackorder
        {
            get { return _AllowBackorder; }
            set { _AllowBackorder = value; }
        }
    }
}
