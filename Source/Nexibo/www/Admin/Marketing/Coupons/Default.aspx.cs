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
using CommerceBuilder.Web.UI;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using System.Collections.Generic;

public partial class Admin_Marketing_Coupons_Default : AbleCommerceAdminPage
{
    protected string GetDiscount(Coupon item)
    {
        if (item.IsPercent) return string.Format("{0:f2}%", item.DiscountAmount);
        return string.Format("{0:lc}", item.DiscountAmount);
    }

    protected string GetNames(Coupon item)
    {
        List<string> groupNames = new List<string>();
        foreach (CouponGroup link in item.CouponGroups)
        {
            groupNames.Add(link.Group.Name);
        }
        return string.Join(", ", groupNames.ToArray());
    }

    protected int GetUseCount(string couponCode)
    {
        return CouponDataSource.CountUses(couponCode);
    }

    protected void CouponGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Coupon coupon = (Coupon)e.Row.DataItem;
            Panel maximumValuePanel = (Panel)e.Row.FindControl("MaxValuePanel");
            if (maximumValuePanel != null) maximumValuePanel.Visible = (coupon.MaxValue > 0);
            Panel minimumPurchasePanel = (Panel)e.Row.FindControl("MinPurchasePanel");
            if (minimumPurchasePanel != null) minimumPurchasePanel.Visible = (coupon.MinPurchase > 0);
            Panel startDatePanel = (Panel)e.Row.FindControl("StartDatePanel");
            if (startDatePanel != null) startDatePanel.Visible = (coupon.StartDate > DateTime.MinValue);
            Panel endDatePanel = (Panel)e.Row.FindControl("EndDatePanel");
            if (endDatePanel != null) endDatePanel.Visible = (coupon.EndDate > DateTime.MinValue);
            Panel maximumUsesPanel = (Panel)e.Row.FindControl("MaximumUsesPanel");
            if (maximumUsesPanel != null) maximumUsesPanel.Visible = (coupon.MaxUses > 0);
            Panel maximumUsesPerCustomerPanel = (Panel)e.Row.FindControl("MaximumUsesPerCustomerPanel");
            if (maximumUsesPerCustomerPanel != null) maximumUsesPerCustomerPanel.Visible = (coupon.MaxUsesPerCustomer > 0);
            Panel groupsPanel = (Panel)e.Row.FindControl("GroupsPanel");
            if (groupsPanel != null) groupsPanel.Visible = (coupon.CouponGroups.Count > 0);
        }
    }

    protected void CouponGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Copy"))
        {
            int couponId = AlwaysConvert.ToInt(e.CommandArgument);
            Coupon coupon = CouponDataSource.Load(couponId);
            if (coupon != null)
            {
                Coupon newCoupon = coupon.Clone(true);
                // THE NAME SHOULD NOT EXCEED THE MAX 100 CHARS
                String newName = "Copy of " + newCoupon.Name;
                if (newName.Length > 100)
                {
                    newName = newName.Substring(0, 97) + "...";
                }
                newCoupon.Name = newName;
                newCoupon.CouponCode = StringHelper.RandomString(12);
                newCoupon.Save();
            }
            CouponGrid.DataBind();
        }
    }
}
