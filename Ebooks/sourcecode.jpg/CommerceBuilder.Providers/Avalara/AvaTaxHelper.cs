using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Xml;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes.Providers.AvaTax
{
    /// <summary>
    /// Static class with helper functions for the AbleCommerce/AvaTax integration
    /// </summary>
    internal static class AvaTaxHelper
    {
        public static string GetAbleCommerceVersion()
        {
            string version;
            string versionFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/VersionInfo.xml");
            try
            {
                XmlDocument versionInfo = new XmlDocument();
                versionInfo.Load(versionFile);
                version = XmlUtility.GetElementValue(versionInfo.DocumentElement, "Version");
                if (string.IsNullOrEmpty(version)) version = "7.0.4";
                string build = XmlUtility.GetElementValue(versionInfo.DocumentElement, "BuildNumber");
                if (string.IsNullOrEmpty(build)) build = "0";
                version += "." + build;
            }
            catch
            {
                version = "7.0.4.0";
            }
            return version;
        }

        public static TaxService.BaseAddress GetBaseAddressForTaxService(Warehouse warehouse)
        {
            return GetBaseAddressForTaxService(warehouse.Address1, warehouse.Address2, warehouse.City, warehouse.Province, warehouse.PostalCode, warehouse.CountryCode);
        }

        public static TaxService.BaseAddress GetBaseAddressForTaxService(CommerceBuilder.Users.Address address)
        {
            return GetBaseAddressForTaxService(address.Address1, address.Address2, address.City, address.Province, address.PostalCode, address.CountryCode);
        }

        public static TaxService.BaseAddress GetBaseAddressForTaxService(Order order)
        {
            return GetBaseAddressForTaxService(order.BillToAddress1, order.BillToAddress2, order.BillToCity, order.BillToProvince, order.BillToPostalCode, order.BillToCountryCode);
        }

        private static TaxService.BaseAddress GetBaseAddressForTaxService(string address1, string address2, string city, string province, string postalCode, string countryCode)
        {
            TaxService.BaseAddress avalaraAddress = new TaxService.BaseAddress();
            avalaraAddress.AddressCode = avalaraAddress.GetHashCode().ToString();
            avalaraAddress.Line1 = address1;
            avalaraAddress.Line2 = address2;
            avalaraAddress.City = city;
            avalaraAddress.Region = province;
            avalaraAddress.PostalCode = postalCode;
            avalaraAddress.Country = countryCode;
            return avalaraAddress;
        }

        public static AddressService.BaseAddress GetBaseAddressForAddressService(Warehouse warehouse)
        {
            return GetBaseAddressForAddressService(warehouse.Address1, warehouse.Address2, warehouse.City, warehouse.Province, warehouse.PostalCode, warehouse.CountryCode);
        }

        public static AddressService.BaseAddress GetBaseAddressForAddressService(CommerceBuilder.Users.Address address)
        {
            return GetBaseAddressForAddressService(address.Address1, address.Address2, address.City, address.Province, address.PostalCode, address.CountryCode);
        }

        private static AddressService.BaseAddress GetBaseAddressForAddressService(string address1, string address2, string city, string province, string postalCode, string countryCode)
        {
            AddressService.BaseAddress avalaraAddress = new AddressService.BaseAddress();
            avalaraAddress.AddressCode = avalaraAddress.GetHashCode().ToString();
            avalaraAddress.Line1 = address1;
            avalaraAddress.Line2 = address2;
            avalaraAddress.City = city;
            avalaraAddress.Region = province;
            avalaraAddress.PostalCode = postalCode;
            avalaraAddress.Country = countryCode;
            return avalaraAddress;
        }

        public static string SerializeAvaTaxObject(object avaTaxObject)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(avaTaxObject.GetType());
                StringWriter sw = new StringWriter();
                x.Serialize(sw, avaTaxObject);
                return sw.ToString();
            }
            catch { }
            return string.Empty;
        }
    }
}