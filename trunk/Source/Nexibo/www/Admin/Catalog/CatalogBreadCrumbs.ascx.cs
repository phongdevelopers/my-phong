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
using System.Collections.Generic;
using CommerceBuilder.Catalog;

public partial class Admin_Catalog_CatalogBreadCrumbs : System.Web.UI.UserControl
{
    int _CategoryId;
    bool initialized = false;

    public int CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; initialized = true; }
    }

    protected void Page_PreRender(object sender, System.EventArgs e)
    {
        if (!initialized) this.CategoryId = PageHelper.GetCategoryId();
        if (this.CategoryId != 0)
        {
            List<CatalogPathNode> path = CatalogDataSource.GetPath(this.CategoryId, false);
            Repeater1.DataSource = path;
            Repeater1.DataBind();
        }
    }
}
