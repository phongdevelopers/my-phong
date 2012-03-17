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

public partial class Admin_Products_Kits_KitProductMenu : System.Web.UI.UserControl
{
    private int _ProductId = 0;
    private Product _Product;

    protected void BindComponentList()
    {
        List<KitComponent> components = new List<KitComponent>();
        foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
        {
            components.Add(pkc.KitComponent);
        }
        ComponentList.DataSource = components;
        ComponentList.DataBind();
        KitComponentPanel.Visible = (ComponentList.Items.Count > 0);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if ((_Product != null) && (!Page.IsPostBack))
        {
            BindComponentList();
        }
    }

}
