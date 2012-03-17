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

public partial class Admin_Catalog_EditCategory : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    int _CategoryId;
    Category _Category;

    protected void Page_Init(object sender, EventArgs e)
    {
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetHtmlEditor(Description, CategoryDescriptionHtml);
        PageHelper.SetPageDefaultButton(Page, FinishButton);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _CategoryId = AlwaysConvert.ToInt(Request.QueryString["CategoryId"]);
        _Category = CategoryDataSource.Load(_CategoryId);
        if (_Category == null) Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=0");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _Category.Name);
            Name.Text = _Category.Name;
            Name.Focus();
            Visibility.SelectedIndex = (int)_Category.Visibility;
            BindDisplayPage();
            UpdateDescription();
            BindThemes();
            HtmlHead.Text = _Category.HtmlHead;
            //DISPLAY PAGE
            BindDisplayPage();
            UpdateDescription();
            //THEMES
            BindThemes();
            //THUMBNAIL
            ThumbnailUrl.Text = _Category.ThumbnailUrl;
            ThumbnailAltText.Text = _Category.ThumbnailAltText;
            //CUSTOM URL
            CustomUrl.Text = _Category.CustomUrl;
            CustomUrlValidator.OriginalValue = _Category.CustomUrl;
            //SUMARY
            Summary.Text = _Category.Summary;
            SummaryCharCount.Text = ((int)(Summary.MaxLength - Summary.Text.Length)).ToString();
            Description.Text = _Category.Description;
            string confirmationJS = String.Format("return confirm('Are you sure you want to delete category \"{0}\" and all its contents?');", _Category.Name);
            DeleteButton.Attributes.Add("onclick", confirmationJS);
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
        DisplayPage.Items.Clear();
        DisplayPage.Items.Add(new ListItem("Use store default", ""));
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Category)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
        if (!Page.IsPostBack)
        {
            string currentDisplayPage = _Category.DisplayPage;
            ListItem selectedItem = DisplayPage.Items.FindByValue(currentDisplayPage);
            if (selectedItem != null) DisplayPage.SelectedIndex = DisplayPage.Items.IndexOf(selectedItem);
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
        RedirectBack();
    }

    protected void FinishButton_Click(object sender, System.EventArgs e)
    {
        SaveCategory(sender, e);
        RedirectBack();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        SaveCategory(sender, e);
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        int parentId = _Category.ParentId;
        if (_Category.Delete())
        {
            Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + parentId.ToString());
        }
    }

    private void SaveCategory(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Category.Name = Name.Text;
            _Category.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
            _Category.DisplayPage = DisplayPage.SelectedValue;
            _Category.Theme = LocalTheme.SelectedValue;
            _Category.HtmlHead = HtmlHead.Text;
            _Category.ThumbnailUrl = ThumbnailUrl.Text;
            _Category.ThumbnailAltText = ThumbnailAltText.Text;
            _Category.CustomUrl = CustomUrl.Text;
            _Category.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
            _Category.Description = Description.Text;
            _Category.Save();
        }
    }

    private void RedirectBack()
    {
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + _Category.ParentId.ToString());
    }

    protected void BindThemes()
    {
        LocalTheme.Items.Clear();
        LocalTheme.Items.Add(new ListItem("Use store default", ""));
        CommerceBuilder.UI.Styles.Theme[] themes = StoreDataHelper.GetStoreThemes();
        foreach (CommerceBuilder.UI.Styles.Theme theme in themes)
        {
            LocalTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
        }
        if (!Page.IsPostBack)
        {
            string currentTheme = _Category.Theme;
            ListItem selectedItem = LocalTheme.Items.FindByValue(currentTheme);
            if (selectedItem != null) LocalTheme.SelectedIndex = LocalTheme.Items.IndexOf(selectedItem);
        }
    }
}