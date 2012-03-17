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
using ComponentArt.Web.UI;

public partial class Admin_Products_EditProductCategories : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductId = 0;
    private Product _Product;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Product.Name);
            BuildCategoryTree();
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (CategoryTree.CheckedNodes.Length > 0)
            {
                //UPDATE CATEGORIES
                _Product.Categories.Clear();
                foreach (TreeViewNode node in CategoryTree.CheckedNodes)
                {
                    int categoryId = AlwaysConvert.ToInt(node.ID);
                    _Product.Categories.Add(categoryId);
                }
                _Product.Save();
                SuccessMessage.Text = string.Format("Product categories updated at {0}", LocaleHelper.LocalNow);
                SuccessMessage.Visible = true;
                FailureMessage.Visible = false;
            }
            else
            {
                FailureMessage.Text = "You must select at least one category.";
                FailureMessage.Visible = true;
                SuccessMessage.Visible = false;
            }
        }
    }

    private void BuildCategoryTree()
    {
        //CONTAINER TO HOLD THE CATEGORY NODES
        ComponentArt.Web.UI.TreeViewNode storeNode = new ComponentArt.Web.UI.TreeViewNode();
        storeNode.ID = "0";
        PopulateTreeNode(storeNode);
        //EXPAND THE TREE OUT
        foreach (int productCategoryId in _Product.Categories)
        {
            TreeViewNode currentNode = storeNode;
            List<CatalogPathNode> pathNodes = CatalogDataSource.GetPath(productCategoryId, false);
            foreach (CatalogPathNode pathNode in pathNodes)
            {
                int index = IndexOfTreeNode(currentNode.Nodes, pathNode.CategoryId.ToString());
                if (index > -1)
                {
                    currentNode = currentNode.Nodes[index];
                    PopulateTreeNode(currentNode);
                }
            }
        }
        //ADD THE CATEGORYES TO THE TREE (SKIP STORE NODE)
        foreach (TreeViewNode topNode in storeNode.Nodes)
        {
            CategoryTree.Nodes.Add(topNode);
        }
    }

    private int IndexOfTreeNode(TreeViewNodeCollection childNodes, string nodeID)
    {
        int index = 0;
        foreach (TreeViewNode childNode in childNodes)
        {
            if (childNode.ID.Equals(nodeID)) return index;
            index++;
        }
        return -1;
    }

    private void PopulateTreeNode(TreeViewNode parentNode)
    {
        if (!parentNode.Expanded)
        {
            parentNode.Expanded = true;
            parentNode.ContentCallbackUrl = string.Empty;
            CategoryCollection children = CategoryDataSource.LoadForParent(AlwaysConvert.ToInt(parentNode.ID), false);
            foreach (Category child in children)
            {
                ComponentArt.Web.UI.TreeViewNode childNode = new ComponentArt.Web.UI.TreeViewNode();
                childNode.Text = child.Name;
                childNode.ID = child.CategoryId.ToString();
                if (CategoryDataSource.CountForParent(child.CategoryId) > 0)
                    childNode.ContentCallbackUrl = "GetCategories.ashx?CategoryId=" + childNode.ID + "&ProductId=" + _ProductId.ToString();
                childNode.Checked = (_Product.Categories.IndexOf(child.CategoryId) > -1);
                childNode.ShowCheckBox = true;
                parentNode.Nodes.Add(childNode);
            }
        }
    }

}
