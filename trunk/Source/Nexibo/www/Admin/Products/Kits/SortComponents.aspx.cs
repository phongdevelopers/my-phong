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

public partial class Admin_Products_Kits_SortComponents : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _CategoryId;
    private Category _Category;
    private int _ProductId;
    private Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx?CategoryId=" + _CategoryId.ToString()));
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        if (!Page.IsPostBack)
        {
            BindComponentList();
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditKit.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SortOrder.Value))
        {
            ProductKitComponentCollection components = _Product.ProductKitComponents;
            string[] componentIds = SortOrder.Value.Split(",".ToCharArray());
            int order = 0;
            foreach(string sPartId in componentIds)
            {
                int componentId = AlwaysConvert.ToInt(sPartId);
                int index = components.IndexOf(_ProductId, componentId);
                if (index > -1) components[index].OrderBy = (short)order;
                order++;
            }
            components.Save();
        }
        Response.Redirect("EditKit.aspx?CategoryId=" + _CategoryId.ToString() + "&ProductId=" + _ProductId.ToString());
    }

    protected void QuickSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KitComponentCollection components = new KitComponentCollection();
        foreach (ProductKitComponent pc in _Product.ProductKitComponents)
        {
            components.Add(pc.KitComponent);
        }
        switch (QuickSort.SelectedIndex)
        {
            case 2:
                components.Sort("Name", GenericComparer.SortDirection.DESC);
                break;
            default:
                components.Sort("Name", GenericComparer.SortDirection.ASC);
                break;
        }
        KitComponentList.DataSource = components;
        KitComponentList.DataBind();
        QuickSort.SelectedIndex = 0;
    }

    private void BindComponentList()
    {
        KitComponentCollection components = new KitComponentCollection();
        foreach (ProductKitComponent pc in _Product.ProductKitComponents)
        {
            components.Add(pc.KitComponent);
        }
        KitComponentList.DataSource = components;
        KitComponentList.DataBind();
    }

    
}
