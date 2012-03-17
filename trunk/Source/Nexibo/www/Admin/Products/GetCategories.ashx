<%@ WebHandler Language="C#" Class="GetCategories" %>

using System;
using System.Collections.Generic;
using System.Web;
using ComponentArt.Web.UI;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;

public class GetCategories : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        int productId = AlwaysConvert.ToInt(context.Request.QueryString["ProductId"]);
        Product product = ProductDataSource.Load(productId);
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
                newNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + newNode.ID + "&ProductId=" + productId.ToString();
            if (product != null)
            {
                newNode.Checked = (product.Categories.IndexOf(child.CategoryId) > -1);
            }
            newNode.ShowCheckBox = true;
            CategoryTree.Nodes.Add(newNode);
        }
        // Use TreeView's GetXml method to return the XML structure   
        context.Response.Write(CategoryTree.GetXml());
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }
}