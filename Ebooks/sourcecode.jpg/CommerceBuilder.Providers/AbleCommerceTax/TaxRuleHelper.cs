using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Web;

namespace CommerceBuilder.Taxes.Providers.AbleCommerce
{
    internal static class TaxRuleHelper
    {
        /// <summary>
        /// Determines the nexus for the tax rule
        /// </summary>
        /// <param name="taxRule">The tax rule to check</param>
        /// <returns>The nexus for the tax rule</returns>
        public static TaxNexus GetNexus(TaxRule taxRule)
        {
            return taxRule.UseBillingAddress ? TaxNexus.PointOfBilling : TaxNexus.PointOfDelivery;
        }

        /// <summary>
        /// Gets the tax rules that may apply
        /// </summary>
        /// <param name="taxCodeId">The tax code of the taxable item</param>
        /// <param name="billingAddress">The billing address for the item</param>
        /// <param name="shippingAddress">The shipping address for the item</param>
        /// <param name="user">The user shopping for the item</param>
        /// <returns>A List of TaxRule records that may apply</returns>
        public static List<TaxRule> GetPotentialTaxRules(int taxCodeId, TaxAddress billingAddress, TaxAddress shippingAddress, User user)
        {
            int[] taxCodeIds = { taxCodeId };
            List<TaxAddress> shippingAddresses = new List<TaxAddress>();
            shippingAddresses.Add(shippingAddress);
            return GetPotentialTaxRules(taxCodeIds, billingAddress, shippingAddresses, user);
        }

        /// <summary>
        /// Gets the tax rules that may apply
        /// </summary>
        /// <param name="taxCodeIds">The tax code of the taxable item(s)</param>
        /// <param name="billingAddress">The billing address for the item(s)</param>
        /// <param name="shippingAddresses">The shipping addresses for the item(s)</param>
        /// <param name="user">The user shopping for the item(s)</param>
        /// <returns>A List of TaxRule records that may apply</returns>
        public static  List<TaxRule> GetPotentialTaxRules(int[] taxCodeIds, TaxAddress billingAddress, List<TaxAddress> shippingAddresses, User user)
        {
            // BUILD THE KEY            
            String cacheKey = GetTaxCodesKey(taxCodeIds) + "~" + GetAddressKey(billingAddress) + "~" + GetAddressKey(shippingAddresses) + "~" + user.UserId;
            Dictionary<string, List<TaxRule>> taxRulesDic = null;
            // CHECK THE HTTP CONTEXT FOR EXISTING RULES
            if(HttpContext.Current != null)
            {
                taxRulesDic = HttpContext.Current.Items["PotentialTaxRules"] as Dictionary<string,List<TaxRule>>;
                if (taxRulesDic != null)
                {
                    if (taxRulesDic.ContainsKey(cacheKey)) return taxRulesDic[cacheKey];
                }
                else
                {
                    taxRulesDic = new Dictionary<string, List<TaxRule>>();
                }
            }

            List<TaxRule> allRules = new List<TaxRule>();
            TaxRuleCollection countryRules = TaxRuleDataSource.LoadForTaxCodes(taxCodeIds, billingAddress, user);
            foreach (TaxRule rule in countryRules) allRules.Add(rule);
            foreach (TaxAddress shippingAddress in shippingAddresses)
            {
                countryRules = TaxRuleDataSource.LoadForTaxCodes(taxCodeIds, shippingAddress, user);
                foreach (TaxRule rule in countryRules)
                {
                    if (!ListContainsTaxRule(allRules, rule.TaxRuleId)) allRules.Add(rule);
                }
            }

            // IF CONTEXT IS AVAILABLE STORE THE RESULTS
            if (HttpContext.Current != null)
            {
                taxRulesDic[cacheKey] = allRules;
                HttpContext.Current.Items["PotentialTaxRules"] = taxRulesDic;
            }
            return allRules;
        }

        private static string GetTaxCodesKey(int[] taxCodes)
        {
            StringBuilder keyBuilder = new StringBuilder();
            for (int i = 0; i < taxCodes.Length; i++)
            {
                if (i > 0) keyBuilder.Append("~");
                keyBuilder.Append(taxCodes[i]);
            }
            return keyBuilder.ToString();
        }

        private static string GetAddressKey(TaxAddress address)
        {
            return address.CountryCode + "~" + address.ProvinceId + "~" + address.PostalCode;
        }

        private static string GetAddressKey(List<TaxAddress> addresses)
        {
            StringBuilder keyBuilder = new StringBuilder();
            for(int i = 0; i < addresses.Count; i++)
            {
                if(i > 0) keyBuilder.Append("~");
                keyBuilder.Append(GetAddressKey(addresses[i]));
            }
            return keyBuilder.ToString();
        }

        /// <summary>
        /// Determines whether a rule appears in the list
        /// </summary>
        /// <param name="rules">list of rules</param>
        /// <param name="taxRuleId">id of rule to look for</param>
        /// <returns>true if the rule is in the list</returns>
        private static bool ListContainsTaxRule(List<TaxRule> rules, int taxRuleId)
        {
            foreach (TaxRule rule in rules) if (rule.TaxRuleId == taxRuleId) return true;
            return false;
        }

        /// <summary>
        /// Calculates the Tax for the given price
        /// </summary>
        /// <param name="unitPrice">The unit price to calculate Tax for</param>
        /// <param name="quantity" >The Quantity of the line item</param>
        /// <param name="taxCodeId">The tax code that applies to this price</param>
        /// <param name="priority">The priority of the item - 0 for products, can be greater than 0 for non products items.</param>
        /// <param name="billingAddress">The billing address that applies to the price</param>
        /// <param name="shippingAddress">The shipping address that applies to the price</param>
        /// <param name="user">The user being taxed</param>
        /// <returns>The Tax details for the given price</returns>
        public static TaxInfo InternalGetTaxInfo(LSDecimal unitPrice, int quantity, int taxCodeId, int priority, TaxAddress billingAddress, TaxAddress shippingAddress, User user, bool priceIncludesTax)
        {
            bool isNegativePrice = (unitPrice < 0);
            LSDecimal absPrice = isNegativePrice ? (unitPrice * -1) : unitPrice;
            //INITIALIZE TAXES
            LSDecimal totalTax = 0;
            LSDecimal totalTaxRate = 0;
            //GET ANY RULES THAT MAY APPLY TO THESE ADDRESSES
            List<TaxRule> taxRules = TaxRuleHelper.GetPotentialTaxRules(taxCodeId, billingAddress, shippingAddress, user);
            List<ShopTaxItem> taxItems = new List<ShopTaxItem>();
            //LOOP ANY RULES AND CALCULATE THE IMPACT OF Tax
            foreach (TaxRule taxRule in taxRules)
            {
                //PREVENT INCORRECT COMPOUNDING, AND ENSURE TAX MEETS ADDRESS CRITERIA
                if (priority < taxRule.Priority && taxRule.AppliesToAddress(billingAddress, shippingAddress))
                {
                    int tempTaxItemCount = taxItems.Count;
                    if (taxRule.AppliesToTaxCode(taxCodeId))
                    {
                        LSDecimal tempTax = LineItemCalculator.CalculateTaxForItem(taxRule, absPrice, quantity, priceIncludesTax);
                        taxItems.Add(new ShopTaxItem(taxRule.TaxCodeId, tempTax, taxRule.TaxRate));
                        totalTax += tempTax;
                        totalTaxRate += taxRule.TaxRate;
                    }
                    //CHECK IF THIS TAX APPLIES ON ANY PREEXISTING SHOPTAX ITEMS
                    for (int i = 0; i < tempTaxItemCount; i++)
                    {
                        ShopTaxItem thisItem = taxItems[i];
                        if (taxRule.AppliesToTaxCode(thisItem.TaxCodeId))
                        {
                            //TAX ON TAX, RATE MUST BE CALCULATED
                            LSDecimal tempTax = LineItemCalculator.CalculateTaxForItem(taxRule, thisItem.Price, 1, priceIncludesTax);
                            taxItems.Add(new ShopTaxItem(taxRule.TaxCodeId, tempTax, taxRule.TaxRate));
                            totalTax += tempTax;
                            totalTaxRate += TaxHelper.Round(((decimal)thisItem.TaxRate * (decimal)taxRule.TaxRate) / 100, 2, taxRule.RoundingRule);
                        }
                    }
                }
            }
            if (priceIncludesTax)
            {
                //IF PRICE IS Tax INCLUSIVE, THEN CALCULATED Tax CANNOT EXCEED PRICE
                //THIS WOULD INDICATE A CONFIGURATION ERROR
                if (totalTax > absPrice) totalTax = unitPrice;
                if (totalTaxRate > 100) totalTaxRate = 100;
                if (isNegativePrice) totalTax = totalTax * -1;
                LSDecimal actualPrice = unitPrice - totalTax;
                return new TaxInfo(actualPrice, totalTax, totalTaxRate);
            }
            else
            {
                if (isNegativePrice) totalTax = totalTax * -1;
                return new TaxInfo(unitPrice, totalTax, totalTaxRate);
            }
        }

        public static int GetBasketItemTaxPriority(BasketItem bi)
        {
            if (bi.OrderItemType == OrderItemType.Tax)
            {
                //THIS IS A TAX, RETURN THE TAX RULE PRIORITY
                //SKU OF ITEM SHOULD BE TAX RULE ID
                int taxRuleId = AlwaysConvert.ToInt(bi.Sku);
                TaxRule rule = TaxRuleDataSource.Load(taxRuleId);
                if (rule != null) return rule.Priority;
            }
            return -1;
        }

        public static int GetOrderItemTaxPriority(OrderItem oi)
        {
            if (oi.OrderItemType == OrderItemType.Tax)
            {
                //THIS IS A TAX, RETURN THE TAX RULE PRIORITY
                //SKU OF ITEM SHOULD BE TAX RULE ID
                int taxRuleId = AlwaysConvert.ToInt(oi.Sku);
                TaxRule rule = TaxRuleDataSource.Load(taxRuleId);
                if (rule != null) return rule.Priority;
            }
            return -1;
        }
    }
}
