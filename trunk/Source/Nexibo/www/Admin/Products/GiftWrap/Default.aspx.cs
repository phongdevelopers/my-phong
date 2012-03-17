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

public partial class Admin_Store_GiftWrap_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            WrapGroup wrapGroup = new WrapGroup();
            wrapGroup.Name = AddName.Text;
            wrapGroup.Save();
            Response.Redirect("EditWrapGroup.aspx?WrapGroupId=" + wrapGroup.WrapGroupId.ToString());
        }
    }

    protected void WrapGroupGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Copy")
        {
            int wrapGroupId = (int)WrapGroupGrid.DataKeys[Int32.Parse(e.CommandArgument.ToString())].Value;
            WrapGroup copy = WrapGroup.Copy(wrapGroupId, true);
            if (copy != null)
            {
                // THE NAME SHOULD NOT EXCEED THE MAX 50 CHARS
                String newName = "Copy of " + copy.Name;
                if (newName.Length > 50)
                {
                    newName = newName.Substring(0, 47) + "...";
                }
                copy.Name = newName;
                copy.Save();
                WrapGroupGrid.DataBind();
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PageHelper.SetDefaultButton(AddName, AddButton.ClientID);
    }

    protected int CountWrapStyles(object dataItem)
    {
        return WrapStyleDataSource.CountForWrapGroup(((WrapGroup)dataItem).WrapGroupId);
    }

    protected int CountProducts(object dataItem)
    {
        return ProductDataSource.CountForWrapGroup(((WrapGroup)dataItem).WrapGroupId);
    }
}
