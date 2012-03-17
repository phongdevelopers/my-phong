using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Support class for various product calculations
    /// </summary>
    public class ProductCalculator
    {
        private int _ProductId;
        private short _Quantity;
        private string _OptionList;
        private int[] _KitProductIds;
        private int _UserId;
        private LSDecimal _Price;
        private LSDecimal _PriceWithTax;
        private LSDecimal _Weight;
        private string _Sku;
        private Special _AppliedSpecial;        
        private bool _Calculated = false;
        private bool _CalculateTax = false;

        /// <summary>
        /// Id of the product
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
        }

        /// <summary>
        /// Quantity
        /// </summary>
        public short Quantity
        {
            get { return _Quantity; }
        }

        /// <summary>
        /// List of option ids if this is a variant
        /// </summary>
        public string OptionList
        {
            get { return _OptionList; }
        }

        /// <summary>
        /// List of Kit Product Ids
        /// </summary>
        public int[] KitProductIds
        {
            get { return _KitProductIds; }
        }

        /// <summary>
        /// User Id
        /// </summary>
        public int UserId
        {
            get { return _UserId; }
        }

        /// <summary>
        /// Price
        /// </summary>
        public LSDecimal Price
        {
            get { return _Price; }
        }

        /// <summary>
        /// Price inclusive of tax
        /// </summary>
        public LSDecimal PriceWithTax
        {
            get { return _PriceWithTax; }
        }

        /// <summary>
        /// Weight
        /// </summary>
        public LSDecimal Weight
        {
            get { return _Weight; }
        }

        /// <summary>
        /// SKU
        /// </summary>
        public string Sku
        {
            get { return _Sku; }
        }

        /// <summary>
        /// Has the calculation been made?
        /// </summary>
        public bool Calculated
        {
            get { return _Calculated; }
        }

        /// <summary>
        /// Get Special, If any special pricing rule is applied on this product, other wise it will return null.        
        /// </summary>
        public Special AppliedSpecial
        {
            get { return _AppliedSpecial; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Product id</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="optionList">List of option ids if this is a variant</param>
        /// <param name="kitList">List of Kit Product Ids</param>
        /// <param name="userId">User Id</param>
        /// <param name="calculateTax">If true, also calculates the PriceWithTax property to determine
        /// the shopping price inclusive of tax.</param>
        private ProductCalculator(int productId, short quantity, string optionList, string kitList, int userId, bool calculateTax)
        {
            _ProductId = productId;
            _Quantity = quantity;
            _OptionList = optionList;
            _KitProductIds = AlwaysConvert.ToIntArray(kitList);
            _UserId = userId;
            _CalculateTax = calculateTax;
            Calculate();
        }
        
        private LSDecimal GetPriceFromRules(Product product)
        {
            LSDecimal basePrice = product.Price;
            if (product.Specials.Count > 0)
            {
                foreach (Special special in product.Specials)
                {
                    if (special.Price < basePrice)
                    {
                        if (special.IsValid(_UserId))
                        {
                            basePrice = special.Price;
                            _AppliedSpecial = special;
                        }
                    }
                }
            }
            return basePrice;
        }

        private void Calculate()
        {
            Product product = ProductDataSource.Load(_ProductId);
            if (product != null)
            {
                if (string.IsNullOrEmpty(_OptionList))
                {
                    //NO VARIANT, SET VALUES FROM BASE PRODCUT
                    _Sku = product.Sku;
                    _Price = GetPriceFromRules(product);
                    _Weight = product.Weight;
                }
                else
                {
                    //VARIANT PRESENT, CHECK VARIANT FOR CORRECT VALUES
                    ProductVariant variant = ProductVariantDataSource.LoadForOptionList(_ProductId, _OptionList);
                    if (variant == null) throw new InvalidOptionsException("The options specified for " + product.Name + " are invalid.");
                    _Sku = variant.Sku;
                    if (variant.Price != 0)
                    {
                        if (variant.PriceMode == ModifierMode.Modify)
                            _Price = GetPriceFromRules(product) + variant.Price;
                        else _Price = variant.Price;
                    }
                    else _Price = GetPriceFromRules(product);
                    if (variant.Weight != 0)
                    {
                        if (variant.WeightMode == ModifierMode.Modify)
                            _Weight = product.Weight + variant.Weight;
                        else _Weight = variant.Weight;
                    }
                    else _Weight = product.Weight;
                }

                // CALCULATE PRICE WITH TAX IF SPECIFIED
                if (_CalculateTax)
                {
                    _PriceWithTax = TaxHelper.GetShopPrice(_Price, product.TaxCodeId);
                }

                //CHECK FOR KIT PRODUCTS
                if (_KitProductIds != null)
                {
                    foreach (int kitProductId in _KitProductIds)
                    {
                        KitProduct kp = KitProductDataSource.Load(kitProductId);
                        if (kp != null)
                        {
                            _Price += kp.CalculatedPrice;
                            _Weight += kp.CalculatedWeight;
                            
                            // CALCULATE PRICE WITH TAX IF SPECIFIED
                            if (_CalculateTax)
                            {
                                int taxCodeId = 0;
                                KitComponent component = kp.KitComponent;
                                if (component.InputType == KitInputType.IncludedHidden) taxCodeId = product.TaxCodeId;
                                else if (kp.Product != null) taxCodeId = kp.Product.TaxCodeId;
                                _PriceWithTax += TaxHelper.GetShopPrice(kp.CalculatedPrice, taxCodeId);
                            }
                        }
                    }
                }
                _Calculated = true;
            }
        }

        /// <summary>
        /// Creates a ProductCalculator object for given product details
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="optionList">List of option ids if this is a variant</param>
        /// <param name="kitList">Generic integer list of the kit product Ids</param>
        /// <returns>A ProductCalculator object</returns>
        [Obsolete("Provided for backward compatibility only.  It is preferred to pass kitList as string variable with a comma delimited list of kit product ids.")]
        public static ProductCalculator LoadForProduct(int productId, short quantity, string optionList, List<int> kitList)
        {
            return LoadForProduct(productId, quantity, optionList, AlwaysConvert.ToList(",", kitList), Token.Instance.UserId, false);
        }

        /// <summary>
        /// Creates a ProductCalculator object for given product details
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="optionList">List of option ids if this is a variant</param>
        /// <param name="kitList">List of the kit product Ids</param>
        /// <returns>A ProductCalculator object</returns>
        public static ProductCalculator LoadForProduct(int productId, short quantity, string optionList, string kitList)
        {
            return LoadForProduct(productId, quantity, optionList, kitList, Token.Instance.UserId, false);
        }

                /// <summary>
        /// Creates a ProductCalculator object for given product details
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="optionList">List of option ids if this is a variant</param>
        /// <param name="kitList">List of the kit product Ids</param>
        /// <param name="userId">User Id</param>
        /// <returns>A ProductCalculator object</returns>
        public static ProductCalculator LoadForProduct(int productId, short quantity, string optionList, string kitList, int userId)
        {
            return LoadForProduct(productId, quantity, optionList, kitList, userId, false);
        }

        /// <summary>
        /// Creates a ProductCalculator object for given product details
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="optionList">List of option ids if this is a variant</param>
        /// <param name="kitList">List of the kit product Ids</param>
        /// <param name="userId">User Id</param>
        /// <param name="calculateTax">If true, also calculates the PriceWithTax property to determine
        /// the shopping price inclusive of tax.</param>
        /// <returns>A ProductCalculator object</returns>
        public static ProductCalculator LoadForProduct(int productId, short quantity, string optionList, string kitList, int userId, bool calculateTax)
        {
            string key = typeof(ProductCalculator).ToString() + GenerateHash(productId, quantity, optionList, kitList, userId, calculateTax);
            ProductCalculator cachedItem = ContextCache.GetObject(key) as ProductCalculator;
            if (cachedItem != null) return cachedItem;
            cachedItem = new ProductCalculator(productId, quantity, optionList, kitList, userId, calculateTax);
            if (cachedItem.Calculated)
            {
                ContextCache.SetObject(key, cachedItem);
                return cachedItem;
            }
            return null;
        }

        private static string GenerateHash(int productId, short quantity, string optionList, string kitList, int userId, bool calculateTax)
        {
            StringBuilder paramValues = new StringBuilder();
            paramValues.Append(productId.ToString() + "_");
            paramValues.Append(quantity.ToString() + "_");
            paramValues.Append(optionList + "_");
            paramValues.Append(kitList + "_");
            paramValues.Append(userId.ToString() + "_");
            paramValues.Append(calculateTax.ToString());
            return StringHelper.CalculateMD5Hash(paramValues.ToString());
        }
    }
}
