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
using CommerceBuilder.Utility;

public partial class Admin_Products_GiftWrap_EditWrapGroup : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _WrapGroupId;
    private WrapGroup _WrapGroup;

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            _WrapGroup.Name = Name.Text;
            _WrapGroup.Save();
            Caption.Text = string.Format((string)ViewState["Caption"], _WrapGroup.Name);
            Response.Redirect("Default.aspx");
        }
    }

    protected void BackButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _WrapGroupId = AlwaysConvert.ToInt(Request.QueryString["WrapGroupId"]);
        _WrapGroup = WrapGroupDataSource.Load(_WrapGroupId);
        if (_WrapGroup == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Name.Text = _WrapGroup.Name;
            AssignedProducts.Text = ProductDataSource.CountForWrapGroup(_WrapGroupId).ToString();
            AssignedProducts.NavigateUrl += "?WrapGroupId=" + _WrapGroupId.ToString();
            ViewState["Caption"] = Caption.Text;
            Caption.Text = string.Format(Caption.Text, _WrapGroup.Name);
            //INITIALIZE THE ADD WRAP GROUP DIALOG
            ASP.admin_products_giftwrap_addwrapstyledialog_ascx addDialog = PageHelper.RecursiveFindControl(this, "AddWrapStyleDialog1") as ASP.admin_products_giftwrap_addwrapstyledialog_ascx;
            if (addDialog != null) addDialog.WrapGroupId = _WrapGroupId;
        }
    }

    protected void WrapStylesGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int wrapStyleId = (int)WrapStylesGrid.DataKeys[e.NewEditIndex].Value;
        WrapStyle style = WrapStyleDataSource.Load(wrapStyleId);
        if (style != null)
        {
            AddPanel.Visible = false;
            EditPanel.Visible = true;
            EditCaption.Text = string.Format(EditCaption.Text, style.Name);
            ASP.admin_products_giftwrap_editwrapstyledialog_ascx editDialog = EditPanel.FindControl("EditWrapStyleDialog1") as ASP.admin_products_giftwrap_editwrapstyledialog_ascx;
            if (editDialog != null) editDialog.WrapStyleId = wrapStyleId;
            AddEditAjax.Update();
        }
    }

    protected void WrapStylesGrid_RowCancelingEdit(object sender, EventArgs e)
    {
        AddPanel.Visible = true;
        EditPanel.Visible = false;
        AddEditAjax.Update();
    }

    protected void WrapStylesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp")
        {
            WrapStyleCollection wrapStyles = WrapStyleDataSource.LoadForWrapGroup(_WrapGroupId);
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex < 1) || (itemIndex > wrapStyles.Count - 1)) return;
            WrapStyle selectedItem = wrapStyles[itemIndex];
            WrapStyle swapItem = wrapStyles[itemIndex - 1];
            wrapStyles.RemoveAt(itemIndex - 1);
            wrapStyles.Insert(itemIndex, swapItem);
            for (int i = 0; i < wrapStyles.Count; i++)
            {
                wrapStyles[i].OrderBy = (short)i;
            }
            wrapStyles.Save();            
        }
        else if (e.CommandName == "MoveDown")
        {
            WrapStyleCollection wrapStyles = WrapStyleDataSource.LoadForWrapGroup(_WrapGroupId);
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex > wrapStyles.Count - 2) || (itemIndex < 0)) return;
            WrapStyle selectedItem = wrapStyles[itemIndex];
            WrapStyle swapItem = wrapStyles[itemIndex + 1];
            wrapStyles.RemoveAt(itemIndex + 1);
            wrapStyles.Insert(itemIndex, swapItem);
            for (int i = 0; i < wrapStyles.Count; i++)
            {
                wrapStyles[i].OrderBy = (short)i;
            }
            wrapStyles.Save();
        }
        WrapStylesGrid.DataBind();
        WrapStylesAjax.Update();
    }

    protected bool ShowThumbnail(object dataItem)
    {
        return (!string.IsNullOrEmpty(((WrapStyle)dataItem).ThumbnailUrl));
    }
}
