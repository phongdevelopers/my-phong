using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommerceBuilder.Orders;
using CommerceBuilder.Shipping;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// DataSource class for TaxRule objects
    /// </summary>
    [DataObject(true)]
    public partial class TaxRuleDataSource
    {
        /// <summary>
        /// Gets the ID of the tax rule given the name
        /// </summary>
        /// <param name="name">Name of the tax rule</param>
        /// <returns>ID of the tax rule, or 0 if not found</returns>
        /// <remarks>Case insensitive match</remarks>
        public static int GetTaxRuleIdByName(string name)
        {
            if (name == null) return 0;
            string upperName = name.Trim().ToUpperInvariant();
            if (upperName.Length == 0) return 0;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT TaxRuleId FROM ac_TaxRules WHERE StoreId=@storeId AND UPPER(Name)=@NAME");
            database.AddInParameter(selectCommand, "storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@NAME", DbType.String, upperName);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of TaxRule objects for the given basket and address
        /// </summary>
        /// <param name="basketId">Id of the basket to load the TaxRule objects for</param>
        /// <param name="address">Tax address of the basket owner</param>
        /// <param name="user">User that owns the basket</param>
        /// <returns>A collection of TaxRule objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static TaxRuleCollection LoadForBasket(int basketId, TaxAddress address, User user)
        {
            List<TaxRule> potentialTaxRules = new List<TaxRule>();
            //GET ALL TAX CODES THAT ARE PRESENT IN THE BASKET
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT TaxCodeId FROM ac_BasketItems");
            selectQuery.Append(" WHERE BI.BasketId = @basketId AND TaxCodeId > 0");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@basketId", System.Data.DbType.Int32, basketId);
            List<int> taxCodes = new List<int>();
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    taxCodes.Add(dr.GetInt32(0));
                }
                dr.Close();
            }
            //THIS WILL RETURN ALL TAX RULES THAT COULD APPLY TO PRODUCT IN THE BASKET
            //FOR THE GIVEN ADDRESS AND USER
            return LoadForTaxCodes(taxCodes.ToArray(), address, user);
        }

        /// <summary>
        /// Loads rules that may apply for the given tax code and address information
        /// </summary>
        /// <param name="taxCodeId">Tax code Id</param>        
        /// <param name="taxAddress">Address for which to load the tax rules</param>
        /// <param name="user">User for which to load the tax rules</param>
        /// <returns>Collection of rules that apply</returns>
        public static TaxRuleCollection LoadForTaxCode(int taxCodeId, TaxAddress taxAddress, User user)
        {
            int[] taxCodeIds = { taxCodeId };
            return LoadForTaxCodes(taxCodeIds, taxAddress, user);
        }

        /// <summary>
        /// Loads rules that may apply for the given tax codes and address information
        /// </summary>
        /// <param name="taxCodeIds">Array of tax code Ids</param>        
        /// <param name="taxAddress">Address for which to load the tax rules</param>
        /// <param name="user">User for which to load the tax rules</param>
        /// <returns>Collection of rules that apply</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static TaxRuleCollection LoadForTaxCodes(int[] taxCodeIds, TaxAddress taxAddress, User user)
        {
            //IF WE DO NOT HAVE VALID TAX CODES THERE ARE NO RULES
            if ((taxCodeIds == null) || (taxCodeIds.Length == 0)) return new TaxRuleCollection();
            TaxRuleCollection potentialTaxRules = new TaxRuleCollection();

            //FIRST DETERMINE THE ZONE(S) FOR THE Tax Address
            ShipZoneCollection shipmentZoneCollection = taxAddress.ShipZones;

            //BUILD A LIST OF ZONEIDS FOR QUERY CRITERIA
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT DISTINCT " + TaxRule.GetColumnNames("TR"));
            selectQuery.Append(" FROM (ac_TaxRules TR INNER JOIN ac_TaxRuleTaxCodes TRTC ON TR.TaxRuleId = TRTC.TaxRuleId)");
            selectQuery.Append(" LEFT JOIN ac_TaxRuleShipZones TRSZ ON TR.TaxRuleId = TRSZ.TaxRuleId");
            selectQuery.Append(" WHERE StoreId = @storeId ");
            if (taxCodeIds.Length == 1)
                selectQuery.Append(" AND TRTC.TaxCodeId = " + taxCodeIds[0].ToString());
            else
                selectQuery.Append(" AND TRTC.TaxCodeId IN (" + AlwaysConvert.ToList(",", taxCodeIds) + ")");

            //PROCESS SHIPZONE EXCLUSION
            selectQuery.Append(" AND (TRSZ.ShipZoneId IS NULL");
            for (int i = 0; i < shipmentZoneCollection.Count; i++)
            {
                selectQuery.Append(" OR TRSZ.ShipZoneId = @shipZoneId" + i.ToString());
            }
            selectQuery.Append(") ORDER BY TR.Priority");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //ADD IN NUMBERED ZONE PARAMETERS
            for (int i = 0; i < shipmentZoneCollection.Count; i++)
            {
                database.AddInParameter(selectCommand, "@shipZoneId" + i.ToString(), System.Data.DbType.Int32, shipmentZoneCollection[i].ShipZoneId);
            }
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    TaxRule taxRule = new TaxRule();
                    TaxRule.LoadDataReader(taxRule, dr);
                    potentialTaxRules.Add(taxRule);
                }
                dr.Close();
            }

            //PROCESS GROUP EXCLUSIONS
            TaxRuleCollection potentialTaxRulesWithGroupFilter = new TaxRuleCollection();
            foreach (TaxRule taxRule in potentialTaxRules)
            {
                if (taxRule.AppliesToUser(user)) potentialTaxRulesWithGroupFilter.Add(taxRule);
            }

            //RETURN POTENTIAL TAXES
            return potentialTaxRulesWithGroupFilter;
        }
    }
}