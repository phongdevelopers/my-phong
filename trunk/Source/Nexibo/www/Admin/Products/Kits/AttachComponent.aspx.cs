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

public partial class Admin_Products_Kits_AttachComponent : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected int _CategoryId;
    protected Category _Category;
    protected int _ProductId;
    protected Product _Product;

    
    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString()));
        SearchResultsGrid.Visible = Page.IsPostBack;
        if (!Page.IsPostBack)
        {
            AddComponent.NavigateUrl += "?ProductId=" + _ProductId.ToString();
        }
    }

    protected string FixInputTypeName(string name)
    {
        switch (name.ToUpperInvariant())
        {
            case "INCLUDEDHIDDEN":
                return "Included - Hidden";
            case "INCLUDEDSHOWN":
                return "Included - Shown";
            default:
                return Regex.Replace(name, "([A-Z])", " $1").Trim();
        }
    }    

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.DataBind();
    }

    protected void SearchResultsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Attach"))
        {
            int index = AlwaysConvert.ToInt(e.CommandArgument);
            int kitComponentId = (int)SearchResultsGrid.DataKeys[index].Value;
            KitComponent kitComponent = KitComponentDataSource.Load(kitComponentId);
            if (kitComponent != null)
            {
                _Product.ProductKitComponents.Add(new ProductKitComponent(_ProductId, kitComponentId));
                _Product.Save();
                Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId));
            }
        } else if (e.CommandName.Equals("Copy"))
        {
            int index = AlwaysConvert.ToInt(e.CommandArgument);
            int kitComponentId = (int)SearchResultsGrid.DataKeys[index].Value;
            KitComponent kitComponent = KitComponentDataSource.Load(kitComponentId);
            if (kitComponent != null)
            {
                KitComponent branchedComponent = kitComponent.Copy(_ProductId);
                branchedComponent.Name = "Copy of " + kitComponent.Name;
                branchedComponent.Save();
                Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", _CategoryId, _ProductId));
            }
        }

    }
    

}
