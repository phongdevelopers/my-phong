using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class that holds summary data about product-breakdown sales
    /// </summary>
    public class ProductBreakdownSummary
    {
        private int _ProductId;
        /// <summary>
        /// The product Id
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }

        private string _name;
        /// <summary>
        /// The product Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _Sku;
        /// <summary>
        /// The SKU
        /// </summary>
        public string Sku
        {
            get { return _Sku; }
            set { _Sku = value; }
        }

        private string _Vendor;
        /// <summary>
        /// The vendor
        /// </summary>
        public string Vendor
        {
            get { return _Vendor; }
            set { _Vendor = value; }
        }

        private int _Quantity;
        /// <summary>
        /// The quantity
        /// </summary>
        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        private CommerceBuilder.Common.LSDecimal _Amount;
        /// <summary>
        /// The amount
        /// </summary>
        public CommerceBuilder.Common.LSDecimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }  

    }
}
