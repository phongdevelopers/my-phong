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
using CommerceBuilder.Catalog;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public partial class Admin_Orders_Print_PullSheet : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private List<string> orderNumbers;

    private string GetOrderNumbers(List<int> orders)
    {
        if (orderNumbers == null)
        {
            orderNumbers = new List<string>();
            foreach (int orderId in orders)
            {
                int orderNumber = OrderDataSource.LookupOrderNumber(orderId);
                orderNumbers.Add(orderNumber.ToString());
            }
        }
        if (orderNumbers.Count == 0) return string.Empty;
        return string.Join(", ", orderNumbers.ToArray());
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> selectedOrders = GetSelectedOrders();
        if ((selectedOrders == null) || (selectedOrders.Count == 0)) Response.Redirect("~/Admin/Orders/Default.aspx");
        ItemGrid.DataSource = OrderPullItemDataSource.GeneratePullSheet(selectedOrders.ToArray());
        ItemGrid.DataBind();
        OrderList.Text = GetOrderNumbers(selectedOrders);
    }

    private List<int> GetSelectedOrders()
    {
        List<int> selectedOrders = null;
        // CHECK FOR QUERYSTRING PARAMETERS
        if (!String.IsNullOrEmpty(Request.QueryString["OrderId"]))
        {
            int orderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            selectedOrders = new List<int>();
            selectedOrders.Add(orderId);
        }
        // CHECK SESSION
        else selectedOrders = Token.Instance.Session.SelectedOrderIds;
        return selectedOrders;
    }
}
