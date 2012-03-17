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
using CommerceBuilder.Marketing;

public partial class Admin_Products_ProductDiscounts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private Category _Category;
    private int _CategoryId;
    private Product _Product;
    private int _ProductId;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString());
        Caption.Text = string.Format(Caption.Text, _Product.Name);
    }

    protected bool IsAttached(int discountId)
    {
        if (_Product != null)
        {
            return (_Product.ProductVolumeDiscounts.IndexOf(_ProductId, discountId) > -1);
        }
        else return false;
    }

    protected string GetNames(VolumeDiscount discount)
    {
        if (discount.VolumeDiscountGroups.Count == 0) return string.Empty;
        List<string> groupNames = new List<string>();
        foreach (VolumeDiscountGroup vdr in discount.VolumeDiscountGroups)
        {
            groupNames.Add(vdr.Group.Name);
        }
        return string.Join(",", groupNames.ToArray());
    }

    protected string GetLevels(VolumeDiscount discount)
    {
        if (discount.Levels.Count == 0) return string.Empty;
        List<string> levels = new List<string>();
        string from;
        string to;
        string amount;
        foreach (VolumeDiscountLevel level in discount.Levels)
        {
            if (level.MinValue == 0) from = "any";
				else from =  level.MinValue.ToString("0.##");
            if (level.MaxValue == 0) to = "any";
				else to = level.MaxValue.ToString("0.##");
            if (level.IsPercent) 
				amount = string.Format("{0:0.##}%", level.DiscountAmount);
			else amount = string.Format("{0:lc}", level.DiscountAmount);
				levels.Add(string.Format("from {0} to {1} - {2}", from, to, amount));
        }
        return string.Join("<br />", levels.ToArray());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        //CLEAR OUT EXISTING ASSIGNMENTS
        _Product.ProductVolumeDiscounts.DeleteAll();
        foreach (GridViewRow row in DiscountGrid.Rows)
        {
            CheckBox attached = (CheckBox)row.FindControl("Attached");
            if ((attached != null) && attached.Checked) {
                int discountId = (int)DiscountGrid.DataKeys[row.DataItemIndex].Value;
                _Product.ProductVolumeDiscounts.Add(new ProductVolumeDiscount(_ProductId, discountId));
            }
        }
        _Product.Save();
        Response.Redirect("EditProduct.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditProduct.aspx?ProductId=" + _ProductId.ToString());
    }

}
