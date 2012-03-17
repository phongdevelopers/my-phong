using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

public partial class Admin_Catalog_EditWebpage : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _WebpageId;
    private Webpage _Webpage;

    protected void Page_Init(object sender, EventArgs e)
    {
        _WebpageId = AlwaysConvert.ToInt(Request.QueryString["WebpageId"]);
        _Webpage = WebpageDataSource.Load(_WebpageId);
        CancelButton.NavigateUrl = "Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString();

        if (_Webpage == null) Response.Redirect(CancelButton.NavigateUrl);

        PreviewButton.NavigateUrl = UrlGenerator.GetBrowseUrl(_WebpageId, CatalogNodeType.Webpage, _Webpage.Name);

        if (!Page.IsPostBack)
        {
            //INITIALIZE FORM ON FIRST VISIT
            Name.Focus();
            UpdateCaption();
            Name.Text = _Webpage.Name;
            Visibility.SelectedIndex = (int)_Webpage.Visibility;
            ThumbnailUrl.Text = _Webpage.ThumbnailUrl;
            ThumbnailAltText.Text = _Webpage.ThumbnailAltText;
            CustomUrl.Text = _Webpage.CustomUrl;
            CustomUrlValidator.OriginalValue = _Webpage.CustomUrl;
            Summary.Text = _Webpage.Summary;
            SummaryCharCount.Text = ((int)(Summary.MaxLength - Summary.Text.Length)).ToString();
            WebpageContent.Text = _Webpage.Description;
            HtmlHead.Text = _Webpage.HtmlHead;
            BindDisplayPage();
            BindThemes();
            Name.Focus();
            UpdateDescription();
        }
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.SetMaxLengthCountDown(Summary, SummaryCharCount);
        PageHelper.SetHtmlEditor(WebpageContent, WebpageContentHtml);
        PageHelper.SetPickImageButton(ThumbnailUrl, BrowseThumbnailUrl);
        PageHelper.SetPageDefaultButton(Page, SaveButton);

        PageHelper.PreventFirefoxSubmitOnKeyPress(Summary, Summary.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(WebpageContent, WebpageContent.ClientID);
        PageHelper.PreventFirefoxSubmitOnKeyPress(HtmlHead, HtmlHead.ClientID);
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        SaveButton_Click(sender, e);
        // RETURN TO BROWSE PARENT CATEGORY
        Response.Redirect("~/Admin/Catalog/Browse.aspx?CategoryId=" + PageHelper.GetCategoryId().ToString());
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            _Webpage.Name = Name.Text;
            _Webpage.Visibility = (CatalogVisibility)Visibility.SelectedIndex;
            _Webpage.ThumbnailUrl = ThumbnailUrl.Text;
            _Webpage.ThumbnailAltText = ThumbnailAltText.Text;
            _Webpage.CustomUrl = CustomUrl.Text;
            _Webpage.Summary = StringHelper.Truncate(Summary.Text, Summary.MaxLength);
            _Webpage.Description = WebpageContent.Text;
            _Webpage.HtmlHead = HtmlHead.Text;
            _Webpage.DisplayPage = DisplayPage.SelectedValue;
            _Webpage.Theme = LocalTheme.SelectedValue;
            _Webpage.Save();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

    protected void UpdateCaption()
    {
        string shortWebpageName = _Webpage.Name;
        if (shortWebpageName.Length > 45) shortWebpageName = shortWebpageName.Substring(0, 40) + "...";
        Caption.Text = string.Format(Caption.Text, shortWebpageName);
    }

    protected void BindDisplayPage()
    {
        List<CommerceBuilder.UI.Styles.DisplayPage> displayPages = CommerceBuilder.UI.Styles.DisplayPageDataSource.Load();
        foreach (CommerceBuilder.UI.Styles.DisplayPage displayPage in displayPages)
        {
            if (displayPage.NodeType == CatalogNodeType.Webpage)
            {
                string displayName = string.Format("{0}", displayPage.Name);
                DisplayPage.Items.Add(new ListItem(displayName, displayPage.DisplayPageFile));
            }
        }
        if (!Page.IsPostBack)
        {
            string currentDisplayPage = _Webpage.DisplayPage;
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
                DisplayPageDescription.Text = page.Description;
            }
        }
        else
        {
            DisplayPageDescription.Text = string.Empty;
        }
    }

    protected void BindThemes()
    {
        LocalTheme.DataSource = StoreDataHelper.GetStoreThemes();
        LocalTheme.DataBind();
        if (!Page.IsPostBack)
        {
            string currentTheme = _Webpage.Theme;
            ListItem selectedItem = LocalTheme.Items.FindByValue(currentTheme);
            if (selectedItem != null) LocalTheme.SelectedIndex = LocalTheme.Items.IndexOf(selectedItem);
        }
    }
}