<%@ WebHandler Language="C#" Class="GetCategories" %>

using System;
using System.Collections.Generic;
using System.Web;
using ComponentArt.Web.UI;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;

public class GetCategories : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        int _VolumeDiscountId = AlwaysConvert.ToInt(context.Request.QueryString["VolumeDiscountId"]);
        VolumeDiscount _VolumeDiscount = VolumeDiscountDataSource.Load(_VolumeDiscountId);
        context.Response.Clear();
        context.Response.Cache.SetNoStore();
        context.Response.ContentType = "text/xml";
        context.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
        ComponentArt.Web.UI.TreeView CategoryTree = new ComponentArt.Web.UI.TreeView();
        int categoryId = PageHelper.GetCategoryId();
        CategoryCollection children = CategoryDataSource.LoadForParent(categoryId, false);
        foreach (Category child in children)
        {
            ComponentArt.Web.UI.TreeViewNode newNode = new ComponentArt.Web.UI.TreeViewNode();
            newNode.Text = child.Name;
            newNode.ID = child.CategoryId.ToString();
            if (CategoryDataSource.CountForParent(child.CategoryId) > 0)
                newNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + newNode.ID + "&VolumeDiscountId=" + _VolumeDiscountId.ToString();
            if (_VolumeDiscount != null)
                newNode.Checked = (_VolumeDiscount.CategoryVolumeDiscounts.IndexOf(child.CategoryId, _VolumeDiscountId) > -1);
            newNode.ShowCheckBox = true;
            CategoryTree.Nodes.Add(newNode);
        }
        context.Response.Write(CategoryTree.GetXml());
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}