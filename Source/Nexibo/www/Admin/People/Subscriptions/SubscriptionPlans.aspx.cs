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

public partial class Admin_People_Subscriptions_SubscriptionPlans : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        SubscriptionPlanCollection plans = SubscriptionPlanDataSource.LoadForStore("Name");
        SubscriptionPlanGrid.Columns[4].Visible = this.ShowGroup(plans);
    }

    private bool ShowGroup(SubscriptionPlanCollection plans)
    {
        foreach (SubscriptionPlan sp in plans)
        {
            if (sp.Group != null) return true;
        }
        return false;
    }

    protected int CountSubscriptions(object dataItem, BitFieldState active)
    {
        SubscriptionPlan subscriptionPlan = (SubscriptionPlan)dataItem;
        return SubscriptionDataSource.SearchCount(subscriptionPlan.ProductId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue, active);
    }

    protected int CountExpiringSubscriptions(Object dataItem, int expDays) 
    {
        SubscriptionPlan subscriptionPlan = (SubscriptionPlan)dataItem;
        DateTime expDateEnd = GetEndOfDay(LocaleHelper.LocalNow.AddDays(expDays));
        return SubscriptionDataSource.SearchCount(subscriptionPlan.ProductId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, expDateEnd, BitFieldState.Any);
    }

    private static DateTime GetEndOfDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Local);
    }
}
