using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Products;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class representing product inventory details
    /// </summary>
    public class ProductInventoryDetail
    {
        private int _ProductId;
        private string _Name;
        private int _ProductVariantId;
        private string _VariantName;
        private int _InStock;
        private int _InStockWarningLevel;
        private ProductVariant _Variant;

        /// <summary>
        /// Id of the product
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
        }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Product variant Id
        /// </summary>
        public int ProductVariantId
        {
            get { return _ProductVariantId; }
        }

        /// <summary>
        /// The product variant 
        /// </summary>
        public ProductVariant Variant
        {
            get
            {
                if (_Variant == null)
                {
                    if (ProductVariantId > 0)
                    {
                        _Variant = ProductVariantDataSource.Load(ProductVariantId);
                    }
                    else
                    {
                        _Variant = null;
                    }
                }
                return _Variant;
            }
        }

        /// <summary>
        /// The name of the product variant
        /// </summary>
        public string VariantName
        {            
            get
            {
                if (ProductVariantId != 0 && Variant!=null)
                {                 
                    return Variant.VariantName;                 
                }
                return "";
            }
        }

        /// <summary>
        /// The number of units in stock
        /// </summary>
        public int InStock
        {
            get { return _InStock; }
        }

        /// <summary>
        /// The warning level for low stock
        /// </summary>
        public int InStockWarningLevel
        {
            get { return _InStockWarningLevel; }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="name">Name of the product</param>
        /// <param name="productVariantId">Id of the product variant</param>
        /// <param name="variantName">Name of the product variant</param>
        /// <param name="inStock">Number of units in stock</param>
        /// <param name="inStockWarningLevel">Low stock warning level</param>
        public ProductInventoryDetail(int productId, string name, int productVariantId, string variantName, int inStock, int inStockWarningLevel)
        {
            _ProductId = productId;
            _Name = name;
            _ProductVariantId = productVariantId;
            _VariantName = variantName;
            _InStock = inStock;
            _InStockWarningLevel = inStockWarningLevel;
        }
    }
}
