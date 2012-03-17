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
using CommerceBuilder.Products;
using System.Collections.Generic;
using CommerceBuilder.Utility;

public partial class Admin_Products_GiftWrap_ViewProducts : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _WrapGroupId;
    private WrapGroup _WrapGroup;
    protected void Page_Load(object sender, EventArgs e)
    {
        _WrapGroupId = AlwaysConvert.ToInt(Request.QueryString["WrapGroupId"]);
        _WrapGroup = WrapGroupDataSource.Load(_WrapGroupId);
        if (_WrapGroup == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _WrapGroup.Name);
    //        List<WrapGroup> targets = new List<WrapGroup>();
    //        WrapGroupCollection allWrapGroups = WrapGroupDataSource.LoadForStore();
    //        foreach (WrapGroup w in allWrapGroups)
    //        {
    //            if (w.WrapGroupId != AlwaysConvert.ToInt(_WrapGroupId)) targets.Add(w);
    //        }
    //        NewWrapGroup.DataSource = targets;
    //        NewWrapGroup.DataBind();
        }
    }

    //protected void ProductGrid_DataBound(object sender, EventArgs e)
    //{
    //    foreach (GridViewRow gvr in ProductGrid.Rows)
    //    {
    //        CheckBox cb = (CheckBox)gvr.FindControl("Selected");
    //        ScriptManager.RegisterArrayDeclaration(ProductGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
    //    }
    //}

    //protected void Page_PreRender(object sender, EventArgs e)
    //{
    //    NewWrapGroupPanel.Visible = ((NewWrapGroup.Items.Count > 0) && (ProductGrid.Rows.Count > 0));
    //}

    //protected void BackButton_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("EditWrapGroup.aspx?WrapGroupId=" + _WrapGroupId.ToString());
    //}

    //protected void NewWrapGroupUpdateButton_Click(object sender, EventArgs e)
    //{
    //    int newWrapGroupId = AlwaysConvert.ToInt(NewWrapGroup.SelectedValue);
    //    WrapGroup newWrapGroup = WrapGroupDataSource.Load(newWrapGroupId);
    //    if (newWrapGroup != null)
    //    {
    //        foreach (GridViewRow row in ProductGrid.Rows)
    //        {
    //            CheckBox selected = (CheckBox)row.FindControl("Selected");
    //            if ((selected != null) && (selected.Checked))
    //            {
    //                int productId = AlwaysConvert.ToInt(ProductGrid.DataKeys[row.DataItemIndex].Value);
    //                Product product = ProductDataSource.Load(productId);
    //                if (product != null)
    //                {
    //                    product.WrapGroupId = newWrapGroupId;
    //                    product.Save();
    //                }
    //            }
    //        }
    //        ProductGrid.DataBind();
    //    }
    //}
}
