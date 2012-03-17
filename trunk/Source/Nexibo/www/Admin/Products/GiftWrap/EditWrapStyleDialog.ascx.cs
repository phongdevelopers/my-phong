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
using CommerceBuilder.Taxes;

public partial class Admin_Products_GiftWrap_EditWrapStyleDialog : System.Web.UI.UserControl
{
    public int WrapStyleId
    {
        get { return AlwaysConvert.ToInt(ViewState["WrapStyleId"]); }
        set { ViewState["WrapStyleId"] = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
            PageHelper.SetPickImageButton(ImageUrl, BrowseImageUrl);
            //BIND TAXES
            TaxCode.DataSource = TaxCodeDataSource.LoadForStore();
            TaxCode.DataBind();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WrapStyle style = WrapStyleDataSource.Load(WrapStyleId);
        if (style != null)
        {
            Name.Text = style.Name;
            ThumbnailUrl.Text = style.ThumbnailUrl;
            ImageUrl.Text = style.ImageUrl;
            Price.Text = string.Format("{0:F2}", style.Price);
            TaxCode.SelectedIndex = -1;
            ListItem item = TaxCode.Items.FindByValue(style.TaxCodeId.ToString());
            if (item != null) item.Selected = true;
        }
    }

    private void DoneEditing()
    {
        UpdatePanel addEditAjax = PageHelper.RecursiveFindControl(this.Page, "AddEditAjax") as UpdatePanel;
        if (addEditAjax != null)
        {
            Panel addPanel = PageHelper.RecursiveFindControl(addEditAjax, "AddPanel") as Panel;
            if (addPanel != null) addPanel.Visible = true;
            Panel editPanel = PageHelper.RecursiveFindControl(addEditAjax, "EditPanel") as Panel;
            if (editPanel != null) editPanel.Visible = false;
            addEditAjax.Update();
        }
        UpdatePanel stylesAjax = PageHelper.RecursiveFindControl(this.Page, "WrapStylesAjax") as UpdatePanel;
        if (stylesAjax != null)
        {
            GridView stylesGrid = PageHelper.RecursiveFindControl(stylesAjax, "WrapStylesGrid") as GridView;
            if (stylesGrid != null)
            {
                stylesGrid.EditIndex = -1;
                stylesGrid.DataBind();
            }
            stylesAjax.Update();
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        DoneEditing();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            WrapStyle style = WrapStyleDataSource.Load(WrapStyleId);
            style.Name = Name.Text;
            style.ThumbnailUrl = ThumbnailUrl.Text;
            style.ImageUrl = ImageUrl.Text;
            style.Price = AlwaysConvert.ToDecimal(Price.Text);
            style.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            style.Save();
            DoneEditing();
        }
    }
}
