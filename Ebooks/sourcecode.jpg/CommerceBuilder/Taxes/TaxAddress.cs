using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Shipping;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Contains address information for tax calculations
    /// </summary>
    public class TaxAddress : IComparable<TaxAddress>
    {
        private string _CountryCode;
        private int _ProvinceId;
        private string _PostalCode;
        private ShipZoneCollection _ShipZones;
        
        /// <summary>
        /// Gets the country code
        /// </summary>
        public string CountryCode
        {
            get { return _CountryCode; }
        }

        /// <summary>
        /// Gets the province Id
        /// </summary>
        public int ProvinceId
        {
            get { return _ProvinceId; }
        }

        /// <summary>
        /// Gets the postal code
        /// </summary>
        public string PostalCode
        {
            get { return _PostalCode; }
        }

        /// <summary>
        /// Creates a new tax address instance
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <param name="provinceId">Province Id</param>
        /// <param name="postalCode">Postal Code</param>
        public TaxAddress(string countryCode, int provinceId, string postalCode)
        {
            _CountryCode = countryCode;
            _ProvinceId = provinceId;
            _PostalCode = postalCode;
        }

        /// <summary>
        /// Creates a new tax address instance
        /// </summary>
        /// <param name="address">User address used to initialize this instance</param>
        public TaxAddress(Address address)
        {
            _CountryCode = address.CountryCode;
            _ProvinceId = address.ProvinceId;
            _PostalCode = address.PostalCode;
        }

        /// <summary>
        /// Ship zones that contain this address
        /// </summary>
        public ShipZoneCollection ShipZones
        {
            get
            {
                if (_ShipZones == null)
                {
                    _ShipZones = ShipZoneDataSource.LoadForAddress(this.CountryCode, this.ProvinceId, this.PostalCode);
                }
                return _ShipZones;
            }
        }

        #region IComparable<TaxAddress> Members

        /// <summary>
        /// Compares this instance to another instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>compare result</returns>
        public int CompareTo(TaxAddress other)
        {
            int result = this.CountryCode.CompareTo(other.CountryCode);
            if (result != 0) return result;
            result = this.ProvinceId.CompareTo(other.ProvinceId);
            if (result != 0) return result;
            return this.PostalCode.CompareTo(other.PostalCode);
        }

        #endregion

        /// <summary>
        /// Tests this instance for equality with another instance
        /// </summary>
        /// <param name="obj">The other instance</param>
        /// <returns>True if the objects are equal</returns>
        public override bool Equals(object obj)
        {
            TaxAddress other = obj as TaxAddress;
            if (other == null) return base.Equals(obj);
            return (this.CompareTo(other) == 0);
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
