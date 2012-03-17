using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes.Providers.AbleCommerce
{
    internal static class AddressHelper
    {
        /// <summary>
        /// Gets the billing address for the basket
        /// </summary>
        /// <param name="basket">The basket to parse; if null the billing address of the current user is returned</param>
        /// <returns>The billing address for the basket, or current user if basket is null</returns>
        public static TaxAddress GetBillingAddress(Basket basket)
        {
            //BY DEFAULT, ASSUME STORE COUNTRY IS BILLING COUNTRY
            string billingCountry = string.Empty;
            int billingProvinceId = 0;
            string billingPostalCode = string.Empty;

            // OBTAIN THE BILLING ADDRESS
            User user = null;
            Address billingAddress = null;
            if (basket != null) user = basket.User;
            else user = Token.Instance.User;
            if (user != null) billingAddress = user.PrimaryAddress;

            // DETERMINE THE TAXABLE ADDRESS
            if (billingAddress != null && billingAddress.IsValid)
            {
                billingCountry = billingAddress.CountryCode;
                billingProvinceId = billingAddress.ProvinceId;
                billingPostalCode = billingAddress.PostalCode;
            }
            else
            {
                Warehouse storeAddress = Token.Instance.Store.DefaultWarehouse;
                billingCountry = storeAddress.CountryCode;
                billingProvinceId = ProvinceDataSource.GetProvinceIdByName(billingCountry, storeAddress.Province);
                billingPostalCode = storeAddress.PostalCode;
            }
            return new TaxAddress(billingCountry, billingProvinceId, billingPostalCode);
        }

        /// <summary>
        /// Gets the shipping addresses for the basket
        /// </summary>
        /// <param name="basket">The basket to parse; can be null to return the default shipping address</param>
        /// <returns>The shipping addresses for the basket</returns>
        public static List<TaxAddress> GetShippingAddresses(Basket basket)
        {
            List<TaxAddress> shippingAddresses = new List<TaxAddress>();
            string shippingCountry;
            int shippingProvinceId;
            string shippingPostalCode;
            if (basket != null)
            {
                foreach (BasketShipment shipment in basket.Shipments)
                {
                    Address shippingAddress = shipment.Address;
                    if (shippingAddress != null && shippingAddress.IsValid)
                    {
                        shippingCountry = shippingAddress.CountryCode;
                        shippingProvinceId = shippingAddress.ProvinceId;
                        shippingPostalCode = shippingAddress.PostalCode;
                        TaxAddress tempShippingAddress = new TaxAddress(shippingCountry, shippingProvinceId, shippingPostalCode);
                        if (shippingAddresses.IndexOf(tempShippingAddress) < 0)
                            shippingAddresses.Add(tempShippingAddress);
                    }
                }
            }
            if (shippingAddresses.Count == 0)
            {
                Warehouse shippingAddress = Token.Instance.Store.DefaultWarehouse;
                shippingCountry = shippingAddress.CountryCode;
                shippingProvinceId = ProvinceDataSource.GetProvinceIdByName(shippingCountry, shippingAddress.Province);
                shippingPostalCode = shippingAddress.PostalCode;
                shippingAddresses.Add(new TaxAddress(shippingCountry, shippingProvinceId, shippingPostalCode));
            }
            return shippingAddresses;
        }

        public static List<TaxAddress> GetShippingAddresses(Order order)
        {
            List<TaxAddress> shippingAddresses = new List<TaxAddress>();
            string shippingCountry;
            int shippingProvinceId;
            string shippingPostalCode;
            if (order != null)
            {
                foreach (OrderShipment shipment in order.Shipments)
                {
                    Address shippingAddress = shipment.Address;
                    if (shippingAddress != null && shippingAddress.IsValid)
                    {
                        shippingCountry = shippingAddress.CountryCode;
                        shippingProvinceId = shippingAddress.ProvinceId;
                        shippingPostalCode = shippingAddress.PostalCode;
                        TaxAddress tempShippingAddress = new TaxAddress(shippingCountry, shippingProvinceId, shippingPostalCode);
                        if (shippingAddresses.IndexOf(tempShippingAddress) < 0)
                            shippingAddresses.Add(tempShippingAddress);
                    }
                }
            }
            if (shippingAddresses.Count == 0)
            {
                Warehouse shippingAddress = Token.Instance.Store.DefaultWarehouse;
                shippingCountry = shippingAddress.CountryCode;
                shippingProvinceId = ProvinceDataSource.GetProvinceIdByName(shippingCountry, shippingAddress.Province);
                shippingPostalCode = shippingAddress.PostalCode;
                shippingAddresses.Add(new TaxAddress(shippingCountry, shippingProvinceId, shippingPostalCode));
            }
            return shippingAddresses;
        }

        /// <summary>
        /// Gets the name of a province given the ID
        /// </summary>
        /// <param name="provinceId">ID of the province</param>
        /// <returns>The name, or string.empty if an invalid ID is given</returns>
        public static string GetProvinceName(int provinceId)
        {
            if (provinceId == 0) return string.Empty;
            Province p = ProvinceDataSource.Load(provinceId);
            if (p == null) return string.Empty;
            return p.Name;
        }
    }
}
