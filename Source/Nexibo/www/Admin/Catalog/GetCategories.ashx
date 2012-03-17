<%@ WebHandler Language="C#" Class="GetCategories" %>

using System;
using System.Collections.Generic;
using System.Web;
using ComponentArt.Web.UI;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using CommerceBuilder.Products;

public class GetCategories : IHttpHandler
{
    
    public void ProcessRequest(HttpContext context)
    {
        int webpageId = 0;
        int linkId = 0;
        Webpage webpage = null;
        Link link = null;
        
        webpageId = PageHelper.GetWebpageId();
        linkId = PageHelper.GetLinkId();
        if (webpageId != 0)
            webpage = WebpageDataSource.Load(webpageId);
        else
            if (linkId != 0)
                link = LinkDataSource.Load(linkId);
            else
                return;
        
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
                if(webpage!=null)
                    newNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + newNode.ID + "&WebpageId=" + webpageId.ToString();
                else
                    newNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + newNode.ID + "&LinkId=" + linkId.ToString();

            if (webpage != null)
            {
                newNode.Checked = (webpage.Categories.IndexOf(child.CategoryId) > -1);
            }
            else
            if(link != null)
            {
                newNode.Checked = (link.Categories.IndexOf(child.CategoryId) > -1);
            }
            newNode.ShowCheckBox = true;
            CategoryTree.Nodes.Add(newNode);
        }
        // Use TreeView's GetXml method to return the XML structure   
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