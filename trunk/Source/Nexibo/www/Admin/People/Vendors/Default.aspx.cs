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

public partial class Admin_People_Vendors_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void AddVendorButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Vendor vendor = new Vendor();
            vendor.Name = AddVendorName.Text;
            vendor.Save();
            // REDIRECT TO EDIT PAGE
            Response.Redirect(("EditVendor.aspx?VendorId=" + vendor.VendorId.ToString()));
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddVendorName, AddVendorButton.ClientID);
    }

    private Dictionary<int, int> _ProductCounts = new Dictionary<int, int>();
    protected int GetProductCount(object dataItem)
    {
        Vendor v = (Vendor)dataItem;
        if (_ProductCounts.ContainsKey(v.VendorId)) return _ProductCounts[v.VendorId];
        int count = ProductDataSource.CountForVendor(v.VendorId);
        _ProductCounts[v.VendorId] = count;
        return count;
    }

    protected bool ShowDeleteButton(object dataItem)
    {
        Vendor v = (Vendor)dataItem;
        return (v.Products.Count <= 0);
    }

    protected bool ShowDeleteLink(object dataItem)
    {
        Vendor v = (Vendor)dataItem;
        return (v.Products.Count > 0);
    }

}
