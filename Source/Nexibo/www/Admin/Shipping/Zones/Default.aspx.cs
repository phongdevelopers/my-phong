using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Shipping;

public partial class Admin_Shipping_Zones_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected string GetCountryNames(object dataItem)
    {
        ShipZone shipZone = (ShipZone)dataItem;
        if (shipZone.CountryRule == FilterRule.All)
        {
            return "All";
        }
        List<string> countries = new List<string>();
        foreach (ShipZoneCountry countryAssn in shipZone.ShipZoneCountries)
        {
            countries.Add(countryAssn.Country.Name);
        }
        string countryList = string.Join(", ", countries.ToArray());
        if (countryList.Length > 100)
        {
            countryList = (countryList.Substring(0, 100) + "...");
        }
        if (shipZone.CountryRule == FilterRule.ExcludeSelected)
            countryList = "All Except: " + countryList;
        return countryList;
    }

    protected string GetProvinceNames(object dataItem)
    {
        ShipZone shipZone = (ShipZone)dataItem;
        if (shipZone.ProvinceRule == FilterRule.All)
        {
            return "All";
        }
        List<string> provinces = new List<string>();
        foreach (ShipZoneProvince provinceAssn in shipZone.ShipZoneProvinces)
        {
            provinces.Add(provinceAssn.Province.Name);
        }
        provinces.Sort();
        string provinceList = string.Join(", ", provinces.ToArray());
        if (provinceList.Length > 100)
        {
            provinceList = (provinceList.Substring(0, 100) + "...");
        }
        if (shipZone.ProvinceRule == FilterRule.ExcludeSelected)
            provinceList = "All Except: " + provinceList;
        return provinceList;
    }

    protected string GetPostalCodes(object dataItem)
    {
        //pad with spaces to allow column to wrap
        ShipZone shipZone = (ShipZone)dataItem;
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(shipZone.PostalCodeFilter))
        {
            string includesCodes = shipZone.PostalCodeFilter;
            if (!includesCodes.StartsWith("@")) includesCodes = includesCodes.Replace(",", ", ");
            if (includesCodes.Length > 50) includesCodes = includesCodes.Substring(0, 47) + "...";
            sb.Append("Inc: " + includesCodes);
        }
        if (!string.IsNullOrEmpty(shipZone.ExcludePostalCodeFilter))
        {
            if (sb.Length > 0) sb.Append("<br />");
            string excludesCodes = shipZone.ExcludePostalCodeFilter;
            if (!excludesCodes.StartsWith("@")) excludesCodes = excludesCodes.Replace(",", ", ");
            if (excludesCodes.Length > 50) excludesCodes = excludesCodes.Substring(0, 47) + "...";
            sb.Append("Exc: " + excludesCodes);
        }
        return sb.ToString();
    }

    protected void AddShipZoneButton_Click(object sender, EventArgs e)
    {
        ShipZone shipZone = new ShipZone();
        shipZone.Name = AddShipZoneName.Text;
        shipZone.Save();
        Response.Redirect("EditShipZone.aspx?ShipZoneId=" + shipZone.ShipZoneId.ToString());
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddShipZoneName, AddShipZoneButton.ClientID);
    }

    protected void ShipZoneGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Copy")
        {
            int shipZoneId = (int)ShipZoneGrid.DataKeys[Int32.Parse(e.CommandArgument.ToString())].Value;
            ShipZone copy = ShipZone.Copy(shipZoneId, true);
            if (copy != null)
            {
                // THE NAME SHOULD NOT EXCEED THE MAX 255 CHARS
                String newName = "Copy of " + copy.Name;
                if (newName.Length > 255)
                {
                    newName = newName.Substring(0, 252) + "...";
                }
                copy.Name = newName;
                copy.Save();
                ShipZoneGrid.DataBind();
            }
        }
    }
}