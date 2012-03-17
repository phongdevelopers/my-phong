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
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using System.Collections.Generic;

public partial class Admin_Catalog_AddCategory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _ParentCategoryId;
    Category _ParentCategory;

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetHtmlEditor(Description, CategoryDescriptionHtml);
        PageHelper.SetPageDefaultButton(Page, FinishButton);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _ParentCategoryId = AlwaysConvert.ToInt(Request.QueryString["ParentCategoryId"]);
        if (_ParentCategoryId != 0)
        {
            _ParentCategory = CategoryDataSource.Load(_ParentCategoryId);
        }
        else
        {
            _ParentCategory = new Category();
            _ParentCategory.Name = "Catalog";
        }
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _ParentCategory.Name);
            Visibility.SelectedIndex = 0;
            Name.Focus();
            BindDisplayPage();
            UpdateDescription();
            BindThemes();
        }
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.ConvertEnterToTab(Visibility);
        PageHelper.DisableValidationScrolling(this);
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);

        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(Description, Description.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
    }

    protected void BindDisplayPage()
    {
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Category)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
    }

    protected void DisplayPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        DisplayPageDescription.Text = "&nbsp;";
        if (DisplayPage.SelectedValue != string.Empty)
        {
            CommerceBuilder.UI.Styles.DisplayPage page = CommerceBuilder.UI.Styles.DisplayPageDataSource.ParseFromFile(Server.MapPath("~/" + DisplayPage.SelectedValue), "/");
            if (page != null)
            {
                DisplayPageDescription.Text = "<strong>" + DisplayPage.SelectedItem.Text + ":</strong> " + page.Description;
            }
        }
        else
        {
            DisplayPageDescription.Text = "<strong>" + DisplayPage.Items[0].Text + ":</strong> Uses the default display page configured for the store.";
        }
    }
    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _ParentCategoryId.ToString());
    }

    protected void FinishButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            Category category = new Category();
            category.ParentId = _ParentCategoryId;
            category.Name = Name.Text;
            category.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
            category.DisplayPage = DisplayPage.SelectedValue;
            category.Theme = LocalTheme.SelectedValue;
            category.HtmlHead = HtmlHead.Text;
            category.ThumbnailUrl = ThumbnailUrl.Text;
            category.ThumbnailAltText = ThumbnailAltText.Text;
            category.CustomUrl = CustomUrl.Text;
            category.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
            category.Description = Description.Text;
            category.Save();
            Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _ParentCategoryId.ToString());
        }
    }

    protected void BindThemes()
    {
        LocalTheme.DataSource = StoreDataHelper.GetStoreThemes();
        LocalTheme.DataBind();
    }
}
