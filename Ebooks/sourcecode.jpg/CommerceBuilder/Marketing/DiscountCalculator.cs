using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Utility class for calculating discounts
    /// </summary>
    public class DiscountCalculator
    {
        /// <summary>
        /// Recalculates the discounts for a basket
        /// </summary>
        /// <param name="basket">The basket to calculte for</param>
        /// <returns>The total discount applied to the basket</returns>
        public static LSDecimal Calculate(Basket basket)
        {
            if (Token.Instance.Store.VolumeDiscountMode == VolumeDiscountMode.LineItem)
            {
                //USE LINE ITEM MODE
                return Calculate_LineItemMode(basket);
            }
            else
            {
                //USE CATEGORY GROUPING MODE
                return Calculate_GroupingMode(basket);
            }
        }

        /// <summary>
        /// Remove any existing discounts from basket
        /// </summary>
        /// <param name="basket">Basket to remove discounts from</param>
        private static void ClearExistingDiscounts(Basket basket)
        {
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if (item.OrderItemType == OrderItemType.Discount)
                {
                    basket.Items.DeleteAt(i);
                }
            }
        }

        private static LSDecimal Calculate_LineItemMode(Basket basket)
        {
            //KEEP TRACK OF TOTAL DISCOUNT APPLIED
            LSDecimal totalDiscount = 0;

            //CLEAR EXISTING DISCOUNTS FROM THE BASKET
            ClearExistingDiscounts(basket);

            //BUILD A LIST OF PRODUCTS THAT HAVE DISCOUNTS SPECIFICALLY APPLIED
            //THESE PRODUCTS CANNOT RECEIVE ANY FURTHER DISCOUNTS
            List<int> productsToExclude = new List<int>();

            //INITIALLY POPULATE THE LIST WITH ANY GIFT CERTIFICATE PRODUCT IS AS THESE
            //PRODUCTS ARE NOT ALLOWED TO BE DISCOUNTED
            productsToExclude.AddRange(DiscountCalculator.GetGiftCertificateProductIds(basket.BasketId));

            //CALCULATE THE PRODUCT LEVEL DISCOUNTS
            totalDiscount += CalculateProductDiscounts(basket, productsToExclude);

            //GET POTENTIAL PRODUCT DISCOUNTS
            Dictionary<int, List<PotentialDiscount>> potentialDiscounts = DiscountCalculator.GetPotentialDiscounts(basket, productsToExclude, GroupingMode.Product);

            //BUILD A QUANTITY AND VALUE LOOKUP TABLE, IF NEEDED TO COMBINE OPTIONS
            Dictionary<int, int> productQuantityLookup = null;
            Dictionary<int, LSDecimal> productValueLookup = null;
            bool combineVariantsInLineItemDiscountMode = Store.GetCachedSettings().CombineVariantsInLineItemDiscountMode;
            if (combineVariantsInLineItemDiscountMode)
            {
                productQuantityLookup = new Dictionary<int, int>();
                productValueLookup = new Dictionary<int, LSDecimal>();
                foreach (BasketItem bi in basket.Items)
                {
                    if (productQuantityLookup.ContainsKey(bi.ProductId))
                    {
                        productQuantityLookup[bi.ProductId] += bi.Quantity;
                        productValueLookup[bi.ProductId] += bi.ExtendedPrice;
                    }
                    else
                    {
                        productQuantityLookup[bi.ProductId] = bi.Quantity;
                        productValueLookup[bi.ProductId] = bi.ExtendedPrice;
                    }
                }
            }

            //LOOP BASKET ITEMS
            List<BasketItem> newItems = new List<BasketItem>();
            foreach (BasketItem bi in basket.Items)
            {
                //SEE WHETHER THIS PRODUCT HAS ANY POTENTIAL DISCOUNTS
                if (potentialDiscounts.ContainsKey(bi.ProductId))
                {
                    //GET THE POTENTIAL DISCOUNTS FOR THIS PRODUCT
                    List<PotentialDiscount> productDiscounts = potentialDiscounts[bi.ProductId];
                    //FIND THE BEST DISCOUNT
                    VolumeDiscount appliedDiscount = null;
                    LSDecimal discountAmount = -1;
                    for (int i = 0; i < productDiscounts.Count; i++)
                    {
                        VolumeDiscount tempDiscount = VolumeDiscountDataSource.Load(productDiscounts[i].VolumeDiscountId);
                        //DETERMINE THE QUANTITY USED TO CALCULATE DISCOUNT
                        int totalProductQuantity;
                        LSDecimal totalProductValue;
                        if (combineVariantsInLineItemDiscountMode)
                        {
                            totalProductQuantity = productQuantityLookup[bi.ProductId];
                            totalProductValue = productValueLookup[bi.ProductId];
                        }
                        else
                        {
                            totalProductQuantity = bi.Quantity;
                            totalProductValue = bi.ExtendedPrice;
                        }
                        //CHECK THIS DISCOUNT AMOUNT
                        LSDecimal tempDiscountAmount = tempDiscount.CalculateDiscount(bi.Quantity, totalProductQuantity, bi.ExtendedPrice, totalProductValue);
                        //SEE WHETHER THIS CALCULATED DISCOUNT IS THE GREATEST VALUE
                        if (tempDiscountAmount > discountAmount)
                        {
                            //THIS DISCOUNT HAS A HIGHER VALUE THAN THE LAST TESTED
                            //MAKE THIS THE APPLIED DISCOUNT
                            discountAmount = tempDiscountAmount;
                            appliedDiscount = tempDiscount;
                        }
                    }

                    //CREATE THE DISCOUNT ITEM IF REQUIRED
                    if (discountAmount > 0)
                    {
                        //DISCOUNT AMOUNT SHOULD NOT BE GREATER THEN PARENT ITEM TOTAL
                        if (discountAmount > bi.ExtendedPrice) discountAmount = bi.ExtendedPrice;

                        BasketItem discountLineItem = new BasketItem();
                        discountLineItem.BasketId = basket.BasketId;
                        discountLineItem.OrderItemType = OrderItemType.Discount;
                        discountLineItem.ParentItemId = bi.BasketItemId;
                        discountLineItem.BasketShipmentId = bi.BasketShipmentId;
                        discountLineItem.Name = appliedDiscount.Name;
                        discountLineItem.Sku = appliedDiscount.VolumeDiscountId.ToString();
                        discountLineItem.Price = (-1 * discountAmount);
                        discountLineItem.Quantity = 1; //bi.Quantity;
                        discountLineItem.TaxCodeId = bi.TaxCodeId;
                        discountLineItem.Shippable = bi.Shippable;
                        newItems.Add(discountLineItem);
                        totalDiscount += discountAmount;
                    }
                }
            }

            //ADD DISCOUNT ITEMS TO BASKET
            foreach (BasketItem bi in newItems)
            {
                basket.Items.Add(bi);
                bi.Save();
            }

            //RETURN THE TOTAL DISCOUNT APPLIED
            return totalDiscount;
        }

        private static LSDecimal Calculate_GroupingMode(Basket basket)
        {
            //KEEP TRACK OF TOTAL DISCOUNT APPLIED
            LSDecimal totalDiscount = 0;

            //CLEAR EXISTING DISCOUNTS FROM THE BASKET
            ClearExistingDiscounts(basket);

            //BUILD A LIST OF PRODUCTS THAT HAVE DISCOUNTS SPECIFICALLY APPLIED
            //THESE PRODUCTS CANNOT RECEIVE ANY FURTHER DISCOUNTS
            List<int> productsToExclude = new List<int>();

            //INITIALLY POPULATE THE LIST WITH ANY GIFT CERTIFICATE PRODUCT IS AS THESE
            //PRODUCTS ARE NOT ALLOWED TO BE DISCOUNTED
            productsToExclude.AddRange(DiscountCalculator.GetGiftCertificateProductIds(basket.BasketId));

            //CALCULATE THE PRODUCT LEVEL DISCOUNTS
            totalDiscount += CalculateProductDiscounts(basket, productsToExclude);

            //GET POTENTIAL CATEGORY DISCOUNTS
            //THE DICTIONARY KEY WILL BE CATEGORY LEVEL
            //POTENTIAL DISCOUNTS ARE THOSE THAT COULD APPLY AT THE SAME SPECIFICITY
            //THE POTENTIAL DISCOUNTS ARE ORDERED FROM MOST SPECIFIC CATEGORY LEVEL TO LEAST
            Dictionary<int, List<PotentialDiscount>> potentialDiscounts = DiscountCalculator.GetPotentialDiscounts(basket, productsToExclude, GroupingMode.Category);

            //LOOP THE DISCOUNTED CATEGORIES
            //WE HAVE TO CHECK ALL POTENTIAL DISCOUNTS AT EACH CATEGORY LEVEL
            //TO DETERMINE WHICH ONE TO APPLY
            List<BasketItem> newItems = new List<BasketItem>();
            foreach (int categoryLevel in potentialDiscounts.Keys)
            {
                //IF WE FIND A DISCOUNT AT THIS LEVEL, IT SHOULD BE APPLIED, THEN WE 
                //SHOULD RECHECK DISCOUNTS AT THIS LEVEL TO SEE IF ANY OTHERS APPLY
                bool recheckThisLevel = false;
                List<int> appliedCategories = new List<int>();
                do
                {
                    LSDecimal appliedDiscountAmount = -1;
                    VolumeDiscount appliedDiscount = null;
                    int appliedCategoryId = 0;
                    List<PotentialDiscount> discountGroup = potentialDiscounts[categoryLevel];
                    foreach (PotentialDiscount pd in discountGroup)
                    {
                        if (appliedCategories.IndexOf(pd.CategoryId) < 0)
                        {
                            //GET ALL BASKET ITEMS ELIGIBLE FOR DISCOUNT IN THIS CATEGORY
                            BasketItemCollection eligibleItems = DiscountCalculator.GetCategoryItems(pd.CategoryId, basket, productsToExclude);
                            //TOTAL UP ITEMS FOR GROUPING MODE
                            int totalQuantity = eligibleItems.TotalQuantity();
                            if (totalQuantity > 0)
                            {
                                LSDecimal totalPrice = eligibleItems.TotalPrice();
                                VolumeDiscount tempDiscount = VolumeDiscountDataSource.Load(pd.VolumeDiscountId);
                                //JUST USE TOTALS TO DETERMINE OVERALL DISCOUNT
                                LSDecimal tempDiscountAmount = tempDiscount.CalculateDiscount(totalQuantity, totalQuantity, totalPrice, totalPrice);
                                if (tempDiscountAmount > appliedDiscountAmount)
                                {
                                    appliedDiscountAmount = tempDiscountAmount;
                                    appliedDiscount = tempDiscount;
                                    appliedCategoryId = pd.CategoryId;
                                }
                            }
                        }
                    }

                    //SEE WHETHER WE FOUND A DISCOUNT AT THIS LEVEL
                    if (appliedDiscount != null)
                    {
                        //GET ALL BASKET ITEMS ELIGIBLE FOR DISCOUNT IN THIS CATEGORY
                        BasketItemCollection eligibleItems = DiscountCalculator.GetCategoryItems(appliedCategoryId, basket, productsToExclude);
                        //TOTAL UP ITEMS FOR GROUPING MODE
                        int totalQuantity = eligibleItems.TotalQuantity();
                        LSDecimal totalPrice = eligibleItems.TotalPrice();
                        //LOOP ALL BASKET ITEMS TO ADD DISCOUNTS TO BASKET
                        foreach (BasketItem bi in eligibleItems)
                        {
                            LSDecimal discountAmount = appliedDiscount.CalculateDiscount(bi.Quantity, totalQuantity, bi.ExtendedPrice, totalPrice);
                            if (discountAmount > 0)
                            {
                                //DISCOUNT AMOUNT SHOULD NOT BE GREATER THEN PARENT ITEM TOTAL
                                if (discountAmount > bi.ExtendedPrice) discountAmount = bi.ExtendedPrice;

                                //DISCOUNT MUST BE ADJUSTED FOR OPTIONS THAT ARE GROUPED
                                BasketItem discountLineItem = new BasketItem();
                                discountLineItem.BasketId = basket.BasketId;
                                discountLineItem.OrderItemType = OrderItemType.Discount;
                                discountLineItem.ParentItemId = bi.BasketItemId;
                                discountLineItem.BasketShipmentId = bi.BasketShipmentId;
                                discountLineItem.Name = appliedDiscount.Name;
                                discountLineItem.Sku = appliedDiscount.VolumeDiscountId.ToString();
                                discountLineItem.Price = (-1 * discountAmount);
                                discountLineItem.Quantity = 1; //bi.Quantity;
                                discountLineItem.TaxCodeId = bi.TaxCodeId;
                                discountLineItem.Shippable = bi.Shippable;
                                basket.Items.Add(discountLineItem);
                                discountLineItem.Save();
                                totalDiscount += discountAmount;
                            }
                            if (productsToExclude.IndexOf(bi.ProductId) < 0) productsToExclude.Add(bi.ProductId);
                        }
                    }
                    //IF A CATEGORY DISCOUNT WAS APPLIED 
                    //AND THERE IS MORE THAN ONE DISCOUNT AT THIS LEVEL,
                    //WE MUST RECHECK THIS LEVEL
                    recheckThisLevel = ((appliedCategoryId > 0) && (potentialDiscounts[categoryLevel].Count > 1));
                } while (recheckThisLevel);
            }

            //ADD DISCOUNT ITEMS TO BASKET
            foreach (BasketItem bi in newItems)
            {
                basket.Items.Add(bi);
                bi.Save();
            }

            //RETURN THE CALCULATED DISCOUNT
            return totalDiscount;
        }

        /// <summary>
        /// Get all basket items that are descendant products of the category and are eligible for discount
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="basket"></param>
        /// <param name="productsToExclude"></param>
        /// <returns></returns>
        private static BasketItemCollection GetCategoryItems(int categoryId, Basket basket, List<int> productsToExclude)
        {
            //BUILD CRITERIA FOR PRODUCTS TO EXCLUDE FROM QUERY
            string excludedProducts = string.Empty;
            if (productsToExclude.Count > 0)
            {
                if (productsToExclude.Count == 1)
                    excludedProducts = " AND BI.ProductId <> " + productsToExclude[0].ToString();
                else
                    excludedProducts = " AND BI.ProductId NOT IN (" + AlwaysConvert.ToList(",", productsToExclude.ToArray()) + ")";
            }

            //FIND ALL PRODUCTS THAT ARE A DESCENDANT OF THE CATEGORY
            StringBuilder categorySql = new StringBuilder();
            categorySql.Append("SELECT DISTINCT BI.BasketItemId");
            categorySql.Append(" FROM (ac_BasketItems BI INNER JOIN ac_CatalogNodes CN ON BI.ProductId = CN.CatalogNodeId");
            categorySql.Append(" INNER JOIN ac_CategoryParents CP ON CN.CategoryId = CP.CategoryId)");
            categorySql.Append(" WHERE BI.BasketId = @basketId");
            categorySql.Append(" AND CN.CatalogNodeTypeId = 1");
            categorySql.Append(" AND CP.ParentId = @categoryId");
            categorySql.Append(excludedProducts);

            //EXECUTE THE QUERY
            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand(categorySql.ToString());
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            database.AddInParameter(command, "@categoryId", System.Data.DbType.Int32, categoryId);
            //BUILD LIST OF BASKET ITEM IDS
            List<int> basketItemIds = new List<int>();
            using (IDataReader dr = database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    basketItemIds.Add(dr.GetInt32(0));
                }
                dr.Close();
            }

            //BUILD THE COLLECTION OF BASKET ITEMS
            BasketItemCollection items = new BasketItemCollection();
            foreach (int id in basketItemIds)
            {
                int index = basket.Items.IndexOf(id);
                if (index > -1) items.Add(basket.Items[index]);
            }

            //RETURN THE BASKET ITEMS THAT ARE DESCENDANTS OF THIS CATEGORY
            return items;
        }

        /// <summary>
        /// Calculates any product specific discounts for the basket
        /// </summary>
        /// <param name="basket">Basket to calculate discounts for</param>
        /// <param name="productsToExclude">Any products to exclude from discounting</param>
        /// <returns>The total discount applied to the basket</returns>
        private static LSDecimal CalculateProductDiscounts(Basket basket, List<int> productsToExclude)
        {
            LSDecimal totalDiscount = 0;
            //GET ALL PRODUCTS FROM THE BASKET THAT HAVE DISCOUNTS DIRECTLY ASSOCIATED
            //TODO: UPDATE THE METHOD BELOW TO FACTOR IN THE COMBINE VARIANTS IN LINE ITEM DISCOUNT MODE SETTING
            DiscountedBasketProduct[] discountedItems = DiscountCalculator.GetDiscountedBasketProducts(basket.BasketId, productsToExclude);
            if (discountedItems != null && discountedItems.Length > 0)
            {
                //EVALUATE EACH ITEM TO SEE WHETHER THE PRODUCT DISCOUNT APPLIES
                foreach (DiscountedBasketProduct discountedProduct in discountedItems)
                {
                    VolumeDiscount appliedDiscount = null;
                    LSDecimal discountAmount = -1;
                    //LOOP ALL AVAILABLE DISCOUNTS AND FIND THE BEST ONE
                    VolumeDiscountCollection availableDiscounts = VolumeDiscountDataSource.LoadForProduct(discountedProduct.ProductId);
                    foreach (VolumeDiscount testDiscount in availableDiscounts)
                    {
                        if (testDiscount.IsValidForUser(basket.User))
                        {
                            LSDecimal tempDiscountAmount = testDiscount.CalculateDiscount(discountedProduct.Quantity, discountedProduct.Quantity, discountedProduct.ExtendedPrice, discountedProduct.ExtendedPrice);
                            if (tempDiscountAmount > discountAmount)
                            {
                                discountAmount = tempDiscountAmount;
                                appliedDiscount = testDiscount;
                            }
                        }
                    }

                    //CHECK WHETHER A DISCOUNT APPLIES TO THIS PRODUCT FOR THIS USER
                    if (appliedDiscount != null)
                    {
                        //productsToExclude.Add(discountedProduct.ProductId);
                        List<BasketItem> newItems = new List<BasketItem>();
                        //LOOP ALL ITEMS IN BASKET, CALCULATE DISCOUNT FOR MATCHING PRODUCTS
                        foreach (BasketItem basketItem in basket.Items)
                        {
                            if (basketItem.ProductId == discountedProduct.ProductId)
                            {
                                //THIS IS A DISCOUNTED PRODUCT, CREATE THE DISCOUNT LINE ITEM
                                discountAmount = appliedDiscount.CalculateDiscount(basketItem.Quantity, discountedProduct.Quantity, basketItem.ExtendedPrice, discountedProduct.ExtendedPrice);
                                if (discountAmount > 0)
                                {
                                    //DISCOUNT AMOUNT SHOULD NOT BE GREATER THEN PARENT ITEM TOTAL
                                    if (discountAmount > basketItem.ExtendedPrice) discountAmount = basketItem.ExtendedPrice;

                                    //DISCOUNT MUST BE ADJUSTED FOR OPTIONS THAT ARE GROUPED
                                    BasketItem discountLineItem = new BasketItem();
                                    discountLineItem.BasketId = basket.BasketId;
                                    discountLineItem.OrderItemType = OrderItemType.Discount;
                                    discountLineItem.ParentItemId = basketItem.BasketItemId;
                                    discountLineItem.BasketShipmentId = basketItem.BasketShipmentId;
                                    discountLineItem.Name = appliedDiscount.Name;
                                    discountLineItem.Sku = appliedDiscount.VolumeDiscountId.ToString();
                                    discountLineItem.Price = (-1 * discountAmount);
                                    discountLineItem.Quantity = 1; // basketItem.Quantity;
                                    discountLineItem.TaxCodeId = basketItem.TaxCodeId;
                                    discountLineItem.Shippable = basketItem.Shippable;
                                    discountLineItem.Save();
                                    newItems.Add(discountLineItem);
                                    totalDiscount += discountAmount;
                                }
                            }
                        }
                        //ADD ANY NEW ITEMS TO THE BASKET COLLECTION
                        foreach (BasketItem basketItem in newItems)
                        {
                            basket.Items.Add(basketItem);
                        }

                        //AT LEAST ONE DISCOUNT WAS APPLICABLE TO THIS USER
                        //ADD THIS PRODUCT TO EXCLUDE LIST
                        if (productsToExclude.IndexOf(discountedProduct.ProductId) < 0)
                            productsToExclude.Add(discountedProduct.ProductId);
                    }
                }
            }
            return totalDiscount;
        }

        /// <summary>
        /// Gets all unique product Ids that are attached to gift certificates in the basket.
        /// </summary>
        /// <param name="basketId">The basket to check for gift certificate products.</param>
        /// <returns>An array of int that contains the gift certificate product Ids.</returns>
        private static int[] GetGiftCertificateProductIds(int basketId)
        {
            List<int> productIds = new List<int>();
            Database database = Token.Instance.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT DISTINCT BI.ProductId");
            sql.Append(" FROM ac_BasketItems BI, ac_Products P");
            sql.Append(" WHERE BI.BasketId = @basketId");
            sql.Append(" AND BI.ProductId = P.ProductId");
            sql.Append(" AND P.IsGiftCertificate = 1");
            DbCommand command = database.GetSqlStringCommand(sql.ToString());
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basketId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    productIds.Add(dr.GetInt32(0));
                }
                dr.Close();
            }
            return productIds.ToArray();
        }

        /// <summary>
        /// Identify the products from the basket that have product level discounts 
        /// </summary>
        /// <param name="basketId">basket to check for product level discounts</param>
        /// <param name="productsToExclude">products that should not be checked for discounts</param>
        /// <returns>An array of discounted products in the basket</returns>
        private static DiscountedBasketProduct[] GetDiscountedBasketProducts(int basketId, List<int> productsToExclude)
        {
            //BUILD CRITERIA FOR PRODUCTS TO EXCLUDE FROM QUERY
            string excludedProducts = string.Empty;
            if (productsToExclude.Count > 0)
            {
                if (productsToExclude.Count == 1)
                    excludedProducts = " WHERE ProductId <> " + productsToExclude[0].ToString();
                else
                    excludedProducts = " WHERE ProductId NOT IN (" + AlwaysConvert.ToList(",", productsToExclude.ToArray()) + ")";
            }

            //EXECUTE THE QUERY
            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand("SELECT ProductId,AVG(Price),SUM(Quantity) FROM ac_BasketItems WHERE BasketId = @basketId AND ProductId IN (SELECT DISTINCT ProductId FROM ac_ProductVolumeDiscounts" + excludedProducts + ") GROUP BY ProductId");
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basketId);

            //BUILD A LIST OF DISCOUNTED PRODUCTS IN THE BASKET
            List<DiscountedBasketProduct> discountedBasketProducts = new List<DiscountedBasketProduct>();
            using (IDataReader dr = database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    discountedBasketProducts.Add(new DiscountedBasketProduct(dr.GetInt32(0), dr.GetDecimal(1), dr.GetInt32(2)));
                }
                dr.Close();
            }

            //RETURN THE DISCOUNTED PRODUCTS
            return discountedBasketProducts.ToArray();
        }

        /// <summary>
        /// Indicate the grouping mode in effect
        /// </summary>
        private enum GroupingMode { Category, Product }


        /// <summary>
        /// Gets a dictionary of potential discounts for the basket.
        /// </summary>
        /// <param name="basket">Basket to check for potential discounts</param>
        /// <param name="productsToExclude">Products that should not be considered for discounts.</param>
        /// <param name="groupingMode">Grouping mode in use</param>
        /// <returns>A dictionary of potential discounts for the order.</returns>
        private static Dictionary<int, List<PotentialDiscount>> GetPotentialDiscounts(Basket basket, List<int> productsToExclude, GroupingMode groupingMode)
        {
            //INITIALIZE THE RETURN VALUE
            Dictionary<int, List<PotentialDiscount>> potentialDiscounts = new Dictionary<int, List<PotentialDiscount>>();

            //KEEP TRACK OF PRODUCTS TO EXCLUDE FROM GLOBAL DISCOUNTS
            List<int> excludeGlobal = new List<int>();

            //BUILD CRITERIA FOR PRODUCTS TO EXCLUDE FROM QUERY
            string excludedProducts = string.Empty;
            if (productsToExclude.Count > 0)
            {
                if (productsToExclude.Count == 1)
                    excludedProducts = " AND ProductId <> " + productsToExclude[0].ToString();
                else
                    excludedProducts = " AND ProductId NOT IN (" + AlwaysConvert.ToList(",", productsToExclude.ToArray()) + ")";
                //ALSO EXCLUDE THESE PRODUCTS FROM GLOBAL DISCOUNTS
                excludeGlobal.AddRange(productsToExclude);
            }

            //FIND ALL DISCOUNTS THAT ARE ASSOCIATED WITH PRODUCTS IN THE BASKET
            StringBuilder categorySql = new StringBuilder();
            categorySql.Append("SELECT DISTINCT CP.ParentId, CP.ParentLevel, CVD.VolumeDiscountId");
            if (groupingMode == GroupingMode.Product) categorySql.Append(", CN.CatalogNodeId");
            categorySql.Append(" FROM (ac_BasketItems BI INNER JOIN ac_CatalogNodes CN ON BI.ProductId = CN.CatalogNodeId");
            categorySql.Append(" INNER JOIN ac_CategoryParents CP ON CN.CategoryId = CP.CategoryId");
            categorySql.Append(" INNER JOIN ac_CategoryVolumeDiscounts CVD ON CP.ParentId = CVD.CategoryId)");
            categorySql.Append(" WHERE BI.BasketId = @basketId ");
            categorySql.Append(" AND CN.CatalogNodeTypeId = 1");
            categorySql.Append(excludedProducts);
            //ORDER FROM THE MOST SPECIFIC CATEGORY TO THE LEAST (PARENTLEVEL)
            categorySql.Append(" ORDER BY CP.ParentLevel DESC");
            if (groupingMode == GroupingMode.Product) categorySql.Append(", CN.CatalogNodeId ASC");

            //CREATE THE COMMAND
            Database database = Token.Instance.Database;
            DbCommand command = database.GetSqlStringCommand(categorySql.ToString());
            database.AddInParameter(command, "@basketId", System.Data.DbType.Int32, basket.BasketId);
            DataSet allDiscounts = database.ExecuteDataSet(command);

            //LOOP ALL DISCOUNTS, BUILD LIST OF POTENTIAL DISCOUNTS THAT COULD APPLY TO USER
            User user = basket.User;
            foreach (DataRow row in allDiscounts.Tables[0].Rows)
            {
                //GET THE DISCOUNTED OBJECT (EITHER PRODUCT OR CATEGORY LEVEL)
                int key;
                if (groupingMode == GroupingMode.Product) key = (int)row[3];
                else key = Convert.ToInt32((byte)row[1]);

                //GET THE LEVEL OF THE CATEGORY (DETERMINES PRECEDENCE)
                int categoryLevel = Convert.ToInt32((byte)row[1]);

                //FIND THE LIST OF POTENTIAL DISCOUNTS FOR THIS KEY
                if (!potentialDiscounts.ContainsKey(key))
                    potentialDiscounts[key] = new List<PotentialDiscount>();
                List<PotentialDiscount> discountGroup = potentialDiscounts[key];

                //DECIDE WHETHER THIS DISCOUNT HAS ENOUGH PRECDENCE TO CHECK
                bool checkDiscount = true;
                if (discountGroup.Count > 0)
                {
                    //THE CURRENT ROW COULD HAVE THE SAME LEVEL AS THE DISCOUNTS ALREADY FOUND
                    if (categoryLevel < discountGroup[0].CategoryLevel) checkDiscount = false;
                    //WE DO NOT HAVE TO CHECK FOR LEVEL BEING GREATER, BECAUSE
                    //THE QUERY INCLUDES PARENT LEVEL IN THE ORDERBY CLAUSE
                }

                if (checkDiscount)
                {
                    //THIS COULD BE A VALID DISCOUNT
                    int volumeDiscountId = (int)row[2];
                    //CHECK FOR USER EXCLUSIONS
                    VolumeDiscount v = VolumeDiscountDataSource.Load(volumeDiscountId);
                    if ((v != null) && (v.IsValidForUser(user)))
                    {
                        //THE DISCOUNT IS VALID FOR THIS USER
                        //ADD TO THE LIST OF POTENTIALS
                        PotentialDiscount pd = new PotentialDiscount();
                        pd.CategoryId = (int)row[0];
                        pd.CategoryLevel = categoryLevel;
                        pd.VolumeDiscountId = volumeDiscountId;
                        if (groupingMode == GroupingMode.Product)
                        {
                            pd.ProductId = key;
                            //SINCE THERE IS A CATEGORY DISCOUNT, WE DO 
                            //NOT WANT TO USE ANY GLOBAL DISCOUNTS FOR THIS PRODUCT
                            excludeGlobal.Add(key);
                        }
                        discountGroup.Add(pd);
                    }
                }
            }

            List<int> globalDiscountProducts = new List<int>();
            if (groupingMode == GroupingMode.Product)
            {
                //CHECK WHETHER ANY PRODUCTS REMAIN IN ORDER THAT ARE ELIGIBLE
                //FOR GLOBAL DISCOUNTS
                foreach (BasketItem item in basket.Items)
                {
                    //MAKE SURE THIS IS A PRODUCT AND IS NOT EXCLUDED FROM GLOBAL DISCOUNT
                    if ((item.ProductId > 0) && (excludeGlobal.IndexOf(item.ProductId) < 0))
                    {
                        //ADD THIS PRODUCT TO THE GLOBAL DISCOUNT LIST (IF NOT ALREADY THERE)
                        if (globalDiscountProducts.IndexOf(item.ProductId) < 0)
                            globalDiscountProducts.Add(item.ProductId);
                    }
                }
            }

            //ARE WE CALCULATING DISCOUNTS USING CATEGORY GROUPING MODE?
            //OR ARE THERE ANY PRODUCTS THAT COULD HAVE GLOBAL DISCOUNTS?
            if (groupingMode == GroupingMode.Category || globalDiscountProducts.Count > 0)
            {
                //FIND ANY GLOBAL DISCOUNTS
                VolumeDiscountCollection globalDiscounts = VolumeDiscountDataSource.LoadGlobal();
                if (globalDiscounts.Count > 0)
                {
                    if (groupingMode == GroupingMode.Product)
                    {
                        foreach (int productId in globalDiscountProducts)
                        {
                            potentialDiscounts[productId] = new List<PotentialDiscount>();
                            List<PotentialDiscount> discountGroup = potentialDiscounts[productId];
                            foreach (VolumeDiscount v in globalDiscounts)
                            {
                                //VERIFY USER RESTRICTION ON GLOBAL DISCOUNT
                                if (v.IsValidForUser(user))
                                {
                                    PotentialDiscount pd = new PotentialDiscount();
                                    pd.CategoryId = 0;
                                    pd.CategoryLevel = 0;
                                    pd.VolumeDiscountId = v.VolumeDiscountId;
                                    pd.ProductId = productId;
                                    discountGroup.Add(pd);
                                }
                            }
                        }
                    }
                    else
                    {
                        potentialDiscounts[0] = new List<PotentialDiscount>();
                        List<PotentialDiscount> discountGroup = potentialDiscounts[0];
                        foreach (VolumeDiscount v in globalDiscounts)
                        {
                            //VERIFY USER RESTRICTION ON GLOBAL DISCOUNT
                            if (v.IsValidForUser(user))
                            {
                                PotentialDiscount pd = new PotentialDiscount();
                                pd.CategoryId = 0;
                                pd.CategoryLevel = 0;
                                pd.VolumeDiscountId = v.VolumeDiscountId;
                                pd.ProductId = 0;
                                discountGroup.Add(pd);
                            }
                        }
                    }
                }
            }

            //REMOVE ANY ENTRIES FROM THE LIST THAT HAVE NO DISCOUNTS
            List<int> emptyKeys = new List<int>();
            foreach (int key in potentialDiscounts.Keys) { if (potentialDiscounts[key].Count == 0) emptyKeys.Add(key); }
            foreach (int key in emptyKeys) { potentialDiscounts.Remove(key); }

            //RETURN POTENTIAL DISCOUNTS FOR BASKET
            return potentialDiscounts;
        }

        private class PotentialDiscount
        {
            private int _ProductId;
            private int _CategoryId;
            private int _CategoryLevel;
            private int _VolumeDiscountId;

            public int ProductId
            {
                get { return _ProductId; }
                set { _ProductId = value; }
            }

            public int CategoryId
            {
                get { return _CategoryId; }
                set { _CategoryId = value; }
            }

            public int CategoryLevel
            {
                get { return _CategoryLevel; }
                set { _CategoryLevel = value; }
            }

            public int VolumeDiscountId
            {
                get { return _VolumeDiscountId; }
                set { _VolumeDiscountId = value; }
            }
        }

        private class DiscountedBasketProduct
        {
            private int _ProductId;
            private LSDecimal _AveragePrice;
            private int _Quantity;
            private LSDecimal _ExtendedPrice;

            public int ProductId
            {
                get { return _ProductId; }
            }

            public LSDecimal AveragePrice
            {
                get { return _AveragePrice; }
            }

            public int Quantity
            {
                get { return _Quantity; }
            }

            public LSDecimal ExtendedPrice
            {
                get { return _ExtendedPrice; }
            }

            public DiscountedBasketProduct(int productId, LSDecimal averagePrice, int quantity)
            {
                _ProductId = productId;
                _AveragePrice = averagePrice;
                _Quantity = quantity;
                _ExtendedPrice = _AveragePrice * Quantity;
            }
        }
    }
}