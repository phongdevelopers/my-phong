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

public partial class Admin_Reports_BasketSummary : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _BasketId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _BasketId = AlwaysConvert.ToInt(Request.QueryString["BasketId"]);
        if (!Page.IsPostBack)
        { 
            BasketItemsGrid.DataSource = BasketItemDataSource.LoadForBasket(_BasketId);
            BasketItemsGrid.DataBind();
        }
    }

}
