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

public partial class Admin_Orders_ViewSubscriptions : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        Caption.Text = string.Format(Caption.Text, _Order.OrderNumber);
    }

    protected void SubscriptionGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int subscriptionId = AlwaysConvert.ToInt(e.CommandArgument);
        Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
        switch (e.CommandName)
        {
            case "Activate":
                subscription.Activate();
                SubscriptionGrid.DataBind();
                break;
            case "CancelSubscription":
                subscription.Delete();
                SubscriptionGrid.DataBind();
                break;
        }
    }

    protected String GetGroupName(object id)
    {
        int groupId = AlwaysConvert.ToInt(id);
        Group group = GroupDataSource.Load(groupId);
        if (group != null) return group.Name;
        else return string.Empty;
    }

    protected void SubscriptionGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int subscriptionId = (int)SubscriptionGrid.DataKeys[e.RowIndex].Value;
        Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
        if (subscription != null)
        {
            subscription.ExpirationDate = GetEndOfDay((DateTime)e.NewValues["ExpirationDate"]);
            subscription.Save();
        }
        SubscriptionGrid.EditIndex = -1;
        e.Cancel = true;
    }

    private static DateTime GetEndOfDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Local);
    }
}
