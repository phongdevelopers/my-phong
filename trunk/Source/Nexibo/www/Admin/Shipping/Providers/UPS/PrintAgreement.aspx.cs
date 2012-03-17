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
using CommerceBuilder.Shipping;
using CommerceBuilder.Shipping.Providers.UPS;

public partial class Admin_Shipping_UPS_PrintAgreement : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        LicenseAgreement.Text = UPS.GetAgreement();               
    }

}
