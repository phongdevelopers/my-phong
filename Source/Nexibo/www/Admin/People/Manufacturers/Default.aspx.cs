using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Reporting;
using System.Collections.Generic;

public partial class Admin_People_Manufacturers_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void AddManufacturerButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Manufacturer manufacturer = ManufacturerDataSource.LoadForName(AddManufacturerName.Text,false);
            // MANUFACTURER NAME SHOULD BE UNIQUE
            if (manufacturer != null)
            {
                AddManufacturerNameRequired.IsValid = false;
                AddManufacturerNameRequired.ErrorMessage = "The manufacturer with name \"" + AddManufacturerName.Text + "\" already exists.";
                return;
            }
            manufacturer = new Manufacturer();
            manufacturer.Name = AddManufacturerName.Text;
            manufacturer.Save();
            AddManufacturerName.Text = String.Empty;
            AddedMessage.Text = string.Format((string)ViewState["AddedMessage.Text"], manufacturer.Name);
            AddedMessage.Visible = true;
            ManufacturerGrid.DataBind();
            SearchAjax.Update();
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddManufacturerName, AddManufacturerButton.ClientID);
        AddManufacturerName.Focus();
        if (!Page.IsPostBack) ViewState["AddedMessage.Text"] = AddedMessage.Text;
        
    }

    private Dictionary<int, int> _ProductCounts = new Dictionary<int, int>();
    protected int GetProductCount(object dataItem)
    {
        Manufacturer m = (Manufacturer)dataItem;
        if (_ProductCounts.ContainsKey(m.ManufacturerId)) return _ProductCounts[m.ManufacturerId];
        int count = ProductDataSource.CountForManufacturer(m.ManufacturerId);
        _ProductCounts[m.ManufacturerId] = count;
        return count;
    }
    
    protected bool HasProducts(object dataItem)
    {
        Manufacturer m = (Manufacturer)dataItem;
        return (ProductDataSource.CountForManufacturer(m.ManufacturerId) > 0);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        AlphabetRepeater.DataSource = GetAlphabetDS();
        AlphabetRepeater.DataBind();
    }

    protected void AlphabetRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if ((e.CommandArgument.ToString().Length == 1))
        {
            SearchText.Text = (e.CommandArgument.ToString());
        }
        else
        {
            SearchText.Text = String.Empty;
        }
        ManufacturerGrid.DataBind();
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        ManufacturerGrid.DataBind();
    }



    protected string[] GetAlphabetDS()
    {
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "All" };
        return alphabet;
    }

}
