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

public partial class Admin_People_Manufacturers_EditManufacturer : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    int _ManufacturerId = 0;
    Manufacturer _Manufacturer;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _ManufacturerId = AlwaysConvert.ToInt(Request.QueryString["ManufacturerId"]);
        _Manufacturer = ManufacturerDataSource.Load(_ManufacturerId);
        if (_Manufacturer == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Name.Text = _Manufacturer.Name;
        }
        Caption.Text = string.Format(Caption.Text, _Manufacturer.Name);
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Manufacturer.Name = Name.Text;
            _Manufacturer.Save();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

}
