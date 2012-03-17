using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes.Providers.WATax
{
    /// <summary>
    /// Contains address information for wa state tax calculations
    /// </summary>
    internal class TaxAddress
    {
        private string _StreetAddress;
        private String _City;
        private String _Province = String.Empty;
        private string _PostalCode;
        
        /// <summary>
        /// Gets the country code
        /// </summary>
        public string StreetAddress
        {
            get { return _StreetAddress; }
        }

        /// <summary>
        /// Gets the province Id
        /// </summary>
        public string City
        {
            get { return _City; }
        }

        /// <summary>
        /// Gets the province
        /// </summary>
        public string Province
        {
            get { return _Province; }
        }

        /// <summary>
        /// Gets the postal code
        /// </summary>
        public string PostalCode
        {
            get { return _PostalCode; }
        }

        /// <summary>
        /// Creates a tax address with the given values
        /// </summary>
        /// <param name="streetAddress">Street address</param>
        /// <param name="city">City</param>
        /// <param name="province">Province</param>
        /// <param name="postalCode">Postal Code</param>
        public TaxAddress(string streetAddress, String city, string province, string postalCode)
        {
            this._StreetAddress = streetAddress;
            this._City = city;
            this._Province = province.ToUpperInvariant();
            this._PostalCode = postalCode;
        }

        public string CacheKey
        {
            get
            {
                string cacheKey = this.StreetAddress + ":" + this.City + ":" + this.PostalCode;
                return cacheKey.ToUpperInvariant();
            }
        }
    }
}