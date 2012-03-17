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
using CommerceBuilder.DigitalDelivery;

public partial class Admin_DigitalGoods_Readmes : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void AddReadmeButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Readme readme = new Readme();
            readme.DisplayName = AddReadmeName.Text;
            readme.Save();
            Response.Redirect("EditReadme.aspx?ReadmeId=" + readme.ReadmeId);
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddReadmeName, AddReadmeButton.ClientID);
        AddReadmeName.Focus();
    }

    private Dictionary<int, int> _ProductCounts = new Dictionary<int, int>();
    protected int GetProductCount(object dataItem)
    {
        Readme m = (Readme)dataItem;
        if (_ProductCounts.ContainsKey(m.ReadmeId)) return _ProductCounts[m.ReadmeId];
        int count = DigitalGoodDataSource.CountForReadme(m.ReadmeId);
        _ProductCounts[m.ReadmeId] = count;
        return count;
    }

    protected bool HasProducts(object dataItem)
    {
        return (GetProductCount(dataItem) > 0);
    }
}
