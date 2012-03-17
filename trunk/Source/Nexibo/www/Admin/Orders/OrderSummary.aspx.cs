using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

public partial class Admin_Orders_OrderSummary : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId;
    protected void Page_Load(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        if (!Page.IsPostBack)
        {
            OrderItemsGrid.DataSource =OrderItemDataSource.LoadForOrder(_OrderId);
            OrderItemsGrid.DataBind();
        }
    }

}
