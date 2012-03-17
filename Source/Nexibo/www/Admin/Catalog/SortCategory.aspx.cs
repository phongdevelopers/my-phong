using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

public partial class Admin_Catalog_SortCategory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    private int _CategoryId;
    private Category _Category;
    private CatalogNodeCollection _CatalogNodes;
    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _Category = CategoryDataSource.Load(_CategoryId);
        _CatalogNodes = CatalogDataSource.LoadForCategory(_CategoryId, false);
        if (_CatalogNodes != null && _CatalogNodes.Count > 0)
        {
            if (_Category == null)
                Caption.Text += " Sort Items in Root Category";
            else
                Caption.Text += " Sort Items in " + _Category.Name;
            if (!Page.IsPostBack)
            {
                BindCatalogNodesList();
            }
        }
        else
            Response.Redirect("Browse.aspx");
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SortOrder.Value))
        {
            CatalogNodeCollection catalogNodes = _CatalogNodes;

            string[] catalogNodesIds = SortOrder.Value.Split(",".ToCharArray());
            int order = 0;
            int tempId = 0;
            int index = -1;
            foreach (string sPartId in catalogNodesIds)
            {
                foreach (CatalogNode cn in _CatalogNodes)
                {
                    tempId = AlwaysConvert.ToInt(sPartId);
                    if (cn.CatalogNodeId == tempId)
                    {
                        index = catalogNodes.IndexOf(cn);
                        if (index > -1)
                            catalogNodes[index].OrderBy = (short)order;
                        order++;
                    }
                }
            }
            catalogNodes.Save();
        }
        Response.Redirect("Browse.aspx?CategoryId=" + _CategoryId.ToString());
    }

    protected void QuickSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        CatalogNodeCollection catalogNodes = new CatalogNodeCollection();
        foreach (CatalogNode catalogNode in _CatalogNodes)
        {
            catalogNodes.Add(catalogNode);
        }
        switch (QuickSort.SelectedIndex)
        {
            case 2:
                catalogNodes.Sort("Name", GenericComparer.SortDirection.DESC);
                break;
            default:
                catalogNodes.Sort("Name", GenericComparer.SortDirection.ASC);
                break;
        }
        CatalogNodeList.DataSource = catalogNodes;
        CatalogNodeList.DataBind();
        QuickSort.SelectedIndex = 0;
    }

    private void BindCatalogNodesList()
    {
        CatalogNodeCollection catalogNodes = new CatalogNodeCollection();
        foreach (CatalogNode catalogNode in _CatalogNodes)
        {
            catalogNodes.Add(catalogNode);
        }
        CatalogNodeList.DataSource = catalogNodes;
        CatalogNodeList.DataBind();
    }
}
    
    

