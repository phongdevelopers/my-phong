using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using CommerceBuilder.Orders;

namespace CommerceBuilder.Products
{    
    /// <summary>
    /// Utility class to support inventory management
    /// </summary>
    public class InventoryManager
    {
        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="productId">Id of the product to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(int productId)
        {
            return CheckStock(productId, string.Empty, null);
        }

        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="productId">Id of the product to check stock for</param>
        /// <param name="optionList">The option list of the product to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(int productId, string optionList)
        {
            return CheckStock(productId, optionList, null);
        }

        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="productId">Id of the product to check stock for</param>
        /// <param name="optionList">The option list of the product to check stock for</param>
        /// <param name="kitProductIds">If its a kit product the Ids of the kit products to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(int productId, string optionList, List<int> kitProductIds)
        {
            Product product = ProductDataSource.Load(productId);
            if (product == null) throw new InvalidProductException();
            if (kitProductIds == null) kitProductIds = new List<int>();
            ProductVariant variant = null;
            if (!string.IsNullOrEmpty(optionList))
            {
                variant = ProductVariantDataSource.LoadForOptionList(productId, optionList);
            }

            List<KitProduct> kitProducts = new List<KitProduct>();
            foreach(int kpid in kitProductIds) 
            {
                KitProduct kp = KitProductDataSource.Load(kpid);
                if(kp!=null) kitProducts.Add(kp);
            }
            return CheckStock(product, variant, kitProducts);
        }

        /// <summary>
        /// Checks stock for given basket item
        /// </summary>
        /// <param name="item">BasketItem that tells the product to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(BasketItem item)
        {
            return CheckStock(item.Product, item.ProductVariant, item.GetKitProducts());
        }

        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="product"></param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(Product product)
        {
            return CheckStock(product, null, null);
        }

        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="product">Product to check stock for</param>
        /// <param name="variant">Product Variant to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(Product product, ProductVariant variant)
        {
            return CheckStock(product, variant, null);
        }

        /// <summary>
        /// Checks stock for given product
        /// </summary>
        /// <param name="product">Product to check stock for</param>
        /// <param name="variant">Product Variant to check stock for</param>
        /// <param name="kitProducts">In case of a kit product, the list of kit products to check stock for</param>
        /// <returns>InventoryManagerData object containing stock details for the given product</returns>
        public static InventoryManagerData CheckStock(Product product, ProductVariant variant, List<KitProduct> kitProducts)
        {
            if (product == null) throw new ArgumentNullException("product");

            // DETERMINE STOCK LEVEL OF THE SPECIFIED PRODUCT
            InventoryManagerData inventoryData;
            switch (product.InventoryMode)
            {
                case InventoryMode.None:
                    inventoryData = new InventoryManagerData(product.ProductId, InventoryMode.None, 0, false);
                    break;
                case InventoryMode.Product:
                    inventoryData = new InventoryManagerData(product.ProductId, InventoryMode.Product, product.InStock, product.AllowBackorder);
                    break;
                default:
                    // VARIANT MODE IS THE ONLY REMAINING OPTION
                    if (variant != null)
                    {
                        inventoryData = new InventoryManagerData(product.ProductId, InventoryMode.Variant, variant.InStock, product.AllowBackorder);
                    }
                    else
                    {
                        // MISSING OR INVALID VARIANT = RETURN AS NO STOCK AVAILABLE
                        inventoryData = new InventoryManagerData(product.ProductId, InventoryMode.Variant, 0, false);
                    }
                    break;
            }

            // IF ASSOCIATED KIT PRODUCTS ARE SPECIFIED, BUILD THE INVENTORY DATA
            if (kitProducts != null && kitProducts.Count > 0)
            {
                // LOOP ALL KIT PROUDCTS
                foreach (KitProduct kp in kitProducts)
                {
                    // GET INVENTORY DATA FOR THE KIT PRODUCT
                    InventoryManagerData kpstock = kp.CheckStock();

                    // SEE WHETHER THIS KIT PRODUCT TRACKS INVENTORY
                    if (kpstock.InventoryMode != InventoryMode.None)
                    {
                        // UNLESS ALL PRODUCTS ALLOW BACKORDER, BACKORDER IS UNAVAILABLE
                        if (!kpstock.AllowBackorder) inventoryData.AllowBackorder = false;

                        // SET MULTIPLIER AND ADD TO SUB-INVENTORY DATA
                        kpstock.Multiplier = kp.Quantity;
                        inventoryData.AddSubItemInventoryData(kpstock);

                        if (!kpstock.AllowBackorder)
                        {
                            //CALCULATE THE AVAILABLE STOCK BASED ON THE QUANTITY REQUIRED BY THE KIT
                            int childAvailableStock = kpstock.InStock / kp.Quantity;
                            if (inventoryData.InventoryMode == InventoryMode.None)
                            {
                                // THE MASTER PRODUCT DOES NOT HAVE INVENTORY TRACKING
                                // RE-INITIALIZE INVENTORY DATA
                                inventoryData.InventoryMode = InventoryMode.Product;
                                inventoryData.InStock = childAvailableStock;
                            }
                            else
                            {
                                // REDUCE AVAILABLE STOCK TO THE LOWEST OF THE BATCH
                                if (inventoryData.InStock > childAvailableStock) inventoryData.InStock = childAvailableStock;
                            }
                        }
                    }
                }
            }

            // RETURN THE COMPILED INVENTORY DATA
            return inventoryData;
        }

        /// <summary>
        /// Destock a given quantity of the given product from inventory
        /// </summary>
        /// <param name="quantity">Quantity of the product to destock</param>
        /// <param name="productId">Id of the product to destock</param>
        /// <param name="lowStock">value returned in this variable indicates whether after destocking the product has reached low stock warning level or not</param>
        public static void Destock(short quantity, int productId, out bool lowStock)
        {
            Destock(quantity, productId, string.Empty, out lowStock);
        }

        /// <summary>
        /// Destock a given quantity of the given product from inventory
        /// </summary>
        /// <param name="quantity">Quantity of the product to destock</param>
        /// <param name="productId">Id of the product to destock</param>
        /// <param name="optionList">The option list of the product to check stock for</param>
        /// <param name="lowStock">value returned in this variable indicates whether after destocking the product has reached low stock warning level or not</param>
        public static void Destock(short quantity, int productId, string optionList, out bool lowStock)
        {
            int stockIndicator;
            Product product = ProductDataSource.Load(productId);
            if (product == null) throw new InvalidProductException();
            if (product.InventoryMode == InventoryMode.None) throw new InvalidProductException();
            if (product.InventoryMode == InventoryMode.Product)
            {
                //DECREMENT PRODUCT STOCK
                Database database = Token.Instance.Database;
                using (DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Products SET InStock = InStock - @quantity WHERE ProductId = @productId;SELECT InStock - InStockWarningLevel As LowStock FROM ac_Products WHERE ProductId = @productId"))
                {
                    database.AddInParameter(updateCommand, "@productId", System.Data.DbType.Int32, productId);
                    database.AddInParameter(updateCommand, "@quantity", System.Data.DbType.Int16, quantity);
                    stockIndicator = AlwaysConvert.ToInt(database.ExecuteScalar(updateCommand));
                }
            }
            else
            {
                //GET THE VARIANT
                ProductVariant variant = ProductVariantDataSource.LoadForOptionList(productId, optionList);
                if (variant != null)
                {
                    //DECREMENT VARIANT STOCK
                    Database database = Token.Instance.Database;
                    using (DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_ProductVariants SET InStock = InStock - @quantity WHERE ProductVariantId = @productVariantId;SELECT InStock - InStockWarningLevel As LowStock FROM ac_ProductVariants WHERE ProductVariantId = @productVariantId"))
                    {
                        database.AddInParameter(updateCommand, "@productVariantId", System.Data.DbType.Int32, variant.ProductVariantId);
                        database.AddInParameter(updateCommand, "@quantity", System.Data.DbType.Int16, quantity);
                        stockIndicator = AlwaysConvert.ToInt(database.ExecuteScalar(updateCommand));
                    }
                }
                else
                {
                    Logger.Error("Could not destock " + quantity.ToString() + " of variant " + optionList + " for Product " + productId.ToString());
                    //DO NOT WARN ON LOW STOCK FOR ERROR CONDITION
                    stockIndicator = 1;
                }
            }
            lowStock = (stockIndicator < 1);
        }

        /// <summary>
        /// Restock a given quantity of the given product to the inventory
        /// </summary>
        /// <param name="quantity">Quantity of the product to restock</param>
        /// <param name="productId">Id of the product to restock</param>
        public static void Restock(short quantity, int productId)
        {
            Restock(quantity, productId, string.Empty);
        }

        /// <summary>
        /// Restock a given quantity of the given product to the inventory
        /// </summary>
        /// <param name="quantity">Quantity of the product to restock</param>
        /// <param name="productId">Id of the product to restock</param>
        /// <param name="optionList">The option list of the product to check stock for</param>
        public static void Restock(short quantity, int productId, string optionList)
        {
            Product product = ProductDataSource.Load(productId);
            if (product == null) throw new InvalidProductException("The specified product does not exist.");
            if (product.InventoryMode == InventoryMode.None) throw new InvalidProductException("The specified product does not have inventory enabled.");
            if (product.InventoryMode == InventoryMode.Product)
            {
                //INCREMENT PRODUCT STOCK
                Database database = Token.Instance.Database;
                using (DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Products SET InStock = InStock + @quantity WHERE ProductId = @productId"))
                {
                    database.AddInParameter(updateCommand, "@productId", System.Data.DbType.Int32, productId);
                    database.AddInParameter(updateCommand, "@quantity", System.Data.DbType.Int32, quantity);
                    database.ExecuteNonQuery(updateCommand);
                }
            }
            else
            {
                //GET THE VARIANT
                ProductVariant variant = ProductVariantDataSource.LoadForOptionList(productId, optionList);
                if (variant != null)
                {
                    //INCREMENT VARIANT STOCK
                    Database database = Token.Instance.Database;
                    using (DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_ProductVariants SET InStock = InStock + @quantity WHERE ProductVariantId = @productVariantId"))
                    {
                        database.AddInParameter(updateCommand, "@productVariantId", System.Data.DbType.Int32, variant.ProductVariantId);
                        database.AddInParameter(updateCommand, "@quantity", System.Data.DbType.Int32, quantity);
                        database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    Logger.Error("Could not restock " + quantity.ToString() + " of variant " + optionList + " for Product " + productId.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Class that holds inventory data for a product
    /// </summary>
    public class InventoryManagerData
    {
        private InventoryMode _InventoryMode;
        private int _InStock;
        private bool _AllowBackorder;
        private List<InventoryManagerData> _ChildItemInventoryData = null;
        private int _Multiplier = 1;
        private int _ProductId;
        private string _OptionList;

        /// <summary>
        /// If the item is a variant then this will hold the option list.
        /// </summary>
        public string OptionList
        {
            get { return _OptionList; }
            set { _OptionList = value; }
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
        /// Quantity in stock
        /// </summary>
        public int InStock
        {
            get { return _InStock; }
            set { _InStock = value; }
        }

        /// <summary>
        /// Backorder is allowed or not?
        /// </summary>
        public bool AllowBackorder
        {
            get { return _AllowBackorder; }
            set { _AllowBackorder = value; }
        }

        /// <summary>
        /// Id of the product this inventory data is related to
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Id of the product this inventory data is related to</param>
        /// <param name="inventoryMode">Inventory Mode</param>
        /// <param name="inStock">Quantity in stock</param>
        /// <param name="allowBackorder">Whether backorder is allowed or not</param>
        public InventoryManagerData(int productId, InventoryMode inventoryMode, int inStock, bool allowBackorder)
        {
            this.InventoryMode = inventoryMode;
            this.InStock = inStock;
            this.AllowBackorder = allowBackorder;
            this.ProductId = productId;
            this._ChildItemInventoryData = new List<InventoryManagerData>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productId">Id of the product this inventory data is related to</param>
        /// <param name="inventoryMode">Inventory Mode</param>
        /// <param name="inStock">Quantity in stock</param>
        /// <param name="allowBackorder">Whether backorder is allowed or not</param>
        /// <param name="optionList">Option List if the data is for a product variant</param>
        public InventoryManagerData(int productId, InventoryMode inventoryMode, int inStock, bool allowBackorder, string optionList):this(productId, inventoryMode, inStock, allowBackorder)
        {
            this.OptionList = optionList;
        }

        /// <summary>
        /// Whether inventory data for sub items exists
        /// </summary>
        /// <returns><b>true</b> if inventory data for sub items exists, <b>false</b> otherwise</returns>
        public bool HasSubItemInventoryData()
        {
            return _ChildItemInventoryData.Count > 0;
        }

        /// <summary>
        /// Sub-Items inventory data details
        /// </summary>
        public List<InventoryManagerData> ChildItemInventoryData
        {
            get { return _ChildItemInventoryData; }
        }

        /// <summary>
        /// Multiplier
        /// </summary>
        public int Multiplier
        {
            get { return _Multiplier; }
            set { _Multiplier = value; }
        }

        /// <summary>
        /// Adds inventory data for a sub item
        /// </summary>
        /// <param name="invdata"></param>
        public void AddSubItemInventoryData(InventoryManagerData invdata)
        {
            if (invdata == null) return;
            if (_ChildItemInventoryData == null)
            {
                _ChildItemInventoryData = new List<InventoryManagerData>();
            }
            _ChildItemInventoryData.Add(invdata);
        }

        /// <summary>
        /// Creates a string representation of the inventory details
        /// </summary>
        /// <returns>A string representation of the inventory details</returns>
        public override string ToString()
        {
            string result = "ProductId : " + _ProductId.ToString();
            if(!String.IsNullOrEmpty(_OptionList)) result += "\tOption List: " + _OptionList;
            result += "\tInventory Mode : " + _InventoryMode.ToString();
            result += "\tIn Stock : " + InStock.ToString();
            result += "\tAllow Backorder : " + AllowBackorder.ToString();
            if (_ChildItemInventoryData != null)
            {
                result += "\r\nSub Items - " + _ChildItemInventoryData.Count + "\r\n" ;
                foreach (InventoryManagerData invD in _ChildItemInventoryData)
                {
                    result += invD.ToString();
                }
            }
            return result + "\r\n";
        }
    }
}
