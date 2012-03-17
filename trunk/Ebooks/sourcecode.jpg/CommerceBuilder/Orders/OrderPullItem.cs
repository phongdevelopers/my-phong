using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Track data necessary to generate an order pull sheet.
    /// </summary>
    public class OrderPullItem
    {
        private String _Sku = string.Empty;
        private String _Name = string.Empty;
        private String _VariantName = string.Empty;
        private Int16 _Quantity;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderPullItem() { }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderItem">The order item associated with this OrdePullItem</param>
        public OrderPullItem(OrderItem orderItem)
        {
            _Sku = orderItem.Sku;
            _Name = orderItem.Name;
            _VariantName = orderItem.VariantName;
            _Quantity = orderItem.Quantity;
        }

        /// <summary>
        /// SKU of the associated order item
        /// </summary>
        public String Sku
        {
            get { return this._Sku; }
            set { this._Sku = value; }
        }

        /// <summary>
        /// Name of the associated order item
        /// </summary>
        public String Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        /// <summary>
        /// Variant name of the associated order item
        /// </summary>
        public String VariantName
        {
            get { return this._VariantName; }
            set { this._VariantName = value; }
        }

        /// <summary>
        /// Quantity of the associated order item
        /// </summary>
        public Int16 Quantity
        {
            get { return this._Quantity; }
            set { this._Quantity = value; }
        }

    }
}
