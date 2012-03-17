using System;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Shipping;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Class representing an Address in database
    /// </summary>
    public partial class Address
    {
        private int _ProvinceId = 0;
        private ShipZoneCollection _ShipZones;

        /// <summary>
        /// The province Id
        /// </summary>
        public int ProvinceId
        {
            get
            {
                if (this._ProvinceId.Equals(0))
                {
                    this._ProvinceId = ProvinceDataSource.GetProvinceIdByName(this.CountryCode, this._Province);
                }
                return this._ProvinceId;
            }
        }

        /// <summary>
        /// The full name in the address
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName).Trim();
            }
            set
            {
                string testValue = (value == null ? string.Empty : value.Trim());
                if (string.IsNullOrEmpty(testValue))
                {
                    this.FirstName = string.Empty;
                    this.LastName = string.Empty;
                }
                else
                {
                    int firstSpace = testValue.IndexOf(" ");
                    if (firstSpace > -1)
                    {
                        this.FirstName = value.Substring(0, firstSpace);
                        this.LastName = value.Substring(firstSpace + 1);
                    }
                    else this.FirstName = value;
                }
            }
        }

        /// <summary>
        /// Ship zones that contain this address
        /// </summary>
        public ShipZoneCollection ShipZones
        {
            get
            {
                if (_ShipZones == null || _IsDirty)
                {
                    _ShipZones = ShipZoneDataSource.LoadForAddress(this.CountryCode, this.ProvinceId, this.PostalCode);
                }
                return _ShipZones;
            }
        }

        /// <summary>
        /// Create string representation of this address
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(true);
        }

        /// <summary>
        /// Create string representation of this address
        /// </summary>
        /// <param name="isHtml">if true creates html output, plain-text otherwise</param>
        /// <returns>String representation of this address</returns>
        public string ToString(bool isHtml)
        {
            if (this.Country != null) return this.ToString(this.Country.AddressFormat, isHtml);
            return this.ToString(string.Empty, isHtml);
        }

        /// <summary>
        /// Create string representation of this address
        /// </summary>
        /// <param name="pattern">The pattern to use for formatting the output</param>
        /// <param name="isHtml">if true creates html output, plain-text otherwise</param>
        /// <returns>String representation of this address</returns>
        public string ToString(string pattern, bool isHtml)
        {
            string countryName = string.Empty;
            if (this.Country != null) countryName = this.Country.Name;
            return CommerceBuilder.Utility.AddressFormatter.Format(pattern, this.FullName, this.Company, this.Address1, this.Address2, this.City, this.Province, this.PostalCode, countryName, this.Phone, this.Fax, this.Email, isHtml);
        }

        /// <summary>
        /// Checks an address for validity.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if ((string.IsNullOrEmpty(this.FirstName) && string.IsNullOrEmpty(this.LastName)) && string.IsNullOrEmpty(this.Company)) return false;
                if (string.IsNullOrEmpty(this.Address1)) return false;
                if (string.IsNullOrEmpty(this.CountryCode)) return false;
                return true;
            }
        }

        /// <summary>
        /// Delete this address object from database
        /// </summary>
        /// <returns>true if delete successful, false otherwise</returns>
        public virtual bool Delete()
        {
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand deleteCommand = database.GetSqlStringCommand("UPDATE ac_BasketShipments SET AddressId = NULL WHERE AddressId = @addressId"))
            {
                database.AddInParameter(deleteCommand, "@addressId", System.Data.DbType.Int32, this.AddressId);
                database.ExecuteNonQuery(deleteCommand);
            }
            return this.BaseDelete();
        }
    }
}
