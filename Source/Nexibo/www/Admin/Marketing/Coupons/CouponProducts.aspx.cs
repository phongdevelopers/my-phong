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

public partial class Admin_Marketing_Coupons_CouponProducts : AbleCommerceAdminPage
{
    private Coupon _Coupon;
    private int _CouponId;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CouponId = AlwaysConvert.ToInt(Request.QueryString["CouponId"]);
        _Coupon = CouponDataSource.Load(_CouponId);
        if (_Coupon == null) Response.Redirect("Default.aspx");
        if (_Coupon.ProductRule == CouponRule.All) RedirectToEdit();
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Coupon.Name);
            AllowSelectedLabel.Visible = (_Coupon.ProductRule == CouponRule.AllowSelected);
            ExcludeSelectedLabel.Visible = !AllowSelectedLabel.Visible;
        }
        PageHelper.SetDefaultButton(SearchName, SearchButton);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        RebindPage();
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        int dataItemIndex;
        if (sender is Button) dataItemIndex = AlwaysConvert.ToInt(((Button)sender).CommandArgument);
        else dataItemIndex = AlwaysConvert.ToInt(((ImageButton)sender).CommandArgument);
        GridView grid = (GridView)((GridViewRow)((Control)sender).NamingContainer).NamingContainer;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, true);
        RebindPage();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        int dataItemIndex;
        if (sender is Button) dataItemIndex = AlwaysConvert.ToInt(((Button)sender).CommandArgument);
        else dataItemIndex = AlwaysConvert.ToInt(((ImageButton)sender).CommandArgument);
        GridView grid = (GridView)((GridViewRow)((Control)sender).NamingContainer).NamingContainer;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, false);
        RebindPage();
    }

    private void SetLink(int productId, bool linked)
    {
        int index = _Coupon.CouponProducts.IndexOf(_CouponId, productId);
        if (linked && (index < 0))
        {
            _Coupon.CouponProducts.Add(new CouponProduct(_CouponId, productId));
            _Coupon.Save();
        }
        else if (!linked && (index > -1))
        {
            _Coupon.CouponProducts.DeleteAt(index);
        }
    }

    private void RebindPage()
    {
        ProductGrid.DataBind();
        SearchResultsGrid.DataBind();
    }

    protected bool IsProductLinked(int productId)
    {
        return (_Coupon.CouponProducts.IndexOf(_CouponId, productId) > -1);
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        string pattern = SearchName.Text.Trim();
        if (string.IsNullOrEmpty(pattern)) pattern = "*";
        //IMPLEMENT A SUBSTRING MATCH UNLESS OTHERWISE SPECIFIED
        if ((!pattern.Contains("*")) && (!pattern.Contains("?"))) pattern = "*" + pattern + "*";
        e.InputParameters["name"] = pattern;
        SearchResultsGrid.Columns[1].Visible = ShowImages.Checked;
    }

    private void RedirectToEdit()
    {
        Response.Redirect("EditCoupon.aspx?CouponId=" + _CouponId.ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        RedirectToEdit();
    }
}
