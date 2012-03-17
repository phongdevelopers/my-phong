using System;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Utility class for tax calculations
    /// </summary>
    public class TaxCalculator
    {
        /// <summary>
        /// Calculates and applies taxes for the given basket.
        /// </summary>
        /// <param name="basket">The basket to calculate taxes on.</param>
        /// <returns>The total amount of tax applied.</returns>
        /// <remarks>Any pre-existing tax line items will be removed from the basket before the calculation.</remarks>
        public static LSDecimal Calculate(Basket basket)
        {
            //CLEAR ANY EXISTING TAXES
            ClearExistingTaxes(basket);

            //DO NOT PROCESS TAXES IF USER BELONGS TO A TEXT EXEMPT GROUP
            User user = basket.User;
            if (user != null)
            {
                foreach (UserGroup userGroup in user.UserGroups)
                {
                    if (userGroup.Group != null && userGroup.Group.IsTaxExempt) return 0;
                }
            }
            
            //INITIALIZE TAXES
            LSDecimal totalTax = 0;

            //FINALIZE ANY TAXES FROM INTEGRATED PROVIDERS
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach (TaxGateway taxGateway in taxGateways)
            {
                ITaxProvider provider = taxGateway.GetProviderInstance();
                if (provider != null)
                {
                    try
                    {
                        totalTax += provider.Calculate(basket);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Could not calculate with the configured tax provider: " + taxGateway.ClassId, ex);
                    }
                }
                else
                {
                    Logger.Error("Could not load the configured tax provider: " + taxGateway.ClassId);
                }
            }
            return totalTax;
        }

        /// <summary>
        /// Clears all existing taxes and calculates and applies taxes for the given order.
        /// </summary>
        /// <param name="basket">The order to calculate taxes on.</param>
        /// <returns>The total amount of tax applied.</returns>
        /// <remarks>Any pre-existing tax line items will be removed from the order before the calculation.</remarks>
        public static LSDecimal Recalculate(Order order)
        {
            //CLEAR ANY EXISTING TAXES
            ClearExistingTaxes(order);

            //DO NOT PROCESS TAXES IF USER BELONGS TO A TEXT EXEMPT GROUP
            User user = order.User;
            if (user != null)
            {
                foreach (UserGroup userGroup in user.UserGroups)
                {
                    if (userGroup.Group != null && userGroup.Group.IsTaxExempt) return 0;
                }
            }

            //INITIALIZE TAXES
            LSDecimal totalTax = 0;

            //FINALIZE ANY TAXES FROM INTEGRATED PROVIDERS
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach (TaxGateway taxGateway in taxGateways)
            {
                ITaxProvider provider = taxGateway.GetProviderInstance();
                if (provider != null)
                {
                    try
                    {
                        totalTax += provider.Recalculate(order);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Could not calculate with the configured tax provider: " + taxGateway.ClassId, ex);
                    }
                }
                else
                {
                    Logger.Error("Could not load the configured tax provider: " + taxGateway.ClassId);
                }
            }
            return totalTax;
        }

        private static void ClearExistingTaxes(Basket basket)
        {
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    basket.Items.DeleteAt(i);
                }
            }
        }

        private static void ClearExistingTaxes(Order order)
        {
            for (int i = order.Items.Count - 1; i >= 0; i--)
            {
                OrderItem item = order.Items[i];
                if (item.OrderItemType == OrderItemType.Tax)
                {
                    order.Items.DeleteAt(i);
                }
            }
        }
    }
}
