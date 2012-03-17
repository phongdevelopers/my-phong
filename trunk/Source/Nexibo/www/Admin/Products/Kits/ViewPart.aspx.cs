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

public partial class Admin_Products_Kits_ViewPart : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _CategoryId = 0;
    private Category _Category;
    private int _ProductId = 0;
    private Product _Product;

    protected int CategoryId
    {
        get
        {
            if (_CategoryId.Equals(0))
            {
                _CategoryId = PageHelper.GetCategoryId();
            }
            return _CategoryId;
        }
    }

    protected Category Category
    {
        get
        {
            if (_Category == null)
            {
                _Category = CategoryDataSource.Load(this.CategoryId);
            }
            return _Category;
        }
    }

    protected Product Product
    {
        get
        {
            if (_Product == null)
            {
                _Product = ProductDataSource.Load(this.ProductId);
            }
            return _Product;
        }
    }

    protected int ProductId
    {
        get
        {
            if (_ProductId.Equals(0))
            {
                _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
            }
            return _ProductId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ComponentList.DataSource = KitComponentDataSource.LoadForMemberProduct(this.ProductId);
            ComponentList.DataBind();
        }
    }

    protected KitComponent GetSelectedComponent()
    {
        KitComponent component = null;
        if (ComponentList.SelectedIndex > -1 && ComponentList.SelectedIndex < ComponentList.DataKeys.Count)
        {
            int componentId = AlwaysConvert.ToInt(ComponentList.DataKeys[ComponentList.SelectedIndex]);
            component = KitComponentDataSource.Load(componentId);
        }
        return component;
    }
    
    protected void BindComponentDetail()
    {
        KitComponent component = GetSelectedComponent();
        if (component != null)
        {
            ComponentDetail.Visible = true;
            SelectedComponentName.Text = component.Name;
            List<Product> productList = new List<Product>();
            foreach (ProductKitComponent pkc in component.ProductKitComponents)
            {
                productList.Add(pkc.Product);
            }
            KitList.DataSource = productList;
            KitList.DataBind();
        }
        else ComponentDetail.Visible = false;
    }
    
    protected void ComponentList_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindComponentDetail();
    }

}
