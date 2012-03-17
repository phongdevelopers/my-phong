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

public partial class Admin_People_Vendors_EditVendor : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    int _VendorId = 0;
    Vendor _Vendor;

    protected void Page_Load(object sender, System.EventArgs e) {
        _VendorId = AlwaysConvert.ToInt(Request.QueryString["VendorId"]);
        _Vendor = VendorDataSource.Load(_VendorId);
        if (_Vendor == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack) {
            Name.Text = _Vendor.Name;
            Email.Text = _Vendor.Email;
        }
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e ) {
        Response.Redirect("Default.aspx");
    }

    protected void SaveButton_Click(object sender, System.EventArgs  e)
    {
        if (Page.IsValid)
        {
            _Vendor.Name = Name.Text;
            _Vendor.Email = Email.Text.Replace(" ", "");
            _Vendor.Save();
            Response.Redirect("Default.aspx");
        }
    }


}
