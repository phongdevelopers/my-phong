using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// DataSource class for ShipZone objects
    /// </summary>
    [DataObject(true)]
    public partial class ShipZoneDataSource
    {
        /// <summary>
        /// Loads a collection of ShipZone objects for the given basket shipment
        /// </summary>
        /// <param name="shipment">BasketShipment to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given basket shipment</returns>
        [Obsolete("Please use shipment.Address.ShipZones instead.", false)]
        public static ShipZoneCollection LoadForShipment(BasketShipment shipment)
        {
            return shipment.Address.ShipZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given address
        /// </summary>
        /// <param name="address">Address to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given address</returns>
        [Obsolete("Please use address.ShipZones instead.", false)]
        public static ShipZoneCollection LoadForAddress(Users.Address address)
        {
            return address.ShipZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given address
        /// </summary>
        /// <param name="address">Address to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given address</returns>
        [Obsolete("Please use address.ShipZones instead.", false)]
        public static ShipZoneCollection LoadForAddress(Taxes.TaxAddress address)
        {
            return address.ShipZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given address
        /// </summary>
        /// <param name="countryCode">The country code; this value is required</param>
        /// <param name="provinceId">The ID for the province, or 0 if none</param>
        /// <param name="postalCode">The postal code or empty string if none</param>
        /// <returns>A collection of ShipZone objects for the given address</returns>
        public static ShipZoneCollection LoadForAddress(string countryCode, int provinceId, string postalCode)
        {
            // VALIDATE INPUT
            if (string.IsNullOrEmpty(countryCode)) throw new ArgumentNullException("countryCode");

            // LOAD UP ANY SHIPZONES THAT MATCH THE COUNTRY AND PROVINCE FILTERS
            // BUILD THE SELECT STATEMENT
            StringBuilder selectSQL = new StringBuilder();
            selectSQL.Append("SELECT " + ShipZone.GetColumnNames(string.Empty) + @" FROM ac_ShipZones
WHERE StoreId = @storeId
AND (CountryRuleId = 0
OR (CountryRuleId = 1 AND ShipZoneId IN (SELECT ShipZoneId FROM
ac_ShipZoneCountries WHERE CountryCode = @countryCode))
OR (CountryRuleId = 2 AND ShipZoneId NOT IN (SELECT ShipZoneId FROM
ac_ShipZoneCountries WHERE CountryCode = @countryCode)))
AND (ProvinceRuleId = 0
OR (ProvinceRuleId = 1 AND ShipZoneId IN (SELECT ShipZoneId FROM
ac_ShipZoneProvinces WHERE ProvinceId = @provinceId))
OR (ProvinceRuleId = 2 AND ShipZoneId NOT IN (SELECT ShipZoneId FROM
ac_ShipZoneProvinces WHERE ProvinceId = @provinceId)))");

            // DETERMINE WHETHER A POSTAL CODE IS PROVIDED FOR THE ADDRESS
            if (string.IsNullOrEmpty(postalCode))
            {
                // NO POSTAL CODE IS PROVIDED, ELIMINATE SHIPZONES THAT CANNOT BE MATCHED
                selectSQL.Append("AND ((PostalCodeFilter IS NULL AND UsePCPM = 0) OR UsePCPM = 1)");
            }
            else
            {
                // POSTAL CODE IS PROVIDED, ONLY INCLUDE SHIPZONES THAT MAY BE MATCHED
                selectSQL.Append(@"AND (((PostalCodeFilter IS NULL OR PostalCodeFilter = @postalCode)
AND (ExcludePostalCodeFilter IS NULL OR ExcludePostalCodeFilter <> @postalCode)
AND UsePCPM = 0) OR UsePCPM = 1)");
            }

            // CREATE COMMAND AND POPULATE PARAMETERS
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(string.Format(selectSQL.ToString(), countryCode, provinceId, postalCode));
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@countryCode", DbType.String, countryCode);
            database.AddInParameter(selectCommand, "@provinceId", DbType.Int32, provinceId);
            if (!string.IsNullOrEmpty(postalCode)) database.AddInParameter(selectCommand, "@postalCode", DbType.String, postalCode);

            // EXECUTE THE COMMAND
            ShipZoneCollection shipZones = new ShipZoneCollection();
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                // LOOP THE QUERY RESULTS
                while (dr.Read())
                {
                    // CONVERT QUERY RECORD TO SHIPZONE OBJECT
                    ShipZone shipZone = new ShipZone();
                    ShipZone.LoadDataReader(shipZone, dr);
                    // IF POSTAL CODE PATTERN MATCHING (PCPM) IS NOT REQUIRED
                    // OR IF THE POSTAL CODE MATCHES THE PATTERN, THIS SHIP ZONE APPLIES TO THE ADDRESS
                    if (!shipZone.UsePCPM || shipZone.IncludesPostalCode(postalCode))
                    {
                        shipZones.Add(shipZone);
                    }
                }
                dr.Close();
            }

            // RETURN THE SHIPZONES THAT APPLY TO THIS ADDRESS
            return shipZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given country
        /// </summary>
        /// <param name="countryCode">The country code of the country to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given country</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipZoneCollection LoadForCountry2(String countryCode)
        {
            ShipZoneCollection ShipZones = new ShipZoneCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + ShipZone.GetColumnNames("ac_ShipZones"));
            selectQuery.Append(" FROM ac_ShipZones LEFT JOIN ac_ShipZoneCountries ON ac_ShipZones.ShipZoneId = ac_ShipZoneCountries.ShipZoneId");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (ac_ShipZoneCountries.CountryCode IS NULL OR ac_ShipZoneCountries.CountryCode = @CountryCode)");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipZone shipZone = new ShipZone();
                    ShipZone.LoadDataReader(shipZone, dr);
                    ShipZones.Add(shipZone);
                }
                dr.Close();
            }
            return ShipZones;
        }

    }
}
