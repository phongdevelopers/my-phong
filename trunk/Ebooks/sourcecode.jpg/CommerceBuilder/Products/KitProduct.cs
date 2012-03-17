using System;
using CommerceBuilder.Common;
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a KitProduct object in the database.
    /// </summary>
    public partial class KitProduct
    {
        /// <summary>
        /// Price Mode
        /// </summary>
        public InheritanceMode PriceMode
        {
            get
            {
                return (InheritanceMode)this.PriceModeId;
            }
            set
            {
                this.PriceModeId = (byte)value;
            }
        }

        /// <summary>
        /// Weight Mode
        /// </summary>
        public InheritanceMode WeightMode
        {
            get
            {
                return (InheritanceMode)this.WeightModeId;
            }
            set
            {
                this.WeightModeId = (byte)value;
            }
        }

        private Product _Product;
        /// <summary>
        /// Associated Product
        /// </summary>
        public Product Product
        {
            get
            {
                if (_Product == null)
                {
                    _Product = ProductDataSource.Load(this.ProductId);
                }
                return _Product;
            }
        }

        private ProductVariant _ProductVariant;
        /// <summary>
        /// Product Variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if ((_ProductVariant == null) && (!string.IsNullOrEmpty(_OptionList)))
                {
                    _ProductVariant = ProductVariantDataSource.LoadForOptionList(this.ProductId, this.OptionList);
                }
                return _ProductVariant;
            }
        }

        /// <summary>
        /// The total price for this kit product for a single quantity of the kit.
        /// </summary>
        public LSDecimal CalculatedPrice
        {
            get
            {
                if (this.PriceMode == InheritanceMode.Override) return this.Price * this.Quantity;
                LSDecimal basePrice = 0;
                if (this.Product != null)
                {
                    string optionList = this.OptionList;
                    if (!string.IsNullOrEmpty(optionList))
                    {
                        if (this.ProductVariant == null)
                        {
                            //VARIANT COULD NOT BE LOADED, DO NOT INCLUDE OPTIONS IN PRICE CALC
                            Logger.Error("Invalid options specified for " + this.Product.Name + " as part of kit component " + this.KitComponent.Name);
                            optionList = string.Empty;
                        }
                    }
                    ProductCalculator pc = ProductCalculator.LoadForProduct(this.ProductId, this.Quantity, optionList, string.Empty);
                    basePrice = pc.Price;
                }
                if (this.PriceMode == InheritanceMode.Inherit) return basePrice * this.Quantity;
                return (basePrice + this.Price) * this.Quantity;
            }
        }

        /// <summary>
        /// Calculated Weight
        /// </summary>
        public LSDecimal CalculatedWeight
        {
            get
            {
                if (this.WeightMode == InheritanceMode.Override) return this.Weight * this.Quantity;
                LSDecimal baseWeight = 0;
                if (this.Product != null)
                {
                    string optionList = this.OptionList;
                    if (!string.IsNullOrEmpty(optionList))
                    {
                        if (this.ProductVariant == null)
                        {
                            //VARIANT COULD NOT BE LOADED, DO NOT INCLUDE OPTIONS IN PRICE CALC
                            Logger.Error("Invalid options specified for " + this.Product.Name + " as part of kit component " + this.KitComponent.Name);
                            optionList = string.Empty;
                        }
                    }
                    ProductCalculator pc = ProductCalculator.LoadForProduct(this.ProductId, this.Quantity, optionList, string.Empty);
                    baseWeight = pc.Weight;
                }
                if (this.WeightMode == InheritanceMode.Inherit) return baseWeight * this.Quantity;
                return (baseWeight + this.Weight) * this.Quantity;
            }
        }

        /// <summary>
        /// Display Name
        /// </summary>
        public string DisplayName
        {
            get
            {
                Product product = this.Product;
                string displayName = this.Name.Replace("$name", product.Name);
                if (displayName.Contains("$options"))
                {
                    ProductVariant variant = this.ProductVariant;
                    string variantName = variant != null ? variant.VariantName : string.Empty;
                    displayName = displayName.Replace("$options", variantName);
                }
                displayName = displayName.Replace("$quantity", this.Quantity.ToString());
                if (displayName.Contains("$price"))
                {
                    LSDecimal shopPrice = TaxHelper.GetShopPrice(this.CalculatedPrice, this.Product.TaxCodeId);
                    displayName = displayName.Replace("$price", shopPrice.ToString("ulc"));
                }
                return displayName;
            }
        }

        /// <summary>
        /// Checks stock for the associated product
        /// </summary>
        /// <returns>InventoryManagerData object containing inventory details for the product</returns>
        public InventoryManagerData CheckStock()
        {
            Product product = this.Product;
            if (product == null) throw new InvalidProductException();
            if (product.InventoryMode == InventoryMode.None) return new InventoryManagerData(product.ProductId, InventoryMode.None, 0, false);
            if (product.InventoryMode == InventoryMode.Product)
            {
                return new InventoryManagerData(product.ProductId, InventoryMode.Product, product.InStock, product.AllowBackorder);
            }
            //HANDLE THE VARIANTS
            ProductVariant variant = this.ProductVariant;
            if (variant != null) return new InventoryManagerData(product.ProductId, InventoryMode.Variant, variant.InStock, product.AllowBackorder, this.OptionList);
            //INVALID VARIANT = NO STOCK AVAILABLE
            return new InventoryManagerData(product.ProductId, InventoryMode.Variant, 0, false);
        }
    }
}
